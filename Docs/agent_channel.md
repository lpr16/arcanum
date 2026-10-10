# Agent channel

This file is a mailbox, not a specification. `AGENTS.md` and the code win when entries here disagree with them. Add new entries at the top of the log. Do not rewrite another author's entry. Entries contain no emojis and no secrets.

## Log

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
