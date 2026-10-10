# Antigravity research program

Leon asked for this on 10 Oct 2026, after the elliptic-curve program finished. Execute these phases in order. One phase is one commit on `main`, pushed to `origin/main`, plus a new entry at the top of `Docs/agent_channel.md`. Then start the next phase. Cursor wrote this plan. Antigravity executes it.

This file is the work order that follows `Docs/antigravity_work_program.md`. That program ended at phase 8, commit `a51c83f`. Do not redo phases 0 through 8. Do not restyle that file. Number these phases 9 through 48 so the channel and the commit subjects stay distinct.

The repository is `lpr16/arcanum`. `origin` is `https://github.com/lpr16/arcanum.git`. If this file disagrees with `AGENTS.md` or the code, `AGENTS.md` and the code win. Code wins over both. Phase 9 is the one phase that edits `AGENTS.md`, and it does that before any later phase relies on the wider scope.

The center remains the group of points on a short Weierstrass curve. This program adds the rest of a cryptography study around that center: exact modular arithmetic, the binary field already used by AES, cited checks of constructions that are already written, textbook public-key arithmetic, historical mechanisms, and one integer lattice. It does not replace the curve.

## How to run

- Start from current `main`. Phase 9 commits this file if it is still untracked. Later phases do not restyle it and do not weaken a law written here.
- Read `AGENTS.md` and the top of `Docs/agent_channel.md` before each phase.
- One phase, one commit, one push. The channel entry for that phase is in the same commit. Commit message shape: `Phase N: <object>`.
- Do not amend. Do not force-push. Do not skip hooks. Do not change git config. Do not commit secrets, keys, or tokens.
- If the push fails, stop the run.
- Target framework stays `net10.0`. F# compile items stay explicit. A new file is appended in the `.fsproj` in dependency order.
- Test runner stays xUnit and FsCheck, already referenced. No new NuGet cryptography package as the implementation of a law.
- The command that must pass before every commit, from the repo root:

```
dotnet test --configuration Release
```

- If a law fails, fix that phase. Do not weaken a law to go green. Weakening includes changing a cited ciphertext, digest, point, or determinant so the current code matches, deleting a locked case, or catching the failure and returning the identity.
- A failed law is a failed test. Do not add a proof file. No emojis.
- Channel entry shape, newest at the top: date, author `Antigravity`, the phase number, what landed, and that `dotnet test --configuration Release` passed. Do not rewrite another author's entry. Do not backfill a phase 2 entry. The source already carries the phase 2 labels, and the log has no phase 2 line.

## Already finished

Phases 0, 1, 3, 4, 5, 6, 7, and 8 are on `main`. The channel records them. In particular:

- `ToyField` is `F_17`. Inverse is `Modular.modInverse`. A non-negative `Modular.modPow` is `BigInteger.ModPow`.
- `EllipticCurve.toy` is `y^2 = x^3 + 1` over that field, 18 points, `G = Point(6, 8)`, `N = 18`, `H = 1`. `Point(0, 1)` has order 3. Associativity was checked on every triple. `scalarMultiply` agrees with repeated addition for residues `0 .. 17`. `scalarMultiply` of `N` is the reduction, not an order proof.
- `secp256k1` and `nistP256` keep their parameters. Unreduced `N * G` is `Infinity` and unreduced `(N - 1) * G` is `-G`.
- `SHA256.hash` is `System.Security.Cryptography.SHA256.HashData`. HMAC and PBKDF2 are F# constructions on that call. The SHA-256("abc") digest and one RFC 4231 HMAC case are already locked. Caesar and Enigma reciprocity tests remain.

Do not delete these. Do not claim the toy curve is a secure group.

## Ground truth

- This repository is a public F# study of cryptography. The center is the short Weierstrass group. The other objects in this program are exact constructions a reader can check: residues, binary fields, one cited block of an existing cipher, textbook RSA and Diffie-Hellman, signatures on the toy group, historical letter ciphers, and a 2-by-2 integer lattice.
- A CLR call stays labeled. A round-trip stays a round-trip. A cited vector names its publication in the test and is not the output of the function under test.
- `F#` `BigInteger` is not a constant-time field. Do not add a constant-time claim unless a test or a cited construction justifies it.
- elementa, mathcore, and studium are other repositories. Do not edit, clone, or import them.
- No UI. No port. No Python in the build. No secrets. No emojis.

## Hard stops

- Do not redo phases 0 through 8.
- Do not claim a CLR hash is an implementation of SHA-256. Do not reimplement SHA-256.
- Do not claim a round-trip, or a whole suite, is a CAVP file.
- Do not delete `toy`, `secp256k1`, or `nistP256`. Do not claim `toy` or `toy2` is a secure group.
- Do not set `toy2.N` to 16. That curve is not cyclic. Its generator in this program has order 4.
- Do not treat `scalarMultiply curve curve.N P` as an order proof.
- Do not add Montgomery, Edwards, pairings, isogenies, rank over `Q`, Mordell-Weil, L-functions, Birch-Swinnerton-Dyer, or Schoof.
- Do not add an LWE sampler or lattice encryption. The lattice phase is a determinant and a Gram matrix.
- Do not extend Enigma. Do not add a rotor.
- Do not invent a published vector by running the function and pasting the output back.
- Do not add a deterministic OAEP vector. `encryptOaep` draws a random seed.
- Do not start Lean or wrap Sage. Do not add a NuGet cryptography package as the implementation.
- Do not weaken a law to go green.

## Phase 9 — Research charter

### Files

- `AGENTS.md`
- This file, if it is not already committed

### Object

No algorithm. Replace the product paragraph and the lattice sentence in `AGENTS.md` with the following, and keep every other rule:

- The repository is a cryptography research study. Its center is the group of points on a short Weierstrass curve over a prime field. The group law is `EllipticCurve.add`. Scalar multiplication is repeated addition.
- Around that center, the study may lock exact constructions already named by `Docs/antigravity_research_program.md`: modular arithmetic, binary fields, cited checks of existing symmetric code, textbook public-key arithmetic, historical letter ciphers, and one integer lattice.
- The integer lattice is a matrix of integers. It is not an encryption scheme.
- The toy curves are not secure groups. `F#` `BigInteger` is not a constant-time field.
- A CLR call is labeled in the module and in the README. `System.Security.Cryptography.SHA256.HashData` is a library call.
- A round-trip test is a round-trip, not a NIST CAVP vector.
- Classical ciphers and the CLI remain historical mechanisms and demonstrations. They do not replace the curve.
- elementa and mathcore are other repositories. Do not edit, clone, or import them.
- This file wins over `Docs/`. Code wins over both.
- Notes between Cursor and Antigravity go in `Docs/agent_channel.md`. A new entry goes at the top. Do not rewrite another author's entry.
- The repository is public. No secrets. No emojis. No new UI. No port.

Remove the sentence "Do not add a lattice." The lattice allowed by that removal is phase 33 only.

### Laws

No mathematical law. The diff does not add a `.fs` algorithm.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 10 — Bézout

### Files

- `tests/Arcanum.Tests/ModularLawsTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

Lock `Modular.xgcd` as it is written. Do not replace it.

### Laws

- `xgcd 240 46` is `(2, -9, 47)`, and `240 * -9 + 46 * 47 = 2`.
- `xgcd 15 25` is `(5, 2, -1)`.
- `xgcd 17 0` is `(17, 1, 0)`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 11 — Chinese remainder

### Files

- `tests/Arcanum.Tests/ModularLawsTests.fs`

### Object

Lock `Modular.crt`. Do not write a second remainder theorem.

### Laws

- `crt` of `(2, 3)`, `(3, 5)`, `(2, 7)` is `Some 23`.
- `23` satisfies all three congruences.
- Two congruences with even moduli and inconsistent residues return `None`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 12 — Jacobi and square roots

### Files

- `tests/Arcanum.Tests/ModularLawsTests.fs`

### Object

Lock `Modular.jacobi` and `Modular.modSqrt` on the toy prime. Do not replace Tonelli-Shanks. A root is accepted whichever sign the algorithm returns. The law is the square.

### Laws

- `jacobi 2 17` is `1`. `jacobi 3 17` is `-1`.
- `modSqrt 2 17` is `Some r` with `r * r ≡ 2 (mod 17)` and `0 <= r < 17`.
- `modSqrt 3 17` is `None`.
- `modSqrt 0 17` is `Some 0`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 13 — Trial primality

### Files

- `tests/Arcanum.Tests/PrimeTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`
- `src/Arcanum.Core/Primes.fs`, comment only if the random source is not already labeled

### Object

`isProbablePrime` decides these inputs by the small-prime trial, before any random round. The comment on the random path names `RandomNumberGenerator.Fill`. Do not describe a probable prime as a proof. Do not call `generatePrime` in the test.

### Laws

- `isProbablePrime 17 1` is true. `isProbablePrime 97 1` is true.
- `isProbablePrime 1 1` is false. `isProbablePrime 4 1` is false. `isProbablePrime 91 1` is false. `91 = 7 * 13`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 14 — GF(2^8)

### Files

- `tests/Arcanum.Tests/Gf256Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

Lock `FiniteFields.GF256`. The polynomial is `0x11B`. Do not write a second field.

### Laws

Cite FIPS 197 section 4.2 for the product. The expected byte is the literal `0xc1`, not a value computed by a second copy of `multiply` in the test.

- `multiply 0x57uy 0x83uy` is `0xc1uy`.
- `add` is XOR: `add 0x57uy 0x83uy` is `0xd4uy`.
- `inverse 0uy` is `0uy`, the convention already written in the module.
- `inverse 0x02uy`, multiplied by `0x02uy`, is `0x01uy`.
- `multiply (inverse 0x53uy) 0x53uy` is `0x01uy`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 15 — AES S-box

### Files

- `tests/Arcanum.Tests/Gf256Tests.fs`

### Object

Lock `GF256.sboxSub` and `generateAesInvSBox` against two FIPS 197 S-box entries. Do not paste a 256-byte table generated by calling `sboxSub`.

### Laws

- `sboxSub 0x00uy` is `0x63uy`.
- `sboxSub 0x01uy` is `0x7cuy`.
- The inverse box sends `0x63` back to `0x00` and `0x7c` back to `0x01`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 16 — AES-128 block

### Files

- `tests/Arcanum.Tests/AesBlockTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`
- `src/Arcanum.Symmetric/Aes.fs` only if this cited block fails

### Object

One FIPS 197 appendix C.1 block, through `Aes.expandKey128` and `Aes.encryptBlock`. The comment names FIPS 197 appendix C.1. This is one block, not a CAVP file.

Key `000102030405060708090a0b0c0d0e0f`. Plaintext `00112233445566778899aabbccddeeff`. Ciphertext `69c4e0d86a7b0430d8cdb78070b4c55a`.

### Laws

- `encryptBlock` of that plaintext under that key is that ciphertext.
- `decryptBlock` of that ciphertext returns the plaintext.

If either fails, fix `Aes.fs`. Do not edit the expected ciphertext.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 17 — PKCS#7

### Files

- `tests/Arcanum.Tests/PaddingTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

Lock `Padding.padPkcs7` and `unpadPkcs7` by the padding bytes. Do not add a constant-time comment.

### Laws

- Three bytes `01 02 03` padded to block size 8 end in five bytes of `0x05`.
- A full block of 16 bytes padded to block size 16 grows by 16 bytes of `0x10`.
- `unpadPkcs7` of a block whose last byte is `0x00` is an error.
- `unpadPkcs7` of the first padded buffer returns the three original bytes.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 18 — ChaCha20 block

### Files

- `tests/Arcanum.Tests/ChaChaTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`
- `src/Arcanum.Symmetric/ChaCha20.fs` only if this cited block fails

### Object

RFC 8439 section 2.3.2, through `ChaCha20.block`. The comment names that section.

Key is the 32 bytes `00 01 02 ... 1f`. Nonce is `00 00 00 09 00 00 00 4a 00 00 00 00`. Counter is `1`.

### Laws

The first 16 bytes of the block are `10f1e7e4d13b5915500fdd1fa32071c4`.

If this fails, fix `ChaCha20.fs`. Do not edit the expected bytes. Do not describe the existing AEAD round-trip as this vector.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 19 — Poly1305 structure

### Files

- `tests/Arcanum.Tests/Poly1305Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

Lock the clamp and the empty message. Do not call this an RFC 8439 sample vector. The empty message has no blocks, so the tag is the `s` half of the key.

### Laws

- `clampR` of 16 bytes of `0xff` has `r[3] = 0x0f`, `r[4] = 0xfc`, `r[7] = 0x0f`, `r[15] = 0x0f`.
- For a 32-byte key whose last 16 bytes are `00 01 02 ... 0f` and whose first 16 bytes are anything of that length, `mac` of the empty message equals those last 16 bytes.

Do not add a constant-time sentence on `verify`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 20 — HMAC, second case

### Files

- `tests/Arcanum.Tests/Tests.fs`, or `tests/Arcanum.Tests/HmacTests.fs` if the fact moves

### Object

Add RFC 4231 test case 2 beside the existing case 1. The hash step remains `SHA256.hash`. Do not reimplement SHA-256.

Key is the ASCII bytes of `Jefe`. Data is the ASCII bytes of `what do ya want for nothing?`.

### Laws

The tag is `5bdcc146bf60754e6a042426089575c75a003f089d2739839dec58b964ec3843`.

The comment names RFC 4231 test case 2. Case 1 stays. If this fails, fix `HMAC.fs`. Do not edit the expected tag.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 21 — PBKDF2, one iteration

### Files

- `tests/Arcanum.Tests/Pbkdf2Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`
- `src/Arcanum.Hashing/PBKDF2.fs` only if this vector fails

### Object

One published PBKDF2-HMAC-SHA256 answer. Password `password`, salt `salt`, `c = 1`, length 32. The PRF is still `HMAC.hmacSha256`, whose hash is the CLR call. The comment says that. This is not a from-scratch SHA-256.

### Laws

The derived key is `120fb6cffcf8b32c43e7225256c4f837a86548c92ccc35480805987cb70be17b`.

If this fails, fix `PBKDF2.fs`. Do not edit the expected key. Do not raise the iteration count in order to avoid the failure.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 22 — Textbook RSA

### Files

- `tests/Arcanum.Tests/RsaTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

A key the reader can check. Do not call `generateKeyPair`. Build the records in the test.

`p = 61`, `q = 53`, `n = 3233`, `e = 17`, `d = 2753`. This key is not a secure modulus. The comment says so.

### Laws

- `encryptRaw` of message `65` is `2790`.
- `2790^2753 ≡ 65 (mod 3233)`, computed with `Modular.modPow`.
- `encryptRaw` refuses a message greater than or equal to `n`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 23 — RSA decryption by the remainder theorem

### Files

- `tests/Arcanum.Tests/RsaTests.fs`

### Object

`decryptCrt` on the phase 22 key. `dp = 53`, `dq = 49`, `qInv = 38`.

### Laws

- `decryptCrt` of ciphertext `2790` is `65`.
- That result equals `Modular.modPow 2790 2753 3233`.

Do not add an OAEP known-answer test.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 24 — Toy Diffie-Hellman

### Files

- `tests/Arcanum.Tests/DhTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

A modulus the reader can check, through `DiffieHellman.computeSharedSecret`. Do not use `rfc3526Group5` as the place where the shared secret is computed. Do not claim this group is RFC 3526.

`p = 23`, `g = 5`, private `6` and `15`.

### Laws

- `5^6 ≡ 8 (mod 23)` and `5^15 ≡ 19 (mod 23)`, via `Modular.modPow`.
- `computeSharedSecret` of private `6` and public `19` is `2`.
- `computeSharedSecret` of private `15` and public `8` is `2`.
- `2` equals `5^(6*15) mod 23`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 25 — ECDH on the toy curve

### Files

- `tests/Arcanum.Tests/ToyEcdhTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`DiffieHellman.computeEcdhSharedSecret` on `EllipticCurve.toy`. Privates `3` and `5`, both smaller than `N`. The comment says `toy` is not a secure group. Do not generate random keys in this test.

### Laws

- The two shared points are equal.
- That point equals `scalarMultiply toy (3 * 5) toy.G`.
- It also equals repeated addition of `G`, fifteen times. Fifteen is `3 * 5`, which is not `N`, so this is not the vacuous reduction.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 26 — ECDSA on the toy curve

### Files

- `tests/Arcanum.Tests/ToyEcdsaTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`Ecdsa.signWithNonce` and `Ecdsa.verify` on `toy`. Private scalar `5`. Nonce `7`. Message is the ASCII bytes of `ec`. The public point is repeated addition of `G`, five times.

`r` is the x coordinate of repeated addition of `G`, seven times, reduced modulo `18`. The test computes that point by `add`, then compares it with the signature. It does not paste a fresh `scalarMultiply` run and call the paste a standard vector.

The message representative `z` is the existing `SHA256.hash` path inside `signWithNonce`. The comment says the hash is the CLR call. This is not RFC 6979. `toy` is not a secure group.

### Laws

- The signature's `R` equals that x coordinate modulo `18`.
- `S` equals `k^{-1} (z + R * d) mod 18`, with `k = 7` and `d = 5`, inverse from `Modular.modInverse`.
- `verify` of the same message is true.
- `verify` of the ASCII bytes of `ex` is false.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 27 — Repeated nonce on the toy curve

### Files

- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

Two signatures on `toy` with the same nonce expose the private scalar. Messages are the ASCII bytes of `ec` and `ex`. Private scalar `5`. Nonce `7` for both. Use `signWithNonce`. Recover `d` in the test from the two `(r, s)` pairs and the two SHA-256 representatives. Do not add a recovery routine to `Ecdsa.fs`. Do not run this against `secp256k1`. The comment says the group has 18 points and is not a place to keep a key.

### Laws

The recovered scalar is `5`. A third signature with nonce `11` is not used. If the two `r` values differ, fix the phase; they must share `k`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 28 — Caesar and Atbash, one letter at a time

### Files

- `tests/Arcanum.Tests/ClassicalLawsTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

Hand checks. The existing round-trip facts stay. Do not extend Enigma.

### Laws

- `caesarEncrypt 13 "A"` is `"N"`. `caesarEncrypt 13 "HELLO"` is `"URYYB"`.
- `atbash "A"` is `"Z"`. `atbash "Z"` is `"A"`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 29 — Affine cipher

### Files

- `tests/Arcanum.Tests/ClassicalLawsTests.fs`

### Object

`Substitution.affineEncrypt` and `affineDecrypt`. The multiplier must be coprime to 26. That check already calls `Modular.xgcd`.

### Laws

- `affineEncrypt 5 8 "A"` is `Ok "I"`, because `(5 * 0 + 8) mod 26 = 8`.
- Decrypting that result with the same coefficients returns `"A"`.
- `affineEncrypt 2 1 "A"` is an error. `2` is not coprime to 26.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 30 — Vigenère

### Files

- `tests/Arcanum.Tests/ClassicalLawsTests.fs`

### Object

One tableau the reader can add by hand.

### Laws

- `vigenereEncrypt "KEY" "HELLO"` is `Ok "RIJVS"`.
- Decrypting `"RIJVS"` with `"KEY"` returns `"HELLO"`.
- `vigenereEncrypt "123" "HELLO"` is an error. The key has no letter.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 31 — Playfair square

### Files

- `tests/Arcanum.Tests/ClassicalLawsTests.fs`

### Object

The square for key `MONARCHY`, which merges `J` into `I`. Do not add a second alphabet.

### Laws

- Row 0 of the grid is `M O N A R`.
- `playfairEncrypt "MONARCHY" "MO"` is `"ON"`. Same row, each letter steps right.
- `playfairDecrypt "MONARCHY" "ON"` is `"MO"`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 32 — Index of coincidence

### Files

- `tests/Arcanum.Tests/ClassicalLawsTests.fs`

### Object

`Cryptanalysis.indexOfCoincidence` and `breakCaesar`. Exact fractions, not a rounded English constant. Do not lock a chi-squared float.

### Laws

- `indexOfCoincidence "AAAA"` is `1`.
- `indexOfCoincidence "ABCD"` is `0`.
- `indexOfCoincidence "A"` is `0`. The function returns `0` when fewer than two letters are present.
- Let `cipher` be `caesarEncrypt 3` of `THEQUICKBROWNFOXJUMPSOVERTHELAZYDOG`. `breakCaesar cipher` returns shift `3` and that plaintext.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 33 — Integer lattice

### Files

- `src/Arcanum.Core/Lattice.fs`
- `src/Arcanum.Core/Arcanum.Core.fsproj`
- `tests/Arcanum.Tests/LatticeTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

One basis. Rows are `(2, 1)` and `(1, 2)`. Entries are `BigInteger`. No `float`.

`determinant` of this 2-by-2 basis is `ad - bc`. `gram` entry `(i, j)` is the integer dot product of row `i` and row `j`. The comment says so.

No sampler. No encryption. No LLL. This file does not reference `EllipticCurve`.

### Laws

- The determinant is `3`.
- The Gram matrix is `[[5; 4]; [4; 5]]`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 34 — Negation is a homomorphism

### Files

- `tests/Arcanum.Tests/ToyHomomorphismTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

On `toy`, negation respects `add`. Walk every pair. Eighteen points.

### Laws

For every enumerated pair `Q`, `R`:

`negate (add Q R) = add (negate Q) (negate R)`.

`negate (negate Q) = Q`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 35 — Scalar distributivity

### Files

- `tests/Arcanum.Tests/ToyHomomorphismTests.fs`

### Object

On `toy`, `scalarMultiply` distributes over `add` for every residue `k` in `0 .. 17` and every pair of points. Also `scalarMultiply (k + m)` equals `add` of the two scalar multiples, for `k` and `m` in `0 .. 17`, on every point. `k + m` may exceed `17`. The reduction modulo `N` is then part of the law, and the comment says so.

Do not sample. If one pair fails, fix `scalarMultiply` or `add`.

### Laws

- `scalarMultiply k (add Q R) = add (scalarMultiply k Q) (scalarMultiply k R)`.
- `scalarMultiply (k + m) Q = add (scalarMultiply k Q) (scalarMultiply m Q)`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 36 — The order-3 subgroup

### Files

- `tests/Arcanum.Tests/ToyHomomorphismTests.fs`

### Object

The cyclic subgroup generated by `Point(0, 1)` on `toy`.

### Laws

The set `{Infinity; Point(0, 1); Point(0, 16)}` is closed under `add`. It has three points. It is not the full group of 18. `Point(6, 8)` is outside it.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 37 — j-invariant of the toy curve

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `jInvariant`
- `tests/Arcanum.Tests/ToyCurveTests.fs`

### Object

`jInvariant : Curve -> BigInteger option`. For `y^2 = x^3 + a x + b` the value is `1728 * (4a)^3 * inverse(Delta)` modulo `P`, where `Delta = -16 * (4 a^3 + 27 b^2)`. A zero `Delta` returns `None`. Use `Modular.modInverse` and `Modular.modPos`. Do not use a floating-point division.

### Laws

- `jInvariant toy` is `Some 0`, because `A` is `0` and `Delta` is not `0`.
- A curve with `A = 0`, `B = 0`, and `P = 17` returns `None`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 38 — A second enumerable curve

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `toy2`
- `tests/Arcanum.Tests/Toy2Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`EllipticCurve.toy2` is a `Curve`:

- `P = 17`, `A = 1`, `B = 0`, so `y^2 = x^3 + x`
- `G = Point(1, 6)`
- `N = 4`
- `H = 4`

The comment says the group is not cyclic, `G` does not generate it, and the curve is not a secure group. Do not pass `toy2` to a random ECDSA key generator in this phase. Do not change `toy`.

`4 A^3 + 27 B^2` is `4`, which is not `0` modulo `17`.

### Laws

- Enumeration by `isOnCurve` finds 16 points, `Infinity` included.
- Repeated addition of `G` reaches `Infinity` at step `4` and not earlier.
- Repeated addition of `Point(0, 0)` reaches `Infinity` at step `2`.
- The subgroup of `G` has 4 points, not 16.
- `add toy2 (Point(1, 6)) (Point(6, 1))` is `Point(11, 4)`. Slope `(1 - 6) * (6 - 1)^(-1) ≡ 16`, then `x ≡ 11` and `y ≡ 4`.
- `jInvariant toy2` is `Some 6`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 39 — Group law on toy2

### Files

- `tests/Arcanum.Tests/Toy2Tests.fs`
- `src/Arcanum.Core/EllipticCurve.fs` only if `add` fails a triple

### Object

The same group laws as phase 5, on the 16 points of `toy2`. Walk every pair and every triple. Do not sample. Do not shrink the curve.

### Laws

Closure, commutativity, and associativity under `EllipticCurve.add`, for the whole enumerated set.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 40 — Scalars on toy2

### Files

- `tests/Arcanum.Tests/Toy2Tests.fs`

### Object

`scalarMultiply` reduces modulo `toy2.N`, and that `N` is `4`. Every point order on this curve divides `4`, so the reduction matches repeated addition for every point. `scalarMultiply toy2 4 Q` is `Infinity` before any addition. That call is not the order proof. The order proof remains the repeated addition in phase 38.

### Laws

For every enumerated point `Q` and every `k` in `0 .. 3`, `scalarMultiply toy2 k Q` equals adding `Q` to `Infinity`, `k` times.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 41 — Point compression on the toy curve

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `compress` and `decompress`
- `tests/Arcanum.Tests/ToyCurveTests.fs`

### Object

`compress` records `Infinity`, or the x coordinate and the least bit of `y` in `0 .. P - 1`. `decompress` recovers the point with `Modular.modSqrt` of `x^3 + a x + b`. If that square root is missing, return `None`. Choose the root whose least bit matches. Do not use a floating-point square root.

### Laws

On `toy`:

- `compress (Point(6, 8))` records `x = 6` and least bit `0`. `8` is even.
- `decompress` of that record is `Point(6, 8)`.
- The other bit at the same `x` is `Point(6, 9)`.
- `compress Infinity` decompresses to `Infinity`.
- `decompress` of `x = 3` returns `None`. `3` is not a square coordinate on `toy`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 42 — GF(2^128) structure

### Files

- `tests/Arcanum.Tests/Gf128Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

Lock `FiniteFields.GF128.multiply` on blocks of 16 bytes. Do not claim a GHASH CAVP vector. Do not add AES-GCM.

### Laws

- `multiply` of any 16-byte block with 16 zero bytes is 16 zero bytes, on both sides.
- `multiply x y` equals `multiply y x` for `x` a block whose first byte is `0x80` and whose other bytes are `0`, and `y` a block whose first byte is `0x02` and whose other bytes are `0`.
- A block that is not 16 bytes is rejected.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 43 — Secret sharing over the toy field

### Files

- `src/Arcanum.Core/Shamir.fs`
- `src/Arcanum.Core/Arcanum.Core.fsproj`
- `tests/Arcanum.Tests/ShamirTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

Threshold-2 sharing in `ToyField`. A share is an x coordinate and a y coordinate, both `Residue`. The polynomial is `secret + coefficient * x`. Reconstruction is the Lagrange line through two shares, using `ToyField.inverse`.

Refuse a share whose x is zero. Refuse two shares with the same x. Do not use `float`.

### Laws

- Secret `5`, coefficient `3`. The share at `1` is `8`. The share at `2` is `11`. The share at `3` is `14`.
- Any two of those three reconstruct `5`.
- A share at `0` is refused.
- Two shares at `1` are refused.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 44 — Doubling the secp256k1 generator

### Files

- `tests/Arcanum.Tests/Secp256k1Tests.fs`

### Object

One point double, checked by `EllipticCurve.double` on `secp256k1.G`. The expected coordinates are literals. They are not the output of a previous run of `scalarMultiply`. The comment says they are the double of the SEC 2 generator.

### Laws

`double secp256k1 secp256k1.G` is the point with

- `x = c6047f9441ed7d6d3045406e95c07cd85c778e4b8cef3ca7abac09b95c709ee5`
- `y = 1ae168fea63dc339a3c58419466ceaeef7f632653266d0e1236431a950cfe52a`

`isOnCurve` of that point is true. If the coordinates disagree, fix `add`. Do not edit the literals.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 45 — ECB carries the cited block

### Files

- `tests/Arcanum.Tests/AesBlockTests.fs`

### Object

`Modes.encryptEcb` pads with PKCS#7, including when the plaintext is already 16 bytes. The first block is the phase 16 ciphertext. The function's extra block is padding, not a second FIPS vector. The comment says that.

### Laws

Using the phase 16 key and plaintext:

- The first 16 bytes of `encryptEcb` equal `69c4e0d86a7b0430d8cdb78070b4c55a`.
- The ciphertext is 32 bytes long.
- `decryptEcb` returns the original 16-byte plaintext.

Do not describe this test as a CAVP file.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 46 — RFC 3526 parameter record

### Files

- `tests/Arcanum.Tests/DhTests.fs`

### Object

Lock the shape of `DiffieHellman.rfc3526Group5`. Do not exponentiate a private key in this phase. The record is a published prime and generator, stored as integers.

### Laws

- `G` is `2`.
- `P` is odd.
- The hexadecimal of `P`, without a leading zero nibble the parser may have required, begins with `FFFFFFFFFFFFFFFFC90FDAA2` and ends with `FFFFFFFFFFFFFFFF`.
- `P` is greater than `2^1535`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 47 — MGF1 counter

### Files

- `tests/Arcanum.Tests/RsaTests.fs`

### Object

`Rsa.mgf1` with an empty seed and length 32 is one counter block. The counter bytes are the big-endian integer `0`, four bytes. The digest is `SHA256.hash` of those four bytes. The test builds the expected mask by calling `SHA256.hash`. That expected value is the library call, and the comment says so. It is not a claim that this repository implements SHA-256.

Do not call `encryptOaep`.

### Laws

`mgf1` of an empty seed and length 32 equals `SHA256.hash` of `00 00 00 00`.

`mgf1` of an empty seed and length 0 is refused or returns an empty array if the current function already does one of those. Match the function. Do not change a thrown exception into a successful mask.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 48 — Docs catch-up

### Files

- `README.md`
- `Docs/agent_channel.md`, one new entry at the top for this phase. Do not rewrite entries from earlier phases.

### Object

The README says the repository is a cryptography research study and that its center is the short Weierstrass group. It keeps the quickstart commands. It names:

- `ToyField.Residue` and `EllipticCurve.toy`, as phase 8 already required.
- `EllipticCurve.toy2`, the non-cyclic curve of 16 points, `N = 4`, not a secure group.
- `jInvariant`, `compress`, and `decompress`.
- `Lattice`, the 2-by-2 basis whose determinant is `3`.
- `Shamir` over `ToyField`.
- The CLR boundary: `SHA256`, and HMAC and PBKDF2 as constructions on that call.
- The cited checks added here: FIPS 197 appendix C.1, RFC 8439 section 2.3.2, RFC 4231 case 2, the one-iteration PBKDF2 answer, and the secp256k1 double. Each is one vector, not a CAVP campaign.
- Classical modules and the CLI as historical mechanisms and demonstrations.

Do not add `Docs/roadmap.md`. Do not promise a production AEAD, a constant-time `secp256k1`, a from-scratch SHA-256, lattice encryption, pairings, or another curve model.

Confirm the channel has one entry for each phase from 9 through 48, and that the phase 0 through 8 entries are still present and unedited. The absent phase 2 line stays absent.

### Laws

No new mathematical law. If a sentence disagrees with a type in the code, correct the sentence.

### Exit

`dotnet test --configuration Release` passes. Commit. Push. Stop.

## Parked until Leon says otherwise

- A from-scratch SHA-256 used as an implementation in this repository.
- AES-GCM, and any GHASH known-answer file beyond the structural checks in phase 42.
- RSA-OAEP known answers. The encryption seed is random.
- RFC 6979.
- Montgomery and Edwards forms. Pairings, isogenies, and rank over `Q`.
- Mordell-Weil, L-functions, Birch-Swinnerton-Dyer, and Schoof.
- LWE, lattice reduction, and lattice encryption.
- A new Enigma rotor or a historical ciphertext campaign.
- Constant-time `secp256k1`.
- The Rust application. Moving the CLI demonstrations into a second repository.
- Lean, or a Sage wrapper.
- A NuGet package.

## Order dependencies

Execute phases 9 through 48 in that order. Do not start phase `N + 1` before phase `N` is pushed. Do not return to phases 0 through 8.

- Phase 9 edits `AGENTS.md` so the wider study is allowed. No later phase may add a lattice encryption scheme on the strength of that edit.
- Phases 10 through 13 depend on `Modular.fs` and `Primes.fs`, which are already on `main`. They add tests. They do not replace `xgcd` or Tonelli-Shanks.
- Phases 14 and 15 depend on `FiniteFields.GF256`. Phase 16 depends on phase 15 only in the sense that the S-box is already the one AES uses. The block law is FIPS 197, not the S-box unit test.
- Phase 17 is the padding AES modes already call. It does not depend on the field.
- Phases 18 and 19 lock `ChaCha20.block` and `Poly1305.mac` as they are. They do not depend on the curve.
- Phases 20 and 21 depend on the CLR hash remaining the hash step. They add cited answers. They do not reimplement SHA-256.
- Phases 22 and 23 are the textbook key. Phase 23's CRT decryption must match phase 22's `modPow`.
- Phase 24 is the 23-element multiplicative group. It does not use `toy`.
- Phases 25 through 27 depend on `toy` from phase 4 and on `scalarMultiply` from phase 6. The nonce-reuse law stays on `toy`.
- Phases 28 through 32 are the historical mechanisms. They do not depend on the curve, and they do not extend Enigma.
- Phase 33 is the lattice. It depends on phase 9 for the charter and on nothing in `EllipticCurve`.
- Phases 34 through 36 walk `toy` again. They depend on phases 5 and 6.
- Phase 37 adds `jInvariant` and uses `Modular.modInverse`.
- Phase 38 adds `toy2` and uses `jInvariant`. Phase 39 walks that group. Phase 40 checks scalars modulo `4`, not modulo `16`.
- Phase 41 depends on phase 12 for `modSqrt` and on `toy` for the point `(6, 8)`.
- Phase 42 is `GF128`. It does not depend on AES-GCM, and it does not add AES-GCM.
- Phase 43 depends on `ToyField` from phase 3.
- Phase 44 depends on `EllipticCurve.double` and on the `secp256k1` parameters. It does not depend on `toy2`.
- Phase 45 depends on phases 16 and 17.
- Phase 46 depends on the parameter record already in `DiffieHellman.fs`. It does not depend on phase 24's shared secret.
- Phase 47 depends on `SHA256.hash` remaining the MGF1 digest. It does not call OAEP.
- Phase 48 is last.
