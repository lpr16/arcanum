# Arcanum ( Geheimnis / Esoteric Knowledge )

> **A Comprehensive, Mathematically-Grounded Study of Cryptography in F#**

```text
     /\                                            
    /  \   _ __ ___ __ _ _ __  _   _ _ __ ___      
   / /\ \ | '__/ __/ _` | '_ \| | | | '_ ` _ \     
  / ____ \| | | (_| (_| | | | | |_| | | | | | |    
 /_/    \_\_|  \___\__,_|_| |_|\__,_|_| |_| |_|    
           A Study in Cryptography with F#         
```

`Arcanum` is a thorough exploration and laboratory of classical, modern symmetric, asymmetric, and exploit-driven cryptography implemented in idiomatic, pure functional **F# (.NET 10)**.

Designed with high mathematical rigor and domain-driven type safety, `Arcanum` avoids typical C/Python memory and state pitfalls by utilizing F#'s algebraic type system, units of measure, and railway-oriented result flows.

---

## 🏛️ Project Architecture

The solution is partitioned into focused, layered modules:

```text
arcanum/
├── src/
│   ├── Arcanum.Core/            # Mathematical & algebraic foundations
│   │   ├── Types.fs             # Units of measure & single-case DUs
│   │   ├── Bytes.fs             # Constant-time equality, bitwise logic, CSPRNG
│   │   ├── Modular.fs           # xGCD, modInverse, CRT, modSqrt (Tonelli-Shanks)
│   │   ├── Primes.fs            # Miller-Rabin primality, safe prime generation
│   │   ├── FiniteFields.fs      # GF(2^8) & algebraic AES S-Box derivation, GF(2^128)
│   │   └── EllipticCurve.fs     # Weierstrass curves: secp256k1 & NIST P-256
│   ├── Arcanum.Classical/       # Historical ciphers & cryptanalysis
│   │   ├── Substitution.fs      # Caesar, Atbash, Affine (coprime mod 26)
│   │   ├── Polyalphabetic.fs    # Vigenère, Beaufort
│   │   ├── Playfair.fs          # 5x5 matrix digraph substitution
│   │   ├── Enigma.fs            # Authentic Wehrmacht M3 simulator (double-stepping)
│   │   └── Cryptanalysis.fs     # Chi-squared English scoring, IoC, Caesar/Vigenère crackers
│   ├── Arcanum.Symmetric/       # Symmetric block & stream ciphers
│   │   ├── Padding.fs           # Constant-time PKCS#7 padding & validation
│   │   ├── Aes.fs               # Pure AES-128 (SubBytes, MixColumns, key expansion)
│   │   ├── Modes.fs             # ECB, CBC, and Counter (CTR) modes
│   │   ├── ChaCha20.fs          # RFC 8439 ChaCha20 stream cipher
│   │   ├── Poly1305.fs          # RFC 8439 one-time authenticator (mod 2^130 - 5)
│   │   └── ChaCha20Poly1305.fs  # RFC 8439 Authenticated Encryption (AEAD)
│   ├── Arcanum.Hashing/         # Hashing, MACs, and Key Derivation
│   │   ├── SHA256.fs            # FIPS 180-4 SHA-256
│   │   ├── HMAC.fs              # RFC 2104 Keyed-Hashing with constant-time verify
│   │   └── PBKDF2.fs            # RFC 8018 password-based key derivation
│   ├── Arcanum.Asymmetric/      # Public-key cryptosystems
│   │   ├── DiffieHellman.fs     # RFC 3526 MODP Group 5 & ECDH (secp256k1)
│   │   ├── Rsa.fs               # RSA keygen, textbook RSA, CRT acceleration, OAEP
│   │   └── Ecdsa.fs             # ECDSA signature generation & verification
│   └── Arcanum.Cli/             # Interactive demonstration runner & Exploit Lab
│       └── Program.fs           # Real-world demos: Padding Oracle, Nonce Reuse
└── tests/
    └── Arcanum.Tests/           # NIST CAVP & RFC test vectors + property tests
        └── Tests.fs
```

---

## ⚡ Quickstart

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the Interactive Suite
Execute the interactive CLI laboratory demonstrating all ciphers, key agreements, digital signatures, and live exploits:

```bash
dotnet run --project src/Arcanum.Cli/Arcanum.Cli.fsproj
```

### Run Automated Tests
Execute the unit and compliance test suite (including RFC 8439, RFC 4231, and NIST test vectors):

```bash
dotnet test
```

---

## 🔬 Core Cryptographic Studies

### 1. Algebraic & Mathematical Foundations
* **Extended Euclidean Algorithm (`xgcd`) & Modular Inverse**: Solves Bézout's identity $a \cdot x + b \cdot y = \gcd(a, b)$ and derives modular multiplicative inverses over arbitrary precision integers (`BigInteger`).
* **Chinese Remainder Theorem (CRT)**: Solves systems of modular congruences; used for accelerating RSA private key decryption by $\sim 4\times$.
* **Finite Fields $\mathbb{F}_{2^8}$ & The AES S-Box**: Demonstrates how the 256-byte AES S-Box is mathematically derived from the multiplicative inverse in $GF(2^8)$ followed by an affine transformation over $\mathbb{F}_2$.
* **Elliptic Curves over Prime Fields**: Weierstrass curve model $y^2 \equiv x^3 + ax + b \pmod p$ with point addition, point doubling, and double-and-add scalar multiplication for both `secp256k1` (Bitcoin/Ethereum) and `NIST P-256`.

### 2. Classical Cryptanalysis
* **Chi-Squared ($\chi^2$) Goodness-of-Fit**: Automated decryption of substitution ciphers by minimizing the distance against natural English letter frequencies:
  $$\chi^2 = \sum_{i=0}^{25} \frac{(O_i - E_i)^2}{E_i}$$
* **Index of Coincidence (IoC / Friedman Test)**: Differentiates monoalphabetic English ($IC \approx 0.0667$) from polyalphabetic or uniform random distributions ($IC \approx 0.0385$) and automatically recovers Vigenère key lengths.
* **Wehrmacht Enigma M3 Simulation**: Accurate simulation including rotors I–V, ring settings (*Ringstellung*), turnover notches, plugboard (*Steckerbrett*), and the historical double-stepping anomaly.

### 3. Symmetric Ciphers & AEAD
* **AES-128**: Pure functional implementation of Rijndael with key schedule expansion, SubBytes, ShiftRows, and MixColumns over $GF(2^8)$.
* **Modes of Operation**: Constant-time PKCS#7 padded CBC mode, CTR mode, and Electronic Codebook (ECB).
* **ChaCha20-Poly1305 AEAD (RFC 8439)**: Authenticated encryption with associated data, enforcing strict constant-time MAC verification prior to decrypting payloads.

### 4. Asymmetric Cryptography
* **Diffie-Hellman & ECDH**: RFC 3526 1536-bit MODP Group 5 and elliptic-curve key exchange over `secp256k1`.
* **RSA (Rivest–Shamir–Adleman)**: Probabilistic prime generation, key generation ($e = 65537$), CRT-accelerated decryption, and RFC 8017 OAEP with SHA-256 and MGF1.
* **ECDSA**: Curve-agnostic digital signatures $(r, s)$ with support for both deterministic (RFC 6979) and CSPRNG nonces.

### 5. The Exploit & Cryptanalysis Lab
* **Serge Vaudenay's CBC Padding Oracle Attack**: Exploits an oracle returning only whether PKCS#7 padding is valid, completely recovering the underlying secret plaintext block-by-block without ever knowing the key.
* **ECDSA Private Key Recovery via Nonce Reuse**: Demonstrates the mathematical vulnerability (famous for the PS3 security break) where signing two distinct messages with the same nonce $k$ allows an attacker to compute:
  $$k \equiv \frac{z_1 - z_2}{s_1 - s_2} \pmod n \implies d_A \equiv \frac{s_1 \cdot k - z_1}{r} \pmod n$$
* **Timing Side-Channel Analysis**: Demonstrates how naive string/byte equality leaking early exits reveals secrets via timing differences, contrasted against constant-time equality primitives.

---

## 🛡️ Functional Programming Principles in Cryptography

1. **Type-Safe Domain Modeling**: Plaintexts, ciphertexts, private keys, public keys, nonces, and initialization vectors are separated by single-case discriminated unions (`Plaintext`, `Ciphertext`, `Key`, `Nonce`, `Iv`), preventing accidental argument swap bugs.
2. **Side-Channel Mitigation**: Memory buffers with secret material support zeroization, and equality checks enforce constant-time execution (`Bytes.constantTimeEquals`).
3. **Railway-Oriented Security**: Cryptographic failures return strongly-typed `Result<'T, CryptoError>` states, eliminating unhandled exceptions and unsafe fallback states.

---

## 📄 License
MIT License. Created for educational and research study in cryptography.
