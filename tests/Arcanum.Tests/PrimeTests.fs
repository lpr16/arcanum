namespace Arcanum.Tests

open System
open System.Numerics
open Xunit
open Arcanum.Core

module PrimeTests =
    [<Fact>]
    let ``Trial division decides small primes and composites deterministically`` () =
        Assert.True(Primes.isProbablePrime 17I 1)
        Assert.True(Primes.isProbablePrime 97I 1)

        Assert.False(Primes.isProbablePrime 1I 1)
        Assert.False(Primes.isProbablePrime 4I 1)
        Assert.False(Primes.isProbablePrime 91I 1) // 91 = 7 * 13

    [<Fact>]
    let ``randomBigInteger is inclusive on both ends`` () =
        Assert.Equal(7I, Primes.randomBigInteger 7I 7I)
        Assert.Throws<ArgumentException>(fun () -> Primes.randomBigInteger 8I 7I |> ignore) |> ignore

        // 256 draws of randomBigInteger 0 1 are all 0 or 1, and both values occur.
        // The chance of a correct function failing this is 2^-255.
        let draws01 = [ for _ in 1 .. 256 -> Primes.randomBigInteger 0I 1I ]
        Assert.All(draws01, fun x -> Assert.True(x = 0I || x = 1I))
        Assert.Contains(0I, draws01)
        Assert.Contains(1I, draws01)

        // 1000 draws of randomBigInteger 1 18 all lie in 1 .. 18, and 18 occurs.
        // The old function could never return 18. The chance of a correct function
        // never drawing it is (17/18)^1000, below 10^-24.
        let draws1to18 = [ for _ in 1 .. 1000 -> Primes.randomBigInteger 1I 18I ]
        Assert.All(draws1to18, fun x -> Assert.True(x >= 1I && x <= 18I))
        Assert.Contains(18I, draws1to18)

    [<Fact>]
    let ``Phase 98 - Miller-Rabin with fixed bases`` () =
        // 561 = 3 * 11 * 17 is a Carmichael number: 2^560 = 1 (mod 561),
        // and 560 = 35 * 2^4 gives 2^35, 2^70, 2^140, 2^280 = 263, 166, 67, 1 (mod 561).
        Assert.Equal(561I, 3I * 11I * 17I)
        Assert.Equal(1I, Modular.modPow 2I 560I 561I)
        Assert.Equal<BigInteger list>(
            [ 263I; 166I; 67I; 1I ],
            [ for e in [ 35I; 70I; 140I; 280I ] -> Modular.modPow 2I e 561I ])
        Assert.False(Primes.millerRabinWithBases 561I [ 2I ])

        // 3215031751 = 151 * 751 * 28351 is a strong pseudoprime to bases 2, 3, 5, 7, but fails base 11.
        let n = 3215031751I
        Assert.Equal(n, 151I * 751I * 28351I)
        Assert.True(Primes.millerRabinWithBases n [ 2I; 3I; 5I; 7I ])
        Assert.False(Primes.millerRabinWithBases n [ 11I ])

        let firstTwelvePrimes = [ 2I; 3I; 5I; 7I; 11I; 13I; 17I; 19I; 23I; 29I; 31I; 37I ]
        for p in [ 2I; 3I; 17I; 19I; 97I ] do
            Assert.True(Primes.millerRabinWithBases p firstTwelvePrimes)
        for c in [ 1I; 4I; 91I; 561I; 1105I; 3215031751I ] do
            Assert.False(Primes.millerRabinWithBases c firstTwelvePrimes)

    [<Fact>]
    let ``Phase 99 - secp256k1 and NIST P-256 field primes and group orders are probable primes`` () =
        // Each value passes millerRabinWithBases as a probable prime (not a primality proof).
        // No group enumeration or discrete logarithm is performed on secp256k1 or nistP256.
        let firstTwelvePrimes = [ 2I; 3I; 5I; 7I; 11I; 13I; 17I; 19I; 23I; 29I; 31I; 37I ]
        Assert.True(Primes.millerRabinWithBases EllipticCurve.secp256k1.P firstTwelvePrimes)
        Assert.True(Primes.millerRabinWithBases EllipticCurve.secp256k1.N firstTwelvePrimes)
        Assert.True(Primes.millerRabinWithBases EllipticCurve.nistP256.P firstTwelvePrimes)
        Assert.True(Primes.millerRabinWithBases EllipticCurve.nistP256.N firstTwelvePrimes)

        // secp256k1.P + 2 = 2^256 - 2^32 - 975 = 0 (mod 3), so millerRabinWithBases returns false.
        Assert.Equal(0I, (EllipticCurve.secp256k1.P + 2I) % 3I)
        Assert.False(Primes.millerRabinWithBases (EllipticCurve.secp256k1.P + 2I) firstTwelvePrimes)
