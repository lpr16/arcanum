namespace Arcanum.Tests

open System
open System.Text
open Xunit
open FsCheck
open FsCheck.Xunit
open Arcanum.Core
open Arcanum.Classical
open Arcanum.Symmetric
open Arcanum.Hashing
open Arcanum.Asymmetric

module Tests =

    [<Fact>]
    let ``Classical - Caesar cipher roundtrip`` () =
        let original = "THEQUICKBROWNFOXJUMPSOVERTHELAZYDOG"
        let encrypted = Substitution.caesarEncrypt 13 original
        let decrypted = Substitution.caesarDecrypt 13 encrypted
        Assert.Equal(original, decrypted)

    [<Fact>]
    let ``Classical - Atbash cipher is self-inverting`` () =
        let original = "ATTACKATDAWN"
        let once = Substitution.atbash original
        let twice = Substitution.atbash once
        Assert.Equal(original, twice)

    [<Fact>]
    let ``Classical - Enigma is self-reciprocal`` () =
        let makeConfig () = {
            Enigma.Left = { RotorType = Enigma.RotorI; RingSetting = 1; Position = 5 }
            Enigma.Middle = { RotorType = Enigma.RotorII; RingSetting = 2; Position = 12 }
            Enigma.Right = { RotorType = Enigma.RotorIII; RingSetting = 3; Position = 20 }
            Enigma.Reflector = Enigma.ReflectorB
            Enigma.Plugboard = [ ('A', 'Z'); ('B', 'Y') ]
        }
        let original = "OPERATIONBARBAROSSA"
        let encrypted = Enigma.processMessage (makeConfig ()) original
        let decrypted = Enigma.processMessage (makeConfig ()) encrypted
        Assert.Equal(original, decrypted)

    [<Fact>]
    let ``Hashing - CLR SHA-256 call matches published digest of abc`` () =
        let digest = SHA256.hashString "abc"
        let hex = Bytes.toHex digest.Value
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hex)

    [<Fact>]
    let ``Hashing - CLR SHA-256 call matches published digest of empty message`` () =
        let digest = SHA256.hash [||]
        let hex = Bytes.toHex digest.Value
        Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", hex)

    [<Fact>]
    let ``Hashing - HMAC-SHA256 RFC 4231 Test Case 1`` () =
        // Key: 20 bytes of 0x0b, Data: "Hi There"
        let keyBytes = Array.create 20 0x0buy
        let key = Key keyBytes
        let data = Encoding.ASCII.GetBytes "Hi There"
        let tag = HMAC.hmacSha256 key data
        let hex = Bytes.toHex tag.Value
        Assert.Equal("b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7", hex)

    [<Fact>]
    let ``Hashing - HMAC-SHA256 RFC 4231 Test Case 2`` () =
        // RFC 4231 Test Case 2
        // Key: "Jefe", Data: "what do ya want for nothing?"
        let key = Key (Encoding.ASCII.GetBytes "Jefe")
        let data = Encoding.ASCII.GetBytes "what do ya want for nothing?"
        let tag = HMAC.hmacSha256 key data
        let hex = Bytes.toHex tag.Value
        Assert.Equal("5bdcc146bf60754e6a042426089575c75a003f089d2739839dec58b964ec3843", hex)

    [<Fact>]
    let ``Symmetric - AES-128 CBC encrypt and decrypt roundtrip`` () =
        let key = Key (Bytes.randomBytes 16)
        let iv = Iv (Bytes.randomBytes 16)
        let original = Plaintext (Encoding.UTF8.GetBytes "Arcanum Cryptography Suite Test")

        let encResult = Modes.encryptCbc key iv original
        Assert.True(Result.isOk encResult)
        let ciphertext = match encResult with Ok c -> c | Error _ -> failwith ""

        let decResult = Modes.decryptCbc key iv ciphertext
        Assert.True(Result.isOk decResult)
        let decrypted = match decResult with Ok p -> p | Error _ -> failwith ""

        Assert.Equal<byte>(original.Value, decrypted.Value)

    [<Fact>]
    let ``Symmetric - ChaCha20-Poly1305 AEAD roundtrip and tampering rejection`` () =
        let key = Key (Bytes.randomBytes 32)
        let nonce = Nonce (Bytes.randomBytes 12)
        let aad = Encoding.UTF8.GetBytes "associated-metadata"
        let message = Plaintext (Encoding.UTF8.GetBytes "Confidential Payload")

        let encRes = ChaCha20Poly1305.encrypt key nonce aad message
        Assert.True(Result.isOk encRes)
        let (ct, tag) = match encRes with Ok res -> res | Error _ -> failwith ""

        // Decrypt valid
        let decRes = ChaCha20Poly1305.decrypt key nonce aad ct tag
        Assert.True(Result.isOk decRes)
        let decrypted = match decRes with Ok p -> p | Error _ -> failwith ""
        Assert.Equal<byte>(message.Value, decrypted.Value)

        // Tamper with ciphertext -> MUST fail authentication
        let tamperedCtBytes = Array.copy ct.Value
        tamperedCtBytes.[0] <- tamperedCtBytes.[0] ^^^ 1uy
        let tamperedCt = Ciphertext tamperedCtBytes
        let failRes = ChaCha20Poly1305.decrypt key nonce aad tamperedCt tag
        Assert.Equal(Error AuthenticationFailed, failRes)

    [<Fact>]
    let ``Asymmetric - ECDH Key Agreement on secp256k1`` () =
        let curve = EllipticCurve.secp256k1
        let alice = DiffieHellman.generateEcdhKeyPair curve
        let bob = DiffieHellman.generateEcdhKeyPair curve

        let s1 = DiffieHellman.computeEcdhSharedSecret curve alice.PrivateKey bob.PublicKey
        let s2 = DiffieHellman.computeEcdhSharedSecret curve bob.PrivateKey alice.PublicKey

        Assert.Equal(s1, s2)

    [<Fact>]
    let ``Asymmetric - ECDSA Sign and Verify on secp256k1`` () =
        let curve = EllipticCurve.secp256k1
        let signer = Ecdsa.generateKeyPair curve
        let doc = Encoding.UTF8.GetBytes "Smart Contract Transaction Hash"

        let sigResult = Ecdsa.sign signer doc
        Assert.True(Result.isOk sigResult)
        let signature = match sigResult with Ok s -> s | Error _ -> failwith ""

        let isValid = Ecdsa.verify curve signer.PublicKey doc signature
        Assert.True(isValid)

        // Tampered doc must fail
        let tamperedDoc = Encoding.UTF8.GetBytes "Fraudulent Transaction"
        let isTamperedValid = Ecdsa.verify curve signer.PublicKey tamperedDoc signature
        Assert.False(isTamperedValid)
