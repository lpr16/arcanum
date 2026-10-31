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
