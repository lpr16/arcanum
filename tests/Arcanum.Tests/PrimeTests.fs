namespace Arcanum.Tests

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
