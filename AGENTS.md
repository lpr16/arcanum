# Arcanum agent instructions

The repository is a cryptography research study. Its center is the group of points on a short Weierstrass curve over a prime field. The group law is `EllipticCurve.add`. Scalar multiplication is repeated addition.

- Around that center, the study may lock exact constructions already named by `Docs/antigravity_research_program.md`: modular arithmetic, binary fields, cited checks of existing symmetric code, textbook public-key arithmetic, historical letter ciphers, and one integer lattice.
- The integer lattice is a matrix of integers. It is not an encryption scheme.
- The small curves `toy`, `toy2`, and `toy3` are enumerated completely, and discrete logarithms may be computed on them.
- The `secp256k1` and NIST P-256 parameter records remain available. Checks on those curves do not take a discrete logarithm or enumerate the group. They may compare against published vectors: RFC 6979 section A.2.5 and RFC 5903 section 8.1.
- The ECDSA message representative follows SEC 1: the leftmost bits of the digest, read big-endian. RFC 6979 is built on `HMAC.hmacSha256`, an F# construction on the CLR SHA-256 call.
- The toy curves are not secure groups. F# `BigInteger` is not a constant-time field.
- Label every CLR call in its module and in the README. `System.Security.Cryptography.SHA256.HashData` is a library call.
- A round-trip test is a round-trip, not a NIST CAVP vector.
- Classical ciphers and the CLI remain historical mechanisms and demonstrations. They do not replace the curve.
- `elementa` and `mathcore` are other repositories. Do not edit, clone, or import them.
- This file wins over `Docs/`. Code wins over both.
- Notes between Cursor and Antigravity go in `Docs/agent_channel.md`. A new entry goes at the top. Do not rewrite another author's entry.
- The repository is public. No secrets. No emojis. No new UI. No port.
