namespace Arcanum.Tests

open System.Globalization
open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Asymmetric

module Rfc5903Tests =

    // RFC 5903 section 8.1: 256-bit Random ECP Group (NIST P-256).
    // This is one published test vector, not a CAVP campaign.

    let private hexInt (s: string) =
        BigInteger.Parse("0" + s, NumberStyles.HexNumber)

    [<Fact>]
    let ``Phase 77 - RFC 5903 section 8.1 public values on NIST P-256`` () =
        let curve = EllipticCurve.nistP256

        let i = hexInt "C88F01F510D9AC3F70A292DAA2316DE544E9AAB8AFE84049C62A9C57862D1433"
        let expectedGi =
            EllipticCurve.Point(
                hexInt "DAD0B65394221CF9B051E1FECA5787D098DFE637FC90B9EF945D0C3772581180",
                hexInt "5271A0461CDB8252D61F1C456FA3E59AB1F45B33ACCF5F58389E0577B8990BB3")

        let r = hexInt "C6EF9C5D78AE012A011164ACB397CE2088685D8F06BF9BE0B283AB46476BEE53"
        let expectedGr =
            EllipticCurve.Point(
                hexInt "D12DFB5289C8D4F81208B70270398C342296970A0BCCB74C736FC7554494BF63",
                hexInt "56FBF3CA366CC23E8157854C13C58D6AAC23F046ADA30F8353E74F33039872AB")

        Assert.Equal(expectedGi, EllipticCurve.scalarMultiply curve i curve.G)
        Assert.Equal(expectedGr, EllipticCurve.scalarMultiply curve r curve.G)

    [<Fact>]
    let ``Phase 78 - RFC 5903 section 8.1 shared value on NIST P-256`` () =
        let curve = EllipticCurve.nistP256

        let i = hexInt "C88F01F510D9AC3F70A292DAA2316DE544E9AAB8AFE84049C62A9C57862D1433"
        let gi =
            EllipticCurve.Point(
                hexInt "DAD0B65394221CF9B051E1FECA5787D098DFE637FC90B9EF945D0C3772581180",
                hexInt "5271A0461CDB8252D61F1C456FA3E59AB1F45B33ACCF5F58389E0577B8990BB3")

        let r = hexInt "C6EF9C5D78AE012A011164ACB397CE2088685D8F06BF9BE0B283AB46476BEE53"
        let gr =
            EllipticCurve.Point(
                hexInt "D12DFB5289C8D4F81208B70270398C342296970A0BCCB74C736FC7554494BF63",
                hexInt "56FBF3CA366CC23E8157854C13C58D6AAC23F046ADA30F8353E74F33039872AB")

        let expectedGirx = hexInt "D6840F6B42F6EDAFD13116E0E12565202FEF8E9ECE7DCE03812464D04B9442DE"
        let expectedGiry = hexInt "522BDE0AF0D8585B8DEF9C183B5AE38F50235206A8674ECB5D98EDB20EB153A2"
        let expectedGir = EllipticCurve.Point(expectedGirx, expectedGiry)

        let sharedI = DiffieHellman.computeEcdhSharedSecret curve i gr
        let sharedR = DiffieHellman.computeEcdhSharedSecret curve r gi

        Assert.Equal(expectedGir, sharedI)
        Assert.Equal(expectedGir, sharedR)
        Assert.Equal(Ok expectedGir, DiffieHellman.computeEcdhSharedSecretChecked curve i gr)
        Assert.Equal(Ok expectedGir, DiffieHellman.computeEcdhSharedSecretChecked curve r gi)

        // The RFC 5903 section 8.1 shared secret value is girx.
        match sharedI with
        | EllipticCurve.Point(girx, _) -> Assert.Equal(expectedGirx, girx)
        | EllipticCurve.Infinity -> Assert.Fail("Expected shared point to be finite")
