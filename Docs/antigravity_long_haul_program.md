# Antigravity long-haul program

Leon asked for this on 31 Oct 2026, after the research program finished at phase 48. Claude Code wrote this plan. Antigravity executes it. Execute these phases in order. One phase is one commit on `main`, pushed to `origin/main`, plus a new entry at the top of `Docs/agent_channel.md`. Then start the next phase.

This file is the work order that follows `Docs/antigravity_research_program.md`. That program ended at phase 48, commit `b048fd5`. Do not redo phases 0 through 48. Number these phases 49 through 79.

The repository is `lpr16/arcanum`. `origin` is `git@github.com:lpr16/arcanum.git`. If this file disagrees with `AGENTS.md` or the code, `AGENTS.md` and the code win. Code wins over both. Phase 49 is the one phase that edits `AGENTS.md`, and it does that before any later phase relies on the wider scope.

## Goal

The curve code is exhaustively self-consistent on small curves. It does not yet meet a published signature or key-agreement vector on a large curve, and its ECDSA message representative reads the digest in the wrong byte order, so it could not. This program ends with NIST P-256 signatures that match RFC 6979 and a P-256 shared point that matches RFC 5903.

On the way, it repairs known defects, adds a prime-order toy curve so toy ECDSA is well-defined, adds SEC 1 point encoding and public-key validation, checks the group law a second way through Jacobian coordinates and a ladder, computes discrete logarithms on the toy curves to show why prime order matters, and counts points by Legendre symbols.

The center does not move. It is still the group of points on a short Weierstrass curve, with `EllipticCurve.add` as the group law.

## How to run

- Start from current `main`. Phase 49 commits this file if it is still untracked. Later phases do not restyle it and do not weaken a law written here.
- Read `AGENTS.md` and the top of `Docs/agent_channel.md` before each phase.
- One phase, one commit, one push. The channel entry for that phase is in the same commit. Commit message shape: `Phase N: <object>`.
- Do not amend. Do not force-push. Do not skip hooks. Do not change git config. Do not commit secrets, keys, or tokens.
- If the push fails, stop the run.
- Target framework stays `net10.0`. F# compile items stay explicit. A new file is appended at the end of its `.fsproj`. That end is always a valid dependency position for the files in this program.
- Test runner stays xUnit and FsCheck, already referenced. No new NuGet package.
- The command that must pass before every commit, from the repo root:

```
dotnet test --configuration Release
```

- If a law fails, fix the code in that phase. Do not weaken a law to go green. Weakening includes changing a cited vector, point, count, or residue so the current code matches, deleting a locked case, or catching the failure and returning the identity. Phase 55 is the one phase allowed to replace locked cases, and it says exactly which ones.
- A failed law is a failed test. Do not add a proof file. No emojis.
- Channel entry shape, newest at the top: date, author `Antigravity`, the phase number, what landed, and that `dotnet test --configuration Release` passed. Do not rewrite another author's entry.

## Already finished

Phases 0 through 48 are on `main`. The channel records them. In particular:

- `ToyField` is `F_17`. `EllipticCurve.toy` is `y^2 = x^3 + 1` over `F_17`, 18 points, `G = (6, 8)`, `N = 18`, `H = 1`. `EllipticCurve.toy2` is `y^2 = x^3 + x` over `F_17`, 16 points, not cyclic, `G = (1, 6)` of order 4, `N = 4`, `H = 4`.
- `jInvariant` uses the standard `-1728 * (4a)^3 / Delta`. `jInvariant toy2` is `Some 11`.
- `compress` and `decompress` exist. `Modular.modSqrt` is Tonelli-Shanks with the `p = 3 mod 4` fast path.
- `secp256k1` and `nistP256` keep their SEC 2 and FIPS 186 parameters. `double secp256k1 G` is locked against literal coordinates.
- `DiffieHellman.rfc3526Group5` holds the RFC 3526 section 2 prime.
- `SHA256.hash` is `System.Security.Cryptography.SHA256.HashData`. HMAC and PBKDF2 are F# constructions on that call.
- The toy ECDSA tests (phases 26 and 27) run on `toy`, whose order 18 is not prime. `Ecdsa.verify` carries a brute-force branch for a non-invertible `S` that exists only because of that.

Do not delete `toy`, `toy2`, `secp256k1`, or `nistP256`. Do not change their parameters. Do not claim any toy curve is a secure group.

## Ground truth

- The repository is a public F# study of cryptography. The center is the short Weierstrass group.
- A CLR call stays labeled. A round-trip stays a round-trip. A cited vector names its publication in the test and is not the output of the function under test.
- F# `BigInteger` is not a constant-time field. The Jacobian formulas and the ladder in this program carry no constant-time claim.
- Discrete logarithms are computed only on `toy`, `toy2`, and `toy3`. No check on `secp256k1` or `nistP256` takes a discrete logarithm or enumerates the group.
- The ECDSA message representative follows SEC 1 section 4.1.3: the leftmost `bitLength N` bits of the SHA-256 digest, read big-endian, then reduced modulo `N`. For every curve in this repository `N` is not a power of two, so this is the same as `ceil(log2 N)` bits and the same as `bits2int` in RFC 6979 section 2.3.2.
- RFC 6979 is implemented with `HMAC.hmacSha256`, which is an F# construction on the CLR SHA-256 call.
- elementa and mathcore are other repositories. Do not edit, clone, or import them.
- No UI. No port. No Python in the build. No secrets. No emojis.

## Provenance of expected values

Every expected value in this file was checked before it was written down. Do not edit any of them to match the code. If a law fails, the code is wrong.

- RFC 6979 values are copied from RFC 6979 section A.2.5 (NIST P-256, SHA-256) and section A.1.2 (`h1`).
- RFC 5903 values are copied from RFC 5903 section 8.1.
- The RFC 7914 value is copied from RFC 7914 section 11.
- Toy-curve values are derived by hand in the law text, and were checked by an independent implementation that shares no code with this repository.

## Hard stops

- Do not redo phases 0 through 48.
- Do not take a discrete logarithm on `secp256k1` or `nistP256`. Do not enumerate their groups. Do not call `countPoints` on them.
- Do not claim constant time for anything in this program.
- Do not add Montgomery-form curves, Edwards curves, pairings, MOV, isogenies, rank over `Q`, Mordell-Weil, L-functions, Birch-Swinnerton-Dyer, or Schoof. The ladder in phase 64 runs on the short Weierstrass curve; do not call it a Montgomery curve.
- Do not add an integer factoring routine. Pohlig-Hellman takes the factorization of the order as an input.
- Do not reimplement SHA-256. It is still parked.
- Do not add a deterministic RSA-OAEP vector. `encryptOaep` draws a random seed.
- Do not invent a published vector by running the function and pasting the output back.
- Do not low-s normalize ECDSA signatures. RFC 6979 does not, and its vectors would fail.
- Do not edit `Docs/antigravity_research_program.md` or `Docs/antigravity_work_program.md`, except the two lines phase 50 names.
- Do not weaken a law to go green.

## Phase 49 — Charter

### Files

- `AGENTS.md`
- This file, if it is not already committed
- `Docs/agent_channel.md`, including the Claude Code entry that announces this program if it is not already committed

### Object

No algorithm. In `AGENTS.md`, replace the bullet that begins "A small curve is enumerated completely" with these three bullets, and keep every other rule:

- The small curves `toy`, `toy2`, and `toy3` are enumerated completely, and discrete logarithms may be computed on them.
- The `secp256k1` and NIST P-256 parameter records remain available. Checks on those curves do not take a discrete logarithm or enumerate the group. They may compare against published vectors: RFC 6979 section A.2.5 and RFC 5903 section 8.1.
- The ECDSA message representative follows SEC 1: the leftmost bits of the digest, read big-endian. RFC 6979 is built on `HMAC.hmacSha256`, an F# construction on the CLR SHA-256 call.

### Laws

No mathematical law. The diff does not add a `.fs` algorithm.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 50 — Corrections and the RFC 7914 vector

### Files

- `Docs/antigravity_research_program.md`, two lines only
- `tests/Arcanum.Tests/Pbkdf2Tests.fs`

### Object

Correct two sentences in the research program, so that it matches the code that phases 37 and 38 shipped:

- In phase 37, the formula `1728 * (4a)^3 * inverse(Delta)` becomes `-1728 * (4a)^3 * inverse(Delta)`.
- In phase 38, the law `` `jInvariant toy2` is `Some 6`. `` becomes `` `jInvariant toy2` is `Some 11`, which is 1728 modulo 17. ``

In `Pbkdf2Tests.fs`, the comment on the existing test cites RFC 6070 and RFC 7914. RFC 6070 is PBKDF2-HMAC-SHA1. RFC 7914 uses password `passwd`, not `password`. Replace the citation with a sentence that says the value is the PBKDF2-HMAC-SHA256 answer for password `password`, salt `salt`, `c = 1`, `dkLen = 32`, and that it is not taken from RFC 6070 or RFC 7914. Do not change the expected hex.

Add one cited vector from RFC 7914 section 11.

### Laws

- `PBKDF2.deriveKey` of password `passwd`, salt `salt`, `c = 1`, `dkLen = 64` is:

```
55ac046e56e3089fec1691c22544b605f94185216dde0465e68b9d57c20dacbc
49ca9cccf179b645991664b39d77ef317c71b845b1e30bd509112041d3a19783
```

The test names RFC 7914 section 11. It says this is one vector, not a CAVP file. Do not add the `c = 80000` vector.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 51 — Coordinates in range

### Files

- `src/Arcanum.Core/EllipticCurve.fs`
- `tests/Arcanum.Tests/ToyCurveTests.fs`

### Object

`isOnCurve` accepts a point whose coordinates are not reduced. `Point(23, 8)` satisfies the toy equation modulo 17, because `23 = 6` and `(6, 8)` is on `toy`, but `Point(23, 8)` and `Point(6, 8)` are different values of `ECPoint`. Make `isOnCurve` return false unless `0 <= x < P` and `0 <= y < P`. `Infinity` stays on the curve.

### Laws

- `isOnCurve toy (Point(23, 8))` is false.
- `isOnCurve toy (Point(6, 8))` is true.
- `isOnCurve toy (Point(6, -9))` is false.
- The 18-point enumeration of `toy` and the 16-point enumeration of `toy2` are unchanged.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 52 — RSA-OAEP length, byte order, and decryption

### Files

- `src/Arcanum.Asymmetric/Rsa.fs`
- `tests/Arcanum.Tests/RsaTests.fs`

### Object

`encryptOaep` claims RFC 8017 but takes `k` as `N.ToByteArray().Length`. That length counts a sign byte, so a 2048-bit modulus gives `k = 257` instead of `256`. The encoded message can then exceed `N`, and `encryptRaw` throws. The ciphertext is also written little-endian.

- Add `Rsa.modulusLength : BigInteger -> int`, the byte length of `N`: `(bitLength N + 7) / 8`.
- `encryptOaep` uses `modulusLength` for `k`, and writes the ciphertext as RFC 8017 I2OSP: big-endian, exactly `k` bytes.
- Add `Rsa.decryptOaep : RsaPrivateKey -> byte[] -> byte[] -> Result<byte[], CryptoError>` (private key, label, ciphertext), RFC 8017 section 7.1.2, using `decryptCrt`. A ciphertext that is not `k` bytes, a leading byte that is not `0`, a label hash that does not match, or a missing `0x01` separator returns an error.

These are round-trips, not known answers. The seed is random.

### Laws

- `modulusLength (2I ** 2047 + 1I)` is `256`. `modulusLength 3233I` is `2`.
- For a key from `Rsa.generateKeyPair 1024`, with an empty label: messages of length `0`, `1`, and `k - 66` decrypt to themselves, and each ciphertext is `k` bytes.
- A message of length `k - 65` is refused.
- Flipping one bit of a ciphertext makes `decryptOaep` return an error. Decrypting with a different label returns an error.

The test says these are round-trips and not an RFC 8017 known-answer vector.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 53 — A prime-order toy curve

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `toy3`
- `tests/Arcanum.Tests/Toy3Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`EllipticCurve.toy3` is a `Curve`:

- `P = 17`, `A = 2`, `B = 2`, so `y^2 = x^3 + 2x + 2`
- `G = Point(5, 1)`
- `N = 19`
- `H = 1`

The comment says the group has prime order 19, so every point other than `Infinity` generates it, and that the curve is not a secure group. Do not change `toy` or `toy2`.

### Laws

- `4 A^3 + 27 B^2` is `4` modulo 17, which is not `0`.
- Enumeration by `isOnCurve` finds 19 points, `Infinity` included.
- Every one of the 18 points other than `Infinity` reaches `Infinity` by repeated addition at step 19 and not earlier.
- `double toy3 (Point(5, 1))` is `Point(6, 3)`. Slope `(3 * 5^2 + 2) * (2 * 1)^(-1) = 77 * 9 = 13`, then `x = 13^2 - 10 = 6` and `y = 13 * (5 - 6) - 1 = 3`, all modulo 17.
- `add toy3 (Point(5, 1)) (Point(6, 3))` is `Point(10, 6)`. Slope `(3 - 1) * (6 - 5)^(-1) = 2`, then `x = 4 - 5 - 6 = 10` and `y = 2 * (5 - 10) - 1 = 6`, all modulo 17.
- `jInvariant toy3` is `Some 3`. By hand: `Delta = -16 * 4 = 4`, `(4a)^3 = 512 = 2`, `4^(-1) = 13`, `-1728 = 6`, so `j = 6 * 2 * 13 = 3`, all modulo 17.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 54 — Group law and scalars on toy3

### Files

- `tests/Arcanum.Tests/Toy3Tests.fs`

### Object

The same group laws as phases 5 and 6, on the 19 points of `toy3`. Walk every pair and every triple. Do not sample.

### Laws

- Closure and commutativity under `add` on all 361 pairs.
- Associativity on all 6859 triples.
- For every point `Q` and every `k` in `0 .. 18`, `scalarMultiply toy3 k Q` equals adding `Q` to `Infinity`, `k` times.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 55 — SEC 1 message representative, and toy ECDSA moves to toy3

### Files

- `src/Arcanum.Asymmetric/Ecdsa.fs`
- `src/Arcanum.Cli/Program.fs`
- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

`Ecdsa.hashToInt` reads the SHA-256 digest little-endian. SEC 1 reads it big-endian and keeps the leftmost bits. Replace it with a public function:

`Ecdsa.messageRepresentative : EllipticCurve.Curve -> byte[] -> BigInteger`

It hashes with `SHA256.hash`, reads the digest as a big-endian unsigned integer, shifts right by `256 - bitLength N` when `bitLength N < 256`, and reduces modulo `N`. `signWithNonce` and `verify` use it. The comment names SEC 1 section 4.1.3 and the CLR hash call.

The CLI nonce-reuse demonstration copies the old little-endian rule in a local `hashToInt`. Replace that copy with a call to `Ecdsa.messageRepresentative`. Do not add new demonstrations.

ECDSA needs a group of prime order. `toy` has order 18. This phase replaces the two locked toy ECDSA tests, phase 26 and phase 27, with the same laws on `toy3`. This is the only replacement of locked cases this program allows. The toy ECDH test of phase 25 stays on `toy`.

### Laws

- `messageRepresentative nistP256 "sample"` (ASCII bytes) equals `h1` from RFC 6979 section A.1.2, read as an integer: `AF2BDBE1AA9B6EC1E2ADE1D694F41FC71A831D0268E9891562113D8A62ADD1BF`. That integer is below the P-256 `N`, so no reduction happens.
- `bitLength 19` is `5`. `SHA256("ec")` begins with the byte `0x1e`, so its leftmost five bits are `00011` and `messageRepresentative toy3 "ec"` is `3`. `SHA256("ex")` begins with `0x53`, so its leftmost five bits are `01010` and the representative is `10`.
- Phase 26 on `toy3`: private key `d = 5`, public key `5G = (9, 16)` by repeated addition, nonce `k = 4`, `4G = (3, 1)` by repeated addition, so `r = 3`. With `4^(-1) = 5` modulo 19:
  - `signWithNonce` of `"ec"` is `(r, s) = (3, 14)`, because `5 * (3 + 3 * 5) = 90 = 14`.
  - `signWithNonce` of `"ex"` is `(3, 11)`, because `5 * (10 + 3 * 5) = 125 = 11`.
  - `verify` accepts each signature on its own message, and rejects the `"ec"` signature on `"ex"`.
- Phase 27 on `toy3`: the two signatures share `r = 3`. The closed form recovers the nonce `k = (e1 - e2) * (s1 - s2)^(-1) = (3 - 10) * 3^(-1) = 12 * 13 = 4` and the private key `d = (s1 * k - e1) * r^(-1) = (56 - 3) * 13 = 15 * 13 = 5`, all modulo 19. No brute force. Both the recovered `k` and the recovered `d` are asserted.
- The existing `secp256k1` sign-then-verify round-trip still passes.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 56 — ECDSA refusals

### Files

- `src/Arcanum.Asymmetric/Ecdsa.fs`
- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

- `verify` returns false for a public key that is `Infinity`. Today `isOnCurve` accepts `Infinity`, and a signature can be forged for it.
- Delete the brute-force branch of `verify` that runs when `S` is not invertible modulo `N`. A non-invertible `S` returns false. On a prime-order curve every `S` in `1 .. N - 1` is invertible, so that branch only served `toy`.
- Lock the refusals `signWithNonce` already makes.

### Laws

- On `toy3`, with public key `Infinity`, message `"ec"` (representative `3`), and signature `(R, S) = (5, 3)`: the old `verify` would accept it, because `u1 = 3 * 3^(-1) = 1`, `u1 G = (5, 1)`, and `x = 5 = R`. The new `verify` returns false. The comment shows that calculation.
- On `toy3` with `d = 5`: `signWithNonce` with `k = 7` returns an error, because `7G = (0, 6)` and `r = 0`.
- On `toy3` with `d = 5`: `signWithNonce` of `"ec"` with `k = 9` returns an error, because `9G = (7, 6)`, `r = 7`, and `9^(-1) * (3 + 7 * 5) = 17 * 38 = 0` modulo 19.
- On `toy`, `verify` with `S = 6`, which is not invertible modulo 18, returns false.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 57 — SEC 1 point encoding on toy

### Files

- `src/Arcanum.Core/Sec1.fs`
- `src/Arcanum.Core/Arcanum.Core.fsproj`
- `tests/Arcanum.Tests/Sec1Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

SEC 1 sections 2.3.3 and 2.3.4. The field element length is `(bitLength P + 7) / 8` bytes, which is 1 byte for `P = 17`.

- `Sec1.encodePoint : Curve -> bool -> ECPoint -> byte[]`, where the `bool` asks for the compressed form. `Infinity` is the single byte `00`. Uncompressed is `04 || X || Y`. Compressed is `02 || X` when `y` is even and `03 || X` when `y` is odd.
- `Sec1.decodePoint : Curve -> byte[] -> Result<ECPoint, CryptoError>`. It refuses a wrong length, an unknown prefix, an uncompressed point not on the curve, and a compressed `x` with no square root. It uses `compress` and `decompress` from phase 41.

### Laws

On `toy`:

- `encodePoint` of `(6, 8)` uncompressed is `04 06 08`. Compressed it is `02 06`, because `8` is even.
- `encodePoint` of `(6, 9)` compressed is `03 06`.
- `encodePoint` of `Infinity` is `00`.
- `decodePoint` returns each of those three points from its encoding.
- `decodePoint` refuses `02 03`, because `3^3 + 1 = 11` is not a square modulo 17.
- `decodePoint` refuses `04 06 07`, because `(6, 7)` is not on `toy`.
- `decodePoint` refuses `04 06` (wrong length) and `05 06` (unknown prefix).

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 58 — Compressed generators of the large curves

### Files

- `tests/Arcanum.Tests/Sec1Tests.fs`

### Object

Encode and decode the generators of `secp256k1` and `nistP256`. Decoding the compressed form takes a 256-bit square root through `Modular.modSqrt`. Both primes are `3` modulo `4`, so it uses the fast path. The expected bytes are the published `Gx` of each curve, with the prefix fixed by the parity of the published `Gy`.

### Laws

- `nistP256` `Gy` ends in the hex digit `5`, so it is odd. Compressed `G` is `03` followed by `6b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296`.
- `secp256k1` `Gy` ends in the hex digit `8`, so it is even. Compressed `G` is `02` followed by `79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798`.
- Each compressed encoding decodes to `G`.
- Each uncompressed encoding is 65 bytes and decodes to `G`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 59 — Public-key validation

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `multiplyUnreduced` and `validatePublicKey`
- `tests/Arcanum.Tests/ValidationTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

SEC 1 section 3.2.2.1.

- `multiplyUnreduced : Curve -> BigInteger -> ECPoint -> ECPoint` is double-and-add on the bits of a non-negative scalar, without reducing it modulo `N`. `scalarMultiply` reduces modulo `N`, so it cannot check `N * Q`.
- `validatePublicKey : Curve -> ECPoint -> Result<unit, CryptoError>` refuses `Infinity`, refuses a point that fails `isOnCurve` (which since phase 51 includes the coordinate range), and refuses a point with `multiplyUnreduced curve curve.N Q <> Infinity`.

The comment says what the last check does not prove. In a group that is not cyclic, `N * Q = Infinity` does not put `Q` in the subgroup generated by `G`.

### Laws

- On `toy3`, all 18 points other than `Infinity` pass.
- `Infinity` is refused on `toy3`.
- On `toy`, `Point(23, 8)` is refused (range), and `Point(1, 0)` is refused (not on the curve: `0 <> 1 + 1`).
- On `toy2`, all 15 points other than `Infinity` pass, because every point order divides 4. The subgroup generated by `G` holds only 3 of them. The test asserts both facts.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 60 — An invalid-curve point on toy

### Files

- `src/Arcanum.Asymmetric/DiffieHellman.fs`, adding `computeEcdhSharedSecretChecked`
- `tests/Arcanum.Tests/ValidationTests.fs`

### Object

`add` never reads `B`. A point that is not on `toy` but satisfies `y^2 = x^3 + b'` for another `b'` is added on that other curve. A victim who multiplies such a point by a secret scalar can leak that scalar modulo a small order.

`computeEcdhSharedSecretChecked : Curve -> BigInteger -> ECPoint -> Result<ECPoint, CryptoError>` calls `validatePublicKey` before it multiplies. Keep `computeEcdhSharedSecret` as it is; the phase 25 test uses it. Its comment says it does not validate the point.

### Laws

- `Point(1, 0)` is not on `toy`, and it is on `y^2 = x^3 + 16`, because `0 = 1 + 16` modulo 17. The test checks the second equation directly.
- For every `d` in `1 .. 17`, `scalarMultiply toy d (Point(1, 0))` is `Infinity` exactly when `d` is even. The result leaks `d` modulo 2.
- `computeEcdhSharedSecretChecked toy d (Point(1, 0))` returns an error.
- For `Q = 5G` on `toy`, `computeEcdhSharedSecretChecked toy 7 Q` returns `Ok` of the same point as `computeEcdhSharedSecret toy 7 Q`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 61 — Jacobian representation

### Files

- `src/Arcanum.Core/Jacobian.fs`
- `src/Arcanum.Core/Arcanum.Core.fsproj`
- `tests/Arcanum.Tests/JacobianTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

A Jacobian point `(X : Y : Z)` stands for the affine point `(X / Z^2, Y / Z^3)`. `Z = 0` stands for `Infinity`. `(X : Y : Z)` and `(lambda^2 X : lambda^3 Y : lambda Z)` stand for the same point for every non-zero `lambda`.

- `Jacobian.JacobianPoint`, a record of three `BigInteger` fields.
- `Jacobian.ofAffine : ECPoint -> JacobianPoint` sets `Z = 1`, and maps `Infinity` to `(1 : 1 : 0)`.
- `Jacobian.toAffine : Curve -> JacobianPoint -> ECPoint` uses `Modular.modInverse` on `Z`.

### Laws

- `toAffine (ofAffine Q) = Q` for every point of `toy`, `toy2`, and `toy3`.
- For `(6, 8)` on `toy` and every `lambda` in `1 .. 16`, `toAffine` of `(6 lambda^2 : 8 lambda^3 : lambda)` reduced modulo 17 is `(6, 8)`.
- Any `Z = 0` maps to `Infinity`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 62 — Jacobian group law on the toy curves

### Files

- `src/Arcanum.Core/Jacobian.fs`
- `tests/Arcanum.Tests/JacobianTests.fs`

### Object

Doubling and addition in Jacobian coordinates for `y^2 = x^3 + a x + b`, all modulo `P`. The comment states the formulas.

Doubling `(X1 : Y1 : Z1)`. If `Z1 = 0` or `Y1 = 0`, the result is `Infinity`. Otherwise:

- `S = 4 X1 Y1^2`, `M = 3 X1^2 + a Z1^4`
- `X3 = M^2 - 2 S`, `Y3 = M (S - X3) - 8 Y1^4`, `Z3 = 2 Y1 Z1`

Addition `(X1 : Y1 : Z1) + (X2 : Y2 : Z2)`. If either `Z` is `0`, return the other point. Otherwise:

- `U1 = X1 Z2^2`, `U2 = X2 Z1^2`, `S1 = Y1 Z2^3`, `S2 = Y2 Z1^3`
- If `U1 = U2`: when `S1 = S2` the result is the double, otherwise it is `Infinity`
- `H = U2 - U1`, `R = S2 - S1`
- `X3 = R^2 - H^3 - 2 U1 H^2`, `Y3 = R (U1 H^2 - X3) - S1 H^3`, `Z3 = H Z1 Z2`

This is a second implementation of the group law. It agrees with `EllipticCurve.add`, or it is wrong. No constant-time claim.

### Laws

For every pair `Q`, `R` of points on `toy`, `toy2`, and `toy3`, with `Q` lifted using `lambda = 3` and `R` lifted using `lambda = 5`:

`toAffine (Jacobian.add curve Q' R') = EllipticCurve.add curve Q R`.

For every point `Q` on the three curves, `toAffine (Jacobian.double curve (ofAffine Q)) = EllipticCurve.double curve Q`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 63 — Jacobian on the large curves

### Files

- `tests/Arcanum.Tests/JacobianTests.fs`

### Object

The Jacobian formulas on `secp256k1` and `nistP256`. Jacobian double-and-add without reduction of the scalar lives in the test file, like `unreducedDoubleAndAdd` in `Secp256k1Tests.fs`.

### Laws

- `toAffine (Jacobian.double secp256k1 (ofAffine secp256k1.G))` is the phase 44 point: `x = c6047f9441ed7d6d3045406e95c07cd85c778e4b8cef3ca7abac09b95c709ee5`, `y = 1ae168fea63dc339a3c58419466ceaeef7f632653266d0e1236431a950cfe52a`.
- On each large curve, Jacobian double-and-add of `N * G` is `Infinity`, and of `(N - 1) * G` is `negate G`, without reducing the scalar.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 64 — Ladder on the toy curves

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `scalarMultiplyLadder`
- `tests/Arcanum.Tests/LadderTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`scalarMultiplyLadder : Curve -> BigInteger -> ECPoint -> ECPoint`. Reduce the scalar modulo `N`, as `scalarMultiply` does. Start with `R0 = Infinity` and `R1 = P`. Walk exactly `bitLength N` bits of the scalar from the top. On a `1` bit, set `R0 = R0 + R1` and `R1 = 2 R1`. On a `0` bit, set `R1 = R0 + R1` and `R0 = 2 R0`. Every step does one addition and one doubling, and `R1 - R0 = P` throughout. Return `R0`.

The comment says the sequence of operations does not depend on the bits, that the code still branches and uses `BigInteger`, and that no constant-time claim is made. It runs on the short Weierstrass curve. It is not a Montgomery-form curve.

### Laws

For every point `Q` and every `k` in `0 .. N - 1`, on `toy` (`N = 18`), `toy2` (`N = 4`), and `toy3` (`N = 19`):

`scalarMultiplyLadder curve k Q = scalarMultiply curve k Q`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 65 — Ladder on the large curves

### Files

- `tests/Arcanum.Tests/LadderTests.fs`

### Object

The ladder on `secp256k1` and `nistP256`.

### Laws

On each large curve:

- `scalarMultiplyLadder curve 1 G = G`.
- `scalarMultiplyLadder curve 2 G = double curve G`.
- `scalarMultiplyLadder curve (N - 1) G = negate curve G`.

`scalarMultiplyLadder curve N G` is `Infinity` because of the reduction. Do not assert it as an order proof; the order checks remain phases 7 and 63.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 66 — Discrete logarithm by brute force

### Files

- `src/Arcanum.Core/DiscreteLog.fs`
- `src/Arcanum.Core/Arcanum.Core.fsproj`
- `tests/Arcanum.Tests/DiscreteLogTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`DiscreteLog.bruteForce : Curve -> ECPoint -> ECPoint -> BigInteger option` (curve, base, target). It adds `base` to `Infinity` step by step until it meets `target`, returning the step count, or returns `None` when it comes back to `Infinity` first. The comment says it is for `toy`, `toy2`, and `toy3` only.

### Laws

- On `toy`, for every `k` in `0 .. 17`, `bruteForce toy G (kG)` is `Some k`, where `kG` is built by repeated addition.
- On `toy3`, for every `k` in `0 .. 18`, `bruteForce toy3 G (kG)` is `Some k`.
- `bruteForce toy (Point(0, 1)) G` is `None`, because `G` is outside the 3-point subgroup of phase 36.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 67 — Baby-step giant-step

### Files

- `src/Arcanum.Core/DiscreteLog.fs`
- `tests/Arcanum.Tests/DiscreteLogTests.fs`

### Object

`DiscreteLog.babyStepGiantStep : Curve -> ECPoint -> ECPoint -> BigInteger -> BigInteger option` (curve, base, target, order of base). `m` is the least integer with `m * m >= order`, computed with integers, not `float`. Baby steps store `j * base` for `j` in `0 .. m - 1`. Giant steps subtract `m * base` from the target up to `m` times.

### Laws

- `m` is `5` for order `18`, `5` for order `19`, and `2` for order `3`.
- On `toy` (order 18) and `toy3` (order 19), for every `k` in `0 .. order - 1`, `babyStepGiantStep` returns `Some k`, and agrees with `bruteForce`.
- With base `Point(0, 1)` and order `3` on `toy`, the target `G` returns `None`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 68 — Pohlig-Hellman

### Files

- `src/Arcanum.Core/DiscreteLog.fs`
- `tests/Arcanum.Tests/DiscreteLogTests.fs`

### Object

`DiscreteLog.pohligHellman : Curve -> ECPoint -> ECPoint -> (BigInteger * int) list -> Result<BigInteger, CryptoError>` (curve, base, target, factorization of the order of base as `(prime, exponent)` pairs). The product of the factors must equal the order of the base found by repeated addition; otherwise return an error. For each prime power `q^e`, recover the `e` base-`q` digits one at a time inside the subgroup of order `q` with `bruteForce`, then combine the residues with `Modular.crt`.

The comment says the work is bounded by the largest prime factor: 3 on `toy`, 19 on `toy3`. That is why ECDSA wants a group of large prime order. No factoring routine is added; the factorization is an input.

### Laws

- On `toy` with factors `[(2, 1); (3, 2)]`, since `18 = 2 * 3^2`: for every `k` in `0 .. 17`, `pohligHellman toy G (kG)` is `Ok k`.
- On `toy3` with factors `[(19, 1)]`: for every `k` in `0 .. 18`, the result is `Ok k`.
- On `toy` with factors `[(2, 1); (3, 1)]`, whose product `6` is not the order `18` of `G`, the result is an error.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 69 — Small-subgroup leak on toy

### Files

- `tests/Arcanum.Tests/ValidationTests.fs`

### Object

On a group whose order is not prime, a valid point of small order leaks the secret scalar modulo that order. Validation does not stop it, because the point really is on the curve and `N * Q = Infinity`.

### Laws

- `validatePublicKey toy (Point(0, 1))` is `Ok`.
- For every `d` in `1 .. 17`, `bruteForce toy (Point(0, 1)) (computeEcdhSharedSecret toy d (Point(0, 1)))` is `Some (d mod 3)`.

The comment contrasts `toy3`, where phase 53 showed every point other than `Infinity` has order 19, so no point of small order exists to send.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 70 — Counting points by Legendre symbols

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `countPoints`
- `tests/Arcanum.Tests/CountingTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`countPoints : BigInteger -> BigInteger -> BigInteger -> BigInteger` (`p`, `a`, `b`) is `p + 1 + sum over x in 0 .. p - 1 of jacobi (x^3 + a x + b) p`, for an odd prime `p`. A zero symbol adds `0` and stands for the single point with `y = 0`. It uses `Modular.jacobi`. The comment says it loops over every `x`, is meant for the toy curves, is not Schoof, and must not be called on the large curves.

### Laws

- `countPoints 17 0 1` is `18`, `countPoints 17 1 0` is `16`, and `countPoints 17 2 2` is `19`.
- Each count equals the length of the `isOnCurve` enumeration of `toy`, `toy2`, and `toy3`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 71 — Hasse bound and trace

### Files

- `tests/Arcanum.Tests/CountingTests.fs`

### Object

The trace of Frobenius is `t = p + 1 - #E`. Hasse's theorem says `t^2 <= 4 p`. Check it with integers, not a square root.

### Laws

- The traces are `0` for `toy`, `2` for `toy2`, and `-1` for `toy3`.
- `t^2` is `0`, `4`, and `1`, each at most `4 * 17 = 68`.

The comment notes that trace `0` makes `toy` supersingular. It does not add a pairing.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 72 — Quadratic twists

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `twistCoefficients`
- `tests/Arcanum.Tests/CountingTests.fs`

### Object

`twistCoefficients : Curve -> BigInteger -> Result<BigInteger * BigInteger, CryptoError>` returns `(a d^2, b d^3)` modulo `P` for a quadratic non-residue `d`, and refuses `d` when `jacobi d P` is not `-1`. The twist of a curve with `p + 1 - t` points has `p + 1 + t` points.

### Laws

- `jacobi 3 17` is `-1`. `jacobi 2 17` is `1`, because `6^2 = 36 = 2` modulo 17, so `d = 2` is refused.
- With `d = 3`, `d^2 = 9` and `d^3 = 27 = 10` modulo 17:
  - The twist of `toy` is `(0, 10)`, `y^2 = x^3 + 10`, with 18 points.
  - The twist of `toy2` is `(9, 0)`, `y^2 = x^3 + 9x`, with 20 points.
  - The twist of `toy3` is `(1, 3)`, because `2 * 9 = 18 = 1` and `2 * 27 = 54 = 3`, so `y^2 = x^3 + x + 3`, with 17 points.
- Each twist count is checked both by `countPoints` and by an `isOnCurve` enumeration. For each pair, `#E + #E' = 2 * 17 + 2 = 36`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 73 — RFC 6979 octet conversions

### Files

- `src/Arcanum.Asymmetric/Rfc6979.fs`
- `src/Arcanum.Asymmetric/Arcanum.Asymmetric.fsproj`
- `tests/Arcanum.Tests/Rfc6979Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

RFC 6979 section 2.3, for a group order `q` with `qlen = bitLength q` and `rlen = 8 * ceil(qlen / 8)`:

- `Rfc6979.bits2int : BigInteger -> byte[] -> BigInteger` (`q`, octets): read big-endian, then shift right by `8 * length - qlen` when that is positive.
- `Rfc6979.int2octets : BigInteger -> BigInteger -> byte[]` (`q`, value): big-endian, exactly `rlen / 8` bytes.
- `Rfc6979.bits2octets : BigInteger -> byte[] -> byte[]` (`q`, octets): `int2octets (bits2int octets mod q)`.

### Laws

With `q` the P-256 `N`, `qlen = 256`, `h1 = SHA256("sample")`, and `x` the RFC 6979 section A.2.5 private key `C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721`:

- `h1` is `AF2BDBE1AA9B6EC1E2ADE1D694F41FC71A831D0268E9891562113D8A62ADD1BF`, from RFC 6979 section A.1.2. This compares the CLR hash call against a published digest.
- `bits2int q h1` is that integer.
- `int2octets q x` is the 32 bytes `C9AFA9D8 ... 120F6721`, in that order.
- `bits2octets q h1` is `h1` itself, because `h1 < q`.
- With `q = 19`, `bits2int 19 (SHA256("ec"))` is `3`, the same value phase 55 locked for `messageRepresentative toy3 "ec"`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 74 — RFC 6979 nonce

### Files

- `src/Arcanum.Asymmetric/Rfc6979.fs`
- `tests/Arcanum.Tests/Rfc6979Tests.fs`

### Object

`Rfc6979.generateK : BigInteger -> BigInteger -> byte[] -> BigInteger` (`q`, private key, message). RFC 6979 section 3.2, steps a through h, with SHA-256 for the digest and `HMAC.hmacSha256` for `HMAC_K`. `V` starts as 32 bytes of `0x01` and `K` as 32 bytes of `0x00`. The comment labels both CLR boundaries: the digest is `SHA256.hash`, and HMAC is the F# construction on it.

### Laws

From RFC 6979 section A.2.5, NIST P-256, SHA-256, with the private key of phase 73:

- Message `"sample"`: `k = A6E3C57DD01ABE90086538398355DD4C3B17AA873382B0F24D6129493D8AAD60`.
- Message `"test"`: `k = D16B6AE827F17175E040871A1C7EC3500192C4C92677336EC2537ACAEE0008E0`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 75 — RFC 6979 key pair

### Files

- `tests/Arcanum.Tests/Rfc6979Tests.fs`

### Object

The public key of the RFC 6979 section A.2.5 private key on NIST P-256.

### Laws

With `x = C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721`:

- `scalarMultiply nistP256 x G` is the point with
  - `Ux = 60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6`
  - `Uy = 7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299`
- `scalarMultiplyLadder nistP256 x G` is the same point.
- `validatePublicKey nistP256 U` is `Ok`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 76 — RFC 6979 signatures

### Files

- `src/Arcanum.Asymmetric/Rfc6979.fs`
- `tests/Arcanum.Tests/Rfc6979Tests.fs`

### Object

`Rfc6979.sign : Ecdsa.EcdsaKeyPair -> byte[] -> Result<EcdsaSignature, CryptoError>` calls `Ecdsa.signWithNonce` with `generateK curve.N keyPair.PrivateKey message`. Do not normalize `s`.

### Laws

From RFC 6979 section A.2.5, NIST P-256, SHA-256, with the key pair of phase 75:

- Message `"sample"`:
  - `r = EFD48B2AACB6A8FD1140DD9CD45E81D69D2C877B56AAF991C34D0EA84EAF3716`
  - `s = F7CB1C942D657C41D436C7A1B6E29F65F3E900DBB9AFF4064DC4AB2F843ACDA8`
- Message `"test"`:
  - `r = F1ABB023518351CD71D881567B1EA663ED3EFCF6C5132B354F28D3B0B7D38367`
  - `s = 019F4113742A2B14BD25926B49C649155F267E60D3814B4C0CC84250E46F0083`
- `Ecdsa.verify` accepts each signature on its own message, and rejects the `"sample"` signature on `"test"`.

The test says each message is one published vector, not a CAVP file. If `r` or `s` disagrees, the likely causes are the message representative of phase 55 or `generateK`. Do not edit the literals.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 77 — RFC 5903 public values

### Files

- `tests/Arcanum.Tests/Rfc5903Tests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

RFC 5903 section 8.1, the 256-bit random ECP group, which is NIST P-256.

### Laws

- With `i = C88F01F510D9AC3F70A292DAA2316DE544E9AAB8AFE84049C62A9C57862D1433`, `scalarMultiply nistP256 i G` is the point with
  - `gix = DAD0B65394221CF9B051E1FECA5787D098DFE637FC90B9EF945D0C3772581180`
  - `giy = 5271A0461CDB8252D61F1C456FA3E59AB1F45B33ACCF5F58389E0577B8990BB3`
- With `r = C6EF9C5D78AE012A011164ACB397CE2088685D8F06BF9BE0B283AB46476BEE53`, `scalarMultiply nistP256 r G` is the point with
  - `grx = D12DFB5289C8D4F81208B70270398C342296970A0BCCB74C736FC7554494BF63`
  - `gry = 56FBF3CA366CC23E8157854C13C58D6AAC23F046ADA30F8353E74F33039872AB`

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 78 — RFC 5903 shared value

### Files

- `tests/Arcanum.Tests/Rfc5903Tests.fs`

### Object

The ECDH common value of RFC 5903 section 8.1, through `DiffieHellman.computeEcdhSharedSecret` and `computeEcdhSharedSecretChecked`.

### Laws

- `computeEcdhSharedSecret nistP256 i (grx, gry)` and `computeEcdhSharedSecret nistP256 r (gix, giy)` are both the point with
  - `girx = D6840F6B42F6EDAFD13116E0E12565202FEF8E9ECE7DCE03812464D04B9442DE`
  - `giry = 522BDE0AF0D8585B8DEF9C183B5AE38F50235206A8674ECB5D98EDB20EB153A2`
- `computeEcdhSharedSecretChecked` returns `Ok` of the same point both ways.
- The shared secret value is `girx`, which RFC 5903 section 8.1 states.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 79 — Docs catch-up

### Files

- `README.md`
- `Docs/agent_channel.md`, one new entry at the top for this phase. Do not rewrite entries from earlier phases.

### Object

The README keeps its structure and quickstart. It adds:

- `EllipticCurve.toy3`, the prime-order curve of 19 points, not a secure group.
- Coordinate range in `isOnCurve`, `Sec1` encoding, `multiplyUnreduced`, and `validatePublicKey`, including what `N * Q = Infinity` does not prove on `toy2`.
- `Jacobian` and `scalarMultiplyLadder` as second implementations checked against `add`, with no constant-time claim.
- `DiscreteLog` on the toy curves only, and why Pohlig-Hellman makes prime order matter.
- `countPoints`, the Hasse check, and `twistCoefficients`.
- The SEC 1 message representative, and `Rfc6979` built on `HMAC.hmacSha256`.
- `Rsa.decryptOaep` and the corrected OAEP length, as round-trips, not RFC 8017 known answers.
- The new cited checks: RFC 7914 section 11, RFC 6979 sections A.1.2 and A.2.5 (two messages), and RFC 5903 section 8.1. Each is one vector, not a CAVP campaign.

Remove the README sentence that `signWithNonce` "is not an implementation of RFC 6979" only if it now misleads; `signWithNonce` still takes a caller-supplied nonce, and `Rfc6979.sign` is the deterministic path.

Do not promise a constant-time curve, a from-scratch SHA-256, AES-GCM, pairings, or another curve model.

Confirm the channel has one entry for each phase from 49 through 79, and that the entries for phases 0 through 48 are present and unedited.

### Laws

No new mathematical law. If a sentence disagrees with a type in the code, correct the sentence.

### Exit

`dotnet test --configuration Release` passes. Commit. Push. Stop.

## Parked until Leon says otherwise

- A from-scratch SHA-256 used as an implementation in this repository.
- AES-GCM, and any GHASH known-answer file beyond the structural checks in phase 42.
- RSA-OAEP known answers. The encryption seed is random.
- Low-s normalization and BIP 340 or any Bitcoin signature format.
- Montgomery and Edwards forms. Pairings, MOV, isogenies, and rank over `Q`.
- Mordell-Weil, L-functions, Birch-Swinnerton-Dyer, and Schoof.
- LWE, lattice reduction, and lattice encryption.
- A new Enigma rotor or a historical ciphertext campaign.
- Constant-time `secp256k1` or P-256.
- The Rust application. Moving the CLI demonstrations into a second repository.
- Lean, or a Sage wrapper.
- A NuGet package.

## Order dependencies

Execute phases 49 through 79 in that order. Do not start phase `N + 1` before phase `N` is pushed. Do not return to phases 0 through 48.

- Phase 49 widens `AGENTS.md` before any later phase computes a discrete logarithm or adds `toy3`.
- Phase 50 corrects documents only and adds one PBKDF2 vector. It does not touch the curve.
- Phase 51 comes before phases 57 and 59, which rely on the range check.
- Phase 52 is independent of the curve.
- Phases 53 and 54 add and walk `toy3`. Phase 55 depends on them, because the toy ECDSA laws move there.
- Phase 55 changes the message representative that phases 56, 73, and 76 depend on. If phase 76 fails, check phase 55 first.
- Phase 56 depends on phase 55. It removes the composite-order branch only after no test needs it.
- Phases 57 and 58 depend on `compress` and `decompress` from phase 41.
- Phase 59 depends on phase 51. Phases 60 and 69 depend on phase 59.
- Phases 61 through 63 add `Jacobian.fs` and depend on nothing after phase 54.
- Phases 64 and 65 add the ladder. Phase 75 uses it.
- Phases 66 through 68 add `DiscreteLog.fs`. Phase 69 uses `bruteForce`.
- Phases 70 through 72 depend on `Modular.jacobi`, locked in phase 12.
- Phases 73 through 76 depend on phase 55 and on `HMAC.hmacSha256`. Phase 74 depends on phase 73, and phase 76 on phases 74 and 75.
- Phases 77 and 78 depend on `nistP256` and on phase 60 for the checked ECDH call.
- Phase 79 is last.
