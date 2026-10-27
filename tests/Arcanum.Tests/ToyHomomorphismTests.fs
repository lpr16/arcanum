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

    [<Fact>]
    let ``Phase 35 - Scalar multiplication distributes over add for every residue and pair`` () =
        for k in 0I .. 17I do
            for q in enumeratedPoints do
                for r in enumeratedPoints do
                    Assert.Equal(add toy (scalarMultiply toy k q) (scalarMultiply toy k r), scalarMultiply toy k (add toy q r))

    [<Fact>]
    let ``Phase 35 - Scalar multiple of a sum of residues is the sum of scalar multiples`` () =
        // k + m ranges over 0 .. 34. When it exceeds 17, scalarMultiply reduces it modulo toy.N = 18,
        // so that reduction is part of this law.
        for k in 0I .. 17I do
            for m in 0I .. 17I do
                for q in enumeratedPoints do
                    Assert.Equal(add toy (scalarMultiply toy k q) (scalarMultiply toy m q), scalarMultiply toy (k + m) q)
