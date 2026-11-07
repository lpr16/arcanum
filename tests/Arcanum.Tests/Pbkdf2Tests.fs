namespace Arcanum.Tests

open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Hashing

module Pbkdf2Tests =
    [<Fact>]
    let ``PBKDF2-HMAC-SHA256 single iteration matches published answer`` () =
        // This value is the PBKDF2-HMAC-SHA256 answer for password "password", salt "salt", c = 1, dkLen = 32; it is not taken from RFC 6070 or RFC 7914.
        // PRF is HMAC.hmacSha256 whose hash step delegates to the CLR SHA-256 call.
        let password = Encoding.ASCII.GetBytes "password"
        let salt = Encoding.ASCII.GetBytes "salt"
        let iterations = 1
        let outputLength = 32

        let key = PBKDF2.deriveKey password salt iterations outputLength
        let hex = Bytes.toHex key.Value
        Assert.Equal("120fb6cffcf8b32c43e7225256c4f837a86548c92ccc35480805987cb70be17b", hex)

    [<Fact>]
    let ``Phase 50 - PBKDF2-HMAC-SHA256 RFC 7914 section 11 single-iteration vector`` () =
        // RFC 7914 section 11: one cited PBKDF2-HMAC-SHA256 vector, not a CAVP file.
        // Password: "passwd", Salt: "salt", c = 1, dkLen = 64.
        // PRF is HMAC.hmacSha256 whose hash step delegates to the CLR SHA-256 call.
        let password = Encoding.ASCII.GetBytes "passwd"
        let salt = Encoding.ASCII.GetBytes "salt"
        let iterations = 1
        let outputLength = 64

        let key = PBKDF2.deriveKey password salt iterations outputLength
        let hex = Bytes.toHex key.Value
        let expected =
            "55ac046e56e3089fec1691c22544b605f94185216dde0465e68b9d57c20dacbc" +
            "49ca9cccf179b645991664b39d77ef317c71b845b1e30bd509112041d3a19783"
        Assert.Equal(expected, hex)

    [<Fact>]
    let ``Phase 97 - PBKDF2-HMAC-SHA256 RFC 7914 section 11 second vector (c = 80000)`` () =
        // RFC 7914 section 11: the second cited PBKDF2-HMAC-SHA256 vector, not a CAVP file.
        // Password: "Password", Salt: "NaCl", c = 80000, dkLen = 64.
        // PRF is HMAC.hmacSha256 whose hash step delegates to the CLR SHA-256 call.
        let password = Encoding.ASCII.GetBytes "Password"
        let salt = Encoding.ASCII.GetBytes "NaCl"
        let key = PBKDF2.deriveKey password salt 80000 64
        let expected =
            "4ddcd8f60b98be21830cee5ef22701f9641a4418d04c0414aeff08876b34ab56" +
            "a1d425a1225833549adb841b51c9b3176a272bdebba1d078478f62b397f33c8d"
        Assert.Equal(expected, Bytes.toHex key.Value)
