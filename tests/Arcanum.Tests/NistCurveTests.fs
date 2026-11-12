namespace Arcanum.Tests

open System.Globalization
open System.Numerics
open Xunit
open Arcanum.Core

module NistCurveTests =

    let private hexInt (s: string) =
        BigInteger.Parse("0" + s, NumberStyles.HexNumber)

    [<Fact>]
    let ``Phase 133 - NIST P-192 parameters and laws from RFC 5114 section 2.4`` () =
        let curve = EllipticCurve.nistP192

        // p = 2^192 - 2^64 - 1, A = p - 3
        let expectedP = (BigInteger.One <<< 192) - (BigInteger.One <<< 64) - BigInteger.One
        Assert.Equal(expectedP, curve.P)
        Assert.Equal(curve.P - 3I, curve.A)

        // Miller-Rabin probable-prime checks on p and N (not a proof)
        let testBases = [ 2I; 3I; 5I; 7I; 11I; 13I; 17I; 19I; 23I; 29I; 31I; 37I ]
        Assert.True(Primes.millerRabinWithBases curve.P testBases)
        Assert.True(Primes.millerRabinWithBases curve.N testBases)

        // G is on the curve
        Assert.True(EllipticCurve.isOnCurve curve curve.G)

        // Unreduced N * G = Infinity and (N - 1) * G = negate G
        Assert.Equal(EllipticCurve.Infinity, EllipticCurve.multiplyUnreduced curve curve.N curve.G)
        Assert.Equal(EllipticCurve.negate curve curve.G, EllipticCurve.multiplyUnreduced curve (curve.N - BigInteger.One) curve.G)

        // N equals the q that RFC 6979 section A.2.3 prints for P-192
        let expectedQ = hexInt "FFFFFFFFFFFFFFFFFFFFFFFF99DEF836146BC9B1B4D22831"
        Assert.Equal(expectedQ, curve.N)

    [<Fact>]
    let ``Phase 134 - NIST P-224 parameters and laws from RFC 5114 section 2.5`` () =
        let curve = EllipticCurve.nistP224

        // p = 2^224 - 2^96 + 1, A = p - 3
        let expectedP = (BigInteger.One <<< 224) - (BigInteger.One <<< 96) + BigInteger.One
        Assert.Equal(expectedP, curve.P)
        Assert.Equal(curve.P - 3I, curve.A)

        // p = 1 modulo 4, and p - 1 = 2^96 * q with q odd (forces Tonelli-Shanks general path in modSqrt)
        Assert.Equal(BigInteger.One, Modular.modPos curve.P 4I)
        let qOdd = (curve.P - BigInteger.One) >>> 96
        Assert.Equal(curve.P - BigInteger.One, qOdd <<< 96)
        Assert.Equal(BigInteger.One, qOdd &&& BigInteger.One)

        // Miller-Rabin probable-prime checks on p and N (not a proof)
        let testBases = [ 2I; 3I; 5I; 7I; 11I; 13I; 17I; 19I; 23I; 29I; 31I; 37I ]
        Assert.True(Primes.millerRabinWithBases curve.P testBases)
        Assert.True(Primes.millerRabinWithBases curve.N testBases)

        // G is on the curve
        Assert.True(EllipticCurve.isOnCurve curve curve.G)

        // Unreduced N * G = Infinity and (N - 1) * G = negate G
        Assert.Equal(EllipticCurve.Infinity, EllipticCurve.multiplyUnreduced curve curve.N curve.G)
        Assert.Equal(EllipticCurve.negate curve curve.G, EllipticCurve.multiplyUnreduced curve (curve.N - BigInteger.One) curve.G)

        // N equals the q that RFC 6979 section A.2.4 prints for P-224
        let expectedQ = hexInt "FFFFFFFFFFFFFFFFFFFFFFFFFFFF16A2E0B8F03E13DD29455C5C2A3D"
        Assert.Equal(expectedQ, curve.N)

