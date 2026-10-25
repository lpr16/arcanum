namespace Arcanum.Tests

open Xunit
open Arcanum.Classical

module ClassicalLawsTests =

    [<Fact>]
    let ``Phase 28 - Caesar and Atbash hand checks`` () =
        Assert.Equal("N", Substitution.caesarEncrypt 13 "A")
        Assert.Equal("URYYB", Substitution.caesarEncrypt 13 "HELLO")
        Assert.Equal("Z", Substitution.atbash "A")
        Assert.Equal("A", Substitution.atbash "Z")
