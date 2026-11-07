namespace Arcanum.Tests

open System.Globalization
open System.Numerics
open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Asymmetric

module ToyEcdsaTests =

    // EllipticCurve.toy3 is a 19-point curve over F_17; toy3 is not a secure group.
    // Ecdsa.messageRepresentative follows SEC 1 section 4.1.3 and uses SHA256.hash,
    // which is the CLR library call System.Security.Cryptography.SHA256.HashData.
    // These tests pass a caller-supplied nonce to signWithNonce; this is not RFC 6979.

    let private repeatedAdd (curve: EllipticCurve.Curve) (count: int) (pt: EllipticCurve.ECPoint) =
        [ 1 .. count ]
        |> List.fold (fun acc _ -> EllipticCurve.add curve acc pt) EllipticCurve.Infinity

    [<Fact>]
    let ``Phase 55 - SEC 1 messageRepresentative on NIST P-256 and toy3`` () =
        let sampleBytes = Encoding.ASCII.GetBytes("sample")
        let expectedH1 =
            BigInteger.Parse(
                "0AF2BDBE1AA9B6EC1E2ADE1D694F41FC71A831D0268E9891562113D8A62ADD1BF",
                NumberStyles.HexNumber)
        Assert.Equal(expectedH1, Ecdsa.messageRepresentative EllipticCurve.nistP256 sampleBytes)

        Assert.Equal(5L, (19I).GetBitLength())
        let msgEc = Encoding.ASCII.GetBytes("ec")
        let msgEx = Encoding.ASCII.GetBytes("ex")
        Assert.Equal(3I, Ecdsa.messageRepresentative EllipticCurve.toy3 msgEc)
        Assert.Equal(10I, Ecdsa.messageRepresentative EllipticCurve.toy3 msgEx)

    [<Fact>]
    let ``Phase 55 - ECDSA signWithNonce and verify on toy3`` () =
        let curve = EllipticCurve.toy3
        let d = 5I
        let k = 4I
        let msgEc = Encoding.ASCII.GetBytes("ec")
        let msgEx = Encoding.ASCII.GetBytes("ex")

        let pubPoint = repeatedAdd curve 5 curve.G
        Assert.Equal(EllipticCurve.Point(9I, 16I), pubPoint)

        let noncePoint = repeatedAdd curve 4 curve.G
        Assert.Equal(EllipticCurve.Point(3I, 1I), noncePoint)

        let keyPair: Ecdsa.EcdsaKeyPair = {
            PrivateKey = d
            PublicKey = pubPoint
            Curve = curve
        }

        // 4^(-1) = 5 (mod 19).
        // For "ec": e1 = 3, r = 3, s1 = 5 * (3 + 3 * 5) = 90 = 14 (mod 19).
        // For "ex": e2 = 10, r = 3, s2 = 5 * (10 + 3 * 5) = 125 = 11 (mod 19).
        match Ecdsa.signWithNonce keyPair msgEc k, Ecdsa.signWithNonce keyPair msgEx k with
        | Ok sigEc, Ok sigEx ->
            Assert.Equal({ R = 3I; S = 14I }, sigEc)
            Assert.Equal({ R = 3I; S = 11I }, sigEx)
            Assert.True(Ecdsa.verify curve pubPoint msgEc sigEc)
            Assert.True(Ecdsa.verify curve pubPoint msgEx sigEx)
            Assert.False(Ecdsa.verify curve pubPoint msgEx sigEc)
        | _ ->
            Assert.Fail("Expected both signatures on toy3 to succeed")

    // The toy3 curve group has 19 points and is not a place to keep a key.
    [<Fact>]
    let ``Phase 55 - Repeated nonce on toy3 exposes nonce and private key in closed form`` () =
        let curve = EllipticCurve.toy3
        let d = 5I
        let k = 4I
        let msgEc = Encoding.ASCII.GetBytes("ec")
        let msgEx = Encoding.ASCII.GetBytes("ex")

        let pubPoint = repeatedAdd curve 5 curve.G
        let keyPair: Ecdsa.EcdsaKeyPair = {
            PrivateKey = d
            PublicKey = pubPoint
            Curve = curve
        }

        match Ecdsa.signWithNonce keyPair msgEc k, Ecdsa.signWithNonce keyPair msgEx k with
        | Ok sig1, Ok sig2 ->
            Assert.Equal(3I, sig1.R)
            Assert.Equal(sig1.R, sig2.R)
            let r = sig1.R
            let e1 = Ecdsa.messageRepresentative curve msgEc
            let e2 = Ecdsa.messageRepresentative curve msgEx

            match Modular.modInverse (Modular.modPos (sig1.S - sig2.S) curve.N) curve.N,
                  Modular.modInverse r curve.N with
            | Some invDeltaS, Some invR ->
                let recoveredK = Modular.modPos ((e1 - e2) * invDeltaS) curve.N
                let recoveredD = Modular.modPos ((sig1.S * recoveredK - e1) * invR) curve.N
                Assert.Equal(4I, recoveredK)
                Assert.Equal(5I, recoveredD)
            | _ ->
                Assert.Fail("Expected s1 - s2 and r to be invertible modulo 19")
        | _ ->
            Assert.Fail("Expected both signatures on toy3 to succeed")

    [<Fact>]
    let ``Phase 56 - ECDSA verify and signWithNonce refusals`` () =
        let curve = EllipticCurve.toy3
        let msgEc = Encoding.ASCII.GetBytes("ec")

        // On toy3 with public key Infinity, message "ec" (representative 3), and signature (R, S) = (5, 3):
        // the old verify would accept it, because u1 = 3 * 3^(-1) = 1 (mod 19), u2 * Infinity = Infinity,
        // u1 * G = (5, 1), and x = 5 = R. The new verify refuses Infinity as a public key.
        Assert.False(Ecdsa.verify curve EllipticCurve.Infinity msgEc { R = 5I; S = 3I })

        let d = 5I
        let pubPoint = repeatedAdd curve 5 curve.G
        let keyPair: Ecdsa.EcdsaKeyPair = {
            PrivateKey = d
            PublicKey = pubPoint
            Curve = curve
        }

        // On toy3 with d = 5: k = 7 gives 7G = (0, 6) and r = 0, so signWithNonce refuses it.
        Assert.Equal(EllipticCurve.Point(0I, 6I), repeatedAdd curve 7 curve.G)
        match Ecdsa.signWithNonce keyPair msgEc 7I with
        | Error _ -> ()
        | Ok _ -> Assert.Fail("Expected signWithNonce with k = 7 (r = 0) to return Error")

        // On toy3 with d = 5: k = 9 gives 9G = (7, 6), r = 7, and 9^(-1) * (3 + 7 * 5) = 17 * 38 = 0 (mod 19),
        // so s = 0 and signWithNonce refuses it.
        Assert.Equal(EllipticCurve.Point(7I, 6I), repeatedAdd curve 9 curve.G)
        match Ecdsa.signWithNonce keyPair msgEc 9I with
        | Error _ -> ()
        | Ok _ -> Assert.Fail("Expected signWithNonce with k = 9 (s = 0) to return Error")

        // On toy (N = 18), S = 6 is not invertible modulo 18, so verify returns false.
        let toyPub = EllipticCurve.toy.G
        Assert.False(Ecdsa.verify EllipticCurve.toy toyPub msgEc { R = 6I; S = 6I })

    [<Fact>]
    let ``Phase 83 - ECDSA signWithNonce checks private-key range [1, n-1]`` () =
        let curve = EllipticCurve.toy3
        let msgEc = Encoding.ASCII.GetBytes("ec")
        let k = 4I

        for badD in [ 0I; 19I; -1I ] do
            let badKey: Ecdsa.EcdsaKeyPair = {
                PrivateKey = badD
                PublicKey = EllipticCurve.Infinity
                Curve = curve
            }
            match Ecdsa.signWithNonce badKey msgEc k with
            | Error _ -> ()
            | Ok _ -> Assert.Fail("Expected signWithNonce with out-of-range private key to return Error")

        let key1: Ecdsa.EcdsaKeyPair = {
            PrivateKey = 1I
            PublicKey = curve.G
            Curve = curve
        }
        match Ecdsa.signWithNonce key1 msgEc k with
        | Ok sig1 ->
            Assert.Equal({ R = 3I; S = 11I }, sig1)
            Assert.True(Ecdsa.verify curve curve.G msgEc sig1)
        | Error err -> Assert.Fail(sprintf "Unexpected error for d = 1: %A" err)

        let key5: Ecdsa.EcdsaKeyPair = {
            PrivateKey = 5I
            PublicKey = EllipticCurve.Point(9I, 16I)
            Curve = curve
        }
        Assert.Equal(Ok { R = 3I; S = 14I }, Ecdsa.signWithNonce key5 msgEc k)

    [<Fact>]
    let ``Phase 112 - ECDSA verify via multiScalarMultiply on toy3`` () =
        let curve = EllipticCurve.toy3
        let pub = EllipticCurve.Point(9I, 16I)
        let msgEc = Encoding.ASCII.GetBytes("ec")
        let msgEx = Encoding.ASCII.GetBytes("ex")

        Assert.True(Ecdsa.verify curve pub msgEc { R = 3I; S = 14I })
        Assert.True(Ecdsa.verify curve pub msgEx { R = 3I; S = 11I })
        Assert.False(Ecdsa.verify curve pub msgEx { R = 3I; S = 14I })
