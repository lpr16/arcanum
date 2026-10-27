namespace Arcanum.Tests

open System
open Xunit
open Arcanum.Core
open Arcanum.Core.FiniteFields

module Gf128Tests =

    // Structural checks of GF128.multiply on 16-byte blocks. These are not a GHASH CAVP vector,
    // and this repository does not add AES-GCM.

    let private block (first: byte) : byte[] =
        Array.init 16 (fun i -> if i = 0 then first else 0uy)

    [<Fact>]
    let ``Phase 42 - Multiplying by the zero block gives the zero block on both sides`` () =
        let zero = Array.zeroCreate<byte> 16
        let samples = [
            block 0x80uy
            block 0x02uy
            Array.create 16 0xffuy
            Bytes.fromHex "000102030405060708090a0b0c0d0e0f"
        ]
        for x in samples do
            Assert.Equal<byte>(zero, GF128.multiply x zero)
            Assert.Equal<byte>(zero, GF128.multiply zero x)

    [<Fact>]
    let ``Phase 42 - Multiplication commutes on the 0x80 and 0x02 blocks`` () =
        let x = block 0x80uy
        let y = block 0x02uy
        Assert.Equal<byte>(GF128.multiply x y, GF128.multiply y x)

        // In the SP 800-38D bit order the leftmost bit is the coefficient of x^0, so the 0x80 block is 1.
        Assert.Equal<byte>(y, GF128.multiply x y)

    [<Fact>]
    let ``Phase 42 - A block that is not 16 bytes is rejected`` () =
        let good = block 0x80uy
        Assert.Throws<ArgumentException>(fun () -> GF128.multiply (Array.zeroCreate 15) good |> ignore) |> ignore
        Assert.Throws<ArgumentException>(fun () -> GF128.multiply good (Array.zeroCreate 17) |> ignore) |> ignore
