namespace Arcanum.Tests

open System
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

    [<Fact>]
    let ``Phase 58 - Compressed and uncompressed generators of NIST P-256 and secp256k1`` () =
        let p256 = EllipticCurve.nistP256
        let k1 = EllipticCurve.secp256k1

        let expectedP256Compressed =
            Convert.FromHexString("036b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296")
        let expectedK1Compressed =
            Convert.FromHexString("0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798")

        let actualP256Compressed = Sec1.encodePoint p256 true p256.G
        let actualK1Compressed = Sec1.encodePoint k1 true k1.G

        Assert.Equal<byte[]>(expectedP256Compressed, actualP256Compressed)
        Assert.Equal<byte[]>(expectedK1Compressed, actualK1Compressed)

        Assert.Equal(Ok p256.G, Sec1.decodePoint p256 actualP256Compressed)
        Assert.Equal(Ok k1.G, Sec1.decodePoint k1 actualK1Compressed)

        let p256Uncompressed = Sec1.encodePoint p256 false p256.G
        let k1Uncompressed = Sec1.encodePoint k1 false k1.G

        Assert.Equal(65, p256Uncompressed.Length)
        Assert.Equal(65, k1Uncompressed.Length)
        Assert.Equal(Ok p256.G, Sec1.decodePoint p256 p256Uncompressed)
        Assert.Equal(Ok k1.G, Sec1.decodePoint k1 k1Uncompressed)
