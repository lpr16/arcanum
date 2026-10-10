namespace Arcanum.Tests

open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Asymmetric

module DhTests =

    // Small toy Diffie-Hellman group: p = 23, g = 5.
    // This group is not RFC 3526 and is not a secure group; it is for hand-verifiable testing only.
    let toyDhGroup: DiffieHellman.DhGroup = { P = 23I; G = 5I }

    [<Fact>]
    let ``Phase 24 - Toy Diffie-Hellman public key exponentiations and shared secret`` () =
        let pubA = Modular.modPow 5I 6I 23I
        let pubB = Modular.modPow 5I 15I 23I
        Assert.Equal(8I, pubA)
        Assert.Equal(19I, pubB)

        let secretFromA = DiffieHellman.computeSharedSecret toyDhGroup 6I pubB
        let secretFromB = DiffieHellman.computeSharedSecret toyDhGroup 15I pubA
        Assert.Equal(2I, secretFromA)
        Assert.Equal(2I, secretFromB)

        let expectedDirect = Modular.modPow 5I (6I * 15I) 23I
        Assert.Equal(2I, expectedDirect)
