namespace Arcanum.Tests

open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Symmetric

module ChaChaTests =
    [<Fact>]
    let ``ChaCha20 block function matches RFC 8439 section 2.3.2 test vector`` () =
        // RFC 8439 section 2.3.2: ChaCha20 Block Function Test Vector
        // Key: 00:01:02:...:1f (32 bytes)
        // Nonce: 00:00:00:09:00:00:00:4a:00:00:00:00 (12 bytes)
        // Counter: 1
        let key = Array.init 32 (fun i -> byte i)
        let nonce = Bytes.fromHex "000000090000004a00000000"
        let counter = 1u

        let block = ChaCha20.block key counter nonce
        Assert.Equal(64, block.Length)

        let expectedFirst16 = Bytes.fromHex "10f1e7e4d13b5915500fdd1fa32071c4"
        let actualFirst16 = Array.sub block 0 16
        Assert.Equal<byte>(expectedFirst16, actualFirst16)

    [<Fact>]
    let ``Phase 89 - ChaCha20 stream encryption matches RFC 8439 section 2.4.2 test vector`` () =
        // RFC 8439 section 2.4.2: Example and Test Vector for the ChaCha20 Cipher
        let key = Key (Array.init 32 byte)
        let nonce = Nonce (Bytes.fromHex "000000000000004a00000000")
        let pt =
            Encoding.ASCII.GetBytes(
                "Ladies and Gentlemen of the class of '99: If I could offer you only one tip for the future, sunscreen would be it.")
        Assert.Equal(114, pt.Length)

        let expectedCt =
            Bytes.fromHex (
                "6e2e359a2568f98041ba0728dd0d6981" +
                "e97e7aec1d4360c20a27afccfd9fae0b" +
                "f91b65c5524733ab8f593dabcd62b357" +
                "1639d624e65152ab8f530c359f0861d8" +
                "07ca0dbf500d6a6156a38e088a22b65e" +
                "52bc514d16ccf806818ce91ab7793736" +
                "5af90bbf74a35be6b40b8eedf2785e42" +
                "874d")

        match ChaCha20.processStream key 1u nonce pt with
        | Ok ct ->
            Assert.Equal<byte>(expectedCt, ct)
            match ChaCha20.processStream key 1u nonce ct with
            | Ok dec -> Assert.Equal<byte>(pt, dec)
            | Error err -> Assert.Fail(sprintf "Unexpected ChaCha20 decrypt error: %A" err)
        | Error err -> Assert.Fail(sprintf "Unexpected ChaCha20 encrypt error: %A" err)

    [<Fact>]
    let ``Phase 90 - ChaCha20 stream encryption matches RFC 8439 Appendix A.2 test vectors 2 and 3`` () =
        // RFC 8439 Appendix A.2 Test Vector #2 (375 bytes, 6 blocks)
        let key2 = Key (Bytes.fromHex "0000000000000000000000000000000000000000000000000000000000000001")
        let nonce2 = Nonce (Bytes.fromHex "000000000000000000000002")
        let pt2 =
            Encoding.ASCII.GetBytes(
                "Any submission to the IETF intended by the Contributor for publication as all or part of an IETF Internet-Draft or RFC and any statement made within the context of an IETF activity is considered an \"IETF Contribution\". Such statements include oral statements in IETF sessions, as well as written and electronic communications made at any time or place, which are addressed to")
        Assert.Equal(375, pt2.Length)

        let expectedCt2 =
            Bytes.fromHex (
                "a3fbf07df3fa2fde4f376ca23e82737041605d9f4f4f57bd8cff2c1d4b7955ec" +
                "2a97948bd3722915c8f3d337f7d370050e9e96d647b7c39f56e031ca5eb6250d" +
                "4042e02785ececfa4b4bb5e8ead0440e20b6e8db09d881a7c6132f420e527950" +
                "42bdfa7773d8a9051447b3291ce1411c680465552aa6c405b7764d5e87bea85a" +
                "d00f8449ed8f72d0d662ab052691ca66424bc86d2df80ea41f43abf937d3259d" +
                "c4b2d0dfb48a6c9139ddd7f76966e928e635553ba76c5c879d7b35d49eb2e62b" +
                "0871cdac638939e25e8a1e0ef9d5280fa8ca328b351c3c765989cbcf3daa8b6c" +
                "cc3aaf9f3979c92b3720fc88dc95ed84a1be059c6499b9fda236e7e818b04b0b" +
                "c39c1e876b193bfe5569753f88128cc08aaa9b63d1a16f80ef2554d7189c411f" +
                "5869ca52c5b83fa36ff216b9c1d30062bebcfd2dc5bce0911934fda79a86f6e6" +
                "98ced759c3ff9b6477338f3da4f9cd8514ea9982ccafb341b2384dd902f3d1ab" +
                "7ac61dd29c6f21ba5b862f3730e37cfdc4fd806c22f221")

        match ChaCha20.processStream key2 1u nonce2 pt2 with
        | Ok ct2 -> Assert.Equal<byte>(expectedCt2, ct2)
        | Error err -> Assert.Fail(sprintf "Unexpected ChaCha20 #2 encrypt error: %A" err)

        // RFC 8439 Appendix A.2 Test Vector #3 (127 bytes, initial counter 42)
        let key3 = Key (Bytes.fromHex "1c9240a5eb55d38af333888604f6b5f0473917c1402b80099dca5cbc207075c0")
        let nonce3 = Nonce (Bytes.fromHex "000000000000000000000002")
        let pt3 =
            Encoding.ASCII.GetBytes(
                "'Twas brillig, and the slithy toves\nDid gyre and gimble in the wabe:\nAll mimsy were the borogoves,\nAnd the mome raths outgrabe.")
        Assert.Equal(127, pt3.Length)

        let expectedCt3 =
            Bytes.fromHex (
                "62e6347f95ed87a45ffae7426f27a1df" +
                "5fb69110044c0d73118effa95b01e5cf" +
                "166d3df2d721caf9b21e5fb14c616871" +
                "fd84c54f9d65b283196c7fe4f60553eb" +
                "f39c6402c42234e32a356b3e764312a6" +
                "1a5532055716ead6962568f87d3f3f77" +
                "04c6a8d1bcd1bf4d50d6154b6da731b1" +
                "87b58dfd728afa36757a797ac188d1")

        match ChaCha20.processStream key3 42u nonce3 pt3 with
        | Ok ct3 -> Assert.Equal<byte>(expectedCt3, ct3)
        | Error err -> Assert.Fail(sprintf "Unexpected ChaCha20 #3 encrypt error: %A" err)
