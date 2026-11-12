namespace Arcanum.Tests

open System
open System.Numerics
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

    [<Fact>]
    let ``Phase 106 - SEC 1 round-trip on every point of toy, toy2, and toy3`` () =
        let allPoints (curve: EllipticCurve.Curve) =
            EllipticCurve.Infinity
            :: [ for x in 0I .. curve.P - 1I do
                   for y in 0I .. curve.P - 1I do
                       let pt = EllipticCurve.Point(x, y)
                       if EllipticCurve.isOnCurve curve pt then
                           yield pt ]

        for (curve, expectedCount) in [ (EllipticCurve.toy, 18); (EllipticCurve.toy2, 16); (EllipticCurve.toy3, 19) ] do
            let pts = allPoints curve
            Assert.Equal(expectedCount, pts.Length)

            for compressed in [ true; false ] do
                let encodings =
                    [ for q in pts ->
                        let enc = Sec1.encodePoint curve compressed q
                        let expectedLen =
                            match q, compressed with
                            | EllipticCurve.Infinity, _ -> 1
                            | _, true -> 2
                            | _, false -> 3
                        Assert.Equal(expectedLen, enc.Length)
                        Assert.Equal(Ok q, Sec1.decodePoint curve enc)
                        Bytes.toHex enc ]
                Assert.Equal(expectedCount, (List.distinct encodings).Length)

    [<Fact>]
    let ``Phase 138 - SEC 1 on NIST P-192 and P-224 generators and leading zero byte round-trip`` () =
        let p192 = EllipticCurve.nistP192
        let p224 = EllipticCurve.nistP224

        // P-192 Gy ends in hex digit 1, so it is odd.
        let p192Gx, p192Gy =
            match p192.G with
            | EllipticCurve.Point(x, y) -> x, y
            | _ -> failwith "unreachable"
        Assert.Equal(BigInteger.One, p192Gy &&& BigInteger.One)

        // Compressed P-192 G is 03 || 24-byte Gx (length 25)
        let p192Compressed = Sec1.encodePoint p192 true p192.G
        Assert.Equal(25, p192Compressed.Length)
        Assert.Equal(0x03uy, p192Compressed.[0])
        Assert.Equal(Ok p192.G, Sec1.decodePoint p192 p192Compressed)

        // Uncompressed P-192 G is 04 || Gx || Gy (length 49)
        let p192Uncompressed = Sec1.encodePoint p192 false p192.G
        Assert.Equal(49, p192Uncompressed.Length)
        Assert.Equal(0x04uy, p192Uncompressed.[0])
        Assert.Equal(Ok p192.G, Sec1.decodePoint p192 p192Uncompressed)

        // P-224 Gy ends in hex digit 4, so it is even.
        let p224Gx, p224Gy =
            match p224.G with
            | EllipticCurve.Point(x, y) -> x, y
            | _ -> failwith "unreachable"
        Assert.Equal(BigInteger.Zero, p224Gy &&& BigInteger.One)

        // Compressed P-224 G is 02 || 28-byte Gx (length 29)
        let p224Compressed = Sec1.encodePoint p224 true p224.G
        Assert.Equal(29, p224Compressed.Length)
        Assert.Equal(0x02uy, p224Compressed.[0])
        Assert.Equal(Ok p224.G, Sec1.decodePoint p224 p224Compressed)

        // Uncompressed P-224 G is 04 || Gx || Gy (length 57)
        let p224Uncompressed = Sec1.encodePoint p224 false p224.G
        Assert.Equal(57, p224Uncompressed.Length)
        Assert.Equal(0x04uy, p224Uncompressed.[0])
        Assert.Equal(Ok p224.G, Sec1.decodePoint p224 p224Uncompressed)

        // Modular.modSqrt of Gx^3 + A Gx + B modulo P-224 prime is Gy or p - Gy (exercises Tonelli-Shanks with S = 96)
        let rhs = Modular.modPos (Modular.modPow p224Gx 3I p224.P + p224.A * p224Gx + p224.B) p224.P
        let sqrtRhs = Modular.modSqrt rhs p224.P
        Assert.True(sqrtRhs = Some p224Gy || sqrtRhs = Some (p224.P - p224Gy))

        // P-224 public key U from Phase 137 whose Ux begins with a zero byte
        let ux = BigInteger.Parse("000CF08DA5AD719E42707FA431292DEA11244D64FC51610D94B130D6C", Globalization.NumberStyles.HexNumber)
        let uy = BigInteger.Parse("0EEAB6F3DEBE455E3DBF85416F7030CBD94F34F2D6F232C69F3C1385A", Globalization.NumberStyles.HexNumber)
        let u = EllipticCurve.Point(ux, uy)

        let compressedU = Sec1.encodePoint p224 true u
        Assert.Equal(29, compressedU.Length)
        Assert.Equal(0x00uy, compressedU.[1]) // leading zero byte is kept
        Assert.Equal(Ok u, Sec1.decodePoint p224 compressedU)

        let uncompressedU = Sec1.encodePoint p224 false u
        Assert.Equal(57, uncompressedU.Length)
        Assert.Equal(0x00uy, uncompressedU.[1]) // leading zero byte is kept
        Assert.Equal(Ok u, Sec1.decodePoint p224 uncompressedU)

