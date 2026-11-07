namespace Arcanum.Tests

open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Symmetric

module AeadTests =

    [<Fact>]
    let ``Phase 93 - ChaCha20-Poly1305 AEAD matches RFC 8439 section 2.8.2 test vector`` () =
        let key = Key (Bytes.fromHex "808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9f")
        let nonce = Nonce (Bytes.fromHex "070000004041424344454647")
        let aad = Bytes.fromHex "50515253c0c1c2c3c4c5c6c7"
        let ptBytes =
            Encoding.ASCII.GetBytes(
                "Ladies and Gentlemen of the class of '99: If I could offer you only one tip for the future, sunscreen would be it.")
        Assert.Equal(114, ptBytes.Length)

        let expectedCt =
            Bytes.fromHex (
                "d31a8d34648e60db7b86afbc53ef7ec2a4aded51296e08fea9e2b5a736ee62d6" +
                "3dbea45e8ca9671282fafb69da92728b1a71de0a9e060b2905d6a5b67ecd3b36" +
                "92ddbd7f2d778b8c9803aee328091b58fab324e4fad675945585808b4831d7bc" +
                "3ff4def08e4b7a9de576d26586cec64b6116")
        let expectedTag = Bytes.fromHex "1ae10b594f09e26a7e902ecbd0600691"

        match ChaCha20Poly1305.encrypt key nonce aad (Plaintext ptBytes) with
        | Ok (ct, tag) ->
            Assert.Equal<byte>(expectedCt, ct.Value)
            Assert.Equal<byte>(expectedTag, tag.Value)

            match ChaCha20Poly1305.decrypt key nonce aad ct tag with
            | Ok dec -> Assert.Equal<byte>(ptBytes, dec.Value)
            | Error err -> Assert.Fail(sprintf "Unexpected AEAD decrypt error: %A" err)

            let badTagBytes = Array.copy tag.Value
            badTagBytes.[0] <- badTagBytes.[0] ^^^ 1uy
            Assert.Equal(Error AuthenticationFailed, ChaCha20Poly1305.decrypt key nonce aad ct (Tag badTagBytes))

            let badAad = Array.copy aad
            badAad.[badAad.Length - 1] <- 0xc8uy
            Assert.Equal(Error AuthenticationFailed, ChaCha20Poly1305.decrypt key nonce badAad ct tag)
        | Error err -> Assert.Fail(sprintf "Unexpected AEAD encrypt error: %A" err)
