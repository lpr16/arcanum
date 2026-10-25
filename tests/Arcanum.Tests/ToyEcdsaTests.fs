namespace Arcanum.Tests

open System
open System.Numerics
open System.Text
open Xunit
open Arcanum.Core
open Arcanum.Hashing
open Arcanum.Asymmetric

module ToyEcdsaTests =

    // EllipticCurve.toy is a 18-point curve over F_17; toy is not a secure group.
    // The message representative z uses SHA256.hash, which is the CLR library call
    // System.Security.Cryptography.SHA256.HashData. This test uses a caller-supplied
    // nonce with signWithNonce; it is not RFC 6979.

    let private repeatedAdd (curve: EllipticCurve.Curve) (count: int) (pt: EllipticCurve.ECPoint) =
        [ 1 .. count ]
        |> List.fold (fun acc _ -> EllipticCurve.add curve acc pt) EllipticCurve.Infinity

    let private messageRepresentative (curve: EllipticCurve.Curve) (message: byte[]) : BigInteger =
        let digest = (SHA256.hash message).Value
        let buf = Array.zeroCreate (digest.Length + 1)
        Array.Copy(digest, 0, buf, 0, digest.Length)
        Modular.modPos (BigInteger(ReadOnlySpan<byte>(buf))) curve.N

    [<Fact>]
    let ``Phase 26 - ECDSA signWithNonce and verify on toy curve`` () =
        let curve = EllipticCurve.toy
        let d = 5I
        let k = 7I
        let msgEc = Encoding.ASCII.GetBytes("ec")
        let msgEx = Encoding.ASCII.GetBytes("ex")

        let pubPoint = repeatedAdd curve 5 curve.G
        let keyPair: Ecdsa.EcdsaKeyPair = {
            PrivateKey = d
            PublicKey = pubPoint
            Curve = curve
        }

        let noncePoint = repeatedAdd curve 7 curve.G
        let expectedR =
            match noncePoint with
            | EllipticCurve.Point(x, _) -> Modular.modPos x 18I
            | EllipticCurve.Infinity ->
                Assert.Fail("Expected finite point for 7 * G on toy")
                0I

        match Ecdsa.signWithNonce keyPair msgEc k with
        | Error err -> Assert.Fail($"Expected signature, got error: {err}")
        | Ok signature ->
            Assert.Equal(expectedR, signature.R)

            let z = messageRepresentative curve msgEc
            match Modular.modInverse k 18I with
            | None -> Assert.Fail("Expected invertible nonce 7 modulo 18")
            | Some kInv ->
                let expectedS = Modular.modPos (kInv * (z + signature.R * d)) 18I
                Assert.Equal(expectedS, signature.S)

            Assert.True(Ecdsa.verify curve pubPoint msgEc signature)
            Assert.False(Ecdsa.verify curve pubPoint msgEx signature)

    // The toy curve group has 18 points and is not a place to keep a key.
    [<Fact>]
    let ``Phase 27 - Repeated nonce on toy curve exposes the private scalar`` () =
        let curve = EllipticCurve.toy
        let d = 5I
        let k = 7I
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
            Assert.Equal(sig1.R, sig2.R)
            let r = sig1.R
            let z1 = messageRepresentative curve msgEc
            let z2 = messageRepresentative curve msgEx
            let deltaS = Modular.modPos (sig1.S - sig2.S) curve.N
            let deltaZ = Modular.modPos (z1 - z2) curve.N

            let recoveredScalars =
                [ 1I .. curve.N - 1I ]
                |> List.choose (fun kCand ->
                    match Modular.modInverse kCand curve.N with
                    | None -> None
                    | Some kInv ->
                        let matchesLinear =
                            Modular.modPos (deltaS * kCand) curve.N = deltaZ
                        let matchesR =
                            match repeatedAdd curve (int kCand) curve.G with
                            | EllipticCurve.Point(x, _) -> Modular.modPos x curve.N = r
                            | EllipticCurve.Infinity -> false
                        if matchesLinear && matchesR then
                            [ 1I .. curve.N - 1I ]
                            |> List.filter (fun dCand ->
                                Modular.modInverse dCand curve.N |> Option.isSome
                                && Modular.modPos (kInv * (z1 + r * dCand)) curve.N = sig1.S
                                && Modular.modPos (kInv * (z2 + r * dCand)) curve.N = sig2.S)
                            |> Some
                        else
                            None)
                |> List.concat

            let recoveredD = Assert.Single(recoveredScalars)
            Assert.Equal(5I, recoveredD)
        | _ ->
            Assert.Fail("Expected both signatures with nonce 7 to succeed")

