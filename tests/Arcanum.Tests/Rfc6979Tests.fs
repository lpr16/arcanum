namespace Arcanum.Tests

open System
open System.Globalization
open System.Numerics
open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Hashing
open Arcanum.Asymmetric

module Rfc6979Tests =

    // SHA256.hash calls the CLR library method System.Security.Cryptography.SHA256.HashData.
    // HMAC.hmacSha256 is the F# HMAC construction built on SHA256.hash.

    let private hexInt (s: string) =
        BigInteger.Parse("0" + s, NumberStyles.HexNumber)

    [<Fact>]
    let ``Phase 73 - RFC 6979 section 2.3 bits2int, int2octets, and bits2octets`` () =
        let q = EllipticCurve.nistP256.N
        Assert.Equal(256L, q.GetBitLength())

        let h1 = (SHA256.hash (Encoding.ASCII.GetBytes("sample"))).Value
        let expectedH1Hex = "AF2BDBE1AA9B6EC1E2ADE1D694F41FC71A831D0268E9891562113D8A62ADD1BF"
        let expectedH1Bytes = Convert.FromHexString(expectedH1Hex)
        let expectedH1Int = hexInt expectedH1Hex

        // Compares the CLR SHA256.HashData call against the published RFC 6979 section A.1.2 digest.
        Assert.Equal<byte[]>(expectedH1Bytes, h1)
        Assert.Equal(expectedH1Int, Rfc6979.bits2int q h1)

        let xHex = "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721"
        let xInt = hexInt xHex
        let expectedXBytes = Convert.FromHexString(xHex)
        Assert.Equal<byte[]>(expectedXBytes, Rfc6979.int2octets q xInt)

        Assert.True(expectedH1Int < q)
        Assert.Equal<byte[]>(h1, Rfc6979.bits2octets q h1)

        let hEc = (SHA256.hash (Encoding.ASCII.GetBytes("ec"))).Value
        Assert.Equal(3I, Rfc6979.bits2int 19I hEc)
