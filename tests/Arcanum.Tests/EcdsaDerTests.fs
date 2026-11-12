namespace Arcanum.Tests

open System
open System.Globalization
open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Asymmetric

module EcdsaDerTests =

    let private hexInt (s: string) =
        BigInteger.Parse("0" + s, NumberStyles.HexNumber)

    [<Fact>]
    let ``Phase 141 - DER encoding of RFC 6979 section A.2.5 ECDSA signatures on NIST P-256`` () =
        let sigSample : EcdsaSignature = {
            R = hexInt "EFD48B2AACB6A8FD1140DD9CD45E81D69D2C877B56AAF991C34D0EA84EAF3716"
            S = hexInt "F7CB1C942D657C41D436C7A1B6E29F65F3E900DBB9AFF4064DC4AB2F843ACDA8"
        }
        let expectedSampleDer =
            Convert.FromHexString("3046022100efd48b2aacb6a8fd1140dd9cd45e81d69d2c877b56aaf991c34d0ea84eaf3716022100f7cb1c942d657c41d436c7a1b6e29f65f3e900dbb9aff4064dc4ab2f843acda8")

        let actualSampleDer = EcdsaDer.encode sigSample
        Assert.Equal(72, actualSampleDer.Length)
        Assert.Equal<byte[]>(expectedSampleDer, actualSampleDer)

        let sigTest : EcdsaSignature = {
            R = hexInt "F1ABB023518351CD71D881567B1EA663ED3EFCF6C5132B354F28D3B0B7D38367"
            S = hexInt "019F4113742A2B14BD25926B49C649155F267E60D3814B4C0CC84250E46F0083"
        }
        let expectedTestDer =
            Convert.FromHexString("3045022100f1abb023518351cd71d881567b1ea663ed3efcf6c5132b354f28d3b0b7d383670220019f4113742a2b14bd25926b49c649155f267e60d3814b4c0cc84250e46f0083")

        let actualTestDer = EcdsaDer.encode sigTest
        Assert.Equal(71, actualTestDer.Length)
        Assert.Equal<byte[]>(expectedTestDer, actualTestDer)
