namespace Arcanum.Tests

open Xunit
open Arcanum.Core

module Sec1Tests =

    // EllipticCurve.toy is a 18-point curve over F_17; toy is not a secure group.

    [<Fact>]
    let ``Phase 57 - SEC 1 encodePoint and decodePoint on toy`` () =
        let curve = EllipticCurve.toy
        let p68 = EllipticCurve.Point(6I, 8I)
        let p69 = EllipticCurve.Point(6I, 9I)

        Assert.Equal<byte[]>([| 0x04uy; 0x06uy; 0x08uy |], Sec1.encodePoint curve false p68)
        Assert.Equal<byte[]>([| 0x02uy; 0x06uy |], Sec1.encodePoint curve true p68)
        Assert.Equal<byte[]>([| 0x03uy; 0x06uy |], Sec1.encodePoint curve true p69)
        Assert.Equal<byte[]>([| 0x00uy |], Sec1.encodePoint curve false EllipticCurve.Infinity)
        Assert.Equal<byte[]>([| 0x00uy |], Sec1.encodePoint curve true EllipticCurve.Infinity)

        Assert.Equal(Ok p68, Sec1.decodePoint curve [| 0x04uy; 0x06uy; 0x08uy |])
        Assert.Equal(Ok p68, Sec1.decodePoint curve [| 0x02uy; 0x06uy |])
        Assert.Equal(Ok p69, Sec1.decodePoint curve [| 0x03uy; 0x06uy |])
        Assert.Equal(Ok EllipticCurve.Infinity, Sec1.decodePoint curve [| 0x00uy |])

        // 3^3 + 1 = 28 = 11 (mod 17), which is not a square modulo 17.
        match Sec1.decodePoint curve [| 0x02uy; 0x03uy |] with
        | Error _ -> ()
        | Ok _ -> Assert.Fail("Expected 02 03 to be refused on toy")

        // (6, 7) is not on toy.
        match Sec1.decodePoint curve [| 0x04uy; 0x06uy; 0x07uy |] with
        | Error _ -> ()
        | Ok _ -> Assert.Fail("Expected 04 06 07 to be refused on toy")

        // 04 06 has wrong length for uncompressed point on toy.
        match Sec1.decodePoint curve [| 0x04uy; 0x06uy |] with
        | Error _ -> ()
        | Ok _ -> Assert.Fail("Expected 04 06 to be refused for wrong length")

        // 05 06 has an unknown SEC 1 prefix.
        match Sec1.decodePoint curve [| 0x05uy; 0x06uy |] with
        | Error _ -> ()
        | Ok _ -> Assert.Fail("Expected 05 06 to be refused for unknown prefix")
