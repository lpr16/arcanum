namespace Arcanum.Tests

open System.Globalization
open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Asymmetric

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

    [<Fact>]
    let ``Phase 135 - RFC 5114 section A.4 key agreement on NIST P-192 (one vector, not a CAVP file)`` () =
        let curve = EllipticCurve.nistP192

        let dA = hexInt "323FA3169D8E9C6593F59476BC142000AB5BE0E249C43426"
        let x_qA = hexInt "CD46489ECFD6C105E7B3D32566E2B122E249ABAADD870612"
        let y_qA = hexInt "68887B4877DF51DD4DC3D6FD11F0A26F8FD3844317916E9A"
        let qA = EllipticCurve.Point(x_qA, y_qA)

        let dB = hexInt "631F95BB4A67632C9C476EEE9AB695AB240A0499307FCF62"
        let x_qB = hexInt "519A121680E0045466BA21DF2EEE47F5973B500577EF13D5"
        let y_qB = hexInt "FF613AB4D64CEE3A20875BDB10F953F6B30CA072C60AA57F"
        let qB = EllipticCurve.Point(x_qB, y_qB)

        let x_Z = hexInt "AD420182633F8526BFE954ACDA376F05E5FF4F837F54FEBE"
        let y_Z = hexInt "4371545ED772A59741D0EDA32C671112B7FDDD51461FCF32"
        let expectedZ = EllipticCurve.Point(x_Z, y_Z)

        // Public key derivation
        Assert.Equal(qA, EllipticCurve.scalarMultiply curve dA curve.G)
        Assert.Equal(qB, EllipticCurve.scalarMultiply curve dB curve.G)

        // Shared secret computation both ways
        Assert.Equal(Ok expectedZ, DiffieHellman.computeEcdhSharedSecretChecked curve dA qB)
        Assert.Equal(Ok expectedZ, DiffieHellman.computeEcdhSharedSecretChecked curve dB qA)

    [<Fact>]
    let ``Phase 135 - RFC 5114 section A.5 key agreement on NIST P-224 (one vector, not a CAVP file)`` () =
        let curve = EllipticCurve.nistP224

        let dA = hexInt "B558EB6C288DA707BBB4F8FBAE2AB9E9CB62E3BC5C7573E22E26D37F"
        let x_qA = hexInt "49DFEF309F81488C304CFF5AB3EE5A2154367DC7833150E0A51F3EEB"
        let y_qA = hexInt "4F2B5EE45762C4F654C1A0C67F54CF88B016B51BCE3D7C228D57ADB4"
        let qA = EllipticCurve.Point(x_qA, y_qA)

        let dB = hexInt "AC3B1ADD3D9770E6F6A708EE9F3B8E0AB3B480E9F27F85C88B5E6D18"
        let x_qB = hexInt "6B3AC96A8D0CDE6A5599BE8032EDF10C162D0A8AD219506DCD42A207"
        let y_qB = hexInt "D491BE99C213A7D1CA3706DEBFE305F361AFCBB33E2609C8B1618AD5"
        let qB = EllipticCurve.Point(x_qB, y_qB)

        let x_Z = hexInt "52272F50F46F4EDC9151569092F46DF2D96ECC3B6DC1714A4EA949FA"
        let y_Z = hexInt "5F30C6AA36DDC403C0ACB712BB88F1763C3046F6D919BD9C524322BF"
        let expectedZ = EllipticCurve.Point(x_Z, y_Z)

        // Public key derivation
        Assert.Equal(qA, EllipticCurve.scalarMultiply curve dA curve.G)
        Assert.Equal(qB, EllipticCurve.scalarMultiply curve dB curve.G)

        // Shared secret computation both ways
        Assert.Equal(Ok expectedZ, DiffieHellman.computeEcdhSharedSecretChecked curve dA qB)
        Assert.Equal(Ok expectedZ, DiffieHellman.computeEcdhSharedSecretChecked curve dB qA)


