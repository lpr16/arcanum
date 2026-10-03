# Antigravity work program

Leon asked for this on 3 Oct 2026, then asked that the program focus on elliptic curves. Execute the phases in order. One phase is one commit on `main`, pushed to `origin/main`, plus a new entry at the top of `Docs/agent_channel.md`. Then start the next phase. Cursor is not doing this run. Cursor wrote this plan. Antigravity executes it.

This file is a work order for the public repository `lpr16/arcanum`. If it disagrees with `AGENTS.md` or the code, `AGENTS.md` and the code win. Code wins over both.

The checkout at the start of this program is commit `bdaf6bdc0252e4df5d730f97f69939a2d430242b`. `origin` is `https://github.com/lpr16/arcanum.git`. The solution file is `Arcanum.slnx`. The F# projects are `Arcanum.Core`, `Arcanum.Classical`, `Arcanum.Symmetric`, `Arcanum.Hashing`, `Arcanum.Asymmetric`, `Arcanum.Cli`, and `tests/Arcanum.Tests`. The directory name of the checkout may differ. Do not rename the checkout as part of a phase.

The product is the group of points on a short Weierstrass curve over a prime field. The field phase exists because the coordinates live in that field. After the labels in phases 1 and 2, every phase is that group. There is no lattice phase.

## How to run

- Start from current `main`. If this file is untracked, phase 0 commits it as it stands. Later phases do not restyle it and do not weaken a law written here.
- Read `AGENTS.md` and the top of `Docs/agent_channel.md` before each phase, once those files exist.
- One phase, one commit, one push. The channel entry for that phase is in the same commit. Then push `origin/main`. Then start the next phase.
- Commit message shape: `Phase N: <object>`. One sentence is enough.
- Do not amend. Do not force-push. Do not skip hooks. Do not change git config. Do not commit secrets, keys, or tokens. The repository is public.
- `origin` already exists. Do not create a second repository. If the push fails, stop the run.
- Target framework: `net10.0`, already chosen in the `.fsproj` files. Later phases do not retarget. Do not multi-target.
- SDK-style projects. F# compile items are explicit `<Compile Include="...">` entries. A phase that adds a file appends it in the `.fsproj` in dependency order. `ToyField.fs` is compiled before `EllipticCurve.fs`. Do not turn on a compile glob.
- Test runner: xUnit. Laws: FsCheck and FsCheck.Xunit, already referenced. No new test package. No new NuGet cryptography package as the implementation of the field or the group law.
- The command that must pass before every commit, from the repo root:

```
dotnet test --configuration Release
```

- If a law fails, fix that phase. Do not weaken a law to go green. Weakening includes deleting a locked case, shrinking the enumerated set, catching a failure and returning the identity, or changing an expected point so the current code matches.
- A failed law is a failed test. FsCheck searches for counterexamples. It does not prove a theorem. Do not add a proof file.
- No emojis in code, tests, comments, commits, or docs.
- Channel entry shape, newest at the top of the log: date, author `Antigravity`, the phase number, what landed, and that `dotnet test --configuration Release` passed. Do not rewrite another author's entry.

## What is already on main

`bdaf6bdc` has no `AGENTS.md` and no `Docs/`. The README describes a laboratory of classical ciphers, AES, ChaCha20-Poly1305, RSA, ECDSA, NIST CAVP vectors, and constant-time implementations. That description is ahead of the tests and ahead of several files.

The curve code that has landed is `EllipticCurve.fs`: short Weierstrass `isOnCurve`, `negate`, `add`, `double`, `scalarMultiply`, and the parameter records `secp256k1` and `nistP256`. `add` is the chord-and-tangent law. `scalarMultiply` reduces the scalar modulo `Curve.N` before the loop, and returns `Infinity` when that residue is zero. `Curve.N` is documented as the order of `G`.

`Modular.fs` is the arithmetic that law uses: `modPos`, `xgcd`, `modInverse`, and `modPow`. `modPow` for a non-negative exponent calls `BigInteger.ModPow`. `crt`, `jacobi`, and `modSqrt` are already written. This program does not extend them.

Library calls, not the curve:

- `SHA256.fs` calls `System.Security.Cryptography.SHA256.HashData`.
- `HMAC.fs` is an F# construction whose hash step is `SHA256.hash`.
- `PBKDF2.fs` is an F# construction whose PRF is `HMAC.hmacSha256`.
- `Bytes.randomBytes` and the random bytes inside `Primes.fs` call `RandomNumberGenerator.Fill`.

`tests/Arcanum.Tests/Tests.fs` is one module. Caesar, Atbash, AES-CBC, and ChaCha20-Poly1305 are round-trips. Enigma checks reciprocity. SHA-256("abc") and the empty message check published digests against the library call. HMAC has one RFC 4231 case. ECDH checks that two local parties agree. ECDSA checks sign-then-verify. None of those is a NIST CAVP file, and none of them is a lock on the group law.

`Ecdsa.signWithNonce` takes a caller-supplied nonce. `Ecdsa.sign` draws a random nonce. There is no RFC 6979 procedure in that file.

## Ground truth

- The product is the group of points on a short Weierstrass curve `y^2 = x^3 + a x + b` over a prime field. Exact integers. A failed law is a failed test.
- The coordinate field is `F_p` for one named prime small enough that every point can be listed. The same group law is then checked on `secp256k1` and on NIST P-256, where listing every point is impossible.
- Scalar multiplication is repeated addition. On the toy curve the test can say that by walking the points. On the large curves the test uses double-and-add and does not take a discrete logarithm.
- It is not a second implementation of TLS. It is not elementa. It is not mathcore. A lattice is not a phase of this program.
- Classical ciphers, the CLI, and the CLR hash stay. They are labeled in phases 1 and 2. Later phases do not edit them.
- A file that calls the CLR says so in the module comment and in the README. A test that is a round-trip is described as a round-trip.
- `System.Security.Cryptography` may remain only where phase 1 or phase 2 names it as a library call.
- `F#` `BigInteger` is not a constant-time field. Do not add a constant-time claim as a comment unless a test or a cited construction justifies it.
- elementa, mathcore, and studium are other repositories. Do not edit them. Do not clone them. Do not import them. Do not retarget their ladders.
- This file wins only where `AGENTS.md` and the code are silent. `AGENTS.md` wins over `Docs/`. Code wins over both.
- No emojis. Do not commit secrets. The repository is public.

## Hard stops

Do not do any of these, even if a phase feels blocked:

- Claim a CLR hash is an implementation of SHA-256.
- Claim a round-trip is a CAVP or RFC vector.
- Add constant-time as a comment unless a test or a cited construction justifies it.
- Import elementa. Retarget the elementa ladder. Edit mathcore or studium.
- Start Lean, Rocq, or another proof assistant. Wrap Sage, Oscar, or SymPy.
- Add Mordell-Weil, L-functions, or Birch-Swinnerton-Dyer. Add pairings, isogenies, or rank over `Q`.
- Add a second model of the curve. Montgomery, Edwards, and Hessian forms are parked. The law in this program is short Weierstrass, in the `add` that already exists.
- Delete the `secp256k1` parameters or the NIST P-256 parameters. Claim the toy curve is a secure group.
- Set `Curve.N` to the order of a point that does not generate the group, and then reduce scalars modulo that order.
- Treat `scalarMultiply curve curve.N P` as a proof that the order is `N`. That call reduces modulo `N` and returns `Infinity` before any addition.
- Replace `Modular.xgcd` or `Modular.modInverse` with `BigInteger.ModPow`. Call `BigInteger.ModPow` the definition of the field without a comment that names the delegation.
- Add a NuGet cryptography package as the implementation of the field or the group law.
- Add a UI, open a port, or add Python. Make the green build depend on any of those.
- Reimplement SHA-256, AES, or ChaCha20. Delete the classical ciphers or the CLI. Extend Enigma.
- Add a lattice, an LWE sampler, or a lattice encryption scheme.
- Generate a new "standard" curve vector by running `scalarMultiply` and pasting the output back as the expected value.
- Weaken a law to go green.

## Already done, do not redo

`EllipticCurve.add` is the group law. Phases 4, 5, and 6 call it. They do not replace it with a second formula. Phase 7 locks the parameter records already written.

`Modular.modInverse` is the division in that law. Phase 3 calls it. Do not rewrite `xgcd`.

Do not rebuild AES, ChaCha20, Poly1305, RSA, ECDSA, the classical ciphers, or the CLI from this plan. Phases 1 and 2 label them. Later phases leave those files alone.

## Phase 0 — Ground rules

### Files

- `AGENTS.md`
- `Docs/agent_channel.md`
- This file, if it is not already committed

### Object

No code change. No `.fsproj` change. No README change.

`AGENTS.md` is one page. It states the product:

- The product is the group of points on a short Weierstrass curve over a prime field. The coordinate field is exact. The group law is `EllipticCurve.add`. Scalar multiplication is repeated addition.
- A small curve is enumerated completely. `secp256k1` and NIST P-256 keep their parameter records. Checks on those curves do not take a discrete logarithm and do not list the group.
- The toy curve is not a secure group. `F#` `BigInteger` is not a constant-time field.
- A CLR call is labeled in the module and in the README. `System.Security.Cryptography.SHA256.HashData` is a library call. It is not an implementation of SHA-256 in this repository.
- A round-trip test is a round-trip. It is not a NIST CAVP vector.
- Classical ciphers and the CLI are historical mechanisms and demonstrations. They are not the product. This program does not add a lattice.
- elementa and mathcore are other repositories. Do not edit them, clone them, or import them.
- This file wins over `Docs/`. Code wins over both.
- Notes between Cursor and Antigravity go in `Docs/agent_channel.md`. A new entry goes at the top. Do not rewrite another author's entry.
- The repository is public. No secrets, keys, or tokens in commits. No emojis. No new UI. No port.

`Docs/agent_channel.md` is a mailbox, not a specification. It states that `AGENTS.md` and the code win, that a new entry goes at the top, and that entries carry no emojis and no secrets. It starts with the phase 0 entry at the top of the log.

### Laws

No new mathematical law. `dotnet test --configuration Release` still passes. The diff has no `.fs` file and no `.fsproj` file.

### Exit

`dotnet test --configuration Release` passes. Commit. Push `origin/main`.

## Phase 1 — README honesty

### Files

- `README.md`
- `tests/Arcanum.Tests/Tests.fs`, only to rename facts whose titles overclaim. Assertions and hex values stay.

### Object

The README says the product is the elliptic-curve group, and that the rest of the tree is present and labeled. Keep the quickstart commands:

```
dotnet run --project src/Arcanum.Cli/Arcanum.Cli.fsproj
dotnet test
```

The Release command may be added beside `dotnet test`. Both existing commands stay.

Remove every emoji, including the section markers. The ASCII banner stays only if it contains no emoji. Do not add a new banner.

Do not describe a lattice, and do not promise one. Do not describe the toy curve; phase 3 has not added it. Name the curve code that exists today: `EllipticCurve.add`, `double`, `scalarMultiply`, `secp256k1`, and `nistP256`. Say that `scalarMultiply` reduces modulo `Curve.N`.

The tree, or the prose that replaces it, names two groups of files.

The group law and the arithmetic it uses:

- `EllipticCurve.fs`
- `Modular.fs`. A non-negative exponent in `modPow` is `BigInteger.ModPow`. Inverse, `xgcd`, `crt`, `jacobi`, and `modSqrt` are written out.

Not the product, and not deleted:

- `SHA256.fs`: `System.Security.Cryptography.SHA256.HashData`
- `HMAC.fs`: F# construction; the hash step is `SHA256.hash`
- `PBKDF2.fs`: F# construction; the PRF is `HMAC.hmacSha256`
- `Bytes.randomBytes` and the random source in `Primes.fs`: `RandomNumberGenerator.Fill`
- `FiniteFields.fs`, and `Arcanum.Symmetric`: `Padding.fs`, `Aes.fs`, `Modes.fs`, `ChaCha20.fs`, `Poly1305.fs`, `ChaCha20Poly1305.fs`
- `Rsa.fs`, `DiffieHellman.fs`, `Ecdsa.fs`. The message representative in `Ecdsa` uses `SHA256.hash`. A caller-supplied nonce is not an implementation of RFC 6979.
- `Arcanum.Classical`: `Substitution.fs`, `Polyalphabetic.fs`, `Playfair.fs`, `Enigma.fs`, `Cryptanalysis.fs`
- `Arcanum.Cli/Program.fs`, as demonstrations

Say what the tests are:

- Caesar, Atbash, AES-CBC, and ChaCha20-Poly1305 are round-trips. The ChaCha20-Poly1305 fact also rejects one tampered ciphertext. That is not an RFC 8439 vector file.
- Enigma is a reciprocity check.
- SHA-256("abc") and the empty message compare the library call with published digests. They do not show an in-repo compression function.
- HMAC has one RFC 4231 case.
- ECDH checks that two local parties agree. It does not check a published shared secret.
- ECDSA checks sign-then-verify. It does not check a published signature.

Remove the README claims that the suite is NIST CAVP, that PKCS#7 or the MAC compare is constant-time as a property of the product, and that `secp256k1` arithmetic is constant-time. Do not describe `BigInteger` as a constant-time field.

Rename these facts. Keep the assertions and the expected strings:

- `Hashing - SHA-256 standard NIST test vector for abc` becomes a name that says the library call matches the published digest of `"abc"`. The expected hex stays `ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad`.
- `Hashing - SHA-256 empty string test vector` becomes a name that says the library call matches the published digest of the empty message. The expected hex stays `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855`.

Do not rename the HMAC fact off RFC 4231. Do not delete the Caesar fact or the Enigma fact.

### Laws

No new mathematical law. The existing assertions still pass, including the two SHA-256 digests, the RFC 4231 HMAC tag `b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7`, the Caesar round-trip of `THEQUICKBROWNFOXJUMPSOVERTHELAZYDOG`, and the Enigma reciprocity check of `OPERATIONBARBAROSSA`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 2 — Label everything that is not the curve

### Files

- `src/Arcanum.Hashing/SHA256.fs`
- `src/Arcanum.Hashing/HMAC.fs`
- `src/Arcanum.Hashing/PBKDF2.fs`
- `src/Arcanum.Classical/Substitution.fs`
- `src/Arcanum.Classical/Polyalphabetic.fs`
- `src/Arcanum.Classical/Playfair.fs`
- `src/Arcanum.Classical/Enigma.fs`
- `src/Arcanum.Classical/Cryptanalysis.fs`
- `src/Arcanum.Cli/Program.fs`, module comment only

### Object

Comments only. No new algorithm. No change to `EllipticCurve.fs`. After this phase, those files are not edited again.

Hashing:

- `SHA256`: the primitive is `System.Security.Cryptography.SHA256.HashData`. This repository does not implement the SHA-256 compression function. Delete the wording "NIST-certified primitive" and any wording that this module is FIPS 180-4 as an implementation.
- `HMAC`: `hmacSha256` is an F# construction. The hash it calls is `SHA256.hash`. The module comment says so. Do not switch it to `HMAC.HashData`.
- `PBKDF2`: `deriveKey` is an F# construction. Its PRF is `HMAC.hmacSha256`. The module comment says the underlying hash primitive is the CLR call above. Do not switch it to `Rfc2898DeriveBytes`.

`HMAC.verifySha256` may keep its call to `Bytes.constantTimeEquals`. Do not add a new sentence that the field, the curve, or this compare is constant-time.

Classical modules and the CLI: each module comment says the file is a historical mechanism or a demonstration and is not the mathematical core. Do not add a rotor, a reflector, or a new Enigma test. Do not remove `Substitution.caesarEncrypt` or `Enigma.processMessage`.

The existing test still locks SHA-256("abc") through `SHA256.hashString`. Do not change the expected digest.

### Laws

No new law. The published digest of `"abc"`, the RFC 4231 case, the Caesar round-trip, and the Enigma reciprocity check still pass. The Enigma source still has rotors I–V and reflectors B and C.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 3 — Coordinate field

### Files

- `src/Arcanum.Core/ToyField.fs`
- `src/Arcanum.Core/Arcanum.Core.fsproj`, appending `ToyField.fs` after `Modular.fs` and before `EllipticCurve.fs`
- `src/Arcanum.Core/Modular.fs`, only the comment on `modPow`
- `tests/Arcanum.Tests/ToyFieldTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`, appending the new test file

### Object

The field the toy curve sits on. One named prime, small enough to list. The prime is 17. The residues are `0` through `16`. This phase does not add a curve.

```fsharp
[<Struct>]
type Residue = private Residue of BigInteger
```

`ToyField.prime` is `17`. `ofBigInteger` reduces with `Modular.modPos` into `0 .. prime - 1`.

`inverse` calls `Modular.modInverse`. It returns `None` when the inverse does not exist. Inverse of zero is that refusal. Do not define inverse as `a^(p-2)`. The chord slope in `EllipticCurve.add` already uses `Modular.modInverse`. Fermat is a law about this field, not a second inverse.

Exponentiation in this module calls `Modular.modPow`. Do not call `BigInteger.ModPow` from `ToyField.fs`.

On `Modular.modPow`, one comment names the delegation: a non-negative exponent is `BigInteger.ModPow`. Do not change the body. Do not replace `xgcd`, `modInverse`, `crt`, `jacobi`, or `modSqrt`.

### Laws

Facts, over the whole residue list, not a random `BigInteger`:

- Every value `0` through `16`, reduced by `ofBigInteger`, is that residue. `ofBigInteger 17` is `0`. `ofBigInteger -1` is `16`.
- `inverse` of `3` is `6`, because `3 * 6 = 18 ≡ 1 (mod 17)`.
- `inverse` of `0` is `None`.
- Every residue from `1` through `16` has an inverse, and the product of a residue with its inverse is `1`.
- Fermat: for every `a` from `1` through `16`, `Modular.modPow a 16 17` is `1`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 4 — Toy curve

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding one toy curve value. Do not change `add`. Do not delete `secp256k1` or `nistP256`.
- `tests/Arcanum.Tests/ToyCurveTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`, appending the new test file

### Object

The short Weierstrass curve over the phase 3 field. Every affine point is enumerable. The group law is the existing `EllipticCurve.add`. Do not write a second addition formula.

`EllipticCurve.toy` is a `Curve`:

- `P` = `ToyField.prime` (`17`)
- `A` = `0`
- `B` = `1`
- so `y^2 = x^3 + 1` over `F_17`
- `G` = `Point(6, 8)`
- `N` = `18`
- `H` = `1`

`G` is a generator of the whole group. `N` is that group order, which is also the order of `G`. `H` is `1`. Phase 6 locks those three claims by walking the points. This phase writes them on the record and locks the hand calculations below.

A comment on `toy` says the curve is not a secure group. Do not pass `toy` to ECDH or ECDSA.

The quantity `4 A^3 + 27 B^2` is `27`, which is not `0` modulo `17`, so the cubic has distinct roots in an algebraic closure and the curve is non-singular. The test locks that this residue is not zero. Do not replace the curve with a singular one.

Enumeration uses `isOnCurve`: every `x` and `y` in `0 .. 16`, plus `Infinity`.

One chord and one tangent, with the slope written in a comment next to the expected point. The expected coordinates are literals. They are not computed by a second copy of `add` inside the test.

- `(0, 1) + (1, 6)`. Slope `(6 - 1) * (1 - 0)^(-1) = 5`. Then `x = 5^2 - 0 - 1 = 24 ≡ 7` and `y = 5 * (0 - 7) - 1 = -36 ≡ 15`. The sum is `Point(7, 15)`.
- `double` of `Point(1, 6)`. Slope `(3 * 1^2 + 0) * (2 * 6)^(-1) = 3 * 12^(-1) = 3 * 10 = 30 ≡ 13`. Then `x = 13^2 - 2 * 1 = 167 ≡ 14` and `y = 13 * (1 - 14) - 6 = -175 ≡ 12`. The double is `Point(14, 12)`.

`Point(0, 1)` has order `3`. The test reaches `Infinity` by adding that point to itself, and not earlier. This is the witness that a point on `toy` need not generate the group. Do not replace `G` with `Point(0, 1)`. `scalarMultiply` reduces modulo `N`. A generator order of `3` would make that reduction the wrong scalar for a point of larger order.

### Laws

- The enumerated set has 18 points, `Infinity` included.
- Every enumerated affine point satisfies the curve equation through `isOnCurve`. `Infinity` does too.
- `4 * A^3 + 27 * B^2` is not `0` modulo `P`.
- `Infinity` is the identity: for every enumerated point `Q`, `add toy Infinity Q = Q` and `add toy Q Infinity = Q`.
- For every enumerated point `Q`, `add toy Q (negate toy Q)` is `Infinity`, and `negate` sends `Point(x, y)` to `Point(x, -y mod P)`.
- `add toy (Point(0, 1)) (Point(1, 6))` is `Point(7, 15)`.
- `double toy (Point(1, 6))` is `Point(14, 12)`.
- Adding `Point(0, 1)` to itself reaches `Infinity` at the third addition and not at the first or the second.
- `secp256k1` and `nistP256` still have their existing parameters.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 5 — Group law on the whole toy group

### Files

- `tests/Arcanum.Tests/ToyCurveTests.fs`, or `tests/Arcanum.Tests/GroupLawTests.fs` if the cubic walk should sit in its own file
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj` if a new file is added
- `src/Arcanum.Core/EllipticCurve.fs` only if `add` fails a law in this phase. Fix `add`. Do not drop the triple.

### Object

The laws that make the enumerated set a commutative group under `EllipticCurve.add`. Identity and inverses already passed in phase 4. This phase walks the whole set. Eighteen points make `18^3 = 5832` triples. That walk is the test. Do not sample it with FsCheck. Do not shrink the prime to make the walk shorter.

### Laws

For every pair of enumerated points `Q` and `R`:

- Closure: `add toy Q R` is in the enumerated set.
- Commutativity: `add toy Q R = add toy R Q`.

For every triple of enumerated points `Q`, `R`, and `S`:

- Associativity: `add toy (add toy Q R) S = add toy Q (add toy R S)`.

If one pair or one triple fails, fix `add` or the enumeration. Do not delete the case. Do not special-case a triple inside `add` to satisfy a single assertion.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 6 — Orders and scalar multiplication

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, comment on `scalarMultiply` only, unless a law shows the reduction is wrong for a point on `toy`. Do not change the reduction on `secp256k1`.
- `tests/Arcanum.Tests/ScalarTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`, appending the new test file

### Object

Scalar multiplication is repeated addition. On `toy` the test can perform the repeated addition, because the group has 18 points.

`scalarMultiply` first replaces `k` with `k mod Curve.N`, and returns `Infinity` when that residue is zero. `scalarMultiply toy toy.N Q` is therefore `Infinity` before any doubling, for every `Q`. That call is not the order check. Do not use it as the law, and do not delete the reduction to make the call look meaningful.

The order of a point is the least positive number of repeated additions that reaches `Infinity`, or `1` for `Infinity` itself. Search by `add`. Bound the search by the 18 points.

The comment on `scalarMultiply` says: the scalar is reduced modulo `Curve.N`, and for `toy`, `secp256k1`, and `nistP256` that `N` is the order of `G`. On `toy` this is the order of the whole group, so `k * Q` and `(k mod N) * Q` are the same point for every `Q` on the curve. The comment does not say the reduction is constant-time.

Do not add a second scalar function. The agreement below is how `scalarMultiply` is tied to `add` for residues `0 .. 17`. The order of `G` is the repeated-addition walk, not `scalarMultiply` of `18`.

### Laws

- Repeated addition of `G = Point(6, 8)` reaches `Infinity` at step `18` and not earlier. `toy.N` is that count. `toy.H` is `1`.
- The set of partial sums `0*G, 1*G, ..., 17*G` is the full enumerated set. `G` generates the group.
- Repeated addition of `Point(0, 1)` reaches `Infinity` at step `3`. Every point order divides `18`.
- For every enumerated point `Q` and every scalar `k` from `0` through `17`, `scalarMultiply toy k Q` equals the point obtained by adding `Q` to `Infinity`, `k` times. `k = 0` is `Infinity`.
- `scalarMultiply toy 18 Q` is `Infinity`, and the test comment says this case is the reduction, not the order proof. The order proof is the repeated-addition bullet above.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 7 — Large curves, no discrete log

### Files

- `tests/Arcanum.Tests/Secp256k1Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`, appending the new test file
- `src/Arcanum.Core/EllipticCurve.fs` only if an unreduced walk cannot be written in the test from `add` and `double`. Prefer the test. Do not change the body of `scalarMultiply`. Do not delete `secp256k1` or `nistP256`. Do not edit `toy`.

### Object

The same laws that can be stated without listing the group, on the two parameter records already in the file. This phase does not use `ToyField`. It does not take a discrete logarithm. It does not add a key-exchange or a signature.

For each of `secp256k1` and `nistP256`, the order check is an unreduced double-and-add built in the test from `EllipticCurve.add` and `EllipticCurve.double`. Walk the bits of `N`. Do not reduce `N` modulo `N` first. `scalarMultiply curve curve.N curve.G` is the vacuous call described in phase 6. It is not this test.

A published test vector for one point addition or one scalar is allowed only when the expected coordinates are copied into the test file and a comment names the publication. The expected value is not the output of this repository's `scalarMultiply`. If the citation is not in the test, do not add the vector. Do not invent a vector and call it standard. Re-typing the hex already stored in the record is not a new vector.

### Laws

For `curve` in `secp256k1` and `nistP256`:

- `isOnCurve curve curve.G` is true.
- Unreduced double-and-add of `curve.N` times `curve.G` is `Infinity`.
- Unreduced double-and-add of `curve.N - 1` times `curve.G` equals `negate curve curve.G`.

If a cited vector is present, `add` or the unreduced walk matches the coordinates written in the test. A mismatch fixes the code or removes the uncited vector. It does not replace the expected coordinates with a fresh run of `scalarMultiply`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 8 — Docs catch-up

### Files

- `README.md`
- `Docs/agent_channel.md`, one new entry at the top for this phase. Do not rewrite the entries from phases 0 through 7.

### Object

The README lists the type that owns the field and the type that owns the curve, and how to run `dotnet test --configuration Release`. The quickstart commands from phase 1 stay.

- The coordinate field is `ToyField.Residue` at `ToyField.prime` (`17`). Inverse is `Modular.modInverse`.
- The curve type is `EllipticCurve.Curve`. The group law is `EllipticCurve.add`. Negation is `EllipticCurve.negate`. Doubling is `EllipticCurve.double`. Scalar multiplication is `EllipticCurve.scalarMultiply`, reduced modulo `Curve.N`, and on `toy` it agrees with repeated addition for residues `0 .. 17`.
- `EllipticCurve.toy` is `y^2 = x^3 + 1` over that field. It has 18 points. `G = Point(6, 8)` generates them. `N` is `18`. `H` is `1`. `Point(0, 1)` has order `3`. The curve is not a secure group.
- `EllipticCurve.secp256k1` and `EllipticCurve.nistP256` are the large parameter records. Their checks are: the generator is on the curve, unreduced `N * G` is `Infinity`, and unreduced `(N - 1) * G` is `-G`.
- CLR modules stay labeled as in phase 2. Classical modules and `Arcanum.Cli` stay labeled as historical mechanisms and demonstrations.

The README may name this file as the work order that was executed. It does not turn the parked list into a schedule.

Do not add `Docs/roadmap.md`. Do not promise a production AEAD, a constant-time `secp256k1`, a from-scratch SHA-256, a lattice, or another curve model.

Confirm `Docs/agent_channel.md` has one entry per phase from 0 through 8, newest at the top.

### Laws

No new mathematical law. The existing suite still passes. If a sentence in the README disagrees with a type in the code, correct the sentence.

### Exit

`dotnet test --configuration Release` passes. Commit. Push. Stop.

## Parked until Leon says otherwise

- A lattice, as a matrix of integers or as an encryption scheme.
- Montgomery, Edwards, and Hessian models. A ladder presented as constant-time.
- Pairings, isogenies, and rank over `Q`.
- Mordell-Weil, L-functions, and Birch-Swinnerton-Dyer.
- Schoof's algorithm, or any point count that is not the enumeration of `toy`.
- A from-scratch SHA-256, AES, or ChaCha20 used as the product.
- The Rust application that uses a library primitive.
- Moving the exploit demos into a second repository.
- Lean, or a Sage wrapper.
- A NuGet package.

## Order dependencies

Execute phases 0 through 8 in that order. Do not pull a later phase forward. Do not start phase `N + 1` before phase `N` is pushed.

- Phase 0 has no code dependency. It names the product as the elliptic-curve group.
- Phase 1 depends on phase 0. It describes the curve code already on `main` and labels the rest of the tree in the README. It does not describe `toy`.
- Phase 2 depends on phase 1. It writes the same labels into the hashing modules, the classical modules, and the CLI. It does not edit `EllipticCurve.fs`. Later phases do not edit the files phase 2 labeled.
- Phase 3 depends on `Modular.fs`. It builds the coordinate field. Inverse is `Modular.modInverse`. The Fermat check calls `Modular.modPow` and does not become the definition of inverse. There is no curve in this phase.
- Phase 4 depends on phase 3 and on `EllipticCurve.add`. The prime is `ToyField.prime`. The law is the existing `add`. `G` is `Point(6, 8)`, not the order-3 point.
- Phase 5 depends on phase 4. Associativity and commutativity walk the 18 points from that phase. They do not depend on `scalarMultiply`.
- Phase 6 depends on phase 5. Orders are repeated addition under the law that phase 5 locked. `scalarMultiply` is checked against that repeated addition for `k = 0 .. 17`. The group order is not `scalarMultiply` of `N`.
- Phase 7 depends on phase 6 only for the shared meaning of `add`, `double`, and the reduction in `scalarMultiply`. It does not use the toy field and it does not use `toy`. The order checks are unreduced walks on `secp256k1` and on `nistP256`.
- Phase 8 is last. It names the field, the toy group, and the two large curves only after those phases are on `main`.
