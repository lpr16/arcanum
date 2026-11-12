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

    [<Fact>]
    let ``Phase 74 - RFC 6979 section A.2.5 deterministic nonces k on NIST P-256`` () =
        let q = EllipticCurve.nistP256.N
        let x = hexInt "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721"

        let kSample = Rfc6979.generateK q x (Encoding.ASCII.GetBytes("sample"))
        let expectedKSample = hexInt "A6E3C57DD01ABE90086538398355DD4C3B17AA873382B0F24D6129493D8AAD60"
        Assert.Equal(expectedKSample, kSample)

        let kTest = Rfc6979.generateK q x (Encoding.ASCII.GetBytes("test"))
        let expectedKTest = hexInt "D16B6AE827F17175E040871A1C7EC3500192C4C92677336EC2537ACAEE0008E0"
        Assert.Equal(expectedKTest, kTest)

    [<Fact>]
    let ``Phase 75 - RFC 6979 section A.2.5 public key U on NIST P-256`` () =
        let curve = EllipticCurve.nistP256
        let x = hexInt "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721"
        let expectedU =
            EllipticCurve.Point(
                hexInt "60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6",
                hexInt "7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299")

        let u = EllipticCurve.scalarMultiply curve x curve.G
        Assert.Equal(expectedU, u)
        Assert.Equal(expectedU, EllipticCurve.scalarMultiplyLadder curve x curve.G)
        Assert.Equal(Ok (), EllipticCurve.validatePublicKey curve u)

    [<Fact>]
    let ``Phase 76 - RFC 6979 section A.2.5 deterministic ECDSA signatures on NIST P-256`` () =
        // Each message ("sample" and "test") is one published RFC 6979 section A.2.5 vector, not a CAVP file.
        let curve = EllipticCurve.nistP256
        let x = hexInt "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721"
        let u =
            EllipticCurve.Point(
                hexInt "60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6",
                hexInt "7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299")
        let keyPair : Ecdsa.EcdsaKeyPair = {
            PrivateKey = x
            PublicKey = u
            Curve = curve
        }

        let msgSample = Encoding.ASCII.GetBytes("sample")
        let msgTest = Encoding.ASCII.GetBytes("test")

        let expectedSampleSig : EcdsaSignature = {
            R = hexInt "EFD48B2AACB6A8FD1140DD9CD45E81D69D2C877B56AAF991C34D0EA84EAF3716"
            S = hexInt "F7CB1C942D657C41D436C7A1B6E29F65F3E900DBB9AFF4064DC4AB2F843ACDA8"
        }
        let expectedTestSig : EcdsaSignature = {
            R = hexInt "F1ABB023518351CD71D881567B1EA663ED3EFCF6C5132B354F28D3B0B7D38367"
            S = hexInt "019F4113742A2B14BD25926B49C649155F267E60D3814B4C0CC84250E46F0083"
        }

        match Rfc6979.sign keyPair msgSample, Rfc6979.sign keyPair msgTest with
        | Ok sigSample, Ok sigTest ->
            Assert.Equal(expectedSampleSig, sigSample)
            Assert.Equal(expectedTestSig, sigTest)
            Assert.True(Ecdsa.verify curve u msgSample sigSample)
            Assert.True(Ecdsa.verify curve u msgTest sigTest)
            Assert.False(Ecdsa.verify curve u msgTest sigSample)
        | _ ->
            Assert.Fail("Expected RFC 6979 signatures for sample and test to succeed")

    [<Fact>]
    let ``Phase 120 - Recovering the RFC 6979 section A.2.5 public key on NIST P-256`` () =
        let curve = EllipticCurve.nistP256
        let u =
            EllipticCurve.Point(
                hexInt "60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6",
                hexInt "7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299")
        let msgSample = Encoding.ASCII.GetBytes("sample")
        let msgTest = Encoding.ASCII.GetBytes("test")

        let sigSample : EcdsaSignature = {
            R = hexInt "EFD48B2AACB6A8FD1140DD9CD45E81D69D2C877B56AAF991C34D0EA84EAF3716"
            S = hexInt "F7CB1C942D657C41D436C7A1B6E29F65F3E900DBB9AFF4064DC4AB2F843ACDA8"
        }
        let sigTest : EcdsaSignature = {
            R = hexInt "F1ABB023518351CD71D881567B1EA663ED3EFCF6C5132B354F28D3B0B7D38367"
            S = hexInt "019F4113742A2B14BD25926B49C649155F267E60D3814B4C0CC84250E46F0083"
        }

        let uSampleOther =
            EllipticCurve.Point(
                hexInt "7f57fdb5eca3f088decb4937710c709185bd17ff938fc41cfd424249db4a2c3e",
                hexInt "58b68842e9cfeb1b1c648966cba4e790b3ce16d067184136f67b8bb3fbd5ba36")
        let uTestOther =
            EllipticCurve.Point(
                hexInt "811406049e3599d8d8747bbd9b6b3b7b0e9e46a65af69c23eba08c626dcd5693",
                hexInt "6a4b877c3a3a9d4b9ee9b7b74d20bf232134139e0770a4e0f20fd62a49258a3c")

        let recSample = Ecdsa.recoverPublicKeys curve msgSample sigSample
        let recTest = Ecdsa.recoverPublicKeys curve msgTest sigTest

        Assert.Equal<EllipticCurve.ECPoint list>([ u; uSampleOther ], recSample)
        Assert.Equal<EllipticCurve.ECPoint list>([ u; uTestOther ], recTest)

        for q in recSample do
            Assert.True(Ecdsa.verify curve q msgSample sigSample)
        for q in recTest do
            Assert.True(Ecdsa.verify curve q msgTest sigTest)

    [<Fact>]
    let ``Phase 121 - ECDSA malleability on NIST P-256 RFC 6979 vectors`` () =
        let curve = EllipticCurve.nistP256
        let x = hexInt "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721"
        let u =
            EllipticCurve.Point(
                hexInt "60FED4BA255A9D31C961EB74C6356D68C049B8923B61FA6CE669622E60F29FB6",
                hexInt "7903FE1008B8BC99A41AE9E95628BC64F2F1B20C2D7E9F5177A3C294D4462299")
        let keyPair : Ecdsa.EcdsaKeyPair = {
            PrivateKey = x
            PublicKey = u
            Curve = curve
        }
        let msgSample = Encoding.ASCII.GetBytes("sample")
        let msgTest = Encoding.ASCII.GetBytes("test")

        let sigSample : EcdsaSignature = {
            R = hexInt "EFD48B2AACB6A8FD1140DD9CD45E81D69D2C877B56AAF991C34D0EA84EAF3716"
            S = hexInt "F7CB1C942D657C41D436C7A1B6E29F65F3E900DBB9AFF4064DC4AB2F843ACDA8"
        }
        let sigTest : EcdsaSignature = {
            R = hexInt "F1ABB023518351CD71D881567B1EA663ED3EFCF6C5132B354F28D3B0B7D38367"
            S = hexInt "019F4113742A2B14BD25926B49C649155F267E60D3814B4C0CC84250E46F0083"
        }

        let expectedSampleNegS = hexInt "0834E36AD29A83BF2BC9385E491D6099C8FDF9D1ED67AA7EA5F51F93782857A9"
        let expectedTestNegS = hexInt "FE60BEEB8BD5D4EC42DA6D94B639B6EA5DC07C4CD3965338E6F1887217F424CE"

        Assert.Equal(expectedSampleNegS, curve.N - sigSample.S)
        Assert.Equal(expectedTestNegS, curve.N - sigTest.S)

        Assert.True(Ecdsa.verify curve u msgSample { sigSample with S = expectedSampleNegS })
        Assert.True(Ecdsa.verify curve u msgTest { sigTest with S = expectedTestNegS })

        Assert.False(Ecdsa.verify curve u msgSample { sigSample with S = sigSample.S + 1I })
        Assert.False(Ecdsa.verify curve u msgTest { sigTest with S = sigTest.S + 1I })

        Assert.True(sigSample.S > curve.N / 2I)
        Assert.True(sigTest.S < curve.N / 2I)

        Assert.Equal(Ok sigSample, Rfc6979.sign keyPair msgSample)
        Assert.Equal(Ok sigTest, Rfc6979.sign keyPair msgTest)

    [<Fact>]
    let ``Phase 132 - RFC 6979 candidates, retries on r = 0 and s = 0, and out-of-range key refusal`` () =
        let curve = EllipticCurve.toy3
        let q = curve.N
        let msgSample = Encoding.ASCII.GetBytes("sample")

        // Refusal of private key outside 1 .. q - 1
        Assert.Throws<ArgumentOutOfRangeException>(fun () ->
            Rfc6979.candidates q 0I msgSample |> Seq.head |> ignore) |> ignore
        Assert.Throws<ArgumentOutOfRangeException>(fun () ->
            Rfc6979.candidates q 19I msgSample |> Seq.head |> ignore) |> ignore

        let badKeyPair0 : Ecdsa.EcdsaKeyPair = { PrivateKey = 0I; PublicKey = curve.G; Curve = curve }
        match Rfc6979.sign badKeyPair0 msgSample with
        | Error (ParameterOutOfRange _) -> ()
        | res -> Assert.Fail($"Expected ParameterOutOfRange for private key 0, got {res}")

        let badKeyPair19 : Ecdsa.EcdsaKeyPair = { PrivateKey = 19I; PublicKey = curve.G; Curve = curve }
        match Rfc6979.sign badKeyPair19 msgSample with
        | Error (ParameterOutOfRange _) -> ()
        | res -> Assert.Fail($"Expected ParameterOutOfRange for private key 19, got {res}")

        // int2octets no longer reduces silently
        Assert.Throws<ArgumentOutOfRangeException>(fun () ->
            Rfc6979.int2octets q -1I |> ignore) |> ignore
        Assert.Throws<ArgumentOutOfRangeException>(fun () ->
            Rfc6979.int2octets q 19I |> ignore) |> ignore

        // Private key 15: first 4 candidates are 7, 7, 6, 2
        let cands15 = Rfc6979.candidates q 15I msgSample |> Seq.take 4 |> Seq.toList
        Assert.Equal<BigInteger list>([ 7I; 7I; 6I; 2I ], cands15)

        let pub15 = EllipticCurve.scalarMultiply curve 15I curve.G
        let keyPair15 : Ecdsa.EcdsaKeyPair = { PrivateKey = 15I; PublicKey = pub15; Curve = curve }
        let expectedSig15 : EcdsaSignature = { R = 16I; S = 15I }
        Assert.Equal(Ok expectedSig15, Rfc6979.sign keyPair15 msgSample)
        Assert.True(Ecdsa.verify curve pub15 msgSample expectedSig15)

        // Private key 7: first candidates are 6, 5
        let cands7 = Rfc6979.candidates q 7I msgSample |> Seq.take 2 |> Seq.toList
        Assert.Equal<BigInteger list>([ 6I; 5I ], cands7)

        let pub7 = EllipticCurve.scalarMultiply curve 7I curve.G
        let keyPair7 : Ecdsa.EcdsaKeyPair = { PrivateKey = 7I; PublicKey = pub7; Curve = curve }
        let expectedSig7 : EcdsaSignature = { R = 9I; S = 13I }
        Assert.Equal(Ok expectedSig7, Rfc6979.sign keyPair7 msgSample)
        Assert.True(Ecdsa.verify curve pub7 msgSample expectedSig7)

