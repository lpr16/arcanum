namespace Arcanum.Tests

open System.Globalization
open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Core.EllipticCurve

module Secp256k1Tests =
    /// Double-and-add scalar multiplication without modular reduction of the scalar.
    /// Walks the scalar bits directly using EllipticCurve.add and EllipticCurve.double.
    let unreducedDoubleAndAdd (curve: Curve) (scalar: BigInteger) (pt: ECPoint) : ECPoint =
        if scalar <= BigInteger.Zero || pt = Infinity then
            Infinity
        else
            let mutable res = Infinity
            let mutable addend = pt
            let mutable s = scalar
            while s > BigInteger.Zero do
                if (s &&& BigInteger.One) = BigInteger.One then
                    res <- add curve res addend
                addend <- double curve addend
                s <- s >>> 1
            res

    [<Fact>]
    let ``secp256k1 generator G lies on the curve`` () =
        Assert.True(isOnCurve secp256k1 secp256k1.G)

    [<Fact>]
    let ``secp256k1 generator order check via unreduced double-and-add`` () =
        // Unreduced double-and-add of N * G reaches Infinity without scalar modular reduction
        let nG = unreducedDoubleAndAdd secp256k1 secp256k1.N secp256k1.G
        Assert.Equal(Infinity, nG)

        // Unreduced double-and-add of (N - 1) * G equals -G
        let nMinusOneG = unreducedDoubleAndAdd secp256k1 (secp256k1.N - BigInteger.One) secp256k1.G
        let negG = negate secp256k1 secp256k1.G
        Assert.Equal(negG, nMinusOneG)

    [<Fact>]
    let ``Phase 44 - Doubling the secp256k1 generator`` () =
        // These literals are the double of the SEC 2 secp256k1 generator G.
        // They are not the output of a previous run of scalarMultiply.
        let hex (s: string) = BigInteger.Parse("0" + s, NumberStyles.HexNumber)
        let expected =
            Point(
                hex "c6047f9441ed7d6d3045406e95c07cd85c778e4b8cef3ca7abac09b95c709ee5",
                hex "1ae168fea63dc339a3c58419466ceaeef7f632653266d0e1236431a950cfe52a")
        Assert.Equal(expected, double secp256k1 secp256k1.G)
        Assert.True(isOnCurve secp256k1 expected)

    [<Fact>]
    let ``nistP256 generator G lies on the curve`` () =
        Assert.True(isOnCurve nistP256 nistP256.G)

    [<Fact>]
    let ``nistP256 generator order check via unreduced double-and-add`` () =
        // Unreduced double-and-add of N * G reaches Infinity without scalar modular reduction
        let nG = unreducedDoubleAndAdd nistP256 nistP256.N nistP256.G
        Assert.Equal(Infinity, nG)

        // Unreduced double-and-add of (N - 1) * G equals -G
        let nMinusOneG = unreducedDoubleAndAdd nistP256 (nistP256.N - BigInteger.One) nistP256.G
        let negG = negate nistP256 nistP256.G
        Assert.Equal(negG, nMinusOneG)

    [<Fact>]
    let ``Phase 117 - The secp256k1 GLV endomorphism beta and lambda`` () =
        // The pairing matters: 2^((N - 1) / 3) mod N is the other cube root, and it pairs with beta^2,
        // not with beta. P-256 has a = -3, so it has no such map.
        let p = secp256k1.P
        let n = secp256k1.N
        let beta = Modular.modPow 2I ((p - 1I) / 3I) p
        let lambda = Modular.modPow 2I (2I * (n - 1I) / 3I) n

        Assert.NotEqual(1I, beta)
        Assert.Equal(1I, Modular.modPow beta 3I p)
        Assert.Equal(0I, Modular.modPos (beta * beta + beta + 1I) p)

        Assert.NotEqual(1I, lambda)
        Assert.Equal(1I, Modular.modPow lambda 3I n)
        Assert.Equal(0I, Modular.modPos (lambda * lambda + lambda + 1I) n)

        let hex (s: string) = BigInteger.Parse("0" + s, NumberStyles.HexNumber)
        let expectedBeta = hex "7ae96a2b657c07106e64479eac3434e99cf0497512f58995c1396c28719501ee"
        let expectedLambda = hex "5363ad4cc05c30e0a5261c028812645a122e22ea20816678df02967c1b23bd72"
        Assert.Equal(expectedBeta, beta)
        Assert.Equal(expectedLambda, lambda)

        match secp256k1.G with
        | Point(gx, gy) ->
            Assert.Equal(
                Point(Modular.modPos (beta * gx) p, gy),
                scalarMultiply secp256k1 lambda secp256k1.G)
            let betaSq = Modular.modPos (beta * beta) p
            let lambdaSq = Modular.modPos (lambda * lambda) n
            Assert.Equal(
                Point(Modular.modPos (betaSq * gx) p, gy),
                scalarMultiply secp256k1 lambdaSq secp256k1.G)
        | Infinity -> Assert.Fail("secp256k1.G is not Infinity")

        let g2x = hex "c6047f9441ed7d6d3045406e95c07cd85c778e4b8cef3ca7abac09b95c709ee5"
        let g2y = hex "1ae168fea63dc339a3c58419466ceaeef7f632653266d0e1236431a950cfe52a"
        let g2 = Point(g2x, g2y)
        Assert.Equal(
            Point(Modular.modPos (beta * g2x) p, g2y),
            scalarMultiply secp256k1 lambda g2)
