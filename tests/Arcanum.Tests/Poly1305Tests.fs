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
