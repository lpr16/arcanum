namespace Arcanum.Tests

open System
open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Asymmetric

module HashToCurveTests =

    [<Fact>]
    let ``Phase 143 - RFC 9380 Appendix K.1 expand_message_xmd 32-byte vectors`` () =
        let dst = Encoding.UTF8.GetBytes("QUUX-V01-CS02-with-expander-SHA256-128")
        let lenInBytes = 0x20

        let cases = [
            ("", "68a985b87eb6b46952128911f2a4412bbc302a9d759667f87f7a21d803f07235")
            ("abc", "d8ccab23b5985ccea865c6c97b6e5b8350e794e603b4b97902f53a8a0d605615")
            ("abcdef0123456789", "eff31487c770a893cfb36f912fbfcbff40d5661771ca4b2cb4eafe524333f5c1")
            ("q128_" + String('q', 128), "b23a1d2b4d97b2ef7785562a7e8bac7eed54ed6e97e29aa51bfe3f12ddad1ff9")
            ("a512_" + String('a', 512), "4623227bcc01293b8c130bf771da8c298dede7383243dc0993d2d94823958c4c")
        ]

        for (msgStr, expectedHex) in cases do
            let msgBytes = Encoding.UTF8.GetBytes(msgStr)
            let expectedBytes = Convert.FromHexString(expectedHex)
            let actual = HashToCurve.expandMessageXmd msgBytes dst lenInBytes
            Assert.Equal(Ok expectedBytes, actual)

        // Refusal of len_in_bytes = 8161 (ell = 256 > 255)
        match HashToCurve.expandMessageXmd [||] dst 8161 with
        | Error (ParameterOutOfRange _) -> ()
        | res -> Assert.Fail($"Expected ParameterOutOfRange for len_in_bytes = 8161, got {res}")
