namespace Arcanum.Tests

open Xunit
open Arcanum.Core

module LadderTests =

    // EllipticCurve.toy, toy2, and toy3 are small curves over F_17 and are not secure groups.
    // scalarMultiplyLadder runs on the short Weierstrass curve and makes no constant-time claim.

    let private allPoints (curve: EllipticCurve.Curve) =
        EllipticCurve.Infinity
        :: [ for x in 0I .. curve.P - 1I do
               for y in 0I .. curve.P - 1I do
                   let pt = EllipticCurve.Point(x, y)
                   if EllipticCurve.isOnCurve curve pt then
                       yield pt ]

    [<Fact>]
    let ``Phase 64 - scalarMultiplyLadder agrees with scalarMultiply on toy, toy2, and toy3`` () =
        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3 ] do
            for q in allPoints curve do
                for k in 0I .. curve.N - 1I do
                    Assert.Equal(
                        EllipticCurve.scalarMultiply curve k q,
                        EllipticCurve.scalarMultiplyLadder curve k q)

    [<Fact>]
    let ``Phase 65 - scalarMultiplyLadder on secp256k1 and NIST P-256`` () =
        // Note: scalarMultiplyLadder reduces its scalar modulo N, so N * G = Infinity would follow from reduction;
        // unreduced order checks remain in phases 7 and 63.
        for curve in [ EllipticCurve.secp256k1; EllipticCurve.nistP256 ] do
            Assert.Equal(curve.G, EllipticCurve.scalarMultiplyLadder curve 1I curve.G)
            Assert.Equal(EllipticCurve.double curve curve.G, EllipticCurve.scalarMultiplyLadder curve 2I curve.G)
            Assert.Equal(
                EllipticCurve.negate curve curve.G,
                EllipticCurve.scalarMultiplyLadder curve (curve.N - 1I) curve.G)
