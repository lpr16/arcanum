namespace Arcanum.Tests

open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Hashing

module HmacTests =

    // HMAC.hmacSha256 is an F# construction whose hash step calls SHA256.hash,
    // which is the CLR library call System.Security.Cryptography.SHA256.HashData.

    [<Fact>]
    let ``Phase 95 - HMAC-SHA256 matches RFC 4231 sections 4.4, 4.5, and 4.6 (test cases 3, 4, and 5)`` () =
        // Test case 3 (RFC 4231 section 4.4)
        let key3 = Key (Array.create 20 0xaauy)
        let data3 = Array.create 50 0xdduy
        let expected3 = Bytes.fromHex "773ea91e36800e46854db8ebd09181a72959098b3ef8c122d9635514ced565fe"
        Assert.Equal<byte>(expected3, (HMAC.hmacSha256 key3 data3).Value)

        // Test case 4 (RFC 4231 section 4.5)
        let key4 = Key (Bytes.fromHex "0102030405060708090a0b0c0d0e0f10111213141516171819")
        let data4 = Array.create 50 0xcduy
        let expected4 = Bytes.fromHex "82558a389a443c0ea4cc819899f2083a85f0faa3e578f8077a2e3ff46729665b"
        Assert.Equal<byte>(expected4, (HMAC.hmacSha256 key4 data4).Value)

        // Test case 5 (RFC 4231 section 4.6): RFC 4231 prints the first 128 bits (16 bytes).
        let key5 = Key (Array.create 20 0x0cuy)
        let data5 = Encoding.ASCII.GetBytes("Test With Truncation")
        let expected5First16 = Bytes.fromHex "a3b6167473100ee06e0c796c2955552b"
        let tag5 = HMAC.hmacSha256 key5 data5
        Assert.Equal<byte>(expected5First16, tag5.Value.[0 .. 15])
