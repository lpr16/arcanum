namespace Arcanum.Tests

open Xunit
open Arcanum.Core
open Arcanum.Symmetric

module AesBlockTests =
    [<Fact>]
    let ``AES-128 block matches FIPS 197 Appendix C.1 test vector`` () =
        // FIPS 197 Appendix C.1: Example vectors for AES-128
        // This is a single cited block check, not a NIST CAVP vector file.
        let key = Bytes.fromHex "000102030405060708090a0b0c0d0e0f"
        let plaintext = Bytes.fromHex "00112233445566778899aabbccddeeff"
        let expectedCiphertext = Bytes.fromHex "69c4e0d86a7b0430d8cdb78070b4c55a"

        let roundKeys = Aes.expandKey128 key
        let actualCiphertext = Aes.encryptBlock roundKeys plaintext
        Assert.Equal<byte>(expectedCiphertext, actualCiphertext)

        let decryptedPlaintext = Aes.decryptBlock roundKeys actualCiphertext
        Assert.Equal<byte>(plaintext, decryptedPlaintext)

    [<Fact>]
    let ``Phase 45 - ECB carries the FIPS 197 Appendix C.1 block`` () =
        // The first block is the FIPS 197 Appendix C.1 ciphertext. encryptEcb pads with PKCS#7 even when the
        // plaintext is already 16 bytes, so the second block is padding, not a second FIPS vector.
        // This is not a NIST CAVP vector file.
        let key = Key (Bytes.fromHex "000102030405060708090a0b0c0d0e0f")
        let plaintext = Bytes.fromHex "00112233445566778899aabbccddeeff"
        let expectedFirstBlock = Bytes.fromHex "69c4e0d86a7b0430d8cdb78070b4c55a"

        let ciphertext = Modes.encryptEcb key (Plaintext plaintext)
        Assert.Equal(32, ciphertext.Length)
        Assert.Equal<byte>(expectedFirstBlock, ciphertext.Value.[0 .. 15])

        match Modes.decryptEcb key ciphertext with
        | Ok decrypted -> Assert.Equal<byte>(plaintext, decrypted.Value)
        | Error err -> Assert.Fail($"Expected decryption, got error: {err}")
