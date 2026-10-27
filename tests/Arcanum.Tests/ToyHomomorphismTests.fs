namespace Arcanum.Tests

open Xunit
open Arcanum.Core.EllipticCurve

module ToyHomomorphismTests =
    // EllipticCurve.toy is y^2 = x^3 + 1 over F_17 with 18 points; toy is not a secure group.
    let enumeratedPoints : ECPoint list = ToyCurveTests.enumeratedPoints

    [<Fact>]
    let ``Phase 34 - Negation respects add on every pair and is an involution`` () =
        Assert.Equal(18, enumeratedPoints.Length)
        for q in enumeratedPoints do
            Assert.Equal(q, negate toy (negate toy q))
            for r in enumeratedPoints do
                Assert.Equal(negate toy (add toy q r), add toy (negate toy q) (negate toy r))
