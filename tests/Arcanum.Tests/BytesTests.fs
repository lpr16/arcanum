namespace Arcanum.Tests

open Xunit
open Arcanum.Core

module BytesTests =

    [<Fact>]
    let ``Phase 85 - Bytes.constantTimeEquals behavior`` () =
        Assert.True(Bytes.constantTimeEquals [| 1uy; 2uy; 3uy |] [| 1uy; 2uy; 3uy |])
        Assert.False(Bytes.constantTimeEquals [| 1uy; 2uy; 3uy |] [| 1uy; 2uy; 4uy |])
        Assert.False(Bytes.constantTimeEquals [| 1uy; 2uy |] [| 1uy; 2uy; 3uy |])
        Assert.True(Bytes.constantTimeEquals [||] [||])
