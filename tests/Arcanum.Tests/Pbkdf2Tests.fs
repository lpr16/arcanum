namespace Arcanum.Tests

open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Hashing

module Pbkdf2Tests =
    [<Fact>]
    let ``PBKDF2-HMAC-SHA256 single iteration matches published answer`` () =
        // Published test vector for PBKDF2 with HMAC-SHA256 (e.g. RFC 6070 / RFC 7914)
        // Password: "password", Salt: "salt", c: 1, dkLen: 32
        // PRF is HMAC.hmacSha256 whose hash step delegates to the CLR SHA-256 call.
        let password = Encoding.ASCII.GetBytes "password"
        let salt = Encoding.ASCII.GetBytes "salt"
        let iterations = 1
        let outputLength = 32

        let key = PBKDF2.deriveKey password salt iterations outputLength
        let hex = Bytes.toHex key.Value
        Assert.Equal("120fb6cffcf8b32c43e7225256c4f837a86548c92ccc35480805987cb70be17b", hex)
