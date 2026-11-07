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

    [<Fact>]
    let ``Phase 94 - ChaCha20-Poly1305 AEAD matches RFC 8439 Appendix A.5 decryption vector`` () =
        let key = Key (Bytes.fromHex "1c9240a5eb55d38af333888604f6b5f0473917c1402b80099dca5cbc207075c0")
        let nonce = Nonce (Bytes.fromHex "000000000102030405060708")
        let aad = Bytes.fromHex "f33388860000000000004e91"
        let tag = Tag (Bytes.fromHex "eead9d67890cbb22392336fea1851f38")

        let ctBytes =
            Bytes.fromHex (
                "64a0861575861af460f062c79be643bd5e805cfd345cf389f108670ac76c8cb2" +
                "4c6cfc18755d43eea09ee94e382d26b0bdb7b73c321b0100d4f03b7f355894cf" +
                "332f830e710b97ce98c8a84abd0b948114ad176e008d33bd60f982b1ff37c855" +
                "9797a06ef4f0ef61c186324e2b3506383606907b6a7c02b0f9f6157b53c867e4" +
                "b9166c767b804d46a59b5216cde7a4e99040c5a40433225ee282a1b0a06c523e" +
                "af4534d7f83fa1155b0047718cbc546a0d072b04b3564eea1b422273f548271a" +
                "0bb2316053fa76991955ebd63159434ecebb4e466dae5a1073a6727627097a10" +
                "49e617d91d361094fa68f0ff77987130305beaba2eda04df997b714d6c6f2c29" +
                "a6ad5cb4022b02709b")
        Assert.Equal(265, ctBytes.Length)

        let expectedPt =
            Bytes.fromHex (
                "496e7465726e65742d4472616674732061726520647261667420646f63756d65" +
                "6e74732076616c696420666f722061206d6178696d756d206f6620736978206d" +
                "6f6e74687320616e64206d617920626520757064617465642c207265706c6163" +
                "65642c206f72206f62736f6c65746564206279206f7468657220646f63756d65" +
                "6e747320617420616e792074696d652e20497420697320696e617070726f7072" +
                "6961746520746f2075736520496e7465726e65742d4472616674732061732072" +
                "65666572656e6365206d6174657269616c206f7220746f206369746520746865" +
                "6d206f74686572207468616e206173202fe2809c776f726b20696e2070726f67" +
                "726573732e2fe2809d")
        Assert.Equal(265, expectedPt.Length)

        match ChaCha20Poly1305.decrypt key nonce aad (Ciphertext ctBytes) tag with
        | Ok dec ->
            Assert.Equal<byte>(expectedPt, dec.Value)
            let prefix = Encoding.ASCII.GetBytes("Internet-Drafts are draft documents")
            Assert.Equal<byte>(prefix, dec.Value.[0 .. prefix.Length - 1])
        | Error err -> Assert.Fail(sprintf "Unexpected Appendix A.5 decrypt error: %A" err)

        match ChaCha20Poly1305.encrypt key nonce aad (Plaintext expectedPt) with
        | Ok (encCt, encTag) ->
            Assert.Equal<byte>(ctBytes, encCt.Value)
            Assert.Equal<byte>(tag.Value, encTag.Value)
        | Error err -> Assert.Fail(sprintf "Unexpected Appendix A.5 encrypt error: %A" err)
