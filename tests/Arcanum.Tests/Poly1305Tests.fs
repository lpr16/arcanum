namespace Arcanum.Tests

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
