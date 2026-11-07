namespace Arcanum.Tests

open System.Globalization
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

    let private unreducedJacobianDoubleAndAdd (curve: EllipticCurve.Curve) (scalar: BigInteger) (pt: EllipticCurve.ECPoint) : EllipticCurve.ECPoint =
        if scalar <= BigInteger.Zero || pt = EllipticCurve.Infinity then
            EllipticCurve.Infinity
        else
            let mutable res = Jacobian.ofAffine EllipticCurve.Infinity
            let mutable addend = Jacobian.ofAffine pt
            let mutable s = scalar
            while s > BigInteger.Zero do
                if (s &&& BigInteger.One) = BigInteger.One then
                    res <- Jacobian.add curve res addend
                addend <- Jacobian.double curve addend
                s <- s >>> 1
            Jacobian.toAffine curve res

    [<Fact>]
    let ``Phase 63 - Jacobian double and unreduced double-and-add on secp256k1 and NIST P-256`` () =
        let hex (s: string) = BigInteger.Parse("0" + s, NumberStyles.HexNumber)
        let expectedDoubleG =
            EllipticCurve.Point(
                hex "c6047f9441ed7d6d3045406e95c07cd85c778e4b8cef3ca7abac09b95c709ee5",
                hex "1ae168fea63dc339a3c58419466ceaeef7f632653266d0e1236431a950cfe52a")
        Assert.Equal(
            expectedDoubleG,
            Jacobian.toAffine EllipticCurve.secp256k1 (Jacobian.double EllipticCurve.secp256k1 (Jacobian.ofAffine EllipticCurve.secp256k1.G)))

        for curve in [ EllipticCurve.secp256k1; EllipticCurve.nistP256 ] do
            Assert.Equal(EllipticCurve.Infinity, unreducedJacobianDoubleAndAdd curve curve.N curve.G)
            Assert.Equal(
                EllipticCurve.negate curve curve.G,
                unreducedJacobianDoubleAndAdd curve (curve.N - 1I) curve.G)

    [<Fact>]
    let ``Phase 113 - addMixed agrees with EllipticCurve.add on toy, toy2, and toy3`` () =
        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3 ] do
            let pts = allPoints curve
            for q in pts do
                let qLifted = liftWithLambda curve 3I q
                for r in pts do
                    Assert.Equal(
                        EllipticCurve.add curve q r,
                        Jacobian.toAffine curve (Jacobian.addMixed curve qLifted r))

    [<Fact>]
    let ``Phase 114 - Jacobian.scalarMultiply on toy curves, NIST P-256 RFC 5903, and secp256k1`` () =
        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3 ] do
            for q in allPoints curve do
                for k in 0I .. curve.N - 1I do
                    Assert.Equal(
                        EllipticCurve.scalarMultiply curve k q,
                        Jacobian.scalarMultiply curve k q)

        let hex (s: string) = BigInteger.Parse("0" + s, NumberStyles.HexNumber)
        let iScalar = hex "C88F01F510D9AC3F70A292DAA2316DE544E9AAB8AFE84049C62A9C57862D1433"
        let expectedI =
            EllipticCurve.Point(
                hex "DAD0B65394221CF9B051E1FECA5787D098DFE637FC90B9EF945D0C3772581180",
                hex "5271A0461CDB8252D61F1C456FA3E59AB1F45B33ACCF5F58389E0577B8990BB3")
        Assert.Equal(expectedI, Jacobian.scalarMultiply EllipticCurve.nistP256 iScalar EllipticCurve.nistP256.G)

        let rScalar = hex "C6EF9C5D78AE012A011164ACB397CE2088685D8F06BF9BE0B283AB46476BEE53"
        let expectedR =
            EllipticCurve.Point(
                hex "D12DFB5289C8D4F81208B70270398C342296970A0BCCB74C736FC7554494BF63",
                hex "56FBF3CA366CC23E8157854C13C58D6AAC23F046ADA30F8353E74F33039872AB")
        Assert.Equal(expectedR, Jacobian.scalarMultiply EllipticCurve.nistP256 rScalar EllipticCurve.nistP256.G)

        let expectedDoubleG =
            EllipticCurve.Point(
                hex "c6047f9441ed7d6d3045406e95c07cd85c778e4b8cef3ca7abac09b95c709ee5",
                hex "1ae168fea63dc339a3c58419466ceaeef7f632653266d0e1236431a950cfe52a")
        Assert.Equal(expectedDoubleG, Jacobian.scalarMultiply EllipticCurve.secp256k1 2I EllipticCurve.secp256k1.G)

    [<Fact>]
    let ``Phase 115 - doubleAMinus3 refuses non-a=-3 curves and agrees with EllipticCurve.double on NIST P-256`` () =
        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3; EllipticCurve.secp256k1 ] do
            match Jacobian.doubleAMinus3 curve (Jacobian.ofAffine curve.G) with
            | Error (ParameterOutOfRange _) -> ()
            | other -> Assert.Fail(sprintf "Expected ParameterOutOfRange, got %A" other)

        let hex (s: string) = BigInteger.Parse("0" + s, NumberStyles.HexNumber)
        let uPt =
            EllipticCurve.Point(
                hex "60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6",
                hex "7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299")
        let iPt =
            EllipticCurve.Point(
                hex "DAD0B65394221CF9B051E1FECA5787D098DFE637FC90B9EF945D0C3772581180",
                hex "5271A0461CDB8252D61F1C456FA3E59AB1F45B33ACCF5F58389E0577B8990BB3")
        let rPt =
            EllipticCurve.Point(
                hex "D12DFB5289C8D4F81208B70270398C342296970A0BCCB74C736FC7554494BF63",
                hex "56FBF3CA366CC23E8157854C13C58D6AAC23F046ADA30F8353E74F33039872AB")

        for pt in [ EllipticCurve.nistP256.G; uPt; iPt; rPt ] do
            let expected = EllipticCurve.double EllipticCurve.nistP256 pt
            for lambda in [ 1I; 2I ] do
                let lifted = liftWithLambda EllipticCurve.nistP256 lambda pt
                match Jacobian.doubleAMinus3 EllipticCurve.nistP256 lifted with
                | Ok doubled ->
                    Assert.Equal(expected, Jacobian.toAffine EllipticCurve.nistP256 doubled)
                | Error e -> Assert.Fail(sprintf "Unexpected error: %A" e)

    [<Fact>]
    let ``Phase 116 - batchToAffine on lifted toy curve points, mixed Infinity lists, and empty list`` () =
        for curve in [ EllipticCurve.toy; EllipticCurve.toy2; EllipticCurve.toy3 ] do
            let pts = allPoints curve
            let mutable finiteIdx = 0
            let liftedPts =
                [ for pt in pts do
                    match pt with
                    | EllipticCurve.Infinity ->
                        yield Jacobian.ofAffine EllipticCurve.Infinity
                    | EllipticCurve.Point _ ->
                        let lambda = 1I + BigInteger(finiteIdx % 16)
                        finiteIdx <- finiteIdx + 1
                        yield liftWithLambda curve lambda pt ]
            let batch = Jacobian.batchToAffine curve liftedPts
            Assert.Equal<EllipticCurve.ECPoint list>(List.map (Jacobian.toAffine curve) liftedPts, batch)
            Assert.Equal<EllipticCurve.ECPoint list>(pts, batch)
            Assert.Equal<EllipticCurve.ECPoint list>([], Jacobian.batchToAffine curve [])

        let mixed =
            [ Jacobian.ofAffine EllipticCurve.Infinity
              liftWithLambda EllipticCurve.toy3 3I EllipticCurve.toy3.G
              Jacobian.ofAffine EllipticCurve.Infinity
              liftWithLambda EllipticCurve.toy3 7I (EllipticCurve.double EllipticCurve.toy3 EllipticCurve.toy3.G)
              Jacobian.ofAffine EllipticCurve.Infinity ]
        let expectedMixed =
            [ EllipticCurve.Infinity
              EllipticCurve.toy3.G
              EllipticCurve.Infinity
              EllipticCurve.double EllipticCurve.toy3 EllipticCurve.toy3.G
              EllipticCurve.Infinity ]
        Assert.Equal<EllipticCurve.ECPoint list>(expectedMixed, Jacobian.batchToAffine EllipticCurve.toy3 mixed)
