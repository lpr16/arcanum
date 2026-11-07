namespace Arcanum.Tests

open System
open System.Globalization
open System.Numerics
open Xunit
open Arcanum.Core

module NafTests =

    [<Fact>]
    let ``Phase 108 - Non-adjacent form (NAF) representation and laws`` () =
        Assert.Equal<int list>([], EllipticCurve.naf 0I)
        Assert.Equal<int list>([ 1 ], EllipticCurve.naf 1I)
        Assert.Equal<int list>([ -1; 0; 0; 1 ], EllipticCurve.naf 7I)
        Assert.Equal<int list>([ 1; 0; -1; 0; 0; 1 ], EllipticCurve.naf 29I)
        Assert.Equal<int list>([ -1; 0; 0; 0; 0; 0; 0; 0; 1 ], EllipticCurve.naf 255I)
        Assert.Equal<int list>([ 0; 0; 0; 1; 0; -1; 0; 0; 0; 0; 1 ], EllipticCurve.naf 1000I)
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> EllipticCurve.naf -1I |> ignore) |> ignore

        for k in 0I .. 2000I do
            let digits = EllipticCurve.naf k
            let rebuilt =
                digits
                |> List.mapi (fun i d -> BigInteger d * (BigInteger.One <<< i))
                |> List.fold (+) BigInteger.Zero
            Assert.Equal(k, rebuilt)

            for d in digits do
                Assert.True(d = -1 || d = 0 || d = 1)

            for (d1, d2) in List.pairwise digits do
                Assert.True(d1 = 0 || d2 = 0)

            Assert.True(digits.Length <= int (k.GetBitLength()) + 1)

    let private allPoints (curve: EllipticCurve.Curve) =
        EllipticCurve.Infinity
        :: [ for x in 0I .. curve.P - 1I do
               for y in 0I .. curve.P - 1I do
                   let pt = EllipticCurve.Point(x, y)
                   if EllipticCurve.isOnCurve curve pt then
                       yield pt ]

    let private hexInt (s: string) =
        BigInteger.Parse("0" + s, NumberStyles.HexNumber)

    [<Fact>]
    let ``Phase 109 - Scalar multiplication by NAF agrees with scalarMultiply on toy curves and large curves`` () =
        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3 ] do
            for q in allPoints curve do
                for k in 0I .. curve.N - 1I do
                    Assert.Equal(
                        EllipticCurve.scalarMultiply curve k q,
                        EllipticCurve.scalarMultiplyNaf curve k q)

        for curve in [ EllipticCurve.secp256k1; EllipticCurve.nistP256 ] do
            Assert.Equal(curve.G, EllipticCurve.scalarMultiplyNaf curve 1I curve.G)
            Assert.Equal(EllipticCurve.double curve curve.G, EllipticCurve.scalarMultiplyNaf curve 2I curve.G)
            Assert.Equal(EllipticCurve.negate curve curve.G, EllipticCurve.scalarMultiplyNaf curve (curve.N - 1I) curve.G)

        let expectedSecp2G =
            EllipticCurve.Point(
                hexInt "c6047f9441ed7d6d3045406e95c07cd85c778e4b8cef3ca7abac09b95c709ee5",
                hexInt "1ae168fea63dc339a3c58419466ceaeef7f632653266d0e1236431a950cfe52a")
        Assert.Equal(expectedSecp2G, EllipticCurve.scalarMultiplyNaf EllipticCurve.secp256k1 2I EllipticCurve.secp256k1.G)

        let x = hexInt "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721"
        let expectedU =
            EllipticCurve.Point(
                hexInt "60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6",
                hexInt "7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299")
        Assert.Equal(expectedU, EllipticCurve.scalarMultiplyNaf EllipticCurve.nistP256 x EllipticCurve.nistP256.G)

    [<Fact>]
    let ``Phase 110 - Width-w NAF representation and scalarMultiplyWnaf`` () =
        Assert.Equal<int list>([ 7 ], EllipticCurve.wnaf 4 7I)
        Assert.Equal<int list>([ -3; 0; 0; 0; 0; 1 ], EllipticCurve.wnaf 4 29I)
        Assert.Equal<int list>([ -1; 0; 0; 0; 0; 0; 0; 0; 1 ], EllipticCurve.wnaf 4 255I)
        Assert.Equal<int list>([ 0; 0; 0; -3; 0; 0; 0; 0; 0; 0; 1 ], EllipticCurve.wnaf 4 1000I)
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> EllipticCurve.wnaf 1 5I |> ignore) |> ignore
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> EllipticCurve.wnaf 4 -1I |> ignore) |> ignore

        for k in 0I .. 2000I do
            Assert.Equal<int list>(EllipticCurve.naf k, EllipticCurve.wnaf 2 k)

            let digits4 = EllipticCurve.wnaf 4 k
            let rebuilt =
                digits4
                |> List.mapi (fun i d -> BigInteger d * (BigInteger.One <<< i))
                |> List.fold (+) BigInteger.Zero
            Assert.Equal(k, rebuilt)

            for d in digits4 do
                if d <> 0 then
                    Assert.Equal(1, abs d % 2)
                    Assert.True(abs d < 8)

            let arr = List.toArray digits4
            for i in 0 .. arr.Length - 1 do
                let windowEnd = min (arr.Length - 1) (i + 3)
                let nonZeroCount =
                    [ i .. windowEnd ]
                    |> List.filter (fun idx -> arr.[idx] <> 0)
                    |> List.length
                Assert.True(nonZeroCount <= 1)

        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3 ] do
            for q in allPoints curve do
                for k in 0I .. curve.N - 1I do
                    Assert.Equal(
                        EllipticCurve.scalarMultiply curve k q,
                        EllipticCurve.scalarMultiplyWnaf curve 4 k q)

        let x = hexInt "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721"
        let expectedU =
            EllipticCurve.Point(
                hexInt "60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6",
                hexInt "7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299")
        Assert.Equal(expectedU, EllipticCurve.scalarMultiplyWnaf EllipticCurve.nistP256 4 x EllipticCurve.nistP256.G)
