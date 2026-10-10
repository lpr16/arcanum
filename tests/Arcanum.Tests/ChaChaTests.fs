namespace Arcanum.Tests

open Xunit
open Arcanum.Core
open Arcanum.Symmetric

module ChaChaTests =
    [<Fact>]
    let ``ChaCha20 block function matches RFC 8439 section 2.3.2 test vector`` () =
        // RFC 8439 section 2.3.2: ChaCha20 Block Function Test Vector
        // Key: 00:01:02:...:1f (32 bytes)
        // Nonce: 00:00:00:09:00:00:00:4a:00:00:00:00 (12 bytes)
        // Counter: 1
        let key = Array.init 32 (fun i -> byte i)
        let nonce = Bytes.fromHex "000000090000004a00000000"
        let counter = 1u

        let block = ChaCha20.block key counter nonce
        Assert.Equal(64, block.Length)

        let expectedFirst16 = Bytes.fromHex "10f1e7e4d13b5915500fdd1fa32071c4"
        let actualFirst16 = Array.sub block 0 16
        Assert.Equal<byte>(expectedFirst16, actualFirst16)
