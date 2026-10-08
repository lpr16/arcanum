namespace Arcanum.Tests

open System
open System.Globalization
open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Core.EllipticCurve

module ToyCurveTests =
    let enumeratedPoints : ECPoint list =
        Infinity :: [
            for x in 0I .. (toy.P - 1I) do
                for y in 0I .. (toy.P - 1I) do
                    let pt = Point(x, y)
                    if isOnCurve toy pt then
                        yield pt
        ]

    [<Fact>]
    let ``Enumerated set has 18 points and all satisfy curve equation`` () =
        Assert.Equal(18, enumeratedPoints.Length)
        for pt in enumeratedPoints do
            Assert.True(isOnCurve toy pt)

    [<Fact>]
    let ``Discriminant is nonzero modulo prime`` () =
        let disc = Modular.modPos (4I * Modular.modPow toy.A 3I toy.P + 27I * Modular.modPow toy.B 2I toy.P) toy.P
        Assert.NotEqual(0I, disc)
        Assert.Equal(10I, disc)

    [<Fact>]
    let ``Infinity is identity on both sides`` () =
        for q in enumeratedPoints do
            Assert.Equal(q, add toy Infinity q)
            Assert.Equal(q, add toy q Infinity)

    [<Fact>]
    let ``Point negation and inverse law`` () =
        for q in enumeratedPoints do
            Assert.Equal(Infinity, add toy q (negate toy q))
            match q with
            | Infinity -> Assert.Equal(Infinity, negate toy Infinity)
            | Point(x, y) ->
                let expectedY = Modular.modPos (-y) toy.P
                Assert.Equal(Point(x, expectedY), negate toy q)

    [<Fact>]
    let ``Chord addition calculation matches manual slope derivation`` () =
        // (0, 1) + (1, 6): slope = (6 - 1) * (1 - 0)^(-1) = 5 (mod 17)
        // x = 5^2 - 0 - 1 = 24 = 7 (mod 17)
        // y = 5 * (0 - 7) - 1 = -36 = 15 (mod 17)
        // Result is Point(7, 15)
        let p1 = Point(0I, 1I)
        let p2 = Point(1I, 6I)
        Assert.Equal(Point(7I, 15I), add toy p1 p2)

    [<Fact>]
    let ``Tangent doubling calculation matches manual slope derivation`` () =
        // double (1, 6): slope = (3 * 1^2 + 0) * (2 * 6)^(-1) = 3 * 12^(-1) = 3 * 10 = 30 = 13 (mod 17)
        // x = 13^2 - 2 * 1 = 167 = 14 (mod 17)
        // y = 13 * (1 - 14) - 6 = -175 = 12 (mod 17)
        // Result is Point(14, 12)
        let p = Point(1I, 6I)
        Assert.Equal(Point(14I, 12I), double toy p)

    [<Fact>]
    let ``Point (0, 1) has order three`` () =
        let p = Point(0I, 1I)
        let p2 = add toy p p
        let p3 = add toy p2 p
        Assert.NotEqual(Infinity, p)
        Assert.NotEqual(Infinity, p2)
        Assert.Equal(Infinity, p3)

    [<Fact>]
    let ``Standard curve parameters remain intact`` () =
        Assert.Equal(BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F", Globalization.NumberStyles.HexNumber), secp256k1.P)
        Assert.Equal(BigInteger.Parse("0FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFF", Globalization.NumberStyles.HexNumber), nistP256.P)
