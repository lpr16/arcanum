namespace Arcanum.Tests

open System.Numerics
open Xunit
open Arcanum.Core

module StructureTests =

    // EllipticCurve.toy, toy2, and toy3 are small curves over F_17 and are not secure groups.

    let private allPoints (curve: EllipticCurve.Curve) =
        EllipticCurve.Infinity
        :: [ for x in 0I .. curve.P - 1I do
               for y in 0I .. curve.P - 1I do
                   let pt = EllipticCurve.Point(x, y)
                   if EllipticCurve.isOnCurve curve pt then
                       yield pt ]

    let private orderHistogram (curve: EllipticCurve.Curve) =
        allPoints curve
        |> List.countBy (DiscreteLog.order curve)
        |> List.sortBy fst

    [<Fact>]
    let ``Phase 104 - Point order histograms on toy, toy2, and toy3`` () =
        let expectedToy = [ (1I, 1); (2I, 1); (3I, 2); (6I, 2); (9I, 6); (18I, 6) ]
        let expectedToy2 = [ (1I, 1); (2I, 3); (4I, 12) ]
        let expectedToy3 = [ (1I, 1); (19I, 18) ]

        Assert.Equal<(BigInteger * int) list>(expectedToy, orderHistogram EllipticCurve.toy)
        Assert.Equal<(BigInteger * int) list>(expectedToy2, orderHistogram EllipticCurve.toy2)
        Assert.Equal<(BigInteger * int) list>(expectedToy3, orderHistogram EllipticCurve.toy3)

        Assert.Equal(18I, DiscreteLog.order EllipticCurve.toy EllipticCurve.toy.G)
        Assert.Equal(4I, DiscreteLog.order EllipticCurve.toy2 EllipticCurve.toy2.G)
        Assert.Equal(19I, DiscreteLog.order EllipticCurve.toy3 EllipticCurve.toy3.G)
