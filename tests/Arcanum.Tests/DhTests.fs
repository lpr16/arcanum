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

    [<Fact>]
    let ``Phase 46 - RFC 3526 group 5 parameter record shape`` () =
        // RFC 3526 section 2: the 1536-bit MODP group with generator 2. No private key is exponentiated here.
        let group = DiffieHellman.rfc3526Group5
        Assert.Equal(2I, group.G)
        Assert.False(group.P.IsEven)

        // BigInteger.ToString("X") prints a leading zero nibble when the top bit is set.
        let hex = group.P.ToString("X").TrimStart('0')
        Assert.StartsWith("FFFFFFFFFFFFFFFFC90FDAA2", hex)
        Assert.EndsWith("FFFFFFFFFFFFFFFF", hex)
        Assert.True(group.P > BigInteger.Pow(2I, 1535))
