namespace Arcanum.Tests

open Xunit
open Arcanum.Core
open Arcanum.Symmetric

module PaddingTests =
    [<Fact>]
    let ``PKCS#7 padding appends expected pad bytes and validates on unpad`` () =
        let data3 = [| 1uy; 2uy; 3uy |]
        let padded8 = Padding.padPkcs7 8 data3
        Assert.Equal(8, padded8.Length)
        Assert.Equal<byte>([| 1uy; 2uy; 3uy; 5uy; 5uy; 5uy; 5uy; 5uy |], padded8)

        // Full block of 16 bytes padded to block size 16 grows by 16 bytes of 0x10
        let full16 = Array.init 16 (fun i -> byte i)
        let padded32 = Padding.padPkcs7 16 full16
        Assert.Equal(32, padded32.Length)
        for i = 16 to 31 do
            Assert.Equal(0x10uy, padded32.[i])

        // unpadPkcs7 of a block whose last byte is 0x00 returns Error
        let invalidBlock = [| 1uy; 2uy; 3uy; 4uy; 5uy; 6uy; 7uy; 0x00uy |]
        match Padding.unpadPkcs7 8 invalidBlock with
        | Error _ -> ()
        | Ok _ -> Assert.Fail("Expected error when unpadding last byte 0x00")

        // unpadPkcs7 of the first padded buffer returns original bytes
        match Padding.unpadPkcs7 8 padded8 with
        | Ok unpadded -> Assert.Equal<byte>(data3, unpadded)
        | Error _ -> Assert.Fail("Expected successful unpadding")
