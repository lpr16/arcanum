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

    [<Fact>]
    let ``Phase 86 - FIPS 197 Appendix A.1 key expansion and Appendix B cipher example`` () =
        // FIPS 197 Appendix A.1 (key expansion) and Appendix B (cipher example), not a NIST CAVP vector file.
        let key = Bytes.fromHex "2b7e151628aed2a6abf7158809cf4f3c"
        let w = Aes.expandKey128 key
        Assert.Equal(44, w.Length)
        Assert.Equal(0x2b7e1516u, w.[0])
        Assert.Equal(0x28aed2a6u, w.[1])
        Assert.Equal(0xabf71588u, w.[2])
        Assert.Equal(0x09cf4f3cu, w.[3])
        Assert.Equal(0xa0fafe17u, w.[4])
        Assert.Equal(0x88542cb1u, w.[5])
        Assert.Equal(0xf2c295f2u, w.[8])
        Assert.Equal(0xd4d1c6f8u, w.[20])
        Assert.Equal(0xd014f9a8u, w.[40])
        Assert.Equal(0xb6630ca6u, w.[43])

        let input = Bytes.fromHex "3243f6a8885a308d313198a2e0370734"
        let expectedCiphertext = Bytes.fromHex "3925841d02dc09fbdc118597196a0b32"
        let actualCiphertext = Aes.encryptBlock w input
        Assert.Equal<byte>(expectedCiphertext, actualCiphertext)
        Assert.Equal<byte>(input, Aes.decryptBlock w actualCiphertext)
