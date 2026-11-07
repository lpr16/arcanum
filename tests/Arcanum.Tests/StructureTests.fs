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

    [<Fact>]
    let ``Phase 105 - Two-torsion points and toy2 isomorphism with Z/4 x Z/4`` () =
        let order2Points (curve: EllipticCurve.Curve) =
            allPoints curve |> List.filter (fun pt -> DiscreteLog.order curve pt = 2I)

        // On toy: x^3 + 1 = 0 (mod 17) has only x = 16, and the unique point of order 2 is (16, 0) = 9G.
        let rootsToy = [ for x in 0I .. 16I do if Modular.modPos (x * x * x + 1I) 17I = 0I then yield x ]
        Assert.Equal<BigInteger list>([ 16I ], rootsToy)
        Assert.Equal<EllipticCurve.ECPoint list>([ EllipticCurve.Point(16I, 0I) ], order2Points EllipticCurve.toy)
        Assert.Equal(EllipticCurve.Point(16I, 0I), EllipticCurve.scalarMultiply EllipticCurve.toy 9I EllipticCurve.toy.G)

        // On toy2: x^3 + x = 0 (mod 17) has roots 0, 4, 13, giving points of order 2: (0, 0), (4, 0), (13, 0).
        let rootsToy2 = [ for x in 0I .. 16I do if Modular.modPos (x * x * x + x) 17I = 0I then yield x ]
        Assert.Equal<BigInteger list>([ 0I; 4I; 13I ], rootsToy2)
        Assert.Equal<EllipticCurve.ECPoint list>(
            [ EllipticCurve.Point(0I, 0I); EllipticCurve.Point(4I, 0I); EllipticCurve.Point(13I, 0I) ],
            order2Points EllipticCurve.toy2)

        // On toy3: x^3 + 2x + 2 = 0 (mod 17) has no roots, and no point has order 2.
        let rootsToy3 = [ for x in 0I .. 16I do if Modular.modPos (x * x * x + 2I * x + 2I) 17I = 0I then yield x ]
        Assert.Empty(rootsToy3)
        Assert.Empty(order2Points EllipticCurve.toy3)

        // On toy2: G = (1, 6) and Q = (3, 8) generate Z/4 x Z/4.
        let curve = EllipticCurve.toy2
        let g = curve.G
        let q = EllipticCurve.Point(3I, 8I)
        let multiplesOf (pt: EllipticCurve.ECPoint) =
            [ for k in 0 .. 3 ->
                [ 1 .. k ] |> List.fold (fun acc _ -> EllipticCurve.add curve acc pt) EllipticCurve.Infinity ]

        let gMults = multiplesOf g
        let qMults = multiplesOf q
        Assert.Equal<EllipticCurve.ECPoint list>(
            [ EllipticCurve.Infinity; EllipticCurve.Point(1I, 6I); EllipticCurve.Point(0I, 0I); EllipticCurve.Point(1I, 11I) ],
            gMults)
        Assert.Equal<EllipticCurve.ECPoint list>(
            [ EllipticCurve.Infinity; EllipticCurve.Point(3I, 8I); EllipticCurve.Point(13I, 0I); EllipticCurve.Point(3I, 9I) ],
            qMults)

        let grid =
            [ for a in 0 .. 3 do
                for b in 0 .. 3 do
                    yield EllipticCurve.add curve gMults.[a] qMults.[b] ]
        Assert.Equal(16, (List.distinct grid).Length)
        for pt in allPoints curve do
            Assert.Contains(pt, grid)

        Assert.Equal(EllipticCurve.Point(4I, 0I), EllipticCurve.add curve gMults.[2] qMults.[2])
