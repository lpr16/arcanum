namespace Arcanum.Tests

open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Symmetric

module Poly1305Tests =
    [<Fact>]
    let ``Poly1305 clamp and empty message laws`` () =
        // clampR of 16 bytes of 0xff
        let allFf = Array.create 16 0xffuy
        let clamped = Poly1305.clampR allFf
        Assert.Equal(0x0fuy, clamped.[3])
        Assert.Equal(0xfcuy, clamped.[4])
        Assert.Equal(0x0fuy, clamped.[7])
        Assert.Equal(0x0fuy, clamped.[15])

        // Empty message: tag equals s (the last 16 bytes of key)
        let rPart = Array.init 16 (fun i -> byte (i + 42))
        let sPart = Array.init 16 (fun i -> byte i)
        let key = Array.append rPart sPart
        let emptyMsg : byte[] = [||]

        let tag = Poly1305.mac key emptyMsg
        Assert.Equal<byte>(sPart, tag.Value)

    [<Fact>]
    let ``Phase 91 - RFC 8439 section 2.5.2 Poly1305 MAC and section 2.6.2 Poly1305 key generation`` () =
        let polyKey = Bytes.fromHex "85d6be7857556d337f4452fe42d506a80103808afb0db2fd4abff6af4149f51b"
        let msg = Encoding.ASCII.GetBytes("Cryptographic Forum Research Group")
        Assert.Equal(34, msg.Length)
        let expectedTag = Bytes.fromHex "a8061dc1305136c6c22b8baf0c0127a9"
        let actualTag = Poly1305.mac polyKey msg
        Assert.Equal<byte>(expectedTag, actualTag.Value)

        let chachaKey = Bytes.fromHex "808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9f"
        let nonce = Bytes.fromHex "000000000001020304050607"
        let block0 = ChaCha20.block chachaKey 0u nonce
        let expectedOtk = Bytes.fromHex "8ad5a08b905f81cc815040274ab29471a833b637e3fd0da508dbb8e2fdd1a646"
        Assert.Equal<byte>(expectedOtk, block0.[0 .. 31])

    [<Fact>]
    let ``Phase 92 - RFC 8439 Appendix A.3 Poly1305 test vectors 1, 4, and 5 through 11`` () =
        // #1: 32 zero bytes key, 64 zero bytes message -> 16 zero bytes tag
        let tag1 = Poly1305.mac (Array.zeroCreate 32) (Array.zeroCreate 64)
        Assert.Equal<byte>(Array.zeroCreate 16, tag1.Value)

        // #4: 127-byte Jabberwocky message
        let key4 = Bytes.fromHex "1c9240a5eb55d38af333888604f6b5f0473917c1402b80099dca5cbc207075c0"
        let msg4 =
            Encoding.ASCII.GetBytes(
                "'Twas brillig, and the slithy toves\nDid gyre and gimble in the wabe:\nAll mimsy were the borogoves,\nAnd the mome raths outgrabe.")
        Assert.Equal(127, msg4.Length)
        let expectedTag4 = Bytes.fromHex "4541669a7eaaee61e708dc7cbcc5eb62"
        Assert.Equal<byte>(expectedTag4, (Poly1305.mac key4 msg4).Value)

        let checkRs (rHex: string) (sHex: string) (dataHex: string) (tagHex: string) =
            let key = Bytes.fromHex (rHex + sHex)
            let data = Bytes.fromHex dataHex
            let expected = Bytes.fromHex tagHex
            Assert.Equal<byte>(expected, (Poly1305.mac key data).Value)

        // #5
        checkRs
            "02000000000000000000000000000000"
            "00000000000000000000000000000000"
            "ffffffffffffffffffffffffffffffff"
            "03000000000000000000000000000000"

        // #6
        checkRs
            "02000000000000000000000000000000"
            "ffffffffffffffffffffffffffffffff"
            "02000000000000000000000000000000"
            "03000000000000000000000000000000"

        // #7
        checkRs
            "01000000000000000000000000000000"
            "00000000000000000000000000000000"
            ("ffffffffffffffffffffffffffffffff" +
             "f0ffffffffffffffffffffffffffffff" +
             "11000000000000000000000000000000")
            "05000000000000000000000000000000"

        // #8
        checkRs
            "01000000000000000000000000000000"
            "00000000000000000000000000000000"
            ("ffffffffffffffffffffffffffffffff" +
             "fbfefefefefefefefefefefefefefefe" +
             "01010101010101010101010101010101")
            "00000000000000000000000000000000"

        // #9
        checkRs
            "02000000000000000000000000000000"
            "00000000000000000000000000000000"
            "fdffffffffffffffffffffffffffffff"
            "faffffffffffffffffffffffffffffff"

        // #10
        checkRs
            "01000000000000000400000000000000"
            "00000000000000000000000000000000"
            ("e33594d7505e43b90000000000000000" +
             "3394d7505e4379cd0100000000000000" +
             "00000000000000000000000000000000" +
             "01000000000000000000000000000000")
            "14000000000000005500000000000000"

        // #11
        checkRs
            "01000000000000000400000000000000"
            "00000000000000000000000000000000"
            ("e33594d7505e43b90000000000000000" +
             "3394d7505e4379cd0100000000000000" +
             "00000000000000000000000000000000")
            "13000000000000000000000000000000"
