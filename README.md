# Arcanum

> A mathematically grounded study of elliptic-curve groups in F#.

Arcanum's product is the group of points on a short Weierstrass curve over a prime field. The repository also retains cryptographic and historical demonstrations that are explicitly separate from that mathematical core.

The project targets F# on .NET 10. It is an educational and research codebase, not a production cryptography library. The work order executed for the elliptic-curve program is documented in `Docs/antigravity_work_program.md`.

## Coordinate field and elliptic-curve group

The mathematical core is implemented in:

- `src/Arcanum.Core/ToyField.fs`: The coordinate field is `ToyField.Residue` at `ToyField.prime` (`17`). Multiplicative inverse is `Modular.modInverse`.
- `src/Arcanum.Core/EllipticCurve.fs`:
  - Curve type is `EllipticCurve.Curve`.
  - Group law is `EllipticCurve.add`. Negation is `EllipticCurve.negate`. Doubling is `EllipticCurve.double`.
  - Scalar multiplication is `EllipticCurve.scalarMultiply`, which reduces the scalar modulo `Curve.N`. On `toy`, it agrees with repeated addition for residues `0 .. 17`.
  - `EllipticCurve.toy` is the curve `y^2 = x^3 + 1` over F_17. It has 18 points. `G = Point(6, 8)` generates them. `N` is `18`. `H` is `1`. `Point(0, 1)` has order `3`. The toy curve is not a secure group.
  - `EllipticCurve.secp256k1` and `EllipticCurve.nistP256` are the large parameter records. Their checks are: the generator is on the curve, unreduced `N * G` is `Infinity`, and unreduced `(N - 1) * G` is `-G`.
- `src/Arcanum.Core/Modular.fs`: `modPos`, `xgcd`, `modInverse`, `crt`, `jacobi`, and `modSqrt` are written in F#. For a non-negative exponent, `modPow` delegates to `BigInteger.ModPow`.

The coordinates use F# `BigInteger`; this is not a constant-time field implementation.

## Other modules

The following code remains in the repository but is not the product:

- `src/Arcanum.Hashing/SHA256.fs` calls `System.Security.Cryptography.SHA256.HashData`. This repository does not implement the SHA-256 compression function.
- `src/Arcanum.Hashing/HMAC.fs` constructs HMAC in F# and uses `SHA256.hash` for its hash step.
- `src/Arcanum.Hashing/PBKDF2.fs` constructs PBKDF2 in F# and uses `HMAC.hmacSha256` as its pseudorandom function.
- `Bytes.randomBytes` and the random source in `Primes.fs` call `RandomNumberGenerator.Fill`.
- `src/Arcanum.Core/FiniteFields.fs` contains finite-field studies.
- `src/Arcanum.Symmetric` contains `Padding.fs`, `Aes.fs`, `Modes.fs`, `ChaCha20.fs`, `Poly1305.fs`, and `ChaCha20Poly1305.fs`.
- `src/Arcanum.Asymmetric` contains `Rsa.fs`, `DiffieHellman.fs`, and `Ecdsa.fs`. The ECDSA message representative uses `SHA256.hash`. `Ecdsa.signWithNonce` accepts a caller-supplied nonce; that is not an implementation of RFC 6979.
- `src/Arcanum.Classical` contains historical mechanisms in `Substitution.fs`, `Polyalphabetic.fs`, `Playfair.fs`, `Enigma.fs`, and `Cryptanalysis.fs`.
- `src/Arcanum.Cli/Program.fs` is a demonstration runner, including attack demonstrations.

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

The current suite checks:

- Caesar, Atbash, AES-CBC, and ChaCha20-Poly1305 round trips. The ChaCha20-Poly1305 fact also rejects one tampered ciphertext; it is not an RFC 8439 vector file.
- Enigma reciprocity.
- The results of the CLR SHA-256 library call for `"abc"` and the empty message against their published digests. These checks do not demonstrate an in-repository SHA-256 compression function.
- One HMAC-SHA256 case from RFC 4231.
- Agreement between two locally generated ECDH parties; this is not a published shared-secret vector.
- ECDSA sign-then-verify and rejection of a modified message; this is not a published signature vector.
- Exact field residues modulo 17, non-zero multiplicative inverses, and Fermat's Little Theorem.
- Complete enumeration of the 18 points on `toy` (y^2 = x^3 + 1 mod 17), non-singularity discriminant, identity, negation, chord and tangent manual calculations, and the order-3 point (0, 1).
- Group law closure, commutativity on all pairs, and associativity across all 5832 triples of points on `toy`.
- Point order verification via repeated addition, proof that G = (6, 8) generates the 18 points, and agreement between `scalarMultiply` and repeated addition for residues 0 .. 17.
- Generator validation on `secp256k1` and NIST P-256, and unreduced double-and-add order checks (N * G = Infinity and (N - 1) * G = -G) without discrete logarithms.

Round-trip checks are not NIST CAVP vectors.

## License

MIT License. Created for educational and research study in cryptography.
