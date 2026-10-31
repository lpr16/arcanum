namespace Arcanum.Tests

open System.Numerics
open Xunit
open Arcanum.Core

module JacobianTests =

    // EllipticCurve.toy, toy2, and toy3 are small curves over F_17 and are not secure groups.

    let private allPoints (curve: EllipticCurve.Curve) =
        EllipticCurve.Infinity
        :: [ for x in 0I .. curve.P - 1I do
               for y in 0I .. curve.P - 1I do
                   let pt = EllipticCurve.Point(x, y)
                   if EllipticCurve.isOnCurve curve pt then
                       yield pt ]

    [<Fact>]
    let ``Phase 61 - Jacobian representation ofAffine, toAffine, and projective equivalence`` () =
        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3 ] do
            for q in allPoints curve do
                Assert.Equal(q, Jacobian.toAffine curve (Jacobian.ofAffine q))

        let p68 = EllipticCurve.Point(6I, 8I)
        for lambda in 1I .. 16I do
            let l2 = Modular.modPos (lambda * lambda) 17I
            let l3 = Modular.modPos (l2 * lambda) 17I
            let jPt : Jacobian.JacobianPoint = {
                X = Modular.modPos (6I * l2) 17I
                Y = Modular.modPos (8I * l3) 17I
                Z = lambda
            }
            Assert.Equal(p68, Jacobian.toAffine EllipticCurve.toy jPt)

        for x in 0I .. 16I do
            for y in 0I .. 16I do
                let zZeroPt : Jacobian.JacobianPoint = { X = x; Y = y; Z = 0I }
                Assert.Equal(EllipticCurve.Infinity, Jacobian.toAffine EllipticCurve.toy zZeroPt)

    let private liftWithLambda (curve: EllipticCurve.Curve) (lambda: BigInteger) (pt: EllipticCurve.ECPoint) : Jacobian.JacobianPoint =
        match pt with
        | EllipticCurve.Infinity -> { X = 1I; Y = 1I; Z = 0I }
        | EllipticCurve.Point(x, y) ->
            let p = curve.P
            let l2 = Modular.modPos (lambda * lambda) p
            let l3 = Modular.modPos (l2 * lambda) p
            {
                X = Modular.modPos (x * l2) p
                Y = Modular.modPos (y * l3) p
                Z = Modular.modPos lambda p
            }

    [<Fact>]
    let ``Phase 62 - Jacobian double and add agree with EllipticCurve group law on toy, toy2, and toy3`` () =
        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3 ] do
            let pts = allPoints curve
            for q in pts do
                Assert.Equal(
                    EllipticCurve.double curve q,
                    Jacobian.toAffine curve (Jacobian.double curve (Jacobian.ofAffine q)))
                let qLifted = liftWithLambda curve 3I q
                for r in pts do
                    let rLifted = liftWithLambda curve 5I r
                    Assert.Equal(
                        EllipticCurve.add curve q r,
                        Jacobian.toAffine curve (Jacobian.add curve qLifted rLifted))
