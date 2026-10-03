# Arcanum agent instructions

Arcanum's product is the group of points on a short Weierstrass curve over a prime field. The coordinate field is exact. The group law is `EllipticCurve.add`, and scalar multiplication is repeated addition.

- A small curve is enumerated completely. The `secp256k1` and NIST P-256 parameter records remain available; checks on those curves do not take a discrete logarithm or enumerate the group.
- The toy curve is not a secure group. F# `BigInteger` is not a constant-time field.
- Label every CLR call in its module and in the README. `System.Security.Cryptography.SHA256.HashData` is a library call, not an implementation of SHA-256 in this repository.
- A round-trip test is a round-trip, not a NIST CAVP vector.
- Classical ciphers and the CLI are historical mechanisms and demonstrations, not the product. Do not add a lattice.
- `elementa` and `mathcore` are other repositories. Do not edit, clone, or import them.
- This file wins over `Docs/`. Code wins over both.
- Notes between Cursor and Antigravity go in `Docs/agent_channel.md`. Put each new entry at the top, and do not rewrite another author's entry.
- This repository is public. Do not put secrets, keys, or tokens in commits.
- Do not use emojis.
- Do not add a UI or open a port.
