namespace Arcanum.Asymmetric

open System
open System.Numerics
open Arcanum.Core
open Arcanum.Hashing

module Ecdsa =
    type EcdsaKeyPair = {
        PrivateKey: BigInteger
        PublicKey: EllipticCurve.ECPoint
        Curve: EllipticCurve.Curve
    }

    /// Generates an ECDSA key pair on the given curve.
    let generateKeyPair (curve: EllipticCurve.Curve) : EcdsaKeyPair =
        let priv = Primes.randomBigInteger BigInteger.One (curve.N - BigInteger.One)
        let pub = EllipticCurve.scalarMultiply curve priv curve.G
        { PrivateKey = priv; PublicKey = pub; Curve = curve }

    /// Converts message bytes to an integer hash representative modulo n.
    let private hashToInt (message: byte[]) (curve: EllipticCurve.Curve) : BigInteger =
        let digest = (SHA256.hash message).Value
        let buf = Array.zeroCreate (digest.Length + 1)
        Array.Copy(digest, 0, buf, 0, digest.Length)
        let z = BigInteger(ReadOnlySpan<byte>(buf))
        Modular.modPos z curve.N

    /// Signs a message using ECDSA with a specified nonce k (useful for testing or RFC 6979).
    let signWithNonce (keyPair: EcdsaKeyPair) (message: byte[]) (k: BigInteger) : Result<EcdsaSignature, CryptoError> =
        let curve = keyPair.Curve
        if k <= BigInteger.Zero || k >= curve.N then
            Error (ParameterOutOfRange "Nonce k must be in range [1, n-1]")
        else
            match EllipticCurve.scalarMultiply curve k curve.G with
            | EllipticCurve.Infinity -> Error (CorruptedState "Nonce resulted in point at infinity")
            | EllipticCurve.Point(x1, _) ->
                let r = Modular.modPos x1 curve.N
                if r = BigInteger.Zero then
                    Error (CorruptedState "Signature r is zero")
                else
                    match Modular.modInverse k curve.N with
                    | None -> Error (NonInvertibleElement "Nonce k is not invertible modulo n")
                    | Some kInv ->
                        let z = hashToInt message curve
                        let s = Modular.modPos (kInv * (z + r * keyPair.PrivateKey)) curve.N
                        if s = BigInteger.Zero then
                            Error (CorruptedState "Signature s is zero")
                        else
                            Ok { R = r; S = s }

    /// Signs a message using ECDSA with a cryptographically secure random nonce.
    let sign (keyPair: EcdsaKeyPair) (message: byte[]) : Result<EcdsaSignature, CryptoError> =
        let curve = keyPair.Curve
        let rec loop () =
            let k = Primes.randomBigInteger BigInteger.One (curve.N - BigInteger.One)
            match signWithNonce keyPair message k with
            | Ok sigVal -> Ok sigVal
            | Error _ -> loop ()
        loop ()

    /// Verifies an ECDSA signature against the public key and message.
    let verify (curve: EllipticCurve.Curve) (pubKey: EllipticCurve.ECPoint) (message: byte[]) (sigVal: EcdsaSignature) : bool =
        if sigVal.R <= BigInteger.Zero || sigVal.R >= curve.N then false
        elif sigVal.S <= BigInteger.Zero || sigVal.S >= curve.N then false
        elif not (EllipticCurve.isOnCurve curve pubKey) then false
        else
            match Modular.modInverse sigVal.S curve.N with
            | None -> false
            | Some w ->
                let z = hashToInt message curve
                let u1 = Modular.modPos (z * w) curve.N
                let u2 = Modular.modPos (sigVal.R * w) curve.N

                let p1 = EllipticCurve.scalarMultiply curve u1 curve.G
                let p2 = EllipticCurve.scalarMultiply curve u2 pubKey
                let pt = EllipticCurve.add curve p1 p2

                match pt with
                | EllipticCurve.Infinity -> false
                | EllipticCurve.Point(x1, _) ->
                    let v = Modular.modPos x1 curve.N
                    v = sigVal.R
