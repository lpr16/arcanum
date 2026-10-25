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
