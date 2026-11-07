# Agent channel

This file is a mailbox, not a specification. `AGENTS.md` and the code win when entries here disagree with them. Add new entries at the top of the log. Do not rewrite another author's entry. Entries contain no emojis and no secrets.

## Log

### 2026-11-07 - Antigravity - Phase 123

Added DiscreteLog.pollardRho in DiscreteLog.fs and locked Pollard's rho across all 19 points of toy3 and subgroup refusal on toy in DiscreteLogTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 122

Locked closed-form recovery of private key d = 5 and nonce k1 = 4 from affine-related nonces k2 = 2 * k1 + 3 on toy3 in ToyEcdsaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 121

Locked ECDSA signature malleability (r, N - s) on the NIST P-256 RFC 6979 vectors in Rfc6979Tests.fs and on toy3 in ToyEcdsaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 120

Locked SEC 1 public-key recovery on the published RFC 6979 section A.2.5 NIST P-256 signatures for "sample" and "test" in Rfc6979Tests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 119

Added Ecdsa.recoverPublicKeys in Ecdsa.fs per SEC 1 section 4.1.6 and locked public-key recovery across all 272 valid (d, k) signatures of "ec" on toy3 in ToyEcdsaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 118

Locked the full nonce sweep k in 1 .. 18 on toy3 for d = 5 on "ec" (14 signatures and 4 refusals at k in {7, 9, 10, 12}) and on "ex" (2 refusals at k in {7, 12}) in ToyEcdsaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 117

Locked the secp256k1 endomorphism cube roots beta mod p and lambda mod N and verified lambda * Q = (beta * x, y) on G and 2G and lambda^2 * G = (beta^2 * Gx, Gy) in Secp256k1Tests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 116

Added Jacobian.batchToAffine in Jacobian.fs and locked batch conversion across lifted points of toy, toy2, and toy3, mixed Infinity lists, and the empty list in JacobianTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 115

Added Jacobian.doubleAMinus3 in Jacobian.fs and locked refusal of curves with a <> -3 mod P and agreement with EllipticCurve.double on NIST P-256 points in JacobianTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 114

Added Jacobian.scalarMultiply in Jacobian.fs and locked agreement across all points and scalars on toy, toy2, and toy3, RFC 5903 section 8.1 points on NIST P-256, and 2G on secp256k1 in JacobianTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 113

Added Jacobian.addMixed in Jacobian.fs and locked mixed Jacobian-affine addition across all point pairs on toy, toy2, and toy3 in JacobianTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 112

Updated Ecdsa.verify to compute u1 * G + u2 * Q with a single EllipticCurve.multiScalarMultiply call, and locked verification on toy3 in ToyEcdsaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 111

Added EllipticCurve.multiScalarMultiply (Shamir's trick) in EllipticCurve.fs and locked all 6859 cases on toy3, all 324 cases with Point(0, 1) on toy, and G + U on NIST P-256 in NafTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 110

Added EllipticCurve.wnaf and EllipticCurve.scalarMultiplyWnaf in EllipticCurve.fs and locked width-4 NAF laws across 0 .. 2000, toy curves, and NIST P-256 in NafTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 109

Added EllipticCurve.scalarMultiplyNaf in EllipticCurve.fs and locked agreement across all points and scalars on toy, toy2, and toy3, and on secp256k1 and NIST P-256 in NafTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 108

Added EllipticCurve.naf in EllipticCurve.fs and locked non-adjacent form examples and laws across 0 .. 2000 in NafTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 107

Locked ECDH agreement and non-Infinity checked shared secrets across all 324 private-key pairs (d1, d2) in 1 .. 18 on prime-order toy3 in ToyEcdhTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 106

Locked SEC 1 compressed and uncompressed encodePoint/decodePoint round-trip, length, and injectivity across all points of toy (18), toy2 (16), and toy3 (19) in Sec1Tests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 105

Locked 2-torsion points on toy, toy2, and toy3 and the Z/4 x Z/4 decomposition aG + bQ on toy2 with G = (1, 6) and Q = (3, 8) in StructureTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 104

Renamed pointOrderByRepeatedAdd to public DiscreteLog.order in DiscreteLog.fs and locked full point-order histograms on toy, toy2, and toy3 in StructureTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 103

Added Modular.reduceP256 per FIPS 186-4 Appendix D.2.3 in Modular.fs and locked agreement with modPos on [0, p^2 - 1] and out-of-range rejection in ModularLawsTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 102

Added Modular.reduceSecp256k1 in Modular.fs and locked agreement with modPos on [0, p^2 - 1] and out-of-range rejection in ModularLawsTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 101

Added Modular.batchInverse (Montgomery's trick with a single modInverse call) in Modular.fs and locked F_17 inversions, empty list, and non-invertible refusals in ModularLawsTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 100

Added Primes.lucasCertificate in Primes.fs and locked Lucas primality certificates for 17 and 19 and primitive-root powers of 3 modulo 17 in PrimeTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 99

Locked probable-prime checks via Primes.millerRabinWithBases on secp256k1.P, secp256k1.N, nistP256.P, and nistP256.N (and composite refusal on secp256k1.P + 2) in PrimeTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 98

Added Primes.millerRabinWithBases in Primes.fs and locked Carmichael number 561, strong pseudoprime 3215031751, and 12-base checks in PrimeTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 97

Locked the second PBKDF2-HMAC-SHA256 vector of RFC 7914 section 11 (`"Password"`, `"NaCl"`, `c = 80000`, `dkLen = 64`) in Pbkdf2Tests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 96

Locked RFC 4231 sections 4.7 and 4.8 (test cases 6 and 7 with 131-byte keys exceeding the 64-byte SHA-256 block size) through HMAC.hmacSha256 in HmacTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 95

Locked RFC 4231 sections 4.4, 4.5, and 4.6 (test cases 3, 4, and 5) through HMAC.hmacSha256 in HmacTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 94

Locked RFC 8439 Appendix A.5 265-byte ChaCha20-Poly1305 AEAD decryption and encryption in AeadTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 93

Locked RFC 8439 section 2.8.2 ChaCha20-Poly1305 AEAD encryption, decryption, tag bit-flip rejection, and AAD byte-modification rejection in AeadTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 92

Locked RFC 8439 Appendix A.3 Poly1305 test vectors #1, #4, and #5 through #11 in Poly1305Tests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 91

Locked RFC 8439 section 2.5.2 Poly1305 MAC and section 2.6.2 one-time Poly1305 key generation via ChaCha20.block in Poly1305Tests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 90

Locked RFC 8439 Appendix A.2 ChaCha20 encryption test vectors #2 (375 bytes across 6 blocks) and #3 (127 bytes with initial counter 42) in ChaChaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 89

Locked RFC 8439 section 2.4.2 114-byte ChaCha20 stream encryption and decryption in ChaChaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 88

Locked NIST SP 800-38A sections F.5.1 and F.5.2 (CTR-AES128.Encrypt and CTR-AES128.Decrypt) through Modes.processCtr in Sp80038aTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 87

Locked NIST SP 800-38A section F.1.1 (ECB-AES128) and section F.2.1 (CBC-AES128) 4-block vectors through Modes.encryptEcb/decryptEcb and Modes.encryptCbc/decryptCbc in Sp80038aTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 86

Locked FIPS 197 Appendix A.1 key expansion words (`w0..w3`, `w4`, `w5`, `w8`, `w20`, `w40`, `w43`) and Appendix B single-block encryption and decryption in AesBlockTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 85

Replaced constant-time claims in Bytes.fs, Padding.fs, Poly1305.fs, HMAC.fs, and ChaCha20Poly1305.fs with comments describing what the code does and stating that no constant-time claim is made, verified with a search over src/ that every remaining match for `constant time` or `constant-time` across source files (`Bytes.fs`, `EllipticCurve.fs`, `Jacobian.fs`, `HMAC.fs`, `ChaCha20Poly1305.fs`, `Padding.fs`, `Poly1305.fs`) states that no constant-time claim is made, and locked Bytes.constantTimeEquals in BytesTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 84

Updated DiffieHellman.computeEcdhSharedSecretChecked to refuse a shared point at infinity per SEC 1 section 3.3.1 step 2 while leaving computeEcdhSharedSecret unchecked, and locked small-subgroup and full-order infinity refusals on toy and toy3 in ValidationTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 83

Checked keyPair.PrivateKey in [1, n-1] before any other work in Ecdsa.signWithNonce, and locked out-of-range refusals (d = 0, 19, -1) and valid signatures (d = 1 and d = 5) on toy3 in ToyEcdsaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 82

Updated Rsa.generateKeyPair to require an even bit length of at least 16 and redraw primes when bitLength (p * q) differs from the requested length, and locked 512-bit modulus length and odd-bit rejection in RsaTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 81

Rewrote Primes.randomBigInteger to cover the inclusive range [minVal, maxVal] by bit-masking RandomNumberGenerator.Fill draws and rejecting candidates above maxVal - minVal, and locked bounds and endpoint reachability in PrimeTests.fs. `dotnet test --configuration Release` passed.

### 2026-11-07 - Antigravity - Phase 80

Committed Docs/antigravity_depth_program.md, the announcing channel entry, and added Docs/antigravity_depth_program.md (phases 80 through 128) to the work order sentence in README.md. `dotnet test --configuration Release` passed.

### 2026-11-07 - Claude Code

The long-haul program ended at phase 79 on `main`. The next work order is `Docs/antigravity_depth_program.md`, phases 80 through 128. It is not started. It stays inside the current `AGENTS.md` charter and adds no curve, hash, cipher, or key size. It repairs five defects found by reading the code, locks the existing symmetric code against FIPS 197, SP 800-38A, RFC 8439, RFC 4231, and RFC 7914, adds further scalar multiplications checked against `add`, and studies ECDSA and discrete logarithms on the toy curves. Every expected value in it was checked before it was written down: published vectors were parsed from the primary texts and recomputed by an independent implementation, and toy-curve values were derived by hand and by an independent implementation. Do not edit them to match the code.

### 2026-10-31 - Antigravity - Phase 79

Updated README.md to document toy3, coordinate range in isOnCurve, Sec1 encoding, multiplyUnreduced, validatePublicKey, Jacobian, scalarMultiplyLadder, DiscreteLog, countPoints, Hasse bound, twistCoefficients, SEC 1 messageRepresentative, Rfc6979, Rsa.decryptOaep, and the cited checks from RFC 7914 section 11, RFC 6979 sections A.1.2 and A.2.5, and RFC 5903 section 8.1. Confirmed Docs/agent_channel.md has one entry for each phase from 49 through 79 and that all entries from phases 0 through 48 remain intact. `dotnet test --configuration Release` passed. The long-haul program is complete.

### 2026-10-31 - Antigravity - Phase 78

Locked the RFC 5903 section 8.1 ECDH shared point (girx, giry) and shared secret girx on NIST P-256 via computeEcdhSharedSecret and computeEcdhSharedSecretChecked. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 77

Locked RFC 5903 section 8.1 ECDH public points (gix, giy) and (grx, gry) on NIST P-256. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 76

Added Rfc6979.sign and locked RFC 6979 section A.2.5 deterministic ECDSA signatures for "sample" and "test" on NIST P-256. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 75

Locked the RFC 6979 section A.2.5 NIST P-256 public key U via scalarMultiply, scalarMultiplyLadder, and validatePublicKey. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 74

Added Rfc6979.generateK per RFC 6979 section 3.2 and locked section A.2.5 deterministic nonces for "sample" and "test" on NIST P-256. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 73

Added Rfc6979.bits2int, Rfc6979.int2octets, and Rfc6979.bits2octets per RFC 6979 section 2.3 and locked laws on NIST P-256 and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 72

Added EllipticCurve.twistCoefficients and locked quadratic twist coefficients, refusal of quadratic residues, and #E + #E' = 2p + 2 on toy, toy2, and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 71

Locked Frobenius traces (0, 2, -1) and integer Hasse bound checks on toy, toy2, and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 70

Added EllipticCurve.countPoints using Modular.jacobi and locked point counts and enumeration agreement on toy, toy2, and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 69

Locked the small-subgroup leak of d mod 3 via Point(0, 1) on composite-order toy even when validatePublicKey passes. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 68

Added DiscreteLog.pohligHellman combining prime-power subgroup digits via bruteForce and Modular.crt, and locked laws on toy and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 67

Added DiscreteLog.babyStepGiantStep with integer ceiling square root and locked agreement with bruteForce on toy and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 66

Added DiscreteLog.bruteForce for the toy curves and locked discrete logarithm laws on toy and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 65

Locked scalarMultiplyLadder checks for 1, 2, and N - 1 on secp256k1 and NIST P-256. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 64

Added EllipticCurve.scalarMultiplyLadder and locked agreement with EllipticCurve.scalarMultiply across all points and scalars on toy, toy2, and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 63

Locked Jacobian doubling of secp256k1.G and unreduced Jacobian double-and-add order checks on secp256k1 and NIST P-256. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 62

Added Jacobian.double and Jacobian.add and locked agreement with EllipticCurve.double and EllipticCurve.add across all points and pairs of toy, toy2, and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 61

Added Jacobian.JacobianPoint, Jacobian.ofAffine, and Jacobian.toAffine and locked affine conversion and projective scaling equivalence on toy, toy2, and toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 60

Added DiffieHellman.computeEcdhSharedSecretChecked and locked the invalid-curve point Point(1, 0) parity leak and refusal on toy. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 59

Added EllipticCurve.multiplyUnreduced and EllipticCurve.validatePublicKey per SEC 1 section 3.2.2.1 and locked public-key validation laws on toy3, toy, and non-cyclic toy2. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 58

Locked SEC 1 compressed and uncompressed encoding and decoding of the secp256k1 and NIST P-256 generators. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 57

Added Sec1.encodePoint and Sec1.decodePoint per SEC 1 sections 2.3.3 and 2.3.4 and locked point encoding, decoding, and refusal laws on toy. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 56

Refused Infinity as a public key and removed the non-invertible S fallback branch in Ecdsa.verify, and locked ECDSA verify and signWithNonce refusal laws on toy3 and toy. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 55

Replaced Ecdsa.hashToInt with SEC 1 Ecdsa.messageRepresentative, updated CLI nonce-reuse demo to call it, and moved the toy ECDSA signature and nonce-reuse laws to prime-order toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 54

Locked group law closure, commutativity on all 361 pairs, associativity on all 6859 triples, and scalar multiplication agreement with repeated addition on toy3. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 53

Added EllipticCurve.toy3 of prime order 19 over F_17 and locked enumeration, point orders, tangent doubling, chord addition, and j-invariant. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 52

Added Rsa.modulusLength and Rsa.decryptOaep, corrected encryptOaep modulus byte length and big-endian I2OSP output, and locked OAEP round-trip and rejection checks. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 51

Updated EllipticCurve.isOnCurve to require 0 <= x < P and 0 <= y < P and locked out-of-range coordinate rejection on toy. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 50

Corrected two j-invariant lines in Docs/antigravity_research_program.md, updated the phase 21 PBKDF2 comment, and locked the RFC 7914 section 11 PBKDF2-HMAC-SHA256 vector. `dotnet test --configuration Release` passed.

### 2026-10-31 - Antigravity - Phase 49

Updated AGENTS.md charter for phases 49 through 79 and committed Docs/antigravity_long_haul_program.md. `dotnet test --configuration Release` passed.

### 2026-10-31 - Claude Code

The research program ended at phase 48 on `main`. The next work order is `Docs/antigravity_long_haul_program.md`, phases 49 through 79. It is not started. Every expected value in it was checked before it was written down: RFC values against the RFC texts, and toy-curve values against an independent implementation. Do not edit them to match the code.

### 2026-10-27 - Claude Code - Phase 48

Updated README to describe the research study and its center, toy and toy2, jInvariant, compress and decompress, Lattice, Shamir, the CLR boundary, the cited single vectors, and the classical modules and CLI as historical mechanisms and demonstrations. The quickstart commands are unchanged. Confirmed the channel has one entry for each phase from 9 through 48, the phase 0 through 8 entries are present and unedited, and there is no phase 2 line. Two notes for the work order: phases 37 and 38 follow the code's standard j-invariant (toy2 is Some 11, not Some 6), and the PBKDF2 test comment cites RFC 6070 and RFC 7914, neither of which contains its password/salt SHA-256 answer. The README does not repeat that citation. `dotnet test --configuration Release` passed. The research program is complete.

### 2026-10-27 - Claude Code - Phase 47

Locked `Rsa.mgf1` of an empty seed at length 32 as `SHA256.hash` of the four-byte big-endian counter 0. The expected value is the CLR library call, labeled as such. Length 0 returns an empty mask, which is what the current function does. `encryptOaep` is not called. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 46

Locked the shape of `DiffieHellman.rfc3526Group5`: G is 2, P is odd, its hex begins FFFFFFFFFFFFFFFFC90FDAA2 and ends FFFFFFFFFFFFFFFF, and P is greater than 2^1535. The law failed on the old record, which held the 1024-bit RFC 2409 Oakley Group 2 prime under the RFC 3526 name. Fixed by replacing the hex with the value printed in RFC 3526 section 2. That value was also checked outside the repository against the RFC formula 2^1536 - 2^1472 - 1 + 2^64 * ([2^1406 pi] + 741804), with pi computed by Machin's formula. The law was not changed. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 45

Locked `Modes.encryptEcb` on the phase 16 key and plaintext: the first block is the FIPS 197 Appendix C.1 ciphertext, the output is 32 bytes because PKCS#7 adds a full padding block, and `decryptEcb` returns the 16-byte plaintext. The padding block is not a second FIPS vector, and this is not a CAVP file. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 44

Locked `double secp256k1 secp256k1.G` against the literal coordinates of 2G for the SEC 2 generator, and checked that point is on the curve. `add` was not changed. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 43

Added `Shamir.fs`: threshold-2 sharing over ToyField with polynomial secret + coefficient * x and Lagrange reconstruction through `ToyField.inverse`. Locked secret 5 and coefficient 3 giving shares 8, 11, and 14 at x = 1, 2, 3, every pair reconstructing 5, and refusal of a share at 0 and of two shares at the same x. No float. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 42

Locked structural checks of `FiniteFields.GF128.multiply`: the zero block absorbs on both sides, the 0x80 and 0x02 blocks commute (and 0x80 acts as 1 in SP 800-38D bit order), and blocks that are not 16 bytes are rejected. Not a GHASH CAVP vector. No AES-GCM. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 41

Added `EllipticCurve.compress` and `decompress` with `Modular.modSqrt`. Locked on toy: (6, 8) compresses to x = 6 with bit 0 and decompresses back, bit 1 at x = 6 gives (6, 9), Infinity round-trips, and x = 3 returns None. `decompress` also returns None when no root has the recorded bit. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 40

Verified on every toy2 point that scalarMultiply k agrees with repeated addition for k in 0 .. 3, and that 4Q is Infinity by repeated addition, so reduction modulo N = 4 is sound. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 39

Verified closure and commutativity on all 256 pairs and associativity on all 4096 triples of the 16 toy2 points. `add` was not changed. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 38

Added `EllipticCurve.toy2`, y^2 = x^3 + x over F_17 with G = (1, 6), N = 4, H = 4, documented as non-cyclic and not a secure group. Locked 16 enumerated points, G of order 4, (0, 0) of order 2, a 4-point subgroup of G, and (1, 6) + (6, 1) = (11, 4). Deviation from the work order: it expects `jInvariant toy2 = Some 6`, which is the value of its sign-dropped formula. The locked law is `Some 11`, because y^2 = x^3 + x has j = 1728 and 1728 = 11 mod 17. The test shows the hand computation. `toy` is unchanged. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 37

Added `EllipticCurve.jInvariant` and locked `jInvariant toy = Some 0` and `None` for A = 0, B = 0, P = 17. Deviation from the work order: its formula `1728 * (4a)^3 * inverse(Delta)` omits a minus sign. The standard value is `-1728 * (4a)^3 / Delta`, which is c4^3 / Delta with c4 = -48a (Silverman III.1), and the code uses it. Both give 0 on toy. They differ on toy2 in phase 38, where y^2 = x^3 + x has j = 1728 = 11 mod 17, not 6. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 36

Locked the order-3 subgroup {Infinity, (0, 1), (0, 16)} on toy: closed under add, three points, not the full group of 18, and (6, 8) lies outside it. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 35

Verified on toy that scalarMultiply k distributes over add for every residue k in 0 .. 17 and every pair, and that (k + m) Q equals kQ + mQ for k and m in 0 .. 17 on every point, where reduction modulo N is part of the law. No sampling. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 34

Verified on all 324 ordered pairs of the 18 toy points that negate (Q + R) equals (-Q) + (-R), and that negation is an involution. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 33

Added `Lattice.fs` with the integer basis rows (2, 1) and (1, 2), its determinant 3, and its Gram matrix [[5; 4]; [4; 5]]. No sampler, no encryption, no LLL, and no reference to `EllipticCurve`. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 32

Locked index of coincidence as exact fractions on AAAA, ABCD, and A, and breakCaesar recovery of shift 3 on the pangram. The chi-squared score is not locked. `dotnet test --configuration Release` passed.

### 2026-10-27 - Claude Code - Phase 31

Locked the Playfair square for key MONARCHY: row 0 is M O N A R, and the same-row pair MO encrypts to ON and decrypts back. Claude Code is executing phases 31 through 48 at Leon's request. `dotnet test --configuration Release` passed.

### 2026-10-25 - Antigravity - Phase 30

Locked Vigenere encryption and decryption of HELLO with KEY and rejection of non-alphabetic key. `dotnet test --configuration Release` passed.

### 2026-10-25 - Antigravity - Phase 29

Locked affine cipher encryption, decryption, and non-coprime multiplier rejection modulo 26. `dotnet test --configuration Release` passed.

### 2026-10-25 - Antigravity - Phase 28

Locked Caesar and Atbash single-letter and word hand checks in ClassicalLawsTests.fs. `dotnet test --configuration Release` passed.

### 2026-10-25 - Antigravity - Phase 27

Verified private scalar recovery on the 18-point toy curve from two ECDSA signatures sharing nonce 7. `dotnet test --configuration Release` passed.

### 2026-10-25 - Antigravity - Phase 26

Verified ECDSA signWithNonce and verify on the toy curve with repeated-addition point checks and CLR SHA-256 representative, including non-unit S verification modulo composite N = 18. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 25

Verified ECDH shared point equality, scalar multiplication, and repeated addition on toy curve. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 24

Verified toy Diffie-Hellman exponentiations and shared secret symmetry on modulus 23. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 23

Verified RSA decryption by the Chinese Remainder Theorem matches modular exponentiation on small key. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 22

Verified textbook RSA encryption, decryption via modular exponentiation, and message bounds on small key. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 21

Verified single-iteration PBKDF2-HMAC-SHA256 against published test vector. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 20

Added HMAC-SHA256 RFC 4231 test case 2. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 19

Locked Poly1305 clamp bitmasks and empty-message s-tag identity. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 18

Verified ChaCha20 block function against RFC 8439 section 2.3.2 test vector. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 17

Locked PKCS#7 padding byte patterns, full-block expansion, and unpadding error handling. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 16

Verified AES-128 block encryption and decryption against FIPS 197 Appendix C.1 test vector. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 15

Locked AES S-box and inverse S-box entries 0x63 and 0x7c against FIPS 197. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 14

Locked GF(2^8) arithmetic against FIPS 197 section 4.2 product 0xc1 and field inverse laws. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 13

Verified deterministic trial primality on small primes and composites, and labeled random source in Primes.fs. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 12

Locked Modular.jacobi and Modular.modSqrt on the toy prime 17. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 11

Locked Modular.crt with simultaneous congruences yielding 23 and rejection of non-coprime even moduli. `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 10

Locked Modular.xgcd with Bezout identity checks on (240, 46), (15, 25), and (17, 0). `dotnet test --configuration Release` passed.

### 2026-10-10 - Antigravity - Phase 9

Updated AGENTS.md with the research charter for phases 9 through 48. `dotnet test --configuration Release` passed.

### 2026-10-10 - Cursor

The elliptic-curve program ended at phase 8 on `main`. The next work order is `Docs/antigravity_research_program.md`, phases 9 through 48. It is not started.

### 2026-10-08 - Antigravity - Phase 8

Updated README to document the exact coordinate field, toy curve group, large curve checks, and executed work program. `dotnet test --configuration Release` passed.

### 2026-10-08 - Antigravity - Phase 7

Verified secp256k1 and NIST P-256 generators lie on curve, with unreduced double-and-add order checks confirming N * G = Infinity and (N - 1) * G = -G without discrete log or modular reduction. `dotnet test --configuration Release` passed.

### 2026-10-08 - Antigravity - Phase 6

Verified point orders by repeated addition, generator G generating all 18 points, scalar multiplication agreement with repeated addition for residues 0 to 17, and scalar reduction modulo N. `dotnet test --configuration Release` passed.

### 2026-10-08 - Antigravity - Phase 5

Verified group law closure, commutativity on all pairs, and associativity across all 5832 triples on the toy curve. `dotnet test --configuration Release` passed.

### 2026-10-08 - Antigravity - Phase 4

Added the toy curve over F_17 with generator (6, 8) and order 18, and verified curve enumeration, non-singularity, identity, negation, chord and tangent calculations, and the order-3 point. `dotnet test --configuration Release` passed.

### 2026-10-03 - Cursor - Phase 3

Added the exact field of residues modulo 17, exhaustive inverse and Fermat checks, and the documented `BigInteger.ModPow` delegation for non-negative exponents. `dotnet test --configuration Release` passed.

### 2026-10-03 - Cursor - Phase 1

Reframed the README around the elliptic-curve group, labeled the other modules and current test coverage accurately, and renamed the two overclaiming SHA-256 test facts without changing their assertions. `dotnet test --configuration Release` passed.

### 2026-10-03 - Cursor - Phase 0

Added the repository ground rules, this agent mailbox, and the elliptic-curve work program. `dotnet test --configuration Release` passed.
