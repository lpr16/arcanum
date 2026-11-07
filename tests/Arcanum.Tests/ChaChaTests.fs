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
