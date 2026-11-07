# Antigravity depth program

Leon asked for this on 7 Nov 2026, after the long-haul program finished at phase 79. Claude Code wrote this plan. Antigravity executes it. Execute these phases in order. One phase is one commit on `main`, pushed to `origin/main`, plus a new entry at the top of `Docs/agent_channel.md`. Then start the next phase.

This file is the work order that follows `Docs/antigravity_long_haul_program.md`. That program ended at phase 79, commit `1c55a59`. Do not redo phases 0 through 79. Number these phases 80 through 128.

The repository is `lpr16/arcanum`. `origin` is `git@github.com:lpr16/arcanum.git`. If this file disagrees with `AGENTS.md` or the code, `AGENTS.md` and the code win. Code wins over both. This program does not edit `AGENTS.md`. Every phase fits the charter as it stands.

## Goal

The long-haul program brought the curve to published vectors on NIST P-256. This program goes deeper instead of wider. It adds no curve, no hash, and no cipher. It does four things:

1. It repairs defects found by reading the code: a random range that never reaches its maximum, RSA moduli one bit short, an unchecked ECDSA private key, an ECDH shared point that may be `Infinity`, and constant-time claims that no test supports.
2. It locks the existing symmetric code against the publications it claims to follow: FIPS 197 Appendices A.1 and B, SP 800-38A, RFC 8439 sections 2 and A, the rest of RFC 4231, and the second vector of RFC 7914 section 11.
3. It checks the group law more ways: NAF, wNAF, Shamir's trick, mixed and `a = -3` Jacobian formulas, batch inversion, special-prime reduction, and the secp256k1 endomorphism. Each one agrees with `EllipticCurve.add`, or it is wrong.
4. It studies the toy groups and ECDSA more closely: the order of every point, the structure of `toy2`, public-key recovery from the published RFC 6979 signatures, malleability, related nonces, and Pollard's rho and kangaroo on `toy3`.

The center does not move. It is still the group of points on a short Weierstrass curve, with `EllipticCurve.add` as the group law.

## How to run

- Start from current `main`. Phase 80 commits this file if it is still untracked. Later phases do not restyle it and do not weaken a law written here.
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

- If a law fails, fix the code in that phase. Do not weaken a law to go green. Weakening includes changing a cited vector, point, digit list, count, or residue so the current code matches, deleting a locked case, sampling where this file says every case, or catching the failure and returning the identity. Phase 112 is the one phase that changes a function every earlier ECDSA law runs through, and those laws must stay green unchanged.
- A failed law is a failed test. Do not add a proof file. No emojis.
- Channel entry shape, newest at the top: date, author `Antigravity`, the phase number, what landed, and that `dotnet test --configuration Release` passed. Do not rewrite another author's entry.
- Hex in this file is case-insensitive. A test may write it in either case. Long hex values are printed 32 bytes to a line; join the lines with nothing between them.

## Already finished

Phases 0 through 79 are on `main`. The channel records them. In particular:

- `ToyField` is `F_17`. `toy` is `y^2 = x^3 + 1`, 18 points, `G = (6, 8)`, `N = 18`. `toy2` is `y^2 = x^3 + x`, 16 points, not cyclic, `G = (1, 6)` of order 4, `N = 4`, `H = 4`. `toy3` is `y^2 = x^3 + 2x + 2`, prime order 19, `G = (5, 1)`, `N = 19`.
- `isOnCurve` requires coordinates in `0 .. P - 1`. `Sec1.encodePoint` and `decodePoint` exist. `multiplyUnreduced` and `validatePublicKey` exist.
- `Jacobian.ofAffine`, `toAffine`, `double`, and `add` exist. `scalarMultiplyLadder` exists.
- `DiscreteLog.bruteForce`, `babyStepGiantStep`, and `pohligHellman` exist, for the toy curves only. `DiscreteLog` has a private `pointOrderByRepeatedAdd`.
- `countPoints`, the Hasse check, and `twistCoefficients` exist.
- `Ecdsa.messageRepresentative` is SEC 1. `messageRepresentative toy3 "ec"` is `3` and `messageRepresentative toy3 "ex"` is `10`. `Rfc6979.sign` matches RFC 6979 section A.2.5 on P-256 for `"sample"` and `"test"`. `computeEcdhSharedSecret` matches RFC 5903 section 8.1.
- `Rsa.decryptOaep` exists. OAEP is checked by round-trips only.
- The cited symmetric checks so far are FIPS 197 Appendix C.1, RFC 8439 section 2.3.2, RFC 4231 test cases 1 and 2, and RFC 7914 section 11 at `c = 1`.

Do not delete `toy`, `toy2`, `toy3`, `secp256k1`, or `nistP256`. Do not change their parameters. Do not claim any toy curve is a secure group.

## Ground truth

- The repository is a public F# study of cryptography. The center is the short Weierstrass group.
- A CLR call stays labeled. A round-trip stays a round-trip. A cited vector names its publication and section in the test and is not the output of the function under test.
- F# `BigInteger` is not a constant-time field. Nothing in this program carries a constant-time claim, and phase 85 removes the claims that are already there.
- Discrete logarithms are computed only on `toy`, `toy2`, and `toy3`. No check on `secp256k1` or `nistP256` takes a discrete logarithm or enumerates the group. Scalar multiplication by a known scalar is not a discrete logarithm. A Miller-Rabin test of a curve parameter is not an enumeration.
- The ECDSA message representative follows SEC 1 section 4.1.3, as phase 55 made it. ECDSA signatures are not low-s normalized.
- The symmetric phases check existing code against its publication. They do not add a cipher, a mode, a hash, or a key size.
- elementa and mathcore are other repositories. Do not edit, clone, or import them.
- No UI. No port. No Python in the build. No secrets. No emojis.

## Provenance of expected values

Every expected value in this file was checked before it was written down. Do not edit any of them to match the code. If a law fails, the code is wrong.

- RFC 8439, RFC 4231, RFC 7914, RFC 6979, and RFC 5903 values were parsed out of the RFC text files from rfc-editor.org. They were then recomputed by an independent implementation that shares no code with this repository: a ChaCha20, Poly1305, and AEAD written from RFC 8439, and the Python standard library `hmac` and `hashlib`.
- FIPS 197 and SP 800-38A values were parsed out of the NIST PDFs and recomputed by an independent AES-128 written from FIPS 197.
- The FIPS 186-4 section D.2.3 reduction formula was copied from the NIST PDF and checked against ordinary remainder.
- Toy-curve, ECDSA, Miller-Rabin, Lucas, NAF, Shamir, Hill, and rail fence values were derived by hand where the law text shows the work. All of them were also checked by an independent implementation that shares no code with this repository.
- The secp256k1 cube roots are derived inside the test from a formula. Their literals are written here only so a reader can compare them.

## Hard stops

- Do not redo phases 0 through 79.
- Do not take a discrete logarithm on `secp256k1` or `nistP256`. Do not enumerate their groups. Do not run `pollardRho`, `kangaroo`, `order`, or `countPoints` on them.
- Do not add a curve record, a curve model, or a coordinate system other than affine and Jacobian. No Montgomery-form curve, Edwards curve, pairing, MOV, isogeny, rank over `Q`, Mordell-Weil, L-function, Birch-Swinnerton-Dyer, or Schoof.
- Do not claim constant time for anything. Phase 85 removes claims. No phase adds one.
- Do not reimplement SHA-256. Do not add AES-192, AES-256, AES-GCM, GHASH vectors, Salsa20, or scrypt.
- Do not add an integer factoring routine. Pohlig-Hellman and the Lucas certificate take factors as input.
- Do not add lattice reduction. The GLV phase checks the endomorphism and does not decompose a scalar.
- Do not low-s normalize ECDSA signatures. Phase 121 shows that `(r, N - s)` also verifies. It does not choose between them.
- Do not add a deterministic RSA-OAEP vector. Do not add a CLI demonstration.
- Do not invent a published vector by running the function and pasting the output back.
- Do not edit `Docs/antigravity_work_program.md`, `Docs/antigravity_research_program.md`, or `Docs/antigravity_long_haul_program.md`.
- Do not weaken a law to go green.

## Phase 80 — Work order

### Files

- This file, if it is not already committed
- `Docs/agent_channel.md`, including the Claude Code entry that announces this program if it is not already committed
- `README.md`, one sentence

### Object

No algorithm. The README sentence that names the work orders ends with `Docs/antigravity_long_haul_program.md` (phases 49 through 79). Add `Docs/antigravity_depth_program.md` (phases 80 through 128) as the last item of that list. Change nothing else in the README.

### Laws

No mathematical law. The diff does not add a `.fs` file.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 81 — An inclusive random range

### Files

- `src/Arcanum.Core/Primes.fs`
- `tests/Arcanum.Tests/PrimeTests.fs`

### Object

`Primes.randomBigInteger minVal maxVal` says it returns a value in `[min, max]`. It returns `minVal + candidate` only when `candidate < maxVal - minVal`, so `maxVal` is never returned. `Ecdsa.generateKeyPair` asks for `1 .. N - 1` and never gets `N - 1`.

Rewrite it:

- `minVal > maxVal` raises `ArgumentException`. `minVal = maxVal` returns `minVal`.
- Let `range = maxVal - minVal`. Draw `ceil(bitLength range / 8)` bytes with `RandomNumberGenerator.Fill`, read them as an unsigned integer, clear the bits above `bitLength range`, and accept the candidate when it is at most `range`. Return `minVal + candidate`.
- The comment names the CLR call and says the range is inclusive on both ends.

The callers in `Ecdsa`, `DiffieHellman`, and `isProbablePrime` keep their arguments. They now reach their upper bound.

### Laws

- `randomBigInteger 7 7` is `7`.
- `randomBigInteger 8 7` raises `ArgumentException`.
- 256 draws of `randomBigInteger 0 1` are all `0` or `1`, and both values occur. The comment says the chance of a correct function failing this is `2^-255`.
- 1000 draws of `randomBigInteger 1 18` all lie in `1 .. 18`, and `18` occurs. The old function could never return `18`. The comment says the chance of a correct function never drawing it is `(17/18)^1000`, below `10^-24`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 82 — RSA moduli of the requested length

### Files

- `src/Arcanum.Asymmetric/Rsa.fs`
- `tests/Arcanum.Tests/RsaTests.fs`

### Object

`Rsa.generateKeyPair bits` multiplies two `bits / 2`-bit primes. Each has its top bit set, so each is at least `2^(bits/2 - 1)`. Their product can be as small as `2^(bits - 2)`, which has `bits - 1` bits.

- `generateKeyPair` raises `ArgumentOutOfRangeException` unless `bits` is even and at least `16`.
- When `bitLength (p * q) <> bits`, it draws new primes. It does not change `Primes.generatePrime`.

### Laws

- Four keys from `generateKeyPair 512` each have `N.GetBitLength() = 512` and `modulusLength N = 64`.
- `generateKeyPair 513` raises `ArgumentOutOfRangeException`.
- The phase 52 OAEP round-trips still pass.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 83 — ECDSA private-key range

### Files

- `src/Arcanum.Asymmetric/Ecdsa.fs`
- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

`Ecdsa.signWithNonce` checks the nonce range and never checks the private key. With `d = 0` it signs for the public key `Infinity`. `signWithNonce` returns an error unless `1 <= keyPair.PrivateKey <= N - 1`. The check comes before any other work.

### Laws

On `toy3`, message `"ec"` (representative `3`), nonce `k = 4`, `4G = (3, 1)`, `r = 3`, `4^(-1) = 5` modulo 19:

- `d = 0`, `d = 19`, and `d = -1` each return an error.
- `d = 1` returns `(3, 11)`, because `5 * (3 + 3 * 1) = 30 = 11`. `verify` accepts it under the public key `G`.
- `d = 5` still returns `(3, 14)`, the phase 55 signature.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 84 — ECDH refuses the point at infinity

### Files

- `src/Arcanum.Asymmetric/DiffieHellman.fs`
- `tests/Arcanum.Tests/ValidationTests.fs`

### Object

SEC 1 section 3.3.1 step 2: if the shared point is `Infinity`, output "invalid". `computeEcdhSharedSecretChecked` validates the peer point and then returns `Ok Infinity` when the product is `Infinity`. Make it return an error instead. The comment names the SEC 1 step. Leave `computeEcdhSharedSecret` unchecked; phases 25 and 69 use it.

### Laws

- On `toy`, `validatePublicKey toy (Point(0, 1))` is `Ok`, which is phase 69.
- For every `d` in `1 .. 17`, `computeEcdhSharedSecretChecked toy d (Point(0, 1))` returns an error exactly when `d` is `3`, `6`, `9`, `12`, or `15`, which are the multiples of the order 3 of that point. Otherwise it returns `Ok` of the same point as `computeEcdhSharedSecret`.
- `computeEcdhSharedSecretChecked toy 18 toy.G` returns an error, because `scalarMultiply` reduces `18` to `0`.
- `computeEcdhSharedSecretChecked toy3 19 toy3.G` returns an error.
- The phase 60 and phase 78 laws still pass.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 85 — No unsupported constant-time claims

### Files

- `src/Arcanum.Core/Bytes.fs`, comment only
- `src/Arcanum.Symmetric/Padding.fs`, comments only
- `src/Arcanum.Symmetric/Poly1305.fs`, comment only
- `src/Arcanum.Hashing/HMAC.fs`, comment only
- `src/Arcanum.Symmetric/ChaCha20Poly1305.fs`, comment only
- `tests/Arcanum.Tests/BytesTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

The work programs say a constant-time claim needs a test or a cited construction. These comments make the claim without one:

- `Bytes.constantTimeEquals`: "Constant-time byte array comparison to mitigate timing side-channel attacks."
- `Padding.unpadPkcs7`: "Strips PKCS#7 padding in constant time", and the inner comment "Constant-time verification of all padding bytes".
- `Poly1305.verify`: "Verifies a Poly1305 tag in constant time."
- `HMAC.verifySha256`: "Verifies HMAC tag in constant time."
- `ChaCha20Poly1305.decrypt`: "Strict constant-time MAC verification before decryption to avoid padding and plaintext oracle leaks!"

Replace each with a statement of what the code does. For example, `constantTimeEquals` compares every byte of two equal-length arrays without an early exit, and no constant-time claim is made about the compiled code. `decrypt` checks the tag before it decrypts. Do not rename `constantTimeEquals`. Do not change any code.

### Laws

- `constantTimeEquals [|1uy; 2uy; 3uy|] [|1uy; 2uy; 3uy|]`, on two separate arrays, is true.
- `constantTimeEquals [|1uy; 2uy; 3uy|] [|1uy; 2uy; 4uy|]` is false. The difference is in the last byte.
- `constantTimeEquals [|1uy; 2uy|] [|1uy; 2uy; 3uy|]` is false.
- `constantTimeEquals null [|1uy|]` is false. `constantTimeEquals null null` is true, because the reference check comes first.
- After the edit, every match of `grep -i "constant time\|constant-time"` over `src/` is a sentence that says no constant-time claim is made. The phase does this check by hand and records it in the channel entry. It is not a test.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 86 — FIPS 197 key expansion and cipher example

### Files

- `tests/Arcanum.Tests/AesBlockTests.fs`

### Object

FIPS 197 Appendix A.1, the expansion of a 128-bit key, and Appendix B, the cipher example. `Aes.expandKey128` returns 44 big-endian words, the same notation as the appendix.

### Laws

With key `2b7e151628aed2a6abf7158809cf4f3c`:

- `expandKey128` returns 44 words.
- Words `w0 .. w3` are `2b7e1516`, `28aed2a6`, `abf71588`, `09cf4f3c`.
- `w4 = a0fafe17`, `w5 = 88542cb1`, `w8 = f2c295f2`, `w20 = d4d1c6f8`, `w40 = d014f9a8`, `w43 = b6630ca6`. These are from the Appendix A.1 table.
- `encryptBlock` of input `3243f6a8885a308d313198a2e0370734` is `3925841d02dc09fbdc118597196a0b32`. Appendix B prints the output column by column as `39 25 84 1d`, `02 dc 09 fb`, `dc 11 85 97`, `19 6a 0b 32`.
- `decryptBlock` of that output returns the input.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 87 — SP 800-38A ECB and CBC

### Files

- `tests/Arcanum.Tests/Sp80038aTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

NIST SP 800-38A Appendix F.1.1 (ECB-AES128.Encrypt) and F.2.1 (CBC-AES128.Encrypt). `Modes.encryptEcb` and `encryptCbc` add PKCS#7 padding. Four full blocks therefore become five. The first four are the cited ciphertext. The fifth is a padding block, and it is not a vector. The test asserts only its length, as phase 45 did.

Key `2b7e151628aed2a6abf7158809cf4f3c`. The 64-byte plaintext is the four blocks:

```
6bc1bee22e409f96e93d7e117393172a
ae2d8a571e03ac9c9eb76fac45af8e51
30c81c46a35ce411e5fbc1191a0a52ef
f69f2445df4f9b17ad2b417be66c3710
```

### Laws

- `encryptEcb` returns 80 bytes. Its first 64 bytes are the F.1.1 ciphertext:

```
3ad77bb40d7a3660a89ecaf32466ef97
f5d3d58503b9699de785895a96fdbaaf
43b1cd7f598ece23881b00e3ed030688
7b0c785e27e8ad3f8223207104725dd4
```

- `decryptEcb` of those 80 bytes returns the 64-byte plaintext.
- With IV `000102030405060708090a0b0c0d0e0f`, `encryptCbc` returns 80 bytes. Its first 64 bytes are the F.2.1 ciphertext:

```
7649abac8119b246cee98e9b12e9197d
5086cb9b507219ee95db113a917678b2
73bed6b8e3c1743b7116e69e22229516
3ff1caa1681fac09120eca307586e1a7
```

- `decryptCbc` of those 80 bytes returns the 64-byte plaintext.

The test says these are the four-block examples of SP 800-38A, not a CAVP file.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 88 — SP 800-38A CTR

### Files

- `tests/Arcanum.Tests/Sp80038aTests.fs`

### Object

SP 800-38A Appendix F.5.1 (CTR-AES128.Encrypt) and F.5.2 (CTR-AES128.Decrypt) through `Modes.processCtr`. The initial counter block is the IV. CTR does not pad.

### Laws

With the key and plaintext of phase 87, and initial counter `f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff`:

- `processCtr` returns 64 bytes:

```
874d6191b620e3261bef6864990db6ce
9806f66b7970fdff8617187bb9fffdff
5ae4df3edbd5d35e5b4f09020db03eab
1e031dda2fbe03d1792170a0f3009cee
```

- `processCtr` of that ciphertext with the same counter returns the plaintext, which is F.5.2.

The second counter block is `f0f1f2f3f4f5f6f7f8f9fafbfcfdff00`. Its last byte wraps from `ff` to `00` and carries into the byte before it, so the law exercises the carry in the counter increment.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 89 — RFC 8439 ChaCha20 encryption

### Files

- `tests/Arcanum.Tests/ChaChaTests.fs`

### Object

RFC 8439 section 2.4.2 through `ChaCha20.processStream`.

### Laws

Key `000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f`, nonce `000000000000004a00000000`, initial counter `1`. The plaintext is the 114 ASCII bytes:

```
Ladies and Gentlemen of the class of '99: If I could offer you only one tip for the future, sunscreen would be it.
```

`processStream` returns:

```
6e2e359a2568f98041ba0728dd0d6981e97e7aec1d4360c20a27afccfd9fae0b
f91b65c5524733ab8f593dabcd62b3571639d624e65152ab8f530c359f0861d8
07ca0dbf500d6a6156a38e088a22b65e52bc514d16ccf806818ce91ab7793736
5af90bbf74a35be6b40b8eedf2785e42874d
```

The plaintext spans two blocks and part of a third, so the counter advances twice.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 90 — RFC 8439 Appendix A.2 encryption vectors

### Files

- `tests/Arcanum.Tests/ChaChaTests.fs`

### Object

RFC 8439 Appendix A.2, test vectors #2 and #3, through `ChaCha20.processStream`.

### Laws

Test vector #2. Key `0000000000000000000000000000000000000000000000000000000000000001`, nonce `000000000000000000000002`, initial counter `1`. The plaintext is these 375 ASCII bytes, one line with single spaces:

```
Any submission to the IETF intended by the Contributor for publication as all or part of an IETF Internet-Draft or RFC and any statement made within the context of an IETF activity is considered an "IETF Contribution". Such statements include oral statements in IETF sessions, as well as written and electronic communications made at any time or place, which are addressed to
```

The ciphertext is:

```
a3fbf07df3fa2fde4f376ca23e82737041605d9f4f4f57bd8cff2c1d4b7955ec
2a97948bd3722915c8f3d337f7d370050e9e96d647b7c39f56e031ca5eb6250d
4042e02785ececfa4b4bb5e8ead0440e20b6e8db09d881a7c6132f420e527950
42bdfa7773d8a9051447b3291ce1411c680465552aa6c405b7764d5e87bea85a
d00f8449ed8f72d0d662ab052691ca66424bc86d2df80ea41f43abf937d3259d
c4b2d0dfb48a6c9139ddd7f76966e928e635553ba76c5c879d7b35d49eb2e62b
0871cdac638939e25e8a1e0ef9d5280fa8ca328b351c3c765989cbcf3daa8b6c
cc3aaf9f3979c92b3720fc88dc95ed84a1be059c6499b9fda236e7e818b04b0b
c39c1e876b193bfe5569753f88128cc08aaa9b63d1a16f80ef2554d7189c411f
5869ca52c5b83fa36ff216b9c1d30062bebcfd2dc5bce0911934fda79a86f6e6
98ced759c3ff9b6477338f3da4f9cd8514ea9982ccafb341b2384dd902f3d1ab
7ac61dd29c6f21ba5b862f3730e37cfdc4fd806c22f221
```

Test vector #3. Key `1c9240a5eb55d38af333888604f6b5f0473917c1402b80099dca5cbc207075c0`, nonce `000000000000000000000002`, initial counter `42`. The plaintext is these 127 ASCII bytes: four lines joined by a single `0x0a`, with no trailing newline:

```
'Twas brillig, and the slithy toves
Did gyre and gimble in the wabe:
All mimsy were the borogoves,
And the mome raths outgrabe.
```

The ciphertext is:

```
62e6347f95ed87a45ffae7426f27a1df5fb69110044c0d73118effa95b01e5cf
166d3df2d721caf9b21e5fb14c616871fd84c54f9d65b283196c7fe4f60553eb
f39c6402c42234e32a356b3e764312a61a5532055716ead6962568f87d3f3f77
04c6a8d1bcd1bf4d50d6154b6da731b187b58dfd728afa36757a797ac188d1
```

The test asserts the plaintext lengths `375` and `127` before it encrypts.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 91 — RFC 8439 Poly1305 and its key generation

### Files

- `tests/Arcanum.Tests/Poly1305Tests.fs`

### Object

RFC 8439 section 2.5.2 (Poly1305) through `Poly1305.mac`, and section 2.6.2 (Poly1305 key generation) through `ChaCha20.block`.

### Laws

- `Poly1305.mac` with the 32-byte key `85d6be7857556d337f4452fe42d506a80103808afb0db2fd4abff6af4149f51b` of the ASCII message `Cryptographic Forum Research Group` (34 bytes) is `a8061dc1305136c6c22b8baf0c0127a9`.
- The first 32 bytes of `ChaCha20.block` with key `808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9f`, counter `0`, and nonce `000000000001020304050607` are `8ad5a08b905f81cc815040274ab29471a833b637e3fd0da508dbb8e2fdd1a646`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 92 — RFC 8439 Appendix A.3 Poly1305 vectors

### Files

- `tests/Arcanum.Tests/Poly1305Tests.fs`

### Object

RFC 8439 Appendix A.3, test vectors #1, #4, and #5 through #11. Vectors #5 through #11 are the cases near the reduction modulo `2^130 - 5` and the final addition modulo `2^128`. Each gives `R` and `S` separately. The `Poly1305.mac` key is `R || S`.

### Laws

- #1: key of 32 zero bytes, message of 64 zero bytes, tag of 16 zero bytes.
- #4: key `1c9240a5eb55d38af333888604f6b5f0473917c1402b80099dca5cbc207075c0`, message the 127-byte text of phase 90 test vector #3, tag `4541669a7eaaee61e708dc7cbcc5eb62`.

| # | R | S | data | tag |
|---|---|---|---|---|
| 5 | `02000000000000000000000000000000` | `00000000000000000000000000000000` | `ffffffffffffffffffffffffffffffff` | `03000000000000000000000000000000` |
| 6 | `02000000000000000000000000000000` | `ffffffffffffffffffffffffffffffff` | `02000000000000000000000000000000` | `03000000000000000000000000000000` |
| 7 | `01000000000000000000000000000000` | `00000000000000000000000000000000` | `ffffffffffffffffffffffffffffffff` `f0ffffffffffffffffffffffffffffff` `11000000000000000000000000000000` | `05000000000000000000000000000000` |
| 8 | `01000000000000000000000000000000` | `00000000000000000000000000000000` | `ffffffffffffffffffffffffffffffff` `fbfefefefefefefefefefefefefefefe` `01010101010101010101010101010101` | `00000000000000000000000000000000` |
| 9 | `02000000000000000000000000000000` | `00000000000000000000000000000000` | `fdffffffffffffffffffffffffffffff` | `faffffffffffffffffffffffffffffff` |
| 10 | `01000000000000000400000000000000` | `00000000000000000000000000000000` | `e33594d7505e43b90000000000000000` `3394d7505e4379cd0100000000000000` `00000000000000000000000000000000` `01000000000000000000000000000000` | `14000000000000005500000000000000` |
| 11 | `01000000000000000400000000000000` | `00000000000000000000000000000000` | `e33594d7505e43b90000000000000000` `3394d7505e4379cd0100000000000000` `00000000000000000000000000000000` | `13000000000000000000000000000000` |

Multi-block data is the listed 16-byte rows joined in order. Each row of the table is one assertion.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 93 — RFC 8439 AEAD

### Files

- `tests/Arcanum.Tests/AeadTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

RFC 8439 section 2.8.2 through `ChaCha20Poly1305.encrypt` and `decrypt`. The 12-byte nonce is the 32-bit fixed part `07000000` followed by the IV `4041424344454647`.

### Laws

Key `808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9f`, nonce `070000004041424344454647`, AAD `50515253c0c1c2c3c4c5c6c7`, and plaintext the 114-byte sunscreen text of phase 89:

- `encrypt` returns the ciphertext

```
d31a8d34648e60db7b86afbc53ef7ec2a4aded51296e08fea9e2b5a736ee62d6
3dbea45e8ca9671282fafb69da92728b1a71de0a9e060b2905d6a5b67ecd3b36
92ddbd7f2d778b8c9803aee328091b58fab324e4fad675945585808b4831d7bc
3ff4def08e4b7a9de576d26586cec64b6116
```

  and the tag `1ae10b594f09e26a7e902ecbd0600691`.
- `decrypt` of that ciphertext and tag returns the plaintext.
- `decrypt` with the low bit of the tag's first byte flipped returns `Error AuthenticationFailed`.
- `decrypt` with the last AAD byte changed to `c8` returns `Error AuthenticationFailed`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 94 — RFC 8439 Appendix A.5 AEAD decryption

### Files

- `tests/Arcanum.Tests/AeadTests.fs`

### Object

RFC 8439 Appendix A.5. A received ciphertext and tag are checked and decrypted. The plaintext contains two UTF-8 quotation marks (`e2 80 9c` and `e2 80 9d`), so it is given in hex, not as ASCII.

### Laws

Key `1c9240a5eb55d38af333888604f6b5f0473917c1402b80099dca5cbc207075c0`, nonce `000000000102030405060708`, AAD `f33388860000000000004e91`, tag `eead9d67890cbb22392336fea1851f38`. The 265-byte ciphertext is:

```
64a0861575861af460f062c79be643bd5e805cfd345cf389f108670ac76c8cb2
4c6cfc18755d43eea09ee94e382d26b0bdb7b73c321b0100d4f03b7f355894cf
332f830e710b97ce98c8a84abd0b948114ad176e008d33bd60f982b1ff37c855
9797a06ef4f0ef61c186324e2b3506383606907b6a7c02b0f9f6157b53c867e4
b9166c767b804d46a59b5216cde7a4e99040c5a40433225ee282a1b0a06c523e
af4534d7f83fa1155b0047718cbc546a0d072b04b3564eea1b422273f548271a
0bb2316053fa76991955ebd63159434ecebb4e466dae5a1073a6727627097a10
49e617d91d361094fa68f0ff77987130305beaba2eda04df997b714d6c6f2c29
a6ad5cb4022b02709b
```

- `decrypt` returns the 265-byte plaintext

```
496e7465726e65742d4472616674732061726520647261667420646f63756d65
6e74732076616c696420666f722061206d6178696d756d206f6620736978206d
6f6e74687320616e64206d617920626520757064617465642c207265706c6163
65642c206f72206f62736f6c65746564206279206f7468657220646f63756d65
6e747320617420616e792074696d652e20497420697320696e617070726f7072
6961746520746f2075736520496e7465726e65742d4472616674732061732072
65666572656e6365206d6174657269616c206f7220746f206369746520746865
6d206f74686572207468616e206173202fe2809c776f726b20696e2070726f67
726573732e2fe2809d
```

  which begins with the ASCII text `Internet-Drafts are draft documents`.
- `encrypt` of that plaintext with the same key, nonce, and AAD returns the same ciphertext and tag.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 95 — RFC 4231 test cases 3, 4, and 5

### Files

- `tests/Arcanum.Tests/HmacTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

RFC 4231 sections 4.4, 4.5, and 4.6 through `HMAC.hmacSha256`. Test cases 1 and 2 stay where they are in `Tests.fs`.

### Laws

- Test case 3: key 20 bytes of `aa`, data 50 bytes of `dd`, HMAC-SHA-256 `773ea91e36800e46854db8ebd09181a72959098b3ef8c122d9635514ced565fe`.
- Test case 4: key the 25 bytes `0102030405060708090a0b0c0d0e0f10111213141516171819`, data 50 bytes of `cd`, HMAC-SHA-256 `82558a389a443c0ea4cc819899f2083a85f0faa3e578f8077a2e3ff46729665b`.
- Test case 5: key 20 bytes of `0c`, data the ASCII `Test With Truncation`. RFC 4231 prints only the first 128 bits: `a3b6167473100ee06e0c796c2955552b`. The test compares the first 16 bytes of the tag. It does not add a truncation function.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 96 — RFC 4231 test cases 6 and 7

### Files

- `tests/Arcanum.Tests/HmacTests.fs`

### Object

RFC 4231 sections 4.7 and 4.8. The 131-byte key is longer than the 64-byte SHA-256 block. `HMAC.hmacSha256` must hash it first, a branch no cited test has reached yet.

### Laws

- Test case 6: key 131 bytes of `aa`, data the ASCII `Test Using Larger Than Block-Size Key - Hash Key First` (54 bytes), HMAC-SHA-256 `60e431591ee0b67f0d8a26aacbf5b77f8e0bc6213728c5140546040f0ee37f54`.
- Test case 7: key 131 bytes of `aa`, data the ASCII `This is a test using a larger than block-size key and a larger than block-size data. The key needs to be hashed before being used by the HMAC algorithm.` (152 bytes, one line, single spaces), HMAC-SHA-256 `9b09ffa71b942fcb27635fbcd5b0e944bfdc63644f0713938a7f51535c3a35e2`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 97 — RFC 7914 section 11, the second vector

### Files

- `tests/Arcanum.Tests/Pbkdf2Tests.fs`

### Object

The second PBKDF2-HMAC-SHA256 vector of RFC 7914 section 11. Phase 50 locked the first, at `c = 1`. This is the first cited check with more than one iteration. It is one vector, not a CAVP file.

### Laws

`PBKDF2.deriveKey` of password `Password`, salt `NaCl`, `c = 80000`, `dkLen = 64` is:

```
4ddcd8f60b98be21830cee5ef22701f9641a4418d04c0414aeff08876b34ab56
a1d425a1225833549adb841b51c9b3176a272bdebba1d078478f62b397f33c8d
```

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 98 — Miller-Rabin with fixed bases

### Files

- `src/Arcanum.Core/Primes.fs`, adding `millerRabinWithBases`
- `tests/Arcanum.Tests/PrimeTests.fs`

### Object

`Primes.millerRabinWithBases : BigInteger -> BigInteger list -> bool` (`n`, bases). No random source. `n < 2` is false. `n = 2` is true. Any other even `n` is false. Otherwise write `n - 1 = d * 2^s` with `d` odd. For each base `a`, reduce it modulo `n` and skip it if that is `0`. Compute `x = a^d mod n`. If `x` is `1` or `n - 1`, the base passes. Otherwise square up to `s - 1` times, and the base passes if `n - 1` appears. If any base fails, the result is false. The comment says a true result means probable prime, not proved prime.

### Laws

- `561 = 3 * 11 * 17` is a Carmichael number. `modPow 2 560 561` is `1`, so a Fermat test to base 2 passes. `millerRabinWithBases 561 [2]` is false. By hand, `560 = 35 * 2^4`, and the sequence `2^35, 2^70, 2^140, 2^280` modulo 561 is `263, 166, 67, 1`. `67^2 = 4489 = 8 * 561 + 1`, so `67` is a square root of 1 other than `1` and `560`.
- `3215031751 = 151 * 751 * 28351`. `millerRabinWithBases 3215031751 [2; 3; 5; 7]` is true, and `millerRabinWithBases 3215031751 [11]` is false.
- With the first twelve primes `[2; 3; 5; 7; 11; 13; 17; 19; 23; 29; 31; 37]` as bases: `2`, `3`, `17`, `19`, and `97` are true, and `1`, `4`, `91`, `561`, `1105`, and `3215031751` are false.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 99 — Large-curve parameters are probable primes

### Files

- `tests/Arcanum.Tests/PrimeTests.fs`

### Object

The field primes and group orders of `secp256k1` and `nistP256` pass `millerRabinWithBases` with the first twelve primes. The test calls each one a probable prime. It is not a proof. It does not enumerate a group or take a discrete logarithm.

### Laws

- `millerRabinWithBases` is true for `secp256k1.P`, `secp256k1.N`, `nistP256.P`, and `nistP256.N` with the bases of phase 98.
- `millerRabinWithBases (secp256k1.P + 2I)` with the same bases is false. This shows the test can fail. `secp256k1.P + 2 = 2^256 - 2^32 - 975`, which is `1 - 1 - 0 = 0` modulo 3, because `2^256 = 1`, `2^32 = 1`, and `975 = 3 * 325`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 100 — Lucas certificates for 17 and 19

### Files

- `src/Arcanum.Core/Primes.fs`, adding `lucasCertificate`
- `tests/Arcanum.Tests/PrimeTests.fs`

### Object

Lucas's test proves `n` prime when some `a` has order exactly `n - 1` modulo `n`. `Primes.lucasCertificate : BigInteger -> BigInteger -> BigInteger list -> bool` (`n`, witness `a`, the distinct primes `q` dividing `n - 1`):

- `n` must be greater than 2.
- Dividing `n - 1` by each listed `q` as many times as it goes must leave `1`. Otherwise the list is incomplete and the result is false.
- `a^(n - 1) mod n` must be `1`, and for every listed `q`, `a^((n - 1) / q) mod n` must not be `1`.

The comment says the listed `q` must themselves be prime and the function does not check that. Here they are `2` and `3`. No factoring routine is added.

### Laws

- `lucasCertificate 17 3 [2]` is true: `3^8 = 16` and `3^16 = 1` modulo 17. So `3` is a primitive root and 17 is prime. `ToyField` is a field.
- `lucasCertificate 17 2 [2]` is false: `2^8 = 1` modulo 17, so 2 has order 8.
- `lucasCertificate 19 2 [2; 3]` is true: `2^9 = 18`, `2^6 = 7`, and `2^18 = 1` modulo 19. So 19, the order of `toy3`, is prime.
- `lucasCertificate 19 2 [2]` is false, because `18 / 2 = 9` leaves `9`, not `1`.
- The powers `3^0 .. 3^15` modulo 17 are `1, 3, 9, 10, 13, 5, 15, 11, 16, 14, 8, 7, 4, 12, 2, 6`, which are the 16 non-zero residues, each once.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 101 — Batch inversion

### Files

- `src/Arcanum.Core/Modular.fs`, adding `batchInverse`
- `tests/Arcanum.Tests/ModularLawsTests.fs`

### Object

Montgomery's trick. `Modular.batchInverse : BigInteger list -> BigInteger -> Result<BigInteger list, CryptoError>` (values, modulus). Form the prefix products. Invert the last product once with `modInverse`. Walk back to recover each inverse. Return them in input order. A value that is not invertible makes the whole result an error. The empty list returns `Ok []`. The comment says there is one call to `modInverse` instead of one per value.

### Laws

- `batchInverse [1 .. 16] 17` is `Ok [1; 9; 6; 13; 7; 3; 5; 15; 2; 12; 14; 10; 4; 11; 8; 16]`, and each entry equals `modInverse` of its input.
- `batchInverse [] 17` is `Ok []`.
- `batchInverse [3; 0; 5] 17` and `batchInverse [3; 17] 17` return an error.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 102 — Reduction modulo the secp256k1 prime

### Files

- `src/Arcanum.Core/Modular.fs`, adding `reduceSecp256k1`
- `tests/Arcanum.Tests/ModularLawsTests.fs`

### Object

`p = 2^256 - 2^32 - 977`, so `2^256 = c` modulo `p` with `c = 2^32 + 977 = 4294968273`. `Modular.reduceSecp256k1 : BigInteger -> BigInteger` takes `0 <= x < p^2`. While `x >= 2^256`, write `x = H * 2^256 + L` and replace `x` by `H * c + L`. Then subtract `p` while `x >= p`. An input outside `0 .. p^2 - 1` raises `ArgumentOutOfRangeException`. The comment says this is a second reduction checked against `modPos`, with no constant-time claim. The prime is written in the module as `2^256 - 2^32 - 977`, and the test checks it equals `secp256k1.P`.

### Laws

With `p = secp256k1.P`:

- For each of `0`, `1`, `p - 1`, `p`, `p + 1`, `2^256 - 1`, `2^256`, `(p - 1)^2`, `Gx * Gy`, `Gx^2`, and `p^2 - 1`, `reduceSecp256k1 x = modPos x p`.
- `reduceSecp256k1 (2^256) = 4294968273`.
- `reduceSecp256k1 ((p - 1)^2) = 1`, because `p - 1 = -1`.
- `reduceSecp256k1 (p * p)` raises `ArgumentOutOfRangeException`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 103 — Reduction modulo the P-256 prime

### Files

- `src/Arcanum.Core/Modular.fs`, adding `reduceP256`
- `tests/Arcanum.Tests/ModularLawsTests.fs`

### Object

FIPS 186-4 Appendix D.2.3. `p = 2^256 - 2^224 + 2^192 + 2^96 - 1`. Write `0 <= A < p^2` as sixteen 32-bit words, `A = (A15 || ... || A0)`. Then `B = T + 2 S1 + 2 S2 + S3 + S4 - D1 - D2 - D3 - D4`, where each term is eight 32-bit words, most significant first:

```
T  = ( A7  || A6  || A5  || A4  || A3  || A2  || A1  || A0  )
S1 = ( A15 || A14 || A13 || A12 || A11 || 0   || 0   || 0   )
S2 = ( 0   || A15 || A14 || A13 || A12 || 0   || 0   || 0   )
S3 = ( A15 || A14 || 0   || 0   || 0   || A10 || A9  || A8  )
S4 = ( A8  || A13 || A15 || A14 || A13 || A11 || A10 || A9  )
D1 = ( A10 || A8  || 0   || 0   || 0   || A13 || A12 || A11 )
D2 = ( A11 || A9  || 0   || 0   || A15 || A14 || A13 || A12 )
D3 = ( A12 || 0   || A10 || A9  || A8  || A15 || A14 || A13 )
D4 = ( A13 || 0   || A11 || A10 || A9  || 0   || A15 || A14 )
```

`Modular.reduceP256 : BigInteger -> BigInteger` returns `B` brought into `0 .. p - 1` by adding or subtracting `p`. An input outside `0 .. p^2 - 1` raises `ArgumentOutOfRangeException`. The comment cites FIPS 186-4 D.2.3 and makes no constant-time claim.

### Laws

With `p = nistP256.P`:

- For each of `0`, `1`, `p - 1`, `p`, `2^256 - 1`, `2^256`, `(p - 1)^2`, `Gx * Gy`, `Gx^2`, and `p^2 - 1`, `reduceP256 x = modPos x p`.
- `reduceP256 (2^256) = 2^224 - 2^192 - 2^96 + 1`, which is `fffffffeffffffffffffffffffffffff000000000000000000000001`.
- `reduceP256 ((p - 1)^2) = 1`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 104 — The order of every toy point

### Files

- `src/Arcanum.Core/DiscreteLog.fs`
- `tests/Arcanum.Tests/StructureTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

Rename the private `pointOrderByRepeatedAdd` to a public `DiscreteLog.order : Curve -> ECPoint -> BigInteger`, with the same body. `order` of `Infinity` is `1`. `pohligHellman` calls the new name. The comment says it adds a point to itself until it reaches `Infinity`, and is for the toy curves only.

### Laws

Over every point of each curve, the number of points of each order is:

- `toy`: order 1: 1, order 2: 1, order 3: 2, order 6: 2, order 9: 6, order 18: 6. These are the counts `phi(d)` for each divisor `d` of 18, as in a cyclic group of order 18.
- `toy2`: order 1: 1, order 2: 3, order 4: 12. No point has order 8 or 16, so the group of 16 points is not cyclic.
- `toy3`: order 1: 1, order 19: 18.
- `order toy toy.G = 18`, `order toy2 toy2.G = 4`, `order toy3 toy3.G = 19`.
- The `pohligHellman` laws of phase 68 still pass.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 105 — Two-torsion, and `toy2` is `Z/4 x Z/4`

### Files

- `tests/Arcanum.Tests/StructureTests.fs`

### Object

A point of order 2 is a point with `y = 0`, whose `x` is a root of `x^3 + a x + b`. In `toy2`, one second point `Q` and the generator `G` together produce every point.

### Laws

- On `toy`, the only root of `x^3 + 1` modulo 17 is `16`, because `16 = -1`. The only point of order 2 is `(16, 0)`, and it equals `scalarMultiply toy 9 G`.
- On `toy2`, the roots of `x^3 + x = x (x^2 + 1)` are `0`, `4`, and `13`, because `4^2 = 16 = -1`. The points of order 2 are `(0, 0)`, `(4, 0)`, and `(13, 0)`.
- On `toy3`, `x^3 + 2x + 2` has no root modulo 17, and no point has order 2. This is consistent with the odd order 19.
- On `toy2`, with `G = (1, 6)` and `Q = (3, 8)`:
  - The multiples of `G` are `Infinity, (1, 6), (0, 0), (1, 11)`.
  - The multiples of `Q` are `Infinity, (3, 8), (13, 0), (3, 9)`.
  - For `a` and `b` in `0 .. 3`, the 16 points `aG + bQ` are distinct and are all 16 points of `toy2`. So `toy2` is `Z/4 x Z/4`.
  - `2G + 2Q = (4, 0)`, the third point of order 2.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 106 — SEC 1 round-trip on every toy point

### Files

- `tests/Arcanum.Tests/Sec1Tests.fs`

### Object

Phase 57 checked SEC 1 on a few points of `toy`. This phase walks every point of the three toy curves in both forms.

### Laws

For every point of `toy` (18), `toy2` (16), and `toy3` (19), compressed and uncompressed:

- `decodePoint curve (encodePoint curve form Q)` is `Ok Q`.
- The encoding is 1 byte for `Infinity`, 2 bytes compressed, and 3 bytes uncompressed.
- Within one curve and one form, no two points share an encoding.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 107 — ECDH agreement on every pair of `toy3` keys

### Files

- `tests/Arcanum.Tests/ToyEcdhTests.fs`

### Object

Phase 25 checked one ECDH exchange on `toy`. This phase walks every pair of private keys on the prime-order curve.

### Laws

For every `d1` and `d2` in `1 .. 18`:

- `computeEcdhSharedSecret toy3 d1 (d2 G)` equals `computeEcdhSharedSecret toy3 d2 (d1 G)`, and both equal `scalarMultiply toy3 (d1 * d2 mod 19) G`.
- `computeEcdhSharedSecretChecked` returns `Ok` of that same point. It is never `Infinity`, because 19 is prime and neither key is a multiple of it.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 108 — Non-adjacent form

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `naf`
- `tests/Arcanum.Tests/NafTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`EllipticCurve.naf : BigInteger -> int list` is the non-adjacent form, least significant digit first. While `k > 0`: if `k` is odd, the digit is `2 - (k mod 4)`, which is `1` or `-1`, and `k` decreases by that digit. Otherwise the digit is `0`. Then halve `k`. A negative input raises `ArgumentOutOfRangeException`.

### Laws

- `naf 0 = []`, `naf 1 = [1]`.
- `naf 7 = [-1; 0; 0; 1]`, because `7 = 8 - 1`.
- `naf 29 = [1; 0; -1; 0; 0; 1]`, because `29 = 32 - 4 + 1`.
- `naf 255 = [-1; 0; 0; 0; 0; 0; 0; 0; 1]`, because `255 = 256 - 1`. Two non-zero digits instead of eight.
- `naf 1000 = [0; 0; 0; 1; 0; -1; 0; 0; 0; 0; 1]`, because `1000 = 1024 - 32 + 8`.
- For every `k` in `0 .. 2000`: the digits rebuild `k` as the sum of `digit * 2^i`, no two adjacent digits are both non-zero, and the list has at most `bitLength k + 1` digits.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 109 — Scalar multiplication by NAF

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `scalarMultiplyNaf`
- `tests/Arcanum.Tests/NafTests.fs`

### Object

`EllipticCurve.scalarMultiplyNaf : Curve -> BigInteger -> ECPoint -> ECPoint`. Reduce the scalar modulo `N`, as `scalarMultiply` does. Start from `Infinity` and walk the `naf` digits from the most significant. At each digit, double, then add `P` for `1` or `negate P` for `-1`. It is a third scalar multiplication checked against `add`. No constant-time claim.

### Laws

- For every point `Q` and every `k` in `0 .. N - 1` on `toy`, `toy2`, and `toy3`, `scalarMultiplyNaf curve k Q = scalarMultiply curve k Q`.
- On `secp256k1` and `nistP256`: `1 G = G`, `2 G = double G`, and `(N - 1) G = negate G`.
- `scalarMultiplyNaf secp256k1 2 G` is the phase 44 point `(c6047f9441ed7d6d3045406e95c07cd85c778e4b8cef3ca7abac09b95c709ee5, 1ae168fea63dc339a3c58419466ceaeef7f632653266d0e1236431a950cfe52a)`.
- `scalarMultiplyNaf nistP256 x G` with the RFC 6979 section A.2.5 key `x = C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721` is `U = (60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6, 7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299)`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 110 — Width-w NAF

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `wnaf` and `scalarMultiplyWnaf`
- `tests/Arcanum.Tests/NafTests.fs`

### Object

- `EllipticCurve.wnaf : int -> BigInteger -> int list` (`w`, `k`), least significant digit first, for `w >= 2`. While `k > 0`: if `k` is odd, the digit is `k mod 2^w`, minus `2^w` when it is at least `2^(w-1)`, and `k` decreases by that digit. Otherwise the digit is `0`. Then halve `k`. `w < 2` or a negative `k` raises `ArgumentOutOfRangeException`.
- `EllipticCurve.scalarMultiplyWnaf : Curve -> int -> BigInteger -> ECPoint -> ECPoint` reduces the scalar modulo `N`. It precomputes `P, 3P, 5P, ..., (2^(w-1) - 1) P` by repeated `add` of `2P`, then walks the digits from the top: double, then add the stored multiple for a positive digit or its negation for a negative digit.

### Laws

- `wnaf 4 7 = [7]`. `wnaf 4 29 = [-3; 0; 0; 0; 0; 1]`, because `29 = 32 - 3`. `wnaf 4 255 = [-1; 0; 0; 0; 0; 0; 0; 0; 1]`. `wnaf 4 1000 = [0; 0; 0; -3; 0; 0; 0; 0; 0; 0; 1]`, because `1000 = 1024 - 24`.
- For every `k` in `0 .. 2000`: `wnaf 2 k = naf k`. The `wnaf 4` digits rebuild `k`, every non-zero digit is odd with absolute value below 8, and any four consecutive digits hold at most one non-zero digit.
- For every point `Q` and every `k` in `0 .. N - 1` on the three toy curves, `scalarMultiplyWnaf curve 4 k Q = scalarMultiply curve k Q`.
- `scalarMultiplyWnaf nistP256 4 x G` is the phase 109 point `U`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 111 — Shamir's trick

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `multiScalarMultiply`
- `tests/Arcanum.Tests/NafTests.fs`

### Object

`EllipticCurve.multiScalarMultiply : Curve -> BigInteger -> ECPoint -> BigInteger -> ECPoint -> ECPoint` computes `u1 P1 + u2 P2` in one pass (Straus, often called Shamir's trick). Reduce both scalars modulo `N`. Precompute `P1 + P2`. Walk the bits of both scalars together from the top bit of the larger one: double, then add `P1`, `P2`, or `P1 + P2` according to the two bits. No constant-time claim.

### Laws

- On `toy3`: for every point `P2` (19), every `u1` in `0 .. 18`, and every `u2` in `0 .. 18`, `multiScalarMultiply toy3 u1 G u2 P2 = add (scalarMultiply u1 G) (scalarMultiply u2 P2)`. That is 6859 cases. Do not sample.
- On `toy`: for `P2 = (0, 1)` and every `u1` and `u2` in `0 .. 17`, the same equation holds.
- On `nistP256`: `multiScalarMultiply 1 G 1 U = add G U`, with `U` from phase 109.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 112 — ECDSA verification by Shamir's trick

### Files

- `src/Arcanum.Asymmetric/Ecdsa.fs`
- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

`Ecdsa.verify` computes `u1 G + u2 Q` with two `scalarMultiply` calls and one `add`. Replace those three calls with one `multiScalarMultiply curve u1 curve.G u2 pubKey`. Every refusal stays as it is.

### Laws

- Every existing ECDSA law passes unchanged: phases 26 and 27 as moved by phase 55, phases 56 and 83, the `secp256k1` round-trip, and phase 76 on RFC 6979.
- On `toy3`, for the phase 55 key `d = 5` and public key `(9, 16)`, `verify` accepts `(3, 14)` on `"ec"` and `(3, 11)` on `"ex"`, and rejects `(3, 14)` on `"ex"`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 113 — Mixed Jacobian-affine addition

### Files

- `src/Arcanum.Core/Jacobian.fs`, adding `addMixed`
- `tests/Arcanum.Tests/JacobianTests.fs`

### Object

`Jacobian.addMixed : Curve -> JacobianPoint -> ECPoint -> JacobianPoint` adds an affine point, which is `Z2 = 1`. If the affine point is `Infinity`, return the first point. If `Z1 = 0`, return `ofAffine` of the affine point. Otherwise `U1 = X1`, `S1 = Y1`, `U2 = x2 Z1^2`, and `S2 = y2 Z1^3`, and the rest follows the phase 62 addition with `Z2 = 1`, including the double and `Infinity` branches. `Z3 = H Z1`. The comment states the formulas. No constant-time claim.

### Laws

For every pair `Q`, `R` of points on `toy`, `toy2`, and `toy3`, with `Q` lifted using `lambda = 3` as in phase 62:

`toAffine (addMixed curve Q' R) = EllipticCurve.add curve Q R`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 114 — Jacobian scalar multiplication

### Files

- `src/Arcanum.Core/Jacobian.fs`, adding `scalarMultiply`
- `tests/Arcanum.Tests/JacobianTests.fs`

### Object

`Jacobian.scalarMultiply : Curve -> BigInteger -> ECPoint -> ECPoint`. Reduce the scalar modulo `N`. Walk its bits from the top with `Jacobian.double` and `addMixed` of the affine input. Convert to affine once at the end with `toAffine`. No constant-time claim.

### Laws

- For every point `Q` and every `k` in `0 .. N - 1` on the three toy curves, `Jacobian.scalarMultiply curve k Q = EllipticCurve.scalarMultiply curve k Q`.
- On `nistP256`, with the RFC 5903 section 8.1 values that phase 77 locked:
  - `i = C88F01F510D9AC3F70A292DAA2316DE544E9AAB8AFE84049C62A9C57862D1433` gives `(DAD0B65394221CF9B051E1FECA5787D098DFE637FC90B9EF945D0C3772581180, 5271A0461CDB8252D61F1C456FA3E59AB1F45B33ACCF5F58389E0577B8990BB3)`.
  - `r = C6EF9C5D78AE012A011164ACB397CE2088685D8F06BF9BE0B283AB46476BEE53` gives `(D12DFB5289C8D4F81208B70270398C342296970A0BCCB74C736FC7554494BF63, 56FBF3CA366CC23E8157854C13C58D6AAC23F046ADA30F8353E74F33039872AB)`.
- `Jacobian.scalarMultiply secp256k1 2 G` is the phase 44 point.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 115 — Doubling with `a = -3`

### Files

- `src/Arcanum.Core/Jacobian.fs`, adding `doubleAMinus3`
- `tests/Arcanum.Tests/JacobianTests.fs`

### Object

When `a = -3`, `M = 3 X1^2 + a Z1^4 = 3 (X1 - Z1^2)(X1 + Z1^2)`. `Jacobian.doubleAMinus3 : Curve -> JacobianPoint -> Result<JacobianPoint, CryptoError>` refuses any curve with `modPos (A + 3) P <> 0`. Otherwise it is the phase 62 doubling with that `M`. Among the curves in this repository only `nistP256` has `a = -3`.

### Laws

- `doubleAMinus3` refuses `toy`, `toy2`, `toy3`, and `secp256k1`.
- On `nistP256`, for each of `G`, `U` of phase 109, and the two RFC 5903 points of phase 114, lifted with `lambda = 1` and with `lambda = 2`: `toAffine` of the result is `EllipticCurve.double` of the point.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 116 — Batch conversion to affine

### Files

- `src/Arcanum.Core/Jacobian.fs`, adding `batchToAffine`
- `tests/Arcanum.Tests/JacobianTests.fs`

### Object

`Jacobian.batchToAffine : Curve -> JacobianPoint list -> ECPoint list` converts many points with one inversion. It takes `batchInverse` of the non-zero `Z` values modulo `P` (phase 101), and maps a point with `Z = 0` to `Infinity`. The output is in input order.

### Laws

- For each toy curve, lift its points in enumeration order. Lift the `i`-th finite point to `(lambda^2 x : lambda^3 y : lambda)` with `lambda = 1 + (i mod 16)`, and `Infinity` to `ofAffine Infinity`. `batchToAffine` of that list equals `List.map toAffine` of it, and equals the original points.
- A list that mixes `ofAffine Infinity` with finite points converts each `Infinity` to `Infinity` in place.
- `batchToAffine curve []` is `[]`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 117 — The secp256k1 endomorphism

### Files

- `tests/Arcanum.Tests/Secp256k1Tests.fs`

### Object

`secp256k1` has `a = 0`, and both `p` and `N` are `1` modulo 3. So there is a cube root of unity `beta` modulo `p` and one `lambda` modulo `N` with `lambda Q = (beta x, y)` for every point `Q = (x, y)` of the group. This map is the endomorphism GLV uses. This phase checks it on two points by ordinary scalar multiplication. It takes no discrete logarithm and does not decompose a scalar, because that needs lattice reduction, which is parked.

The test derives both roots from formulas:

- `beta = 2^((p - 1) / 3) mod p`
- `lambda = 2^(2 (N - 1) / 3) mod N`

The literals, for comparison only:

- `beta = 7ae96a2b657c07106e64479eac3434e99cf0497512f58995c1396c28719501ee`
- `lambda = 5363ad4cc05c30e0a5261c028812645a122e22ea20816678df02967c1b23bd72`

### Laws

- `beta <> 1`, `beta^3 = 1`, and `beta^2 + beta + 1 = 0` modulo `p`. The same three facts hold for `lambda` modulo `N`.
- The derived `beta` and `lambda` equal the literals above.
- `scalarMultiply secp256k1 lambda G = Point(beta * Gx mod p, Gy)`.
- With `G2` the phase 44 point, `scalarMultiply secp256k1 lambda G2 = Point(beta * G2x mod p, G2y)`.
- `scalarMultiply secp256k1 (lambda^2 mod N) G = Point(beta^2 * Gx mod p, Gy)`.

The comment says the pairing matters. `2^((N - 1) / 3) mod N` is the other cube root, and it pairs with `beta^2`, not with `beta`. It also says P-256 has `a = -3`, so it has no such map.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 118 — Every nonce on `toy3`

### Files

- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

Sign `"ec"` (representative `3`) on `toy3` with the phase 55 key `d = 5` under every nonce `k` in `1 .. 18`. Since `s = k^(-1) (3 + 5 r)`, two nonces `k` and `19 - k` share `r` and give opposite `s`.

### Laws

- `signWithNonce` returns an error exactly for `k` in `{7, 9, 10, 12}`:
  - `7G = (0, 6)` and `12G = (0, 11)`, so `r = 0`.
  - `9G = (7, 6)` and `10G = (7, 11)`, so `r = 7`, and `3 + 7 * 5 = 38 = 0` modulo 19, so `s = 0`.
- The other fourteen signatures are:

| k | (r, s) | k | (r, s) |
|---|---|---|---|
| 1 | (5, 9) | 11 | (13, 1) |
| 2 | (6, 7) | 13 | (16, 2) |
| 3 | (10, 5) | 14 | (9, 17) |
| 4 | (3, 14) | 15 | (3, 5) |
| 5 | (9, 2) | 16 | (10, 14) |
| 6 | (16, 17) | 17 | (6, 12) |
| 8 | (13, 18) | 18 | (5, 10) |

- `verify` accepts each of the fourteen on `"ec"` under `(9, 16)`.
- For `"ex"` (representative `10`), the same sweep refuses only `k = 7` and `k = 12`, because `10 + 35 = 45 = 7` modulo 19 is not `0`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 119 — Public-key recovery on `toy3`

### Files

- `src/Arcanum.Asymmetric/Ecdsa.fs`, adding `recoverPublicKeys`
- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

SEC 1 section 4.1.6. `Ecdsa.recoverPublicKeys : Curve -> byte[] -> EcdsaSignature -> ECPoint list` (curve, message, signature). With `e = messageRepresentative curve message`, for `j = 0, 1, ...` while `r + j N < P`:

- Let `x = r + j N`. For `yBit` `0` then `1`, take `R = decompress curve (Compressed(x, yBit))`. Skip it when it is `None`, or when `multiplyUnreduced curve N R` is not `Infinity`.
- `Q = r^(-1) (s R - e G)` modulo `N`, built from `scalarMultiply`, `negate`, and `add`.
- Keep `Q` only when `verify curve Q message signature` is true. SEC 1 step 1.6.2 says the same.

The output is in that order: even `y` of `R` first, matching the SEC 1 prefixes `02` then `03`. A signature out of range returns `[]`.

### Laws

- On `toy3`, `recoverPublicKeys toy3 "ec" (3, 14)` is `[Point(0, 11); Point(9, 16)]`. By hand, `x = 3` gives `R = (3, 16) = 15G` (even) and `R = (3, 1) = 4G` (odd). With `3^(-1) = 13` modulo 19, the even `R` gives `13 * (14 * 15 - 3) = 12`, so `12G = (0, 11)`. The odd `R` gives `13 * (14 * 4 - 3) = 5`, so `5G = (9, 16)`, the true key.
- For every `d` and `k` in `1 .. 18` for which `signWithNonce` of `"ec"` succeeds (272 signatures), the recovered list contains `scalarMultiply toy3 d G`. 256 lists hold two points and 16 hold one. In those 16, the other candidate is `Infinity`, and `verify` refuses `Infinity`.
- One of those 16 by name: `d = 1`, `k = 8` gives `(13, 2)`, and the list is `[G]`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 120 — Recovering the RFC 6979 public key

### Files

- `tests/Arcanum.Tests/Rfc6979Tests.fs`

### Object

Recover `U` from the published RFC 6979 section A.2.5 signatures alone. `r + N >= P` for both signatures, so only `j = 0` is tried, and the square root uses the `p = 3 mod 4` fast path.

### Laws

- `recoverPublicKeys nistP256 "sample" (r, s)` with the phase 76 signature has two points. The first, from the even `R`, is `U`. The second is `(7f57fdb5eca3f088decb4937710c709185bd17ff938fc41cfd424249db4a2c3e, 58b68842e9cfeb1b1c648966cba4e790b3ce16d067184136f67b8bb3fbd5ba36)`.
- `recoverPublicKeys nistP256 "test" (r, s)` has two points. The first is `U`. The second is `(811406049e3599d8d8747bbd9b6b3b7b0e9e46a65af69c23eba08c626dcd5693, 6a4b877c3a3a9d4b9ee9b7b74d20bf232134139e0770a4e0f20fd62a49258a3c)`.
- `verify` accepts each signature under each recovered point. That is why one signature does not identify its signer.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 121 — Malleability

### Files

- `tests/Arcanum.Tests/Rfc6979Tests.fs`
- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

If `(r, s)` verifies, so does `(r, N - s)`, because `-k` gives the point `-R`, which has the same `x`. This repository does not normalize `s`, and RFC 6979 does not either.

### Laws

- On `nistP256`, `verify` accepts `(r, N - s)` for both phase 76 signatures. For `"sample"`, `N - s = 0834E36AD29A83BF2BC9385E491D6099C8FDF9D1ED67AA7EA5F51F93782857A9`. For `"test"`, `N - s = FE60BEEB8BD5D4EC42DA6D94B639B6EA5DC07C4CD3965338E6F1887217F424CE`.
- `verify` rejects `(r, s + 1)` for both.
- The published `"sample"` `s` is greater than `N / 2` and the published `"test"` `s` is less. `Rfc6979.sign` still returns both exactly as published.
- On `toy3`, `(3, 5)` verifies on `"ec"` under `(9, 16)`. It is `(3, 19 - 14)`, and it is the `k = 15` signature of phase 118.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 122 — Related nonces

### Files

- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

Phase 27 recovered the key from a repeated nonce. This phase recovers it from two nonces in a known affine relation `k2 = a k1 + b`. From `s1 k1 = e1 + r1 d` and `s2 k2 = e2 + r2 d`:

`d = (e2 - s2 b - s2 a e1 s1^(-1)) * (s2 a r1 s1^(-1) - r2)^(-1) mod N`.

The closed form lives in the test. It is not a library function, the same as phase 27.

### Laws

On `toy3` with `d = 5`, `a = 2`, `b = 3`, `k1 = 4`, and `k2 = 2 * 4 + 3 = 11`:

- `signWithNonce` of `"ec"` with `k1` is `(3, 14)`. `signWithNonce` of `"ex"` with `k2` is `(13, 12)`, because `11G = (13, 10)` and `11^(-1) = 7`, so `7 * (10 + 13 * 5) = 7 * 75 = 12` modulo 19.
- With `s1^(-1) = 14^(-1) = 15`: the numerator is `10 - 36 - 12 * 2 * 3 * 15 = 15` and the denominator is `12 * 2 * 3 * 15 - 13 = 3` modulo 19. So `d = 15 * 3^(-1) = 15 * 13 = 5`.
- `k1 = (e1 + r1 d) s1^(-1) = 18 * 15 = 4` modulo 19.
- The test asserts both the recovered `d` and the recovered `k1`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 123 — Pollard's rho on `toy3`

### Files

- `src/Arcanum.Core/DiscreteLog.fs`, adding `pollardRho`
- `tests/Arcanum.Tests/DiscreteLogTests.fs`

### Object

`DiscreteLog.pollardRho : Curve -> ECPoint -> ECPoint -> BigInteger -> BigInteger option` (curve, base `P`, target `T`, prime order `n` of the base). If `T` is `Infinity`, return `Some 0`. Otherwise the walk tracks `(X, a, b)` with `X = aP + bT`, and its step depends on `X`, with `Infinity` in class 0:

- class `x mod 3 = 0`: `(X + P, a + 1, b)`
- class `1`: `(2X, 2a, 2b)`
- class `2`: `(X + T, a, b + 1)`

All of these are taken modulo `n`. For attempt `t = 0, 1, ..., n - 1`, start from `(tP + T, t, 1)`. Run Floyd's cycle finding from the start: the tortoise takes one step and the hare two, until their points meet. If the two `b` values agree modulo `n`, move to the next attempt. Otherwise `k = (a_t - a_h)(b_h - b_t)^(-1) mod n`. Return `Some k` only when `kP = T`. If every attempt fails, return `None`.

The comment says it is deterministic, for the toy curves only, and needs `n` prime for the division. Rho needs on the order of `sqrt n` steps. That is why `N` near `2^256` is out of reach. No such run is made here.

### Laws

- On `toy3`, for every `k` in `0 .. 18`, `pollardRho toy3 G (kG) 19` is `Some k`, and it agrees with `bruteForce`.
- On `toy`, `pollardRho toy (Point(0, 1)) G 3` is `None`, because `G` is outside the 3-point subgroup. It never returns a wrong logarithm.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 124 — Pollard's kangaroo on `toy3`

### Files

- `src/Arcanum.Core/DiscreteLog.fs`, adding `kangaroo`
- `tests/Arcanum.Tests/DiscreteLogTests.fs`

### Object

`DiscreteLog.kangaroo : Curve -> ECPoint -> ECPoint -> BigInteger -> BigInteger -> BigInteger option` (curve, base `P`, target `T`, `lo`, `hi`) looks for `k` in `lo .. hi`. The jump sizes are `[1; 2; 4]`, indexed by `x mod 3` of the current point, with `Infinity` at index `0`.

- The tame kangaroo starts at `hi * P` with distance `0` and takes exactly 2 jumps. Its end point is the trap, at distance `dt`.
- The wild kangaroo starts at `T` with distance `dw = 0`. While `dw <= (hi - lo) + dt`: if its point equals the trap, return `Some ((hi + dt - dw) mod N)`. Otherwise it jumps.
- Return `None` when it passes the bound.

When it returns, the answer is correct, because `T + dw P = (hi + dt) P`. The comment says this walk was checked to meet the trap for every case below, and that the method is for the toy curves only.

### Laws

- On `toy3`, the trap: `18G = (5, 16)`. `5 mod 3 = 2` gives a jump of 4, to `22G = 3G = (10, 6)`. `10 mod 3 = 1` gives a jump of 2, to `5G = (9, 16)`. So `dt = 6`.
- For every `k` in `0 .. 18`, `kangaroo toy3 G (kG) 0 18` is `Some k`.
- For every `k` in `5 .. 12`, `kangaroo toy3 G (kG) 5 12` is `Some k`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 125 — Shamir sharing at any threshold

### Files

- `src/Arcanum.Core/Shamir.fs`, adding `shareWith` and `reconstructMany`
- `tests/Arcanum.Tests/ShamirTests.fs`

### Object

Phase 43 is threshold 2. Add, over `ToyField`:

- `Shamir.shareWith : ToyField.Residue list -> ToyField.Residue -> Result<Share, CryptoError>` (coefficients with the secret first, `x`). It evaluates the polynomial at `x`. It refuses `x = 0` and an empty coefficient list.
- `Shamir.reconstructMany : Share list -> Result<ToyField.Residue, CryptoError>` evaluates the Lagrange interpolation of the shares at `x = 0` through `ToyField.inverse`. It refuses an empty list, a share at `x = 0`, and two shares with the same `x`.

`share` and `reconstruct` stay unchanged. F_17 is still not a place to keep a secret.

### Laws

With the polynomial `5 + 3x + 2x^2`, coefficients `[5; 3; 2]`:

- The shares at `x = 1 .. 5` are `10, 2, 15, 15, 2`. By hand: `10`, `19 = 2`, `32 = 15`, `49 = 15`, `70 = 2`, modulo 17.
- Each of the ten 3-share subsets reconstructs `5`, as do each 4-share subset and all five shares.
- `reconstructMany` of the two shares `(1, 10)` and `(2, 2)` is `1`, not `5`. It is the value at 0 of the line through them.
- For every candidate secret `s'` in `F_17`, exactly one pair `(c1, c2)` in `F_17 x F_17` has `shareWith [s'; c1; c2]` give `10` at `x = 1` and `2` at `x = 2`. Two shares rule out no secret. For `s' = 0` the pair is `(2, 8)`. The test walks all 289 pairs for each of the 17 secrets.
- `reconstructMany` of the phase 43 shares `(1, 8)` and `(2, 11)` is `5`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 126 — Hill cipher

### Files

- `src/Arcanum.Classical/Hill.fs`
- `src/Arcanum.Classical/Arcanum.Classical.fsproj`
- `tests/Arcanum.Tests/ClassicalLawsTests.fs`

### Object

A historical letter cipher. It does not replace the curve. The key is a 2-by-2 matrix `(a, b, c, d)` in row order, modulo 26, acting on column vectors of letter pairs `A = 0 .. Z = 25`:

- `Hill.encrypt : int * int * int * int -> string -> Result<string, CryptoError>` maps each pair `(p1, p2)` to `(a p1 + b p2, c p1 + d p2)` modulo 26.
- `Hill.decrypt` uses the inverse matrix `det^(-1) * (d, -b, -c, a)` modulo 26, with `det^(-1)` from `Modular.modInverse`.
- Both refuse a key whose determinant is not invertible modulo 26, and text that is not an even number of the letters `A` to `Z`. No padding.

### Laws

- Key `(3, 3, 2, 5)`: `det = 15 - 6 = 9`, and `9^(-1) = 3` modulo 26 because `27 = 1`. The inverse matrix is `(15, 17, 20, 9)`.
- `encrypt (3, 3, 2, 5) "HELP"` is `"HIAT"`. By hand: `(7, 4)` maps to `(33, 34) = (7, 8)`, which is `HI`. `(11, 15)` maps to `(78, 97) = (0, 19)`, which is `AT`.
- `decrypt (3, 3, 2, 5) "HIAT"` is `"HELP"`.
- `encrypt (3, 3, 2, 5) "SHORTEXAMPLE"` is `"XTPJRGRUDVTQ"`, and `decrypt` returns `"SHORTEXAMPLE"`.
- Keys `(2, 4, 1, 2)` (`det = 0`), `(1, 0, 0, 13)` (`det = 13`), and `(2, 0, 0, 1)` (`det = 2`) are refused.
- `"HEL"` and `"HE1P"` are refused.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 127 — Rail fence

### Files

- `src/Arcanum.Classical/Transposition.fs`
- `src/Arcanum.Classical/Arcanum.Classical.fsproj`
- `tests/Arcanum.Tests/ClassicalLawsTests.fs`

### Object

A historical transposition cipher. It does not replace the curve. `Transposition.railFenceEncrypt : int -> string -> Result<string, CryptoError>` (rails, text) writes the text in a zigzag down and up the rails, starting on the top rail, and reads the rails top to bottom. `railFenceDecrypt` inverts it. Both refuse fewer than 2 rails. Characters are moved, not changed.

### Laws

With the text `WEAREDISCOVEREDFLEEATONCE` (25 letters):

- 3 rails give `WECRLTEERDSOEEFEAOCAIVDEN`.
- 2 rails give `WAEICVRDLETNEERDSOEEFEAOC`.
- 4 rails give `WIREEEDSEEEACAECVDLTNROFO`.
- `railFenceDecrypt` with the same number of rails returns the text, for 2, 3, and 4 rails.
- 25 rails return the text unchanged, because the zigzag never turns.
- 1 rail is refused.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 128 — Docs catch-up

### Files

- `README.md`
- `Docs/agent_channel.md`, one new entry at the top for this phase. Do not rewrite entries from earlier phases.

### Object

The README keeps its structure and quickstart. It adds:

- The inclusive `randomBigInteger`, the exact RSA modulus length, the ECDSA private-key range, and the `Infinity` refusal in `computeEcdhSharedSecretChecked`.
- That `constantTimeEquals` compares every byte without an early exit, and that no constant-time claim is made anywhere in the repository.
- `millerRabinWithBases` (probable primes), `lucasCertificate`, `batchInverse`, `reduceSecp256k1`, and `reduceP256`.
- `DiscreteLog.order`, the order histograms, `toy2` as `Z/4 x Z/4`, and `pollardRho` and `kangaroo` on the toy curves only.
- `naf`, `wnaf`, `scalarMultiplyNaf`, `scalarMultiplyWnaf`, `multiScalarMultiply`, `Jacobian.addMixed`, `Jacobian.scalarMultiply`, `doubleAMinus3`, and `batchToAffine`, all as further implementations checked against `add`, with no constant-time claim.
- The secp256k1 endomorphism check, without a GLV decomposition.
- `recoverPublicKeys`, malleability without normalization, and the related-nonce recovery on `toy3`.
- `Shamir.shareWith` and `reconstructMany`, `Hill`, and `Transposition` as historical mechanisms.
- The new cited checks: FIPS 197 Appendices A.1 and B; SP 800-38A F.1.1, F.2.1, F.5.1, and F.5.2; RFC 8439 sections 2.4.2, 2.5.2, 2.6.2, and 2.8.2 and Appendices A.2 (#2 and #3), A.3 (#1, #4, #5 through #11), and A.5; RFC 4231 test cases 3 through 7; and the second vector of RFC 7914 section 11. Each is a published example, not a CAVP campaign.

Do not promise a constant-time curve, a from-scratch SHA-256, AES-GCM, another key size, a GLV decomposition, lattice reduction, or another curve model.

Confirm the channel has one entry for each phase from 80 through 128, and that the entries for phases 0 through 79 are present and unedited.

### Laws

No new mathematical law. If a sentence disagrees with a type in the code, correct the sentence.

### Exit

`dotnet test --configuration Release` passes. Commit. Push. Stop.

## Parked until Leon says otherwise

- A from-scratch SHA-256 used as an implementation in this repository.
- AES-GCM, and any GHASH known-answer file beyond the structural checks in phase 42. AES-192 and AES-256.
- RSA-OAEP known answers. The encryption seed is random.
- Low-s normalization and BIP 340 or any Bitcoin signature format.
- More curve records, including P-192, P-224, P-384, and P-521.
- Montgomery and Edwards forms. Pairings, MOV, isogenies, and rank over `Q`.
- Mordell-Weil, L-functions, Birch-Swinnerton-Dyer, and Schoof.
- LWE, lattice reduction, lattice encryption, and the GLV scalar decomposition that would need it.
- A new Enigma rotor or a historical ciphertext campaign.
- Constant-time `secp256k1` or P-256.
- The Rust application. Moving the CLI demonstrations into a second repository.
- Lean, or a Sage wrapper.
- A NuGet package.

## Order dependencies

Execute phases 80 through 128 in that order. Do not start phase `N + 1` before phase `N` is pushed. Do not return to phases 0 through 79.

- Phase 80 commits this file before any later phase relies on it.
- Phases 81 through 85 are independent repairs. Phase 83 changes `signWithNonce`, which phases 118 through 122 call. Phase 84 changes `computeEcdhSharedSecretChecked`, which phase 107 calls.
- Phases 86 through 97 touch tests only, unless a cited vector fails. Then the fix goes in the module that phase names, in that phase.
- Phase 98 adds `millerRabinWithBases`. Phase 99 uses it. Phase 100 is independent of both.
- Phase 101 adds `batchInverse`. Phase 116 uses it.
- Phases 102 and 103 are independent of the curve code.
- Phase 104 makes `DiscreteLog.order` public. Phase 105 uses it.
- Phase 106 depends on `Sec1` from phase 57. Phase 107 depends on phase 84.
- Phase 108 adds `naf`. Phases 109 and 110 use it. Phase 111 adds `multiScalarMultiply`, and phase 112 puts it inside `Ecdsa.verify`. Every later ECDSA phase runs through that `verify`.
- Phases 113 through 116 extend `Jacobian.fs`. Phase 114 uses `addMixed`. Phase 116 uses `batchInverse` from phase 101.
- Phase 117 depends only on the `secp256k1` record and the phase 44 point.
- Phases 118 through 122 depend on phases 55, 83, and 112. Phase 120 uses `recoverPublicKeys` from phase 119. Phase 121 refers to the phase 118 table.
- Phases 123 and 124 add to `DiscreteLog.fs` and compare against `bruteForce`.
- Phases 125 through 127 are independent of the curve.
- Phase 128 is last.
