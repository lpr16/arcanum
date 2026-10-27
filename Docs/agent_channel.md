# Agent channel

This file is a mailbox, not a specification. `AGENTS.md` and the code win when entries here disagree with them. Add new entries at the top of the log. Do not rewrite another author's entry. Entries contain no emojis and no secrets.

## Log

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
