namespace Arcanum.Asymmetric

open System
open System.Numerics
open Arcanum.Core
open Arcanum.Hashing

module Rsa =
    type RsaPublicKey = {
        N: BigInteger
        E: BigInteger
    }

    type RsaPrivateKey = {
        N: BigInteger
        E: BigInteger
        D: BigInteger
        P: BigInteger
        Q: BigInteger
        Dp: BigInteger
        Dq: BigInteger
        QInv: BigInteger
    }

    type RsaKeyPair = {
        PublicKey: RsaPublicKey
        PrivateKey: RsaPrivateKey
    }

    /// Generates an RSA key pair with the exact specified bit length (even, >= 16).
    let generateKeyPair (bitLength: int) : RsaKeyPair =
        if bitLength < 16 || bitLength % 2 <> 0 then
            raise (ArgumentOutOfRangeException(nameof bitLength, "Bit length must be even and at least 16"))

        let primeBits = bitLength / 2
        let e = 65537I

        let rec findPrimes () =
            let p = Primes.generatePrime primeBits 40
            let q = Primes.generatePrime primeBits 40
            let n = p * q
            if p = q || int (n.GetBitLength()) <> bitLength then findPrimes ()
            else
                let phi = (p - BigInteger.One) * (q - BigInteger.One)
                match Modular.modInverse e phi with
                | None -> findPrimes () // e and phi not coprime
                | Some d ->
                    let dp = d % (p - BigInteger.One)
                    let dq = d % (q - BigInteger.One)
                    let qInv =
                        match Modular.modInverse q p with
                        | Some inv -> inv
                        | None -> BigInteger.Zero

                    let pubKey = { N = n; E = e }
                    let privKey = {
                        N = n; E = e; D = d
                        P = p; Q = q
                        Dp = dp; Dq = dq
                        QInv = qInv
                    }
                    { PublicKey = pubKey; PrivateKey = privKey }

        findPrimes ()

    /// Textbook RSA Encryption: c = m^e mod n.
    /// WARNING: Malleable and insecure for raw messages without padding!
    let encryptRaw (pubKey: RsaPublicKey) (message: BigInteger) : BigInteger =
        if message >= pubKey.N then
            raise (ArgumentOutOfRangeException(nameof message, "Message representative out of range (>= N)"))
        Modular.modPow message pubKey.E pubKey.N

    /// Textbook RSA Decryption using Chinese Remainder Theorem (CRT) acceleration:
    /// m1 = c^dp mod p, m2 = c^dq mod q, h = qInv * (m1 - m2) mod p, m = m2 + h * q.
    let decryptCrt (privKey: RsaPrivateKey) (ciphertext: BigInteger) : BigInteger =
        let m1 = Modular.modPow ciphertext privKey.Dp privKey.P
        let m2 = Modular.modPow ciphertext privKey.Dq privKey.Q
        let h = Modular.modPos (privKey.QInv * (m1 - m2)) privKey.P
        m2 + h * privKey.Q

    /// Mask Generation Function 1 (MGF1) per PKCS#1 v2.2 using SHA-256.
    let mgf1 (seed: byte[]) (maskLen: int) : byte[] =
        let hLen = 32
        let count = (maskLen + hLen - 1) / hLen
        let mask = Array.zeroCreate maskLen

        for counter = 0 to count - 1 do
            let cBytes = [|
                byte (counter >>> 24)
                byte (counter >>> 16)
                byte (counter >>> 8)
                byte counter
            |]
            let hashInput = Array.append seed cBytes
            let digest = (SHA256.hash hashInput).Value
            let offset = counter * hLen
            let len = min hLen (maskLen - offset)
            Array.Copy(digest, 0, mask, offset, len)

        mask

    /// Byte length of an RSA modulus N: (bitLength N + 7) / 8.
    let modulusLength (n: BigInteger) : int =
        if n <= BigInteger.Zero then 0
        else int ((n.GetBitLength() + 7L) / 8L)

    let private i2osp (value: BigInteger) (len: int) : byte[] =
        let raw = value.ToByteArray(isUnsigned = true, isBigEndian = true)
        let result = Array.zeroCreate len
        let copyLen = min len raw.Length
        Array.Copy(raw, raw.Length - copyLen, result, len - copyLen, copyLen)
        result

    /// RSA-OAEP Encryption with SHA-256 per RFC 8017.
    let encryptOaep (pubKey: RsaPublicKey) (label: byte[]) (plaintext: byte[]) : Result<byte[], CryptoError> =
        let k = modulusLength pubKey.N
        let hLen = 32
        let maxMlen = k - 2 * hLen - 2

        if maxMlen < 0 || plaintext.Length > maxMlen then
            Error (ParameterOutOfRange $"Message too long for RSA key size (max {maxMlen} bytes)")
        else
            let lHash = (SHA256.hash label).Value
            let psLen = k - plaintext.Length - 2 * hLen - 2
            let ps = Array.zeroCreate psLen

            // DB = lHash || PS || 0x01 || M
            let db = Array.concat [ lHash; ps; [| 0x01uy |]; plaintext ]
            let seed = Bytes.randomBytes hLen

            let dbMask = mgf1 seed (k - hLen - 1)
            let maskedDb = Bytes.xor db dbMask

            let seedMask = mgf1 maskedDb hLen
            let maskedSeed = Bytes.xor seed seedMask

            // EM = 0x00 || maskedSeed || maskedDB
            let em = Array.concat [ [| 0x00uy |]; maskedSeed; maskedDb ]
            let mInt = BigInteger(ReadOnlySpan<byte>(em), isUnsigned = true, isBigEndian = true)
            let cInt = encryptRaw pubKey mInt
            Ok (i2osp cInt k)

    /// RSA-OAEP Decryption with SHA-256 per RFC 8017 section 7.1.2, using decryptCrt.
    let decryptOaep (privKey: RsaPrivateKey) (label: byte[]) (ciphertext: byte[]) : Result<byte[], CryptoError> =
        let k = modulusLength privKey.N
        let hLen = 32

        if ciphertext.Length <> k || k < 2 * hLen + 2 then
            Error (DecryptionFailed "Invalid RSA-OAEP ciphertext length")
        else
            let cInt = BigInteger(ReadOnlySpan<byte>(ciphertext), isUnsigned = true, isBigEndian = true)
            if cInt >= privKey.N then
                Error (DecryptionFailed "Ciphertext representative out of range")
            else
                let mInt = decryptCrt privKey cInt
                let em = i2osp mInt k
                let y = em.[0]
                let maskedSeed = em.[1 .. hLen]
                let maskedDb = em.[hLen + 1 .. k - 1]

                let seedMask = mgf1 maskedDb hLen
                let seed = Bytes.xor maskedSeed seedMask
                let dbMask = mgf1 seed (k - hLen - 1)
                let db = Bytes.xor maskedDb dbMask

                let lHash = (SHA256.hash label).Value
                let lHashPrime = db.[0 .. hLen - 1]
                let rest = db.[hLen .. db.Length - 1]
                let sepIdx = rest |> Array.tryFindIndex (fun b -> b <> 0uy)

                if y <> 0uy || lHashPrime <> lHash then
                    Error (DecryptionFailed "RSA-OAEP header or label hash mismatch")
                else
                    match sepIdx with
                    | Some idx when rest.[idx] = 0x01uy ->
                        Ok rest.[idx + 1 .. rest.Length - 1]
                    | _ ->
                        Error (DecryptionFailed "RSA-OAEP missing 0x01 separator")
