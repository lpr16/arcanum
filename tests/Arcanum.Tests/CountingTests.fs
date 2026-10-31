namespace Arcanum.Tests

open System.Numerics
open Xunit
open Arcanum.Core

module CountingTests =

    // EllipticCurve.toy, toy2, and toy3 are small curves over F_17 and are not secure groups.
    // EllipticCurve.countPoints loops over every x in 0 .. p - 1 and is for the toy curves only.

    let private allPoints (curve: EllipticCurve.Curve) =
        EllipticCurve.Infinity
        :: [ for x in 0I .. curve.P - 1I do
               for y in 0I .. curve.P - 1I do
                   let pt = EllipticCurve.Point(x, y)
                   if EllipticCurve.isOnCurve curve pt then
                       yield pt ]

    [<Fact>]
    let ``Phase 70 - Counting points by Legendre symbols on toy, toy2, and toy3`` () =
        let countToy = EllipticCurve.countPoints 17I 0I 1I
        let countToy2 = EllipticCurve.countPoints 17I 1I 0I
        let countToy3 = EllipticCurve.countPoints 17I 2I 2I

        Assert.Equal(18I, countToy)
        Assert.Equal(16I, countToy2)
        Assert.Equal(19I, countToy3)

        Assert.Equal(BigInteger (allPoints EllipticCurve.toy).Length, countToy)
        Assert.Equal(BigInteger (allPoints EllipticCurve.toy2).Length, countToy2)
        Assert.Equal(BigInteger (allPoints EllipticCurve.toy3).Length, countToy3)

    [<Fact>]
    let ``Phase 71 - Frobenius traces and Hasse bound on toy, toy2, and toy3`` () =
        // Trace of Frobenius t = p + 1 - #E; Hasse's theorem says t^2 <= 4p.
        // Trace 0 makes toy supersingular over F_17 (no pairing is added here).
        let p = 17I
        let tToy = p + 1I - EllipticCurve.countPoints p 0I 1I
        let tToy2 = p + 1I - EllipticCurve.countPoints p 1I 0I
        let tToy3 = p + 1I - EllipticCurve.countPoints p 2I 2I

        Assert.Equal(0I, tToy)
        Assert.Equal(2I, tToy2)
        Assert.Equal(-1I, tToy3)

        Assert.Equal(0I, tToy * tToy)
        Assert.Equal(4I, tToy2 * tToy2)
        Assert.Equal(1I, tToy3 * tToy3)

        let fourP = 4I * p
        Assert.Equal(68I, fourP)
        Assert.True(tToy * tToy <= fourP)
        Assert.True(tToy2 * tToy2 <= fourP)
        Assert.True(tToy3 * tToy3 <= fourP)
