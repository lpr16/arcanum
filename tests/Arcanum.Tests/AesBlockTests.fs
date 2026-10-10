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
