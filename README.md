# Arcanum

> A cryptography research study in F#, centered on elliptic-curve groups.

Arcanum is a cryptography research study. Its center is the group of points on a short Weierstrass curve over a prime field. The group law is `EllipticCurve.add`, and scalar multiplication is repeated addition. Around that center, the study locks exact constructions a reader can check: modular arithmetic, binary fields, cited checks of existing symmetric code, textbook public-key arithmetic, historical letter ciphers, and one integer lattice.

The project targets F# on .NET 10. It is an educational and research codebase, not a production cryptography library. The work orders are `Docs/antigravity_work_program.md` (phases 0 through 8) and `Docs/antigravity_research_program.md` (phases 9 through 48). `Docs/agent_channel.md` records each phase.

## Coordinate field and elliptic-curve group

The mathematical core is implemented in:

- `src/Arcanum.Core/ToyField.fs`: The coordinate field is `ToyField.Residue` at `ToyField.prime` (`17`). Multiplicative inverse is `Modular.modInverse`.
- `src/Arcanum.Core/EllipticCurve.fs`:
  - Curve type is `EllipticCurve.Curve`.
  - Group law is `EllipticCurve.add`. Negation is `EllipticCurve.negate`. Doubling is `EllipticCurve.double`.
  - Scalar multiplication is `EllipticCurve.scalarMultiply`, which reduces the scalar modulo `Curve.N`. On `toy`, it agrees with repeated addition for residues `0 .. 17`.
  - `EllipticCurve.toy` is the curve `y^2 = x^3 + 1` over F_17. It has 18 points. `G = Point(6, 8)` generates them. `N` is `18`. `H` is `1`. `Point(0, 1)` has order `3`. The toy curve is not a secure group.
  - `EllipticCurve.toy2` is the curve `y^2 = x^3 + x` over F_17. It has 16 points and its group is not cyclic. `G = Point(1, 6)` has order 4 and does not generate the group. `N` is `4`, the order of `G`, and every point order divides 4. `H` is `4`. The toy2 curve is not a secure group.
  - `EllipticCurve.jInvariant` returns `-1728 * (4a)^3 / Delta` modulo `P`, with `Delta = -16 * (4a^3 + 27b^2)`, or `None` when `Delta` is zero. `jInvariant toy` is `Some 0`. `jInvariant toy2` is `Some 11`, which is 1728 modulo 17.
  - `EllipticCurve.compress` records `Infinity`, or the x coordinate and the least bit of `y`. `EllipticCurve.decompress` recovers the point with `Modular.modSqrt`, and returns `None` when the square root is missing.
  - `EllipticCurve.secp256k1` and `EllipticCurve.nistP256` are the large parameter records. Their checks are: the generator is on the curve, unreduced `N * G` is `Infinity`, and unreduced `(N - 1) * G` is `-G`. These checks do not take a discrete logarithm or enumerate the group.
- `src/Arcanum.Core/Modular.fs`: `modPos`, `xgcd`, `modInverse`, `crt`, `jacobi`, and `modSqrt` are written in F#. For a non-negative exponent, `modPow` delegates to `BigInteger.ModPow`.

The coordinates use F# `BigInteger`; this is not a constant-time field implementation.

## Other exact constructions

- `src/Arcanum.Core/Lattice.fs`: `Lattice.basis` is the 2-by-2 integer basis with rows `(2, 1)` and `(1, 2)`. Its determinant is `3`, and its Gram matrix is `[[5; 4]; [4; 5]]`. The lattice is a matrix of integers. It is not an encryption scheme.
- `src/Arcanum.Core/Shamir.fs`: `Shamir` is threshold-2 secret sharing over `ToyField`. The polynomial is `secret + coefficient * x`, and reconstruction is the Lagrange line through two shares. A share at `x = 0` and two shares with the same `x` are refused. F_17 is not a place to keep a secret.
- `src/Arcanum.Core/FiniteFields.fs` contains `GF256`, the AES field, and `GF128`, the field multiplication described in NIST SP 800-38D. This repository does not implement AES-GCM.

## CLR boundary

- `src/Arcanum.Hashing/SHA256.fs` calls `System.Security.Cryptography.SHA256.HashData`. This repository does not implement the SHA-256 compression function.
- `src/Arcanum.Hashing/HMAC.fs` constructs HMAC in F# and uses `SHA256.hash` for its hash step.
- `src/Arcanum.Hashing/PBKDF2.fs` constructs PBKDF2 in F# and uses `HMAC.hmacSha256` as its pseudorandom function.
- `Rsa.mgf1` uses `SHA256.hash` as its digest.
- `Bytes.randomBytes` and the random source in `Primes.fs` call `RandomNumberGenerator.Fill`.
- `Modular.modPow` delegates to `BigInteger.ModPow` for a non-negative exponent.

## Symmetric and public-key modules

- `src/Arcanum.Symmetric` contains `Padding.fs`, `Aes.fs`, `Modes.fs`, `ChaCha20.fs`, `Poly1305.fs`, and `ChaCha20Poly1305.fs`.
- `src/Arcanum.Asymmetric` contains `Rsa.fs`, `DiffieHellman.fs`, and `Ecdsa.fs`.
  - `DiffieHellman.rfc3526Group5` is the 1536-bit MODP group from RFC 3526 section 2, with generator 2.
  - The ECDSA message representative uses `SHA256.hash`. `Ecdsa.signWithNonce` accepts a caller-supplied nonce; that is not an implementation of RFC 6979.

## Historical mechanisms and demonstrations

- `src/Arcanum.Classical` contains historical mechanisms in `Substitution.fs`, `Polyalphabetic.fs`, `Playfair.fs`, `Enigma.fs`, and `Cryptanalysis.fs`.
- `src/Arcanum.Cli/Program.fs` is a demonstration runner, including attack demonstrations.

These are historical mechanisms and demonstrations. They do not replace the curve.

## Solution structure

```text
arcanum/
|-- src/
|   |-- Arcanum.Core/
|   |-- Arcanum.Classical/
|   |-- Arcanum.Symmetric/
|   |-- Arcanum.Hashing/
|   |-- Arcanum.Asymmetric/
|   `-- Arcanum.Cli/
`-- tests/
    `-- Arcanum.Tests/
```

F# compile items are listed explicitly in each SDK-style project file. The solution is `Arcanum.slnx`.

## Quickstart

### Prerequisite

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

Run the interactive demonstrations:

```bash
dotnet run --project src/Arcanum.Cli/Arcanum.Cli.fsproj
```

Run the tests:

```bash
dotnet test
```

Run the required Release configuration:

```bash
dotnet test --configuration Release
```

## Current test coverage

### Curves

- Complete enumeration of the 18 points on `toy`, non-singularity discriminant, identity, negation, chord and tangent manual calculations, and the order-3 point (0, 1).
- Group law closure, commutativity on all pairs, and associativity across all 5832 triples of points on `toy`.
- Point order verification via repeated addition, proof that G = (6, 8) generates the 18 points, and agreement between `scalarMultiply` and repeated addition for residues 0 .. 17.
- Negation as a homomorphism on every pair, scalar distributivity over `add` for every residue and pair, and the closed order-3 subgroup {Infinity, (0, 1), (0, 16)} on `toy`.
- `jInvariant` on `toy`, on `toy2`, and on the singular curve `y^2 = x^3`.
- Complete enumeration of the 16 points on `toy2`, the orders of `G` and (0, 0), the 4-point subgroup of `G`, one chord calculation, closure, commutativity, associativity across all 4096 triples, and agreement between `scalarMultiply` and repeated addition for residues 0 .. 3.
- `compress` and `decompress` on `toy`.
- Generator validation on `secp256k1` and NIST P-256, and unreduced double-and-add order checks (N * G = Infinity and (N - 1) * G = -G) without discrete logarithms.
- ECDH shared point equality on `toy`, ECDSA `signWithNonce` and `verify` on `toy`, and recovery of the private scalar from two signatures that reuse a nonce.

### Arithmetic and other constructions

- Exact field residues modulo 17, non-zero multiplicative inverses, and Fermat's Little Theorem.
- `xgcd` Bezout identities, `crt`, `jacobi`, and `modSqrt` on the toy prime, and trial-division primality on small primes and composites.
- GF(2^8) arithmetic and AES S-box entries, and structural checks of GF(2^128) multiplication. The GF(2^128) checks are not a GHASH vector.
- The lattice determinant and Gram matrix.
- Shamir shares, reconstruction from every pair, and refusal of invalid shares.
- Textbook RSA on a small key, decryption by the Chinese Remainder Theorem, toy Diffie-Hellman modulo 23, the shape of the RFC 3526 group 5 record, and one MGF1 counter block.

### Cited checks

Each of these is one vector, not a CAVP campaign:

- AES-128 block encryption and decryption against FIPS 197 Appendix C.1. ECB carries the same block, followed by a PKCS#7 padding block that is not a second vector.
- The ChaCha20 block function against RFC 8439 section 2.3.2.
- HMAC-SHA256 against RFC 4231 test cases 1 and 2.
- The one-iteration PBKDF2-HMAC-SHA256 answer for password `password` and salt `salt`.
- The double of the SEC 2 `secp256k1` generator.
- The results of the CLR SHA-256 library call for `"abc"` and the empty message against their published digests. These checks do not demonstrate an in-repository SHA-256 compression function.

### Round trips and demonstrations

- Caesar, Atbash, affine, Vigenere, and Playfair hand checks, index of coincidence fractions, and a Caesar break by chi-squared.
- Enigma reciprocity.
- AES-CBC, PKCS#7, and ChaCha20-Poly1305 round trips. The ChaCha20-Poly1305 fact also rejects one tampered ciphertext; it is not an RFC 8439 vector file. Poly1305 clamp and empty-message checks are structural.
- Agreement between two locally generated ECDH parties; this is not a published shared-secret vector.
- ECDSA sign-then-verify and rejection of a modified message; this is not a published signature vector.

Round-trip checks are not NIST CAVP vectors.

## License

MIT License. Created for educational and research study in cryptography.
