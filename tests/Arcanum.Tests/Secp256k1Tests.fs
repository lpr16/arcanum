namespace Arcanum.Tests

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
