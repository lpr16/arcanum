namespace Arcanum.Tests

open System
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
