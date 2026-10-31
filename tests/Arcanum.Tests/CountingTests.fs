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
