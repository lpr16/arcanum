namespace Arcanum.Tests

open Xunit
open Arcanum.Core
open Arcanum.Core.FiniteFields

module Gf256Tests =
    [<Fact>]
    let ``GF(2^8) arithmetic satisfies FIPS 197 section 4.2 product and field axioms`` () =
        // FIPS 197 section 4.2 example: {57} * {83} = {c1} modulo polynomial m(x) = x^8 + x^4 + x^3 + x + 1 (0x11B)
        Assert.Equal(0xc1uy, GF256.multiply 0x57uy 0x83uy)

        // Addition is bitwise XOR: 0x57 ^ 0x83 = 0xd4
        Assert.Equal(0xd4uy, GF256.add 0x57uy 0x83uy)

        // Inverse of zero is zero by cryptographic convention
        Assert.Equal(0uy, GF256.inverse 0uy)

        // Non-zero element inverses
        Assert.Equal(0x01uy, GF256.multiply (GF256.inverse 0x02uy) 0x02uy)
        Assert.Equal(0x01uy, GF256.multiply (GF256.inverse 0x53uy) 0x53uy)

    [<Fact>]
    let ``AES S-box and inverse S-box values match FIPS 197 entries`` () =
        Assert.Equal(0x63uy, GF256.sboxSub 0x00uy)
        Assert.Equal(0x7cuy, GF256.sboxSub 0x01uy)

        let invSBox = GF256.generateAesInvSBox ()
        Assert.Equal(0x00uy, invSBox.[0x63])
        Assert.Equal(0x01uy, invSBox.[0x7c])
