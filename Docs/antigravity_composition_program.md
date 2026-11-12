# Antigravity composition program

Leon asked for this on 12 Nov 2026, after the depth program finished at phase 128. Claude Code wrote this plan. Antigravity executes it. Execute these phases in order. One phase is one commit on `main`, pushed to `origin/main`, plus a new entry at the top of `Docs/agent_channel.md`. Then start the next phase.

This file is the work order that follows `Docs/antigravity_depth_program.md`. That program ended at phase 128, commit `16dfbf5`. Do not redo phases 0 through 128. Number these phases 129 through 164.

The repository is `lpr16/arcanum`. `origin` is `git@github.com:lpr16/arcanum.git`. If this file disagrees with `AGENTS.md` or the code, `AGENTS.md` and the code win. Code wins over both. Phase 129 is the one phase that edits `AGENTS.md`, and it does that before any later phase relies on the wider scope.

## Goal

The pieces are each locked on their own: the P-256 group, SEC 1 encoding, ECDH, HMAC, and ChaCha20-Poly1305. Nothing yet runs them together. This program locks them as compositions, each against a published vector that crosses module boundaries:

1. It repairs three defects found by reading phases 80 through 128: `add` returns the identity when an inverse fails, `Ecdsa.sign` loops forever on an out-of-range private key, and `Rfc6979.sign` gives up where RFC 6979 says to continue.
2. It adds the NIST P-192 and P-224 records, so that RFC 6979's truncation of a 256-bit digest is checked on published vectors and Tonelli-Shanks runs its general path on a large prime.
3. It adds HKDF (RFC 5869) and DER encoding of ECDSA signatures.
4. It hashes to the curve (RFC 9380): `expand_message_xmd`, `hash_to_field`, and the simplified SWU map, exhaustively on a toy curve and against the published P-256 suites.
5. It ends with HPKE (RFC 9180 section A.5): DHKEM over P-256, HKDF-SHA256, and ChaCha20-Poly1305, in all four modes.

The center does not move. It is still the group of points on a short Weierstrass curve, with `EllipticCurve.add` as the group law. Hash-to-curve makes a curve point the output of a function, and HPKE makes the group the root of a published end-to-end vector.

## How to run

- Start from current `main`. Phase 129 commits this file if it is still untracked. Later phases do not restyle it and do not weaken a law written here.
- Read `AGENTS.md` and the top of `Docs/agent_channel.md` before each phase.
- One phase, one commit, one push. The channel entry for that phase is in the same commit. Commit message shape: `Phase N: <object>`.
- Do not amend. Do not force-push. Do not skip hooks. Do not change git config. Do not commit secrets, keys, or tokens.
- If the push fails, stop the run.
- Target framework stays `net10.0`. F# compile items stay explicit. A new file is appended at the end of its `.fsproj`. That end is always a valid dependency position for the files in this program.
- Test runner stays xUnit and FsCheck, already referenced. No new NuGet package. Phase 156 adds one project reference, `Arcanum.Asymmetric` to `Arcanum.Symmetric`; no other phase changes project references.
- The command that must pass before every commit, from the repo root:

```
dotnet test --configuration Release
```

- If a law fails, fix the code in that phase. Do not weaken a law to go green. Weakening includes changing a cited vector, point, count, or residue so the current code matches, deleting a locked case, or catching the failure and returning the identity.
- A failed law is a failed test. Do not add a proof file. No emojis.
- Channel entry shape, newest at the top: date, author `Antigravity`, the phase number, what landed, and that `dotnet test --configuration Release` passed. Do not rewrite another author's entry.

## Already finished

Phases 0 through 128 are on `main`. The channel records them. In particular:

- `toy`, `toy2`, and `toy3` are over `F_17`. `toy3` is `y^2 = x^3 + 2x + 2`, 19 points, `G = (5, 1)`. Phase 72 showed its quadratic twist by `d = 3` is `y^2 = x^3 + x + 3`, with 17 points.
- `secp256k1` and `nistP256` keep their parameters. RFC 6979 section A.2.5 and RFC 5903 section 8.1 are locked on P-256.
- `Ecdsa.messageRepresentative` keeps the leftmost `bitLength N` bits of the SHA-256 digest. `Rfc6979.bits2int`, `int2octets`, `bits2octets`, `generateK`, and `sign` exist. Only `toy3` (5 bits) has exercised the truncation.
- `Sec1.encodePoint` and `decodePoint` exist. `Modular.modSqrt` takes the `p = 3 mod 4` fast path on both large primes.
- `HMAC.hmacSha256` is locked against RFC 4231 cases 1 through 7. `ChaCha20Poly1305` is locked against RFC 8439, including section 2.8.2 and Appendix A.5.
- `Primes.millerRabinWithBases` and `Primes.lucasCertificate` exist.
- `SHA256.hash` is `System.Security.Cryptography.SHA256.HashData`.

Do not delete or change `toy`, `toy2`, `toy3`, `secp256k1`, or `nistP256`. Do not claim any toy curve is a secure group.

## Ground truth

- The repository is a public F# study of cryptography. The center is the short Weierstrass group.
- A CLR call stays labeled. A round-trip stays a round-trip. A cited vector names its publication in the test and is not the output of the function under test.
- F# `BigInteger` is not a constant-time field. Nothing in this program is constant time, and no comment says otherwise.
- Discrete logarithms are computed only on the toy curves. No check on `secp256k1`, P-192, P-224, or P-256 takes a discrete logarithm or enumerates the group.
- HKDF, `expand_message_xmd`, and HPKE are F# constructions on `HMAC.hmacSha256` and the CLR SHA-256 call.
- elementa and mathcore are other repositories. Do not edit, clone, or import them.
- No UI. No port. No Python in the build. No secrets. No emojis.

## Provenance of expected values

Every expected value in this file was checked before it was written down, by an independent implementation that shares no code with this repository. Do not edit any of them to match the code. If a law fails, the code is wrong.

- The published vectors were copied into this file by a script that reads the RFC texts, not by hand: RFC 5114 sections 2.4, 2.5, A.4, and A.5; RFC 6979 sections A.2.3 and A.2.4; RFC 5869 Appendix A.1 to A.3; RFC 9380 Appendix K.1 and sections J.1.1 and J.1.2; RFC 9180 section A.5.
- The DER encodings in phases 141 and 142 are derived from the RFC 6979 `r` and `s` by the DER rules stated in those phases.
- Toy-curve values are derived by hand in the law text.

## Hard stops

- Do not redo phases 0 through 128.
- Do not take a discrete logarithm on `secp256k1`, P-192, P-224, or P-256. Do not enumerate their groups. Do not run `pollardRho`, `kangaroo`, `order`, or `countPoints` on them.
- Do not add P-384, P-521, or any curve record not named here. Do not add Montgomery-form curves, Edwards curves, X25519, X448, pairings, MOV, isogenies, rank over `Q`, Mordell-Weil, L-functions, Birch-Swinnerton-Dyer, or Schoof.
- Do not add the secp256k1 hash-to-curve suite. It needs a 3-isogeny.
- Do not add AES-GCM. RFC 9180 sections A.1, A.3, A.4, and A.6 are out of scope. Section A.2 needs X25519 and is out of scope.
- Do not reimplement SHA-256. Do not add SHA-384, SHA-512, SHAKE, or `expand_message_xof`.
- Do not claim constant time for anything.
- Do not low-s normalize ECDSA signatures.
- Do not add an integer factoring routine or lattice reduction.
- Do not add a deterministic RSA-OAEP vector. Do not add a CLI demonstration.
- Do not invent a published vector by running the function and pasting the output back.
- Do not edit `Docs/antigravity_work_program.md`, `Docs/antigravity_research_program.md`, `Docs/antigravity_long_haul_program.md`, or `Docs/antigravity_depth_program.md`.
- Do not weaken a law to go green.

## Phase 129 — Charter

### Files

- `AGENTS.md`
- This file, if it is not already committed
- `Docs/agent_channel.md`, including the Claude Code entry that announces this program if it is not already committed

### Object

No algorithm. In `AGENTS.md`:

- In the first bullet, after "already named by `Docs/antigravity_research_program.md`", add "and by the later work orders in `Docs/`".
- Replace the bullet that begins "The small curves" with: "The small curves `toy`, `toy2`, `toy3`, and `toy3Twist` are enumerated completely, and discrete logarithms may be computed on them."
- Replace the bullet that begins "The `secp256k1` and NIST P-256 parameter records" with: "The `secp256k1`, NIST P-192, NIST P-224, and NIST P-256 parameter records remain available. Checks on those curves do not take a discrete logarithm or enumerate the group. They may compare against published vectors: RFC 6979 sections A.2.3 to A.2.5, RFC 5114 sections A.4 and A.5, RFC 5903 section 8.1, RFC 9380 section J.1, and RFC 9180 section A.5."
- At the end of the bullet that begins "The ECDSA message representative", add: "HKDF, `expand_message_xmd`, and HPKE are F# constructions on `HMAC.hmacSha256` and the CLR SHA-256 call."

Keep every other rule.

### Laws

No mathematical law. The diff does not add a `.fs` algorithm.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 130 — The group law refuses a failed inverse

### Files

- `src/Arcanum.Core/EllipticCurve.fs`
- `src/Arcanum.Core/Jacobian.fs`
- `tests/Arcanum.Tests/ToyCurveTests.fs`
- `tests/Arcanum.Tests/JacobianTests.fs`

### Object

`EllipticCurve.add` returns `Infinity` when `Modular.modInverse` of its denominator fails. That is the identity returned for an error, which earlier work orders call weakening. On a prime field with reduced coordinates the branch cannot be reached, so it only ever hides a wrong input. `Point(23, 8)` is not reduced: `23 - 6 = 17 = 0` modulo 17, so `add toy (Point(6, 8)) (Point(23, 8))` returns `Infinity` today.

Both `None` branches of `add` raise `ArgumentException`, with a message that names a non-reduced coordinate or a non-prime modulus as the cause. `Jacobian.toAffine` gets the same change for a non-zero `Z` with no inverse.

### Laws

- `add toy (Point(6, 8)) (Point(23, 8))` raises `ArgumentException`.
- `Jacobian.toAffine` on a curve record `{ toy with P = 15I }` of the point `(1 : 1 : 3)` raises `ArgumentException`, because 3 has no inverse modulo 15.
- Every earlier test still passes. In particular the 5832 toy triples, the 4096 `toy2` triples, and the 6859 `toy3` triples are unchanged.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 131 — ECDSA signing refuses an out-of-range key

### Files

- `src/Arcanum.Asymmetric/Ecdsa.fs`
- `tests/Arcanum.Tests/ToyEcdsaTests.fs`

### Object

`Ecdsa.sign` draws a nonce and retries on any error from `signWithNonce`. Since phase 83, `signWithNonce` refuses a private key outside `1 .. N - 1`, and no nonce fixes that, so `sign` loops forever. `sign` checks the private key first and returns that error. It retries only for errors that depend on the nonce: a nonce point at infinity, `r = 0`, a nonce with no inverse, and `s = 0`.

### Laws

On `toy3`, with public key `G`:

- `Ecdsa.sign` with private key `0` returns an error, and returns.
- `Ecdsa.sign` with private key `19` (which is `N`) returns an error.
- `Ecdsa.sign` with private key `5` returns a signature that `verify` accepts on the same message. That is a round-trip.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 132 — RFC 6979 continues past r = 0 and s = 0

### Files

- `src/Arcanum.Asymmetric/Rfc6979.fs`
- `tests/Arcanum.Tests/Rfc6979Tests.fs`

### Object

RFC 6979 section 3.4: if the candidate `k` gives `r = 0` or `s = 0`, the signer continues the loop of section 3.2 step h.3 for a new `k`. `Rfc6979.sign` instead returns the error from `signWithNonce`.

- Add `Rfc6979.candidates : BigInteger -> BigInteger -> byte[] -> seq<BigInteger>` (`q`, private key, message), the lazy sequence of every `k` that step h produces, in order. `generateK` is its first element.
- `Rfc6979.sign` walks `candidates` and signs with the first `k` that gives non-zero `r` and `s`.
- `candidates` and `sign` refuse a private key outside `1 .. q - 1` by raising `ArgumentOutOfRangeException` and by returning an error respectively. `int2octets` no longer reduces silently.

### Laws

On `toy3` (`q = 19`, 5 bits), message `"sample"`. `SHA256("sample")` begins with the byte `0xAF = 10101111`, so the representative is `10101 = 21 = 2` modulo 19.

- Private key `15`: the first four candidates are `7, 7, 6, 2`. `7G = (0, 6)` gives `r = 0` twice. `6G = (16, 13)` gives `r = 16` and `s = 6^(-1) * (2 + 16 * 15) = 16 * 14 = 15` modulo 19. `Rfc6979.sign` returns `(r, s) = (16, 15)`.
- Private key `7`: the first candidates are `6, 5`. `k = 6` gives `r = 16` and `s = 6^(-1) * (2 + 16 * 7) = 6^(-1) * 114 = 0` modulo 19, because `114 = 6 * 19`. `k = 5` gives `5G = (9, 16)`, `r = 9`, and `s = 5^(-1) * (2 + 9 * 7) = 4 * 65 = 13` modulo 19. `Rfc6979.sign` returns `(9, 13)`.
- `verify` accepts both signatures.
- The RFC 6979 section A.2.5 signatures of phase 76 are unchanged.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 133 — The P-192 record

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `nistP192`
- `tests/Arcanum.Tests/NistCurveTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`EllipticCurve.nistP192`, with `H = 1`, from RFC 5114 section 2.4. The comment names RFC 5114 section 2.4 and says P-192 is NIST P-192.

```
P  = FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFF
A  = FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFC
B  = 64210519E59C80E70FA7E9AB72243049FEB8DEECC146B9B1
Gx = 188DA80EB03090F67CBF20EB43A18800F4FF0AFD82FF1012
Gy = 07192B95FFC8DA78631011ED6B24CDD573F977A11E794811
N  = FFFFFFFFFFFFFFFFFFFFFFFF99DEF836146BC9B1B4D22831
```

### Laws

- `p = 2^192 - 2^64 - 1`. `A = p - 3`.
- `Primes.millerRabinWithBases` accepts `p` and `N` with the bases `2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37`. The test says this is a probable-prime check, not a proof.
- `G` is on the curve. Unreduced `N * G` is `Infinity` and unreduced `(N - 1) * G` is `negate G`, by `multiplyUnreduced`.
- `N` equals the `q` that RFC 6979 section A.2.3 prints for P-192.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 134 — The P-224 record

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `nistP224`
- `tests/Arcanum.Tests/NistCurveTests.fs`

### Object

`EllipticCurve.nistP224`, with `H = 1`, from RFC 5114 section 2.5. The comment names RFC 5114 section 2.5 and says P-224 is NIST P-224.

```
P  = FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF000000000000000000000001
A  = FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFE
B  = B4050A850C04B3ABF54132565044B0B7D7BFD8BA270B39432355FFB4
Gx = B70E0CBD6BB4BF7F321390B94A03C1D356C21122343280D6115C1D21
Gy = BD376388B5F723FB4C22DFE6CD4375A05A07476444D5819985007E34
N  = FFFFFFFFFFFFFFFFFFFFFFFFFFFF16A2E0B8F03E13DD29455C5C2A3D
```

### Laws

- `p = 2^224 - 2^96 + 1`. `A = p - 3`.
- `p = 1` modulo 4, and `p - 1 = 2^96 * q` with `q` odd. This is the first large prime in the repository for which `Modular.modSqrt` cannot take the fast path.
- `Primes.millerRabinWithBases` accepts `p` and `N` with the bases of phase 133.
- `G` is on the curve. Unreduced `N * G` is `Infinity` and unreduced `(N - 1) * G` is `negate G`.
- `N` equals the `q` that RFC 6979 section A.2.4 prints for P-224.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 135 — RFC 5114 key agreement on P-192 and P-224

### Files

- `tests/Arcanum.Tests/NistCurveTests.fs`

### Object

RFC 5114 sections A.4 and A.5, through `scalarMultiply` and `DiffieHellman.computeEcdhSharedSecretChecked`.

RFC 5114 section A.4, P-192:

```
dA   = 323FA3169D8E9C6593F59476BC142000AB5BE0E249C43426
x_qA = CD46489ECFD6C105E7B3D32566E2B122E249ABAADD870612
y_qA = 68887B4877DF51DD4DC3D6FD11F0A26F8FD3844317916E9A
dB   = 631F95BB4A67632C9C476EEE9AB695AB240A0499307FCF62
x_qB = 519A121680E0045466BA21DF2EEE47F5973B500577EF13D5
y_qB = FF613AB4D64CEE3A20875BDB10F953F6B30CA072C60AA57F
x_Z  = AD420182633F8526BFE954ACDA376F05E5FF4F837F54FEBE
y_Z  = 4371545ED772A59741D0EDA32C671112B7FDDD51461FCF32
```

RFC 5114 section A.5, P-224:

```
dA   = B558EB6C288DA707BBB4F8FBAE2AB9E9CB62E3BC5C7573E22E26D37F
x_qA = 49DFEF309F81488C304CFF5AB3EE5A2154367DC7833150E0A51F3EEB
y_qA = 4F2B5EE45762C4F654C1A0C67F54CF88B016B51BCE3D7C228D57ADB4
dB   = AC3B1ADD3D9770E6F6A708EE9F3B8E0AB3B480E9F27F85C88B5E6D18
x_qB = 6B3AC96A8D0CDE6A5599BE8032EDF10C162D0A8AD219506DCD42A207
y_qB = D491BE99C213A7D1CA3706DEBFE305F361AFCBB33E2609C8B1618AD5
x_Z  = 52272F50F46F4EDC9151569092F46DF2D96ECC3B6DC1714A4EA949FA
y_Z  = 5F30C6AA36DDC403C0ACB712BB88F1763C3046F6D919BD9C524322BF
```

### Laws

On each curve: `dA * G = (x_qA, y_qA)`, `dB * G = (x_qB, y_qB)`, and `computeEcdhSharedSecretChecked` gives `Ok (x_Z, y_Z)` both ways. Each test names its RFC 5114 section and says it is one vector, not a CAVP file.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 136 — RFC 6979 on P-192, a 64-bit truncation

### Files

- `tests/Arcanum.Tests/Rfc6979Tests.fs`

### Object

RFC 6979 section A.2.3, NIST P-192, SHA-256. `bitLength N = 192`, so the 256-bit digest loses its last 64 bits. `h1 = SHA256("sample")` is `AF2BDBE1AA9B6EC1E2ADE1D694F41FC71A831D0268E9891562113D8A62ADD1BF` (RFC 6979 section A.1.2).

```
q        = FFFFFFFFFFFFFFFFFFFFFFFF99DEF836146BC9B1B4D22831
x        = 6FAB034934E4C0FC9AE67F5B5659A9D7D1FEFD187EE09FD4
Ux       = AC2C77F529F91689FEA0EA5EFEC7F210D8EEA0B9E047ED56
Uy       = 3BC723E57670BD4887EBC732C523063D0A7C957BC97C1C43
sample k = 32B1B6D7D42A05CB449065727A84804FB1A3E34D8F261496
sample r = 4B0B8CE98A92866A2820E20AA6B75B56382E0F9BFD5ECB55
sample s = CCDB006926EA9565CBADC840829D8C384E06DE1F1E381B85
test k   = 5C4CE89CF56D9E7C77C8585339B006B97B5F0680B4306C6C
test r   = 3A718BD8B4926C3B52EE6BBE67EF79B18CB6EB62B1AD97AE
test s   = 5662E6848A4A19B1F1AE2F72ACD4B8BBE50F1EAC65D9124F
```

### Laws

- `messageRepresentative nistP192 "sample"` is the first 24 bytes of `h1`: `AF2BDBE1AA9B6EC1E2ADE1D694F41FC71A831D0268E98915`. That integer is below `N`, so no reduction happens.
- `scalarMultiply nistP192 x G = (Ux, Uy)`.
- `Rfc6979.generateK` gives the printed `k` for both messages. `Rfc6979.sign` gives the printed `(r, s)`. `Ecdsa.verify` accepts each signature on its own message and rejects the `"sample"` signature on `"test"`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 137 — RFC 6979 on P-224, a 32-bit truncation

### Files

- `tests/Arcanum.Tests/Rfc6979Tests.fs`

### Object

RFC 6979 section A.2.4, NIST P-224, SHA-256. `bitLength N = 224`, so the digest loses its last 32 bits.

```
q        = FFFFFFFFFFFFFFFFFFFFFFFFFFFF16A2E0B8F03E13DD29455C5C2A3D
x        = F220266E1105BFE3083E03EC7A3A654651F45E37167E88600BF257C1
Ux       = 00CF08DA5AD719E42707FA431292DEA11244D64FC51610D94B130D6C
Uy       = EEAB6F3DEBE455E3DBF85416F7030CBD94F34F2D6F232C69F3C1385A
sample k = AD3029E0278F80643DE33917CE6908C70A8FF50A411F06E41DEDFCDC
sample r = 61AA3DA010E8E8406C656BC477A7A7189895E7E840CDFE8FF42307BA
sample s = BC814050DAB5D23770879494F9E0A680DC1AF7161991BDE692B10101
test k   = FF86F57924DA248D6E44E8154EB69F0AE2AEBAEE9931D0B5A969F904
test r   = AD04DDE87B84747A243A631EA47A1BA6D1FAA059149AD2440DE6FBA6
test s   = 178D49B1AE90E3D8B629BE3DB5683915F4E8C99FDF6E666CF37ADCFD
```

### Laws

- `messageRepresentative nistP224 "sample"` is the first 28 bytes of `h1`: `AF2BDBE1AA9B6EC1E2ADE1D694F41FC71A831D0268E9891562113D8A`, below `N`.
- `scalarMultiply nistP224 x G = (Ux, Uy)`. Note that `Ux` begins with a zero byte.
- `generateK`, `sign`, and `verify` as in phase 136, with the printed values.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 138 — SEC 1 on P-192 and P-224

### Files

- `tests/Arcanum.Tests/Sec1Tests.fs`

### Object

Encode and decode the generators of the two new curves. The field lengths are 24 and 28 bytes. Decoding a compressed P-224 point runs `Modular.modSqrt` through the general Tonelli-Shanks loop with `S = 96`.

### Laws

- P-192 `Gy` ends in the hex digit `1`, so it is odd. Compressed `G` is `03` followed by the 24 bytes of `Gx`.
- P-224 `Gy` ends in the hex digit `4`, so it is even. Compressed `G` is `02` followed by the 28 bytes of `Gx`.
- Each compressed encoding decodes to `G`. Each uncompressed encoding is `1 + 2 * 24 = 49` or `1 + 2 * 28 = 57` bytes and decodes to `G`.
- `Modular.modSqrt` of `Gx^3 + A Gx + B` modulo the P-224 prime is `Gy` or `p - Gy`.
- The P-224 public key `U` of phase 137, whose `Ux` begins with a zero byte, round-trips through both encodings. The leading zero byte is kept.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 139 — HKDF

### Files

- `src/Arcanum.Hashing/HKDF.fs`
- `src/Arcanum.Hashing/Arcanum.Hashing.fsproj`
- `tests/Arcanum.Tests/HkdfTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

RFC 5869 section 2, with `HMAC.hmacSha256` as the HMAC.

- `HKDF.extract : byte[] -> byte[] -> byte[]` (salt, input keying material). An empty salt means 32 zero bytes, which HMAC treats the same as an empty key.
- `HKDF.expand : byte[] -> byte[] -> int -> Result<byte[], CryptoError>` (pseudorandom key, info, length). `T(0)` is empty and `T(i) = HMAC(PRK, T(i - 1) || info || i)` with `i` as one byte.

The comment labels the CLR boundary: HMAC is the F# construction on `SHA256.hash`.

### Laws

RFC 5869 Appendix A.1:

```
IKM  = 0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b
salt = 000102030405060708090a0b0c
info = f0f1f2f3f4f5f6f7f8f9
L    = 42
PRK  = 077709362c2e32df0ddc3f0dc47bba6390b6c73bb50f9c3122ec844ad7c2b3e5
OKM  = 3cb25f25faacd57a90434f64d0362f2a2d2d0a90cf1a5a4c5db02d56ecc4c5bf34007208d5b887185865
```

`extract salt IKM = PRK` and `expand PRK info 42 = OKM`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 140 — HKDF, longer and empty inputs

### Files

- `tests/Arcanum.Tests/HkdfTests.fs`

### Object

RFC 5869 Appendix A.2 and A.3, and the length limit of section 2.3.

RFC 5869 Appendix A.2:

```
IKM  = 000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f
salt = 606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeaf
info = b0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfeff
L    = 82
PRK  = 06a6b88c5853361a06104c9ceb35b45cef760014904671014a193f40c15fc244
OKM  = b11e398dc80327a1c8e7f78c596a49344f012eda2d4efad8a050cc4c19afa97c59045a99cac7827271cb41c65e590e09da3275600c2f09b8367793a9aca3db71cc30c58179ec3e87c14c01d5c1f3434f1d87
```

RFC 5869 Appendix A.3:

```
IKM  = 0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b
salt = (empty)
info = (empty)
L    = 42
PRK  = 19ef24a32c717b167f33a91d6f648bdf96596776afdb6377ac434c1c293ccb04
OKM  = 8da4e775a563c18f715f802a063c5a31b8a11f5c5ee1879ec3454e5f3c738d2d9d201395faa4b61a96c8
```

### Laws

- Both cases give the printed `PRK` and `OKM`.
- `expand` accepts length `255 * 32 = 8160` and refuses `8161`.
- `expand` of length `0` returns an empty array.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 141 — DER encoding of ECDSA signatures

### Files

- `src/Arcanum.Asymmetric/EcdsaDer.fs`
- `src/Arcanum.Asymmetric/Arcanum.Asymmetric.fsproj`
- `tests/Arcanum.Tests/EcdsaDerTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

`Ecdsa-Sig-Value ::= SEQUENCE { r INTEGER, s INTEGER }` (RFC 3279 section 2.2.3, SEC 1 section C.8), in DER.

- An `INTEGER` is `02`, a one-byte length, and the minimal big-endian bytes of the value. A leading `00` is added exactly when the first byte would otherwise have its top bit set.
- The `SEQUENCE` is `30`, a one-byte length, and the two integers. Every length here is below 128, so the short length form is the only one needed.
- `EcdsaDer.encode : EcdsaSignature -> byte[]`.

### Laws

The RFC 6979 section A.2.5 signatures of phase 76:

```
sample r              = EFD48B2AACB6A8FD1140DD9CD45E81D69D2C877B56AAF991C34D0EA84EAF3716
sample s              = F7CB1C942D657C41D436C7A1B6E29F65F3E900DBB9AFF4064DC4AB2F843ACDA8
sample DER (72 bytes) = 3046022100efd48b2aacb6a8fd1140dd9cd45e81d69d2c877b56aaf991c34d0ea84eaf3716022100f7cb1c942d657c41d436c7a1b6e29f65f3e900dbb9aff4064dc4ab2f843acda8
test r                = F1ABB023518351CD71D881567B1EA663ED3EFCF6C5132B354F28D3B0B7D38367
test s                = 019F4113742A2B14BD25926B49C649155F267E60D3814B4C0CC84250E46F0083
test DER (71 bytes)   = 3045022100f1abb023518351cd71d881567b1ea663ed3efcf6c5132b354f28d3b0b7d383670220019f4113742a2b14bd25926b49c649155f267e60d3814b4c0cc84250e46f0083
```

- `"sample"`: `r` and `s` both begin with a byte whose top bit is set (`0xEF`, `0xF7`), so each integer is `02 21 00` followed by 32 bytes. The sequence body is `2 * 35 = 70` bytes, so the encoding is `30 46` followed by the body: 72 bytes.
- `"test"`: `r` begins with `0xF1` and gets the `00`. `s` begins with `0x01` and does not. The body is `35 + 34 = 69` bytes, so the encoding is `30 45` followed by the body: 71 bytes.
- `encode` gives exactly the bytes above.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 142 — Strict DER decoding

### Files

- `src/Arcanum.Asymmetric/EcdsaDer.fs`
- `tests/Arcanum.Tests/EcdsaDerTests.fs`

### Object

`EcdsaDer.decode : byte[] -> Result<EcdsaSignature, CryptoError>`. It accepts only the encoding `encode` produces.

### Laws

- `decode` of each phase 141 encoding returns the RFC 6979 `(r, s)`.
- `decode` refuses each of these, built from the `"test"` encoding. In the first two, the sequence length is adjusted to the new body, so that only the integer is wrong:
  - an extra `00` before `s` (`02 21 00 01 9F ...`), which is not minimal;
  - `r` without its `00` (`02 20 F1 ...`), which would be negative;
  - a sequence length that does not match the body;
  - one trailing byte after the sequence;
  - a first byte other than `30`.
- `decode` refuses `30 06 02 01 00 02 01 01`, because `r = 0`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 143 — expand_message_xmd

### Files

- `src/Arcanum.Asymmetric/HashToCurve.fs`
- `src/Arcanum.Asymmetric/Arcanum.Asymmetric.fsproj`
- `tests/Arcanum.Tests/HashToCurveTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

RFC 9380 section 5.3.1 with SHA-256 (`b_in_bytes = 32`, `s_in_bytes = 64`):

`HashToCurve.expandMessageXmd : byte[] -> byte[] -> int -> Result<byte[], CryptoError>` (message, DST, `len_in_bytes`). `DST_prime = DST || I2OSP(len(DST), 1)`, `msg_prime = Z_pad || msg || I2OSP(len_in_bytes, 2) || I2OSP(0, 1) || DST_prime` with 64 zero bytes in `Z_pad`, `b_0 = H(msg_prime)`, `b_1 = H(b_0 || I2OSP(1, 1) || DST_prime)`, and `b_i = H(strxor(b_0, b_(i - 1)) || I2OSP(i, 1) || DST_prime)`. Refuse `ell > 255`, `len_in_bytes > 65535`, and a DST longer than 255 bytes. The comment labels `SHA256.hash` as the CLR call.

### Laws

RFC 9380 Appendix K.1, `DST = "QUUX-V01-CS02-with-expander-SHA256-128"`, `len_in_bytes = 0x20`. The message `q128_...` is `"q128_"` followed by 128 `q` characters (133 bytes). The message `a512_...` is `"a512_"` followed by 512 `a` characters (517 bytes). The test builds them that way.

```
msg           = ""
uniform_bytes = 68a985b87eb6b46952128911f2a4412bbc302a9d759667f87f7a21d803f07235
msg           = "abc"
uniform_bytes = d8ccab23b5985ccea865c6c97b6e5b8350e794e603b4b97902f53a8a0d605615
msg           = "abcdef0123456789"
uniform_bytes = eff31487c770a893cfb36f912fbfcbff40d5661771ca4b2cb4eafe524333f5c1
msg           = "q128_" + 128 "q"
uniform_bytes = b23a1d2b4d97b2ef7785562a7e8bac7eed54ed6e97e29aa51bfe3f12ddad1ff9
msg           = "a512_" + 512 "a"
uniform_bytes = 4623227bcc01293b8c130bf771da8c298dede7383243dc0993d2d94823958c4c
```

Also: `len_in_bytes = 8161` (`ell = 256`) is refused.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 144 — expand_message_xmd, 128 bytes

### Files

- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

The same five messages and DST with `len_in_bytes = 0x80`, which takes `ell = 4` blocks.

### Laws

RFC 9380 Appendix K.1:

```
msg           = ""
uniform_bytes = af84c27ccfd45d41914fdff5df25293e221afc53d8ad2ac06d5e3e29485dadbee0d121587713a3e0dd4d5e69e93eb7cd4f5df4cd103e188cf60cb02edc3edf18eda8576c412b18ffb658e3dd6ec849469b979d444cf7b26911a08e63cf31f9dcc541708d3491184472c2c29bb749d4286b004ceb5ee6b9a7fa5b646c993f0ced
msg           = "abc"
uniform_bytes = abba86a6129e366fc877aab32fc4ffc70120d8996c88aee2fe4b32d6c7b6437a647e6c3163d40b76a73cf6a5674ef1d890f95b664ee0afa5359a5c4e07985635bbecbac65d747d3d2da7ec2b8221b17b0ca9dc8a1ac1c07ea6a1e60583e2cb00058e77b7b72a298425cd1b941ad4ec65e8afc50303a22c0f99b0509b4c895f40
msg           = "abcdef0123456789"
uniform_bytes = ef904a29bffc4cf9ee82832451c946ac3c8f8058ae97d8d629831a74c6572bd9ebd0df635cd1f208e2038e760c4994984ce73f0d55ea9f22af83ba4734569d4bc95e18350f740c07eef653cbb9f87910d833751825f0ebefa1abe5420bb52be14cf489b37fe1a72f7de2d10be453b2c9d9eb20c7e3f6edc5a60629178d9478df
msg           = "q128_" + 128 "q"
uniform_bytes = 80be107d0884f0d881bb460322f0443d38bd222db8bd0b0a5312a6fedb49c1bbd88fd75d8b9a09486c60123dfa1d73c1cc3169761b17476d3c6b7cbbd727acd0e2c942f4dd96ae3da5de368d26b32286e32de7e5a8cb2949f866a0b80c58116b29fa7fabb3ea7d520ee603e0c25bcaf0b9a5e92ec6a1fe4e0391d1cdbce8c68a
msg           = "a512_" + 512 "a"
uniform_bytes = 546aff5444b5b79aa6148bd81728704c32decb73a3ba76e9e75885cad9def1d06d6792f8a7d12794e90efed817d96920d728896a4510864370c207f99bd4a608ea121700ef01ed879745ee3e4ceef777eda6d9e5e38b90c86ea6fb0b36504ba4a45d22e86f6db5dd43d98a294bebb9125d5b794e9d2a81181066eb954966a487
```

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 145 — Field helpers of RFC 9380 section 4

### Files

- `src/Arcanum.Asymmetric/HashToCurve.fs`
- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

For a prime field, `m = 1`:

- `sgn0 : BigInteger -> BigInteger -> int` (`p`, `x`) is `x mod 2` of the reduced `x`.
- `isSquare : BigInteger -> BigInteger -> bool` (`p`, `x`) is true for `0` and for every quadratic residue. It uses `Modular.jacobi`.
- `inv0 : BigInteger -> BigInteger -> BigInteger` (`p`, `x`) is `0` for `0` and `Modular.modInverse` otherwise.

### Laws

Modulo 17:

- `sgn0` of `0 .. 16` is `0, 1, 0, 1, ...`, and `sgn0 17 (-1) = 0`, because `-1 = 16`.
- `isSquare` is true exactly for `0, 1, 2, 4, 8, 9, 13, 15, 16`.
- `inv0 17 0 = 0`, and `inv0 17 x * x = 1` for every `x` in `1 .. 16`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 146 — toy3 has no SSWU constant

### Files

- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

RFC 9380 section 6.6.2 requires a constant `Z` with four properties: (1) `Z` is not a square, (2) `Z <> -1`, (3) `g(x) - Z` is irreducible over the field, where `g(x) = x^3 + A x + B`, and (4) `g(B / (Z A))` is a square. For a cubic over a field, (3) holds exactly when `g(x) - Z` has no root, which is when `Z` is not a value of `g`.

Add `HashToCurve.isValidSswuZ : Curve -> BigInteger -> bool`, which checks the four properties by enumerating `x` in `0 .. P - 1`. The comment says the enumeration is for the toy curves only.

### Laws

On `toy3`, `g(x) = x^3 + 2x + 2`:

- The values of `g` over `F_17` are `1, 2, 3, 5, 6, 7, 9, 12, 14, 15, 16`.
- The non-squares are `3, 5, 6, 7, 10, 11, 12, 14`. Every one of them except `10` and `11` is a value of `g`, so property (3) fails for them.
- For `Z = 10` and `Z = 11`, property (4) fails.
- So `isValidSswuZ toy3 Z` is false for every `Z` in `0 .. 16`. The simplified SWU map does not apply to `toy3`. The test asserts all three facts.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 147 — The twist of toy3, and its constant

### Files

- `src/Arcanum.Core/EllipticCurve.fs`, adding `toy3Twist`
- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

`EllipticCurve.toy3Twist` is a `Curve`:

- `P = 17`, `A = 1`, `B = 3`, so `y^2 = x^3 + x + 3`, the quadratic twist of `toy3` by `d = 3` from phase 72
- `G = Point(2, 8)`
- `N = 17`
- `H = 1`

The comment says the group has prime order 17, that its order equals `P`, and that the curve is not a secure group. Do not change `toy3`.

RFC 9380 Appendix H.2 searches for `Z` in the order `1, -1, 2, -2, 3, -3, ...` and returns the first valid one. Add `HashToCurve.findSswuZ : Curve -> BigInteger option` that does that search over `1 .. P - 1` and returns `None` when nothing qualifies.

### Laws

- Enumeration by `isOnCurve` finds 17 points, and `(2, 8)` reaches `Infinity` by repeated addition at step 17 and not earlier.
- `twistCoefficients toy3 3` is `Ok (1, 3)`, the coefficients of `toy3Twist`.
- `findSswuZ toy3` is `None`.
- `findSswuZ toy3Twist` is `Some 11`, which is `-6`: the candidates `1, -1, 2, -2, 3, -3, 4, -4, 5, -5, 6` fail and `-6` passes. For `Z = 11`: it is not a square, it is not `-1 = 16`, it is not a value of `g` (the values are `1, 2, 3, 4, 5, 7, 9, 10, 13, 14, 16`), and `B / (Z A) = 3 * 11^(-1) = 3 * 14 = 8` with `g(8) = 512 + 8 + 3 = 523 = 13 = 8^2` modulo 17.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 148 — The simplified SWU map, exhaustively

### Files

- `src/Arcanum.Asymmetric/HashToCurve.fs`
- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

`HashToCurve.mapToCurveSswu : Curve -> BigInteger -> BigInteger -> ECPoint` (curve, `Z`, `u`), the straight-line description of RFC 9380 section 6.6.2:

1. `tv1 = inv0(Z^2 u^4 + Z u^2)`
2. `x1 = (-B / A) * (1 + tv1)`
3. If `tv1 = 0`, set `x1 = B / (Z A)`
4. `gx1 = x1^3 + A x1 + B`
5. `x2 = Z u^2 x1`
6. `gx2 = x2^3 + A x2 + B`
7. If `isSquare gx1`, set `x = x1` and `y = sqrt(gx1)`; otherwise set `x = x2` and `y = sqrt(gx2)`
8. If `sgn0 u <> sgn0 y`, set `y = -y`

The square root is `Modular.modSqrt`. Which root it returns does not matter, because step 8 fixes the sign. `A` and `B` must both be non-zero; refuse otherwise.

### Laws

On `toy3Twist` with `Z = 11`, for every `u` in `0 .. 16`:

- The result is on the curve, and `sgn0 y = sgn0 u`.
- For `u <> 0`, `map (-u) = negate (map u)`.

By hand:

- `u = 0`: `tv1 = inv0 0 = 0`, so `x1 = B / (Z A) = 8` and `gx1 = 13 = 8^2`. `sgn0 0 = 0`, so `y` is the even root: the point is `(8, 8)`.
- `u = 1`: `Z^2 + Z = 121 + 11 = 132 = 13`, `tv1 = 13^(-1) = 4`, `x1 = -3 * (1 + 4) = -15 = 2`, `gx1 = 8 + 2 + 3 = 13`, a square. `sgn0 1 = 1`, so `y = 9`: the point is `(2, 9)`.

The full table, which the test asserts:

```
u =  0  ->  ( 8,  8)
u =  1  ->  ( 2,  9)
u =  2  ->  ( 3,  4)
u =  3  ->  ( 2,  9)
u =  4  ->  ( 3,  4)
u =  5  ->  ( 3, 13)
u =  6  ->  ( 2,  8)
u =  7  ->  ( 3, 13)
u =  8  ->  ( 2,  8)
u =  9  ->  ( 2,  9)
u = 10  ->  ( 3,  4)
u = 11  ->  ( 2,  9)
u = 12  ->  ( 3,  4)
u = 13  ->  ( 3, 13)
u = 14  ->  ( 2,  8)
u = 15  ->  ( 3, 13)
u = 16  ->  ( 2,  8)
```

The map hits 5 of the 17 points. The test says so. It is not a bijection, and RFC 9380 does not claim one.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 149 — hash_to_field on P-256

### Files

- `src/Arcanum.Asymmetric/HashToCurve.fs`
- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

RFC 9380 section 5.2 for `m = 1`: `HashToCurve.hashToField : BigInteger -> byte[] -> byte[] -> int -> Result<BigInteger list, CryptoError>` (`p`, message, DST, count). `L = ceil((ceil(log2 p) + k) / 8)`, which is `48` for P-256 with `k = 128`. It calls `expandMessageXmd` for `count * L` bytes and reads each `L`-byte slice as a big-endian integer modulo `p`.

### Laws

With the P-256 prime:

- `hashToField` with the RO DST and `count = 2` gives `u[0]` and `u[1]` of RFC 9380 section J.1.1 for the messages `""` and `"abc"` (phase 151 lists them).
- `hashToField` with the NU DST and `count = 1` gives `u[0]` of section J.1.2 for the messages `""` and `"abc"` (phase 150 lists them).

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 150 — encode_to_curve on P-256

### Files

- `src/Arcanum.Asymmetric/HashToCurve.fs`
- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

The suite `P256_XMD:SHA-256_SSWU_NU_` of RFC 9380 section 8.2: `expand_message_xmd` with SHA-256, `L = 48`, `Z = -10`, `h_eff = 1`. `HashToCurve.encodeToCurveP256 : byte[] -> byte[] -> Result<ECPoint, CryptoError>` (message, DST) is `mapToCurveSswu nistP256 (-10) (hashToField ... 1)`.

`Z = -10` is the suite's constant. The test checks properties (1), (2), and (4) of phase 146 for it on P-256. Property (3) cannot be checked by enumeration on P-256; the test says that it is taken from RFC 9380 section 8.2, not re-proved here.

### Laws

RFC 9380 section J.1.2, `DST = "QUUX-V01-CS02-with-P256_XMD:SHA-256_SSWU_NU_"`. For each message, `u[0]` matches, `Q = P`, and `encodeToCurveP256` gives `P`. The long messages are built as in phase 143.

```
msg    = ""
P.x    = f871caad25ea3b59c16cf87c1894902f7e7b2c822c3d3f73596c5ace8ddd14d1
P.y    = 87b9ae23335bee057b99bac1e68588b18b5691af476234b8971bc4f011ddc99b
u[0]   = b22d487045f80e9edcb0ecc8d4bf77833e2bf1f3a54004d7df1d57f4802d311f
Q.x    = f871caad25ea3b59c16cf87c1894902f7e7b2c822c3d3f73596c5ace8ddd14d1
Q.y    = 87b9ae23335bee057b99bac1e68588b18b5691af476234b8971bc4f011ddc99b
```

```
msg    = "abc"
P.x    = fc3f5d734e8dce41ddac49f47dd2b8a57257522a865c124ed02b92b5237befa4
P.y    = fe4d197ecf5a62645b9690599e1d80e82c500b22ac705a0b421fac7b47157866
u[0]   = c7f96eadac763e176629b09ed0c11992225b3a5ae99479760601cbd69c221e58
Q.x    = fc3f5d734e8dce41ddac49f47dd2b8a57257522a865c124ed02b92b5237befa4
Q.y    = fe4d197ecf5a62645b9690599e1d80e82c500b22ac705a0b421fac7b47157866
```

```
msg    = "abcdef0123456789"
P.x    = f164c6674a02207e414c257ce759d35eddc7f55be6d7f415e2cc177e5d8faa84
P.y    = 3aa274881d30db70485368c0467e97da0e73c18c1d00f34775d012b6fcee7f97
u[0]   = 314e8585fa92068b3ea2c3bab452d4257b38be1c097d58a21890456c2929614d
Q.x    = f164c6674a02207e414c257ce759d35eddc7f55be6d7f415e2cc177e5d8faa84
Q.y    = 3aa274881d30db70485368c0467e97da0e73c18c1d00f34775d012b6fcee7f97
```

```
msg    = "q128_" + 128 "q"
P.x    = 324532006312be4f162614076460315f7a54a6f85544da773dc659aca0311853
P.y    = 8d8197374bcd52de2acfefc8a54fe2c8d8bebd2a39f16be9b710e4b1af6ef883
u[0]   = 752d8eaa38cd785a799a31d63d99c2ae4261823b4a367b133b2c6627f48858ab
Q.x    = 324532006312be4f162614076460315f7a54a6f85544da773dc659aca0311853
Q.y    = 8d8197374bcd52de2acfefc8a54fe2c8d8bebd2a39f16be9b710e4b1af6ef883
```

```
msg    = "a512_" + 512 "a"
P.x    = 5c4bad52f81f39c8e8de1260e9a06d72b8b00a0829a8ea004a610b0691bea5d9
P.y    = c801e7c0782af1f74f24fc385a8555da0582032a3ce038de637ccdcb16f7ef7b
u[0]   = 0e1527840b9df2dfbef966678ff167140f2b27c4dccd884c25014dce0e41dfa3
Q.x    = 5c4bad52f81f39c8e8de1260e9a06d72b8b00a0829a8ea004a610b0691bea5d9
Q.y    = c801e7c0782af1f74f24fc385a8555da0582032a3ce038de637ccdcb16f7ef7b
```

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 151 — hash_to_curve on P-256

### Files

- `src/Arcanum.Asymmetric/HashToCurve.fs`
- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

The suite `P256_XMD:SHA-256_SSWU_RO_`: `HashToCurve.hashToCurveP256 : byte[] -> byte[] -> Result<ECPoint, CryptoError>` is `add (map u[0]) (map u[1])` with `count = 2`.

### Laws

RFC 9380 section J.1.1, `DST = "QUUX-V01-CS02-with-P256_XMD:SHA-256_SSWU_RO_"`, messages `""` and `"abc"`. Each of `u[0]`, `u[1]`, `Q0 = map u[0]`, `Q1 = map u[1]`, and `P = Q0 + Q1` matches.

```
msg    = ""
P.x    = 2c15230b26dbc6fc9a37051158c95b79656e17a1a920b11394ca91c44247d3e4
P.y    = 8a7a74985cc5c776cdfe4b1f19884970453912e9d31528c060be9ab5c43e8415
u[0]   = ad5342c66a6dd0ff080df1da0ea1c04b96e0330dd89406465eeba11582515009
u[1]   = 8c0f1d43204bd6f6ea70ae8013070a1518b43873bcd850aafa0a9e220e2eea5a
Q0.x   = ab640a12220d3ff283510ff3f4b1953d09fad35795140b1c5d64f313967934d5
Q0.y   = dccb558863804a881d4fff3455716c836cef230e5209594ddd33d85c565b19b1
Q1.x   = 51cce63c50d972a6e51c61334f0f4875c9ac1cd2d3238412f84e31da7d980ef5
Q1.y   = b45d1a36d00ad90e5ec7840a60a4de411917fbe7c82c3949a6e699e5a1b66aac
```

```
msg    = "abc"
P.x    = 0bb8b87485551aa43ed54f009230450b492fead5f1cc91658775dac4a3388a0f
P.y    = 5c41b3d0731a27a7b14bc0bf0ccded2d8751f83493404c84a88e71ffd424212e
u[0]   = afe47f2ea2b10465cc26ac403194dfb68b7f5ee865cda61e9f3e07a537220af1
u[1]   = 379a27833b0bfe6f7bdca08e1e83c760bf9a338ab335542704edcd69ce9e46e0
Q0.x   = 5219ad0ddef3cc49b714145e91b2f7de6ce0a7a7dc7406c7726c7e373c58cb48
Q0.y   = 7950144e52d30acbec7b624c203b1996c99617d0b61c2442354301b191d93ecf
Q1.x   = 019b7cb4efcfeaf39f738fe638e31d375ad6837f58a852d032ff60c69ee3875f
Q1.y   = 589a62d2b22357fed5449bc38065b760095ebe6aeac84b01156ee4252715446e
```

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 152 — hash_to_curve, the remaining messages

### Files

- `tests/Arcanum.Tests/HashToCurveTests.fs`

### Object

The other three messages of RFC 9380 section J.1.1. `P` is also checked with `Jacobian.add` of `Q0` and `Q1` and with `isOnCurve`.

### Laws

```
msg    = "abcdef0123456789"
P.x    = 65038ac8f2b1def042a5df0b33b1f4eca6bff7cb0f9c6c1526811864e544ed80
P.y    = cad44d40a656e7aff4002a8de287abc8ae0482b5ae825822bb870d6df9b56ca3
u[0]   = 0fad9d125a9477d55cf9357105b0eb3a5c4259809bf87180aa01d651f53d312c
u[1]   = b68597377392cd3419d8fcc7d7660948c8403b19ea78bbca4b133c9d2196c0fb
Q0.x   = a17bdf2965eb88074bc01157e644ed409dac97cfcf0c61c998ed0fa45e79e4a2
Q0.y   = 4f1bc80c70d411a3cc1d67aeae6e726f0f311639fee560c7f5a664554e3c9c2e
Q1.x   = 7da48bb67225c1a17d452c983798113f47e438e4202219dd0715f8419b274d66
Q1.y   = b765696b2913e36db3016c47edb99e24b1da30e761a8a3215dc0ec4d8f96e6f9
```

```
msg    = "q128_" + 128 "q"
P.x    = 4be61ee205094282ba8a2042bcb48d88dfbb609301c49aa8b078533dc65a0b5d
P.y    = 98f8df449a072c4721d241a3b1236d3caccba603f916ca680f4539d2bfb3c29e
u[0]   = 3bbc30446f39a7befad080f4d5f32ed116b9534626993d2cc5033f6f8d805919
u[1]   = 76bb02db019ca9d3c1e02f0c17f8baf617bbdae5c393a81d9ce11e3be1bf1d33
Q0.x   = c76aaa823aeadeb3f356909cb08f97eee46ecb157c1f56699b5efebddf0e6398
Q0.y   = 776a6f45f528a0e8d289a4be12c4fab80762386ec644abf2bffb9b627e4352b1
Q1.x   = 418ac3d85a5ccc4ea8dec14f750a3a9ec8b85176c95a7022f391826794eb5a75
Q1.y   = fd6604f69e9d9d2b74b072d14ea13050db72c932815523305cb9e807cc900aff
```

```
msg    = "a512_" + 512 "a"
P.x    = 457ae2981f70ca85d8e24c308b14db22f3e3862c5ea0f652ca38b5e49cd64bc5
P.y    = ecb9f0eadc9aeed232dabc53235368c1394c78de05dd96893eefa62b0f4757dc
u[0]   = 4ebc95a6e839b1ae3c63b847798e85cb3c12d3817ec6ebc10af6ee51adb29fec
u[1]   = 4e21af88e22ea80156aff790750121035b3eefaa96b425a8716e0d20b4e269ee
Q0.x   = d88b989ee9d1295df413d4456c5c850b8b2fb0f5402cc5c4c7e815412e926db8
Q0.y   = bb4a1edeff506cf16def96afff41b16fc74f6dbd55c2210e5b8f011ba32f4f40
Q1.x   = a281e34e628f3a4d2a53fa87ff973537d68ad4fbc28d3be5e8d9f6a2571c5a4b
Q1.y   = f6ed88a7aab56a488100e6f1174fa9810b47db13e86be999644922961206e184
```

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 153 — HPKE labels

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `src/Arcanum.Asymmetric/Arcanum.Asymmetric.fsproj`
- `tests/Arcanum.Tests/HpkeTests.fs`
- `tests/Arcanum.Tests/Arcanum.Tests.fsproj`

### Object

RFC 9180 section 4, for `DHKEM(P-256, HKDF-SHA256)` (`kem_id = 0x0010`), `HKDF-SHA256` (`kdf_id = 0x0001`), and `ChaCha20Poly1305` (`aead_id = 0x0003`):

- `Hpke.kemSuiteId` is `"KEM" || I2OSP(0x0010, 2)`, the 5 bytes `4b454d0010`.
- `Hpke.hpkeSuiteId` is `"HPKE" || I2OSP(0x0010, 2) || I2OSP(0x0001, 2) || I2OSP(0x0003, 2)`, the 10 bytes `48504b45001000010003`.
- `Hpke.labeledExtract suiteId salt label ikm = HKDF.extract salt ("HPKE-v1" || suiteId || label || ikm)`.
- `Hpke.labeledExpand suiteId prk label info L = HKDF.expand prk (I2OSP(L, 2) || "HPKE-v1" || suiteId || label || info) L`.

### Laws

- The two suite identifiers are the bytes above.
- For RFC 9180 section A.5.1, `info = "Ode on a Grecian Urn"` and empty `psk_id`: `00 || labeledExtract hpkeSuiteId "" "psk_id_hash" "" || labeledExtract hpkeSuiteId "" "info_hash" info` equals the printed `key_schedule_context` listed in phase 154.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 154 — DeriveKeyPair for P-256

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

RFC 9180 section 7.1.3: `dkp_prk = LabeledExtract("", "dkp_prk", ikm)`, then for `counter = 0, 1, ...` up to 255, `bytes = LabeledExpand(dkp_prk, "candidate", I2OSP(counter, 1), 32)`, `bytes[0] = bytes[0] & 0xff`, `sk = OS2IP(bytes)`, until `0 < sk < N`. More than 256 tries is an error. The labels use `kemSuiteId`.

`Hpke.deriveKeyPair : byte[] -> Result<BigInteger * ECPoint, CryptoError>`. `Hpke.serializePublicKey` is `Sec1.encodePoint nistP256 false`. `Hpke.deserializePublicKey` is `Sec1.decodePoint nistP256` followed by `validatePublicKey`, and refuses a compressed encoding.

### Laws

RFC 9180 section A.5.1, base mode. `info` is the ASCII string `"Ode on a Grecian Urn"`. Phases 155 through 158 use these values too.

```
ikmE                 = f1f1a3bc95416871539ecb51c3a8f0cf608afb40fbbe305c0a72819d35c33f1f
pkEm                 = 04c07836a0206e04e31d8ae99bfd549380b072a1b1b82e563c935c095827824fc1559eac6fb9e3c70cd3193968994e7fe9781aa103f5b50e934b5b2f387e381291
skEm                 = 7550253e1147aae48839c1f8af80d2770fb7a4c763afe7d0afa7e0f42a5b3689
ikmR                 = 61092f3f56994dd424405899154a9918353e3e008171517ad576b900ddb275e7
pkRm                 = 04a697bffde9405c992883c5c439d6cc358170b51af72812333b015621dc0f40bad9bb726f68a5c013806a790ec716ab8669f84f6b694596c2987cf35baba2a006
skRm                 = a4d1c55836aa30f9b3fbb6ac98d338c877c2867dd3a77396d13f68d3ab150d3b
enc                  = 04c07836a0206e04e31d8ae99bfd549380b072a1b1b82e563c935c095827824fc1559eac6fb9e3c70cd3193968994e7fe9781aa103f5b50e934b5b2f387e381291
shared_secret        = 806520f82ef0b03c823b7fc524b6b55a088f566b9751b89551c170f4113bd850
key_schedule_context = 00b738cd703db7b4106e93b4621e9a19c89c838e55964240e5d3f331aaf8b0d58b2e986ea1c671b61cf45eec134dac0bae58ec6f63e790b1400b47c33038b0269c
secret               = fe891101629aa355aad68eff3cc5170d057eca0c7573f6575e91f9783e1d4506
key                  = a8f45490a92a3b04d1dbf6cf2c3939ad8bfc9bfcb97c04bffe116730c9dfe3fc
base_nonce           = 726b4390ed2209809f58c693
exporter_secret      = 4f9bd9b3a8db7d7c3a5b9d44fdc1f6e37d5d77689ade5ec44a7242016e6aa205
```

`deriveKeyPair ikmE = (skEm, pkEm)` and `deriveKeyPair ikmR = (skRm, pkRm)`. Both succeed at `counter = 0`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 155 — DHKEM encapsulation

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

RFC 9180 section 4.1. `DH(sk, pk)` is the 32-byte big-endian `x` coordinate of `sk * pk`, through `computeEcdhSharedSecretChecked`. `ExtractAndExpand(dh, kem_context)` is `LabeledExpand(LabeledExtract("", "eae_prk", dh), "shared_secret", kem_context, 32)`.

- `Hpke.encapDeterministic : byte[] -> ECPoint -> Result<byte[] * byte[], CryptoError>` (`ikmE`, `pkR`) returns `(shared_secret, enc)` with `enc = serializePublicKey pkE` and `kem_context = enc || serializePublicKey pkR`. The name says the ephemeral key comes from a caller-supplied `ikmE`; it is for test vectors.
- `Hpke.decap : byte[] -> BigInteger -> Result<byte[], CryptoError>` (`enc`, `skR`).

### Laws

RFC 9180 section A.5.1, with the values listed in phase 154: `encapDeterministic ikmE pkR` returns the printed `enc` and `shared_secret`, and `decap enc skR` returns the same `shared_secret`. `decap` refuses an `enc` whose point is not on P-256.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 156 — The key schedule, base mode

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `src/Arcanum.Asymmetric/Arcanum.Asymmetric.fsproj`, adding the project reference to `Arcanum.Symmetric`
- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

RFC 9180 section 5.1. `mode_base = 0`. `VerifyPSKInputs` refuses a PSK without a PSK identifier and the reverse, a PSK in a mode without one, and a missing PSK in a mode with one. `key_schedule_context = mode || psk_id_hash || info_hash`, `secret = LabeledExtract(shared_secret, "secret", psk)`, `key = LabeledExpand(secret, "key", key_schedule_context, 32)`, `base_nonce = LabeledExpand(secret, "base_nonce", key_schedule_context, 12)`, `exporter_secret = LabeledExpand(secret, "exp", key_schedule_context, 32)`. The labels use `hpkeSuiteId`.

`Hpke.Context` is a record of `key`, `base_nonce`, `exporter_secret`, and a sequence number starting at `0`. `Arcanum.Asymmetric` gains a project reference to `Arcanum.Symmetric` for `ChaCha20Poly1305` in phase 157. That reference is added here, so the phase that uses it changes no project file.

### Laws

RFC 9180 section A.5.1, base mode, with the values listed in phase 154. The key schedule from `shared_secret` and `info` gives the printed `key_schedule_context`, `secret`, `key`, `base_nonce`, and `exporter_secret`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 157 — Seal and open

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

RFC 9180 section 5.2. `ComputeNonce(seq) = base_nonce XOR I2OSP(seq, 12)`. `Seal` encrypts with `ChaCha20Poly1305` under `key` and that nonce, returns ciphertext followed by the 16-byte tag, and then increments the sequence number. `Open` does the reverse and increments only on success. A sequence number of `2^96 - 1` is refused before use (`MessageLimitReachedError`).

- `Hpke.computeNonce : byte[] -> BigInteger -> byte[]`
- `Hpke.seal : Context -> byte[] -> byte[] -> Result<byte[] * Context, CryptoError>` (context, aad, plaintext)
- `Hpke.openCiphertext : Context -> byte[] -> byte[] -> Result<byte[] * Context, CryptoError>` (context, aad, ciphertext)

### Laws

RFC 9180 section A.5.1.1. The plaintext of every row is the ASCII string `"Beauty is truth, truth beauty"`, and the `aad` of sequence number `n` is the ASCII string `"Count-n"`, with `n` in decimal.

```
seq 0 nonce   = 726b4390ed2209809f58c693
seq 0 ct      = 6469c41c5c81d3aa85432531ecf6460ec945bde1eb428cb2fedf7a29f5a685b4ccb0d057f03ea2952a27bb458b
seq 1 nonce   = 726b4390ed2209809f58c692
seq 1 ct      = f1564199f7e0e110ec9c1bcdde332177fc35c1adf6e57f8d1df24022227ffa8716862dbda2b1dc546c9d114374
seq 2 nonce   = 726b4390ed2209809f58c691
seq 2 ct      = 39de89728bcb774269f882af8dc5369e4f3d6322d986e872b3a8d074c7c18e8549ff3f85b6d6592ff87c3f310c
seq 4 nonce   = 726b4390ed2209809f58c697
seq 4 ct      = bc104a14fbede0cc79eeb826ea0476ce87b9c928c36e5e34dc9b6905d91473ec369a08b1a25d305dd45c6c5f80
seq 255 nonce = 726b4390ed2209809f58c66c
seq 255 ct    = 8f2814a2c548b3be50259713c6724009e092d37789f6856553d61df23ebc079235f710e6af3c3ca6eaba7c7c6c
seq 256 nonce = 726b4390ed2209809f58c793
seq 256 ct    = b45b69d419a9be7219d8c94365b89ad6951caf4576ea4774ea40e9b7047a09d6537d1aa2f7c12d6ae4b729b4d0
```

- `computeNonce base_nonce seq` gives each printed `nonce`.
- Sealing from a context whose sequence number is set to each `seq` gives each printed `ct`. Opening each `ct` at that `seq` gives the plaintext.
- Sealing sequence numbers `0, 1, 2` in a row from a fresh context gives the first three rows.
- Opening the sequence-0 `ct` with `aad` `"Count-1"` fails and leaves the sequence number at `0`.
- A context at sequence number `2^96 - 1` refuses to seal.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 158 — Export

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

RFC 9180 section 5.3: `Export(exporter_context, L) = LabeledExpand(exporter_secret, "sec", exporter_context, L)`. `Hpke.export : Context -> byte[] -> int -> Result<byte[], CryptoError>`.

### Laws

RFC 9180 section A.5.1.2:

```
exporter_context (empty), L 32                = 9b13c510416ac977b553bf1741018809c246a695f45eff6d3b0356dbefe1e660
exporter_context 00, L 32                     = 6c8b7be3a20a5684edecb4253619d9051ce8583baf850e0cb53c402bdcaf8ebb
exporter_context 54657374436f6e74657874, L 32 = 477a50d804c7c51941f69b8e32fe8288386ee1a84905fe4938d58972f24ac938
```

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 159 — PSK mode

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

`mode_psk = 1`, with `psk` and `psk_id` in the key schedule. Same `info`, plaintext, and `aad` rule as base mode.

### Laws

RFC 9180 section A.5.2. Every printed value below matches: the derived key pairs, `enc`, `shared_secret`, the key schedule, every encryption row, and every export.

```
ikmE                 = e1a4e1d50c4bfcf890f2b4c7d6b2d2aca61368eddc3c84162df2856843e1057a
pkEm                 = 04f336578b72ad7932fe867cc4d2d44a718a318037a0ec271163699cee653fa805c1fec955e562663e0c2061bb96a87d78892bff0cc0bad7906c2d998ebe1a7246
skEm                 = 7d6e4e006cee68af9b3fdd583a0ee8962df9d59fab029997ee3f456cbc857904
ikmR                 = ee51dec304abf993ef8fd52aacdd3b539108bbf6e491943266c1de89ec596a17
pkRm                 = 041eb8f4f20ab72661af369ff3231a733672fa26f385ffb959fd1bae46bfda43ad55e2d573b880831381d9367417f554ce5b2134fbba5235b44db465feffc6189e
skRm                 = 12ecde2c8bc2d5d7ed2219c71f27e3943d92b344174436af833337c557c300b3
psk                  = 0247fd33b913760fa1fa51e1892d9f307fbe65eb171e8132c2af18555a738b82
psk_id               = 456e6e796e20447572696e206172616e204d6f726961
enc                  = 04f336578b72ad7932fe867cc4d2d44a718a318037a0ec271163699cee653fa805c1fec955e562663e0c2061bb96a87d78892bff0cc0bad7906c2d998ebe1a7246
shared_secret        = ac4f260dce4db6bf45435d9c92c0e11cfdd93743bd3075949975974cc2b3d79e
key_schedule_context = 01622b72afcc3795841596c67ea74400ca3b029374d7d5640bda367c5d67b3fbeb2e986ea1c671b61cf45eec134dac0bae58ec6f63e790b1400b47c33038b0269c
secret               = 858c8087a1c056db5811e85802f375bb0c19b9983204a1575de4803575d23239
key                  = 6d61cb330b7771168c8619498e753f16198aad9566d1f1c6c70e2bc1a1a8b142
base_nonce           = 0de7655fb65e1cd51a38864e
exporter_secret      = 754ca00235b245e72d1f722a7718e7145bd113050a2aa3d89586d4cb7514bfdb
```

Encryptions:

```
seq 0 nonce   = 0de7655fb65e1cd51a38864e
seq 0 ct      = 21433eaff24d7706f3ed5b9b2e709b07230e2b11df1f2b1fe07b3c70d5948a53d6fa5c8bed194020bd9df0877b
seq 1 nonce   = 0de7655fb65e1cd51a38864f
seq 1 ct      = c74a764b4892072ea8c2c56b9bcd46c7f1e9ca8cb0a263f8b40c2ba59ac9c857033f176019562218769d3e0452
seq 2 nonce   = 0de7655fb65e1cd51a38864c
seq 2 ct      = dc8cd68863474d6e9cbb6a659335a86a54e036249d41acf909e738c847ff2bd36fe3fcacda4ededa7032c0a220
seq 4 nonce   = 0de7655fb65e1cd51a38864a
seq 4 ct      = cd54a8576353b1b9df366cb0cc042e46eef6f4cf01e205fe7d47e306b2fdd90f7185f289a26c613ca094e3be10
seq 255 nonce = 0de7655fb65e1cd51a3886b1
seq 255 ct    = 6324570c9d542c70c7e70570c1d8f4c52a89484746bf0625441890ededcc80c24ef2301c38bfd34d689d19f67d
seq 256 nonce = 0de7655fb65e1cd51a38874e
seq 256 ct    = 1ea6326c8098ed0437a553c466550114fb2ca1412cca7de98709b9ccdf19206e52c3d39180e2cf62b3e9f4baf4
```

Exports:

```
exporter_context (empty), L 32                = 530bbc2f68f078dccc89cc371b4f4ade372c9472bafe4601a8432cbb934f528d
exporter_context 00, L 32                     = 6e25075ddcc528c90ef9218f800ca3dfe1b8ff4042de5033133adb8bd54c401d
exporter_context 54657374436f6e74657874, L 32 = 6f6fbd0d1c7733f796461b3235a856cc34f676fe61ed509dfc18fa16efe6be78
```

- The key schedule in PSK mode refuses an empty `psk`, and refuses a `psk` with an empty `psk_id`.
- The key schedule in base mode refuses a non-empty `psk`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 160 — Auth mode

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

RFC 9180 section 4.1, `AuthEncap` and `AuthDecap`: `dh = DH(skE, pkR) || DH(skS, pkR)` (64 bytes) and `kem_context = enc || pkRm || pkSm`. `mode_auth = 2`.

- `Hpke.authEncapDeterministic : byte[] -> ECPoint -> BigInteger -> Result<byte[] * byte[], CryptoError>` (`ikmE`, `pkR`, `skS`)
- `Hpke.authDecap : byte[] -> BigInteger -> ECPoint -> Result<byte[], CryptoError>` (`enc`, `skR`, `pkS`)

### Laws

RFC 9180 section A.5.3. The sender key pair comes from `deriveKeyPair ikmS`. Every printed value matches, and `authDecap` gives the same `shared_secret` as `authEncapDeterministic`.

```
ikmE                 = 0ecd212019008138a31f9104d5dba76b9f8e34d5b996041fff9e3df221dd0d5d
pkEm                 = 040d5176aedba55bc41709261e9195c5146bb62d783031280775f32e507d79b5cbc5748b6be6359760c73cfe10ca19521af704ca6d91ff32fc0739527b9385d415
skEm                 = 085fd5d5e6ce6497c79df960cac93710006b76217d8bcfafbd2bb2c20ea03c42
ikmR                 = d32236d8378b9563840653789eb7bc33c3c720e537391727bf1c812d0eac110f
pkRm                 = 0444f6ee41818d9fe0f8265bffd016b7e2dd3964d610d0f7514244a60dbb7a11ece876bb110a97a2ac6a9542d7344bf7d2bd59345e3e75e497f7416cf38d296233
skRm                 = 3cb2c125b8c5a81d165a333048f5dcae29a2ab2072625adad66dbb0f48689af9
ikmS                 = 0e6be0851283f9327295fd49858a8c8908ea9783212945eef6c598ee0a3cedbb
pkSm                 = 04265529a04d4f46ab6fa3af4943774a9f1127821656a75a35fade898a9a1b014f64d874e88cddb24c1c3d79004d3a587db67670ca357ff4fba7e8b56ec013b98b
skSm                 = 39b19402e742d48d319d24d68e494daa4492817342e593285944830320912519
enc                  = 040d5176aedba55bc41709261e9195c5146bb62d783031280775f32e507d79b5cbc5748b6be6359760c73cfe10ca19521af704ca6d91ff32fc0739527b9385d415
shared_secret        = 1a45aa4792f4b166bfee7eeab0096c1a6e497480e2261b2a59aad12f2768d469
key_schedule_context = 02b738cd703db7b4106e93b4621e9a19c89c838e55964240e5d3f331aaf8b0d58b2e986ea1c671b61cf45eec134dac0bae58ec6f63e790b1400b47c33038b0269c
secret               = 9193210815b87a4c5496c9d73e609a6c92665b5ea0d760866294906d089ebb57
key                  = cf292f8a4313280a462ce55cde05b5aa5744fe4ca89a5d81b0146a5eaca8092d
base_nonce           = 7e45c21e20e869ae00492123
exporter_secret      = dba6e307f71769ba11e2c687cc19592f9d436da0c81e772d7a8a9fd28e54355f
```

Encryptions:

```
seq 0 nonce   = 7e45c21e20e869ae00492123
seq 0 ct      = 25881f219935eec5ba70d7b421f13c35005734f3e4d959680270f55d71e2f5cb3bd2daced2770bf3d9d4916872
seq 1 nonce   = 7e45c21e20e869ae00492122
seq 1 ct      = 653f0036e52a376f5d2dd85b3204b55455b7835c231255ae098d09ed138719b97185129786338ab6543f753193
seq 2 nonce   = 7e45c21e20e869ae00492121
seq 2 ct      = 60878706117f22180c788e62df6a595bc41906096a11a9513e84f0141e43239e81a98d7a235abc64112fcb8ddd
seq 4 nonce   = 7e45c21e20e869ae00492127
seq 4 ct      = 0f9094dd08240b5fa7a388b824d19d5b4b1e126cebfd67a062c32f9ba9f1f3866cc38de7df2702626e2ab65c0f
seq 255 nonce = 7e45c21e20e869ae004921dc
seq 255 ct    = dd29319e08135c5f8401d6537a364e92172c0e3f095f3fd18923881d11c0a6839345dd0b54acd0edd8f8344792
seq 256 nonce = 7e45c21e20e869ae00492023
seq 256 ct    = e2276ec5047bc4b6ed57d6da7da2fb47a77502f0a30f17d040247c73da336d722bc6c89adf68396a0912c6d152
```

Exports:

```
exporter_context (empty), L 32                = 56c4d6c1d3a46c70fd8f4ecda5d27c70886e348efb51bd5edeaa39ff6ce34389
exporter_context 00, L 32                     = d2d3e48ed76832b6b3f28fa84be5f11f09533c0e3c71825a34fb0f1320891b51
exporter_context 54657374436f6e74657874, L 32 = eb0d312b6263995b4c7761e64b688c215ffd6043ff3bad2368c862784cbe6eff
```

`authDecap` with a different sender public key, `pkEm`, gives a different `shared_secret`.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 161 — AuthPSK mode

### Files

- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

`mode_auth_psk = 3`: `AuthEncap` with `psk` and `psk_id` in the key schedule. No new code is expected. If a law fails, fix `Hpke.fs` in this phase.

### Laws

RFC 9180 section A.5.4. Every printed value matches.

```
ikmE                 = f3a07f194703e321ef1f753a1b9fe27a498dfdfa309151d70bedd896c239c499
pkEm                 = 043539917ee26f8ae0aa5f784a387981b13de33124a3cde88b94672030183110f331400115855808244ff0c5b6ca6104483ac95724481d41bdcd9f15b430ad16f6
skEm                 = 11b7e4de2d919240616a31ab14944cced79bc2372108bb98f6792e3b645fe546
ikmR                 = 1240e55a0a03548d7f963ef783b6a7362cb505e6b31dfd04c81d9b294543bfbd
pkRm                 = 04d383fd920c42d018b9d57fd73a01f1eee480008923f67d35169478e55d2e8817068daf62a06b10e0aad4a9e429fa7f904481be96b79a9c231a33e956c20b81b6
skRm                 = c29fc577b7e74d525c0043f1c27540a1248e4f2c8d297298e99010a92e94865c
ikmS                 = ce2a0387a2eb8870a3a92c34a2975f0f3f271af4384d446c7dc1524a6c6c515a
pkSm                 = 0492cf8c9b144b742fe5a63d9a181a19d416f3ec8705f24308ad316564823c344e018bd7c03a33c926bb271b28ef5bf28c0ca00abff249fee5ef7f33315ff34fdb
skSm                 = 53541bd995f874a67f8bfd8038afa67fd68876801f42ff47d0dc2a4deea067ae
psk                  = 0247fd33b913760fa1fa51e1892d9f307fbe65eb171e8132c2af18555a738b82
psk_id               = 456e6e796e20447572696e206172616e204d6f726961
enc                  = 043539917ee26f8ae0aa5f784a387981b13de33124a3cde88b94672030183110f331400115855808244ff0c5b6ca6104483ac95724481d41bdcd9f15b430ad16f6
shared_secret        = 87584311791036a3019bc36803cdd42e9a8931a98b13c88835f2f8a9036a4fd6
key_schedule_context = 03622b72afcc3795841596c67ea74400ca3b029374d7d5640bda367c5d67b3fbeb2e986ea1c671b61cf45eec134dac0bae58ec6f63e790b1400b47c33038b0269c
secret               = fe52b4412590e825ea2603fa88e145b2ee014b942a774b55fab4f081301f16f4
key                  = 31e140c8856941315d4067239fdc4ebe077fbf45a6fc78a61e7a6c8b3bacb10a
base_nonce           = 75838a8010d2e4760254dd56
exporter_secret      = 600895965755db9c5027f25f039a6e3e506c35b3b7084ce33c4a48d59ee1f0e3
```

Encryptions:

```
seq 0 nonce   = 75838a8010d2e4760254dd56
seq 0 ct      = 9eadfa0f954835e7e920ffe56dec6b31a046271cf71fdda55db72926e1d8fae94cc6280fcfabd8db71eaa65c05
seq 1 nonce   = 75838a8010d2e4760254dd57
seq 1 ct      = e357ad10d75240224d4095c9f6150a2ed2179c0f878e4f2db8ca95d365d174d059ff8c3eb38ea9a65cfc8eaeb8
seq 2 nonce   = 75838a8010d2e4760254dd54
seq 2 ct      = 2fa56d00f8dd479d67a2ec3308325cf3bbccaf102a64ffccdb006bd7dcb932685b9a7b49cdc094a85fec1da5ef
seq 4 nonce   = 75838a8010d2e4760254dd52
seq 4 ct      = 1fe9d6db14965003ed81a39abf240f9cd7c5a454bca0d69ef9a2de16d537364fbbf110b9ef11fa4a7a0172f0ce
seq 255 nonce = 75838a8010d2e4760254dda9
seq 255 ct    = eaf4041a5c9122b22d1f8d698eeffe45d64b4ae33d0ddca3a4cdf4a5f595acc95a1a9334d06cc4d000df6aaad6
seq 256 nonce = 75838a8010d2e4760254dc56
seq 256 ct    = fb857f4185ce5286c1a52431867537204963ea66a3eee8d2a74419fd8751faee066d08277ac7880473aa4143ba
```

Exports:

```
exporter_context (empty), L 32                = c52b4592cd33dd38b2a3613108ddda28dcf7f03d30f2a09703f758bfa8029c9a
exporter_context 00, L 32                     = 2f03bebc577e5729e148554991787222b5c2a02b77e9b1ac380541f710e5a318
exporter_context 54657374436f6e74657874, L 32 = e01dd49e8bfc3d9216abc1be832f0418adf8b47a7b5a330a7436c31e33d765d7
```

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 162 — HPKE refusals

### Files

- `src/Arcanum.Asymmetric/Hpke.fs`
- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

The error paths of RFC 9180 sections 4.1, 5.1, 5.2, and 7.1.

### Laws

- `deserializePublicKey` refuses the 33-byte compressed form of `pkRm`, the point at infinity `00`, and an uncompressed point that is not on P-256: `pkRm` with its last byte `06` changed to `07`.
- `decap` refuses an `enc` of the wrong length.
- `export` refuses `L = 255 * 32 + 1 = 8161`.
- Every error is a `Result` error, not an exception, and no refusal returns a default key, nonce, or point.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 163 — One vector through every layer

### Files

- `tests/Arcanum.Tests/HpkeTests.fs`

### Object

One test that starts from the RFC 9180 section A.5.1 inputs `ikmE`, `ikmR`, and `info`, and reaches the sequence-0 ciphertext with no printed intermediate value. The comment lists the modules it crosses: `HKDF` on `HMAC.hmacSha256` on the CLR SHA-256 call, `EllipticCurve.scalarMultiply` and `add` on `nistP256`, `Sec1.encodePoint`, `DiffieHellman.computeEcdhSharedSecretChecked`, and `ChaCha20Poly1305`.

### Laws

- From `ikmE`, `ikmR`, `info`, the plaintext, and `"Count-0"` alone, the test reaches the sequence-0 `ct` of phase 157.
- The receiver, from `skRm`, `enc`, and `info` alone, opens it.

### Exit

`dotnet test --configuration Release` passes. Commit. Push.

## Phase 164 — Docs catch-up

### Files

- `README.md`
- `Docs/agent_channel.md`, one new entry at the top for this phase. Do not rewrite entries from earlier phases.

### Object

The README keeps its structure and quickstart. It adds:

- The three repairs of phases 130 through 132.
- `nistP192` and `nistP224`, from RFC 5114, and that P-224 is where `modSqrt` runs its general path on a large prime.
- `toy3Twist`, and that `toy3` has no simplified SWU constant.
- `HKDF`, `EcdsaDer`, `HashToCurve`, and `Hpke`, with the CLR boundary of each: everything rests on `HMAC.hmacSha256` and `SHA256.hash`.
- The new cited checks: RFC 5114 sections A.4 and A.5, RFC 6979 sections A.2.3 and A.2.4, RFC 5869 Appendix A.1 to A.3, RFC 9380 Appendix K.1 and sections J.1.1 and J.1.2, and RFC 9180 sections A.5.1 to A.5.4. Each is a published vector, not a CAVP campaign.

Do not promise P-384, P-521, X25519, AES-GCM, a from-scratch SHA-256, or constant time.

Confirm the channel has one entry for each phase from 129 through 164, and that the entries for phases 0 through 128 are present and unedited.

### Laws

No new mathematical law. If a sentence disagrees with a type in the code, correct the sentence.

### Exit

`dotnet test --configuration Release` passes. Commit. Push. Stop.

## Parked until Leon says otherwise

- A from-scratch SHA-256 used as an implementation in this repository. SHA-384, SHA-512, and SHAKE.
- AES-GCM, AES-192, and AES-256, and so RFC 9180 sections A.1, A.3, A.4, and A.6.
- P-384, P-521, and their RFC 6979, RFC 5114, RFC 9380, and RFC 9180 vectors.
- X25519, X448, Montgomery and Edwards forms, and RFC 9180 section A.2.
- The secp256k1 hash-to-curve suite, which needs a 3-isogeny.
- RSA-OAEP known answers. The encryption seed is random.
- Low-s normalization and BIP 340 or any Bitcoin signature format.
- Pairings, MOV, isogenies, rank over `Q`, Mordell-Weil, L-functions, Birch-Swinnerton-Dyer, and Schoof.
- LWE, lattice reduction, lattice encryption, and the GLV scalar decomposition.
- A new Enigma rotor or a historical ciphertext campaign.
- Constant time anywhere.
- The Rust application. Moving the CLI demonstrations into a second repository.
- Lean, or a Sage wrapper.
- A NuGet package.

## Order dependencies

Execute phases 129 through 164 in that order. Do not start phase `N + 1` before phase `N` is pushed. Do not return to phases 0 through 128.

- Phase 129 widens `AGENTS.md` before any later phase adds a curve record or a new published source.
- Phases 130 through 132 are repairs and come first. Phase 132 changes `Rfc6979.sign`, which phases 136 and 137 use.
- Phases 133 and 134 add the records that phases 135 through 138 use.
- Phases 139 and 140 add HKDF, which phases 153 through 163 use.
- Phases 141 and 142 depend on the RFC 6979 signatures of phase 76 only.
- Phase 143 comes before phases 144 and 149 through 152.
- Phases 145 through 148 build the map on the toy curves. Phase 147 adds `toy3Twist`, which phase 148 uses.
- Phases 149 through 152 depend on phases 143 through 148.
- Phases 153 through 163 depend on phases 139 and 140, on `Sec1`, on `computeEcdhSharedSecretChecked`, and on `ChaCha20Poly1305`. Phase 156 adds the project reference that phase 157 needs.
- Phase 164 is last.
