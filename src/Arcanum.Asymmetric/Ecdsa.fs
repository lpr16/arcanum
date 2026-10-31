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

    /// Converts message bytes to an ECDSA integer hash representative per SEC 1 section 4.1.3.
    /// Hashes with SHA256.hash (which calls the CLR System.Security.Cryptography.SHA256.HashData),
    /// reads the 256-bit digest as a big-endian unsigned integer, keeps the leftmost bitLength N bits
    /// when bitLength N < 256, and reduces modulo N.
    let messageRepresentative (curve: EllipticCurve.Curve) (message: byte[]) : BigInteger =
        let digest = (SHA256.hash message).Value
        let raw = BigInteger(ReadOnlySpan<byte>(digest), isUnsigned = true, isBigEndian = true)
        let nBits = int (curve.N.GetBitLength())
        let shifted =
            if nBits < 256 then
                raw >>> (256 - nBits)
            else
                raw
        Modular.modPos shifted curve.N

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
                        let z = messageRepresentative curve message
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
            | None ->
                let z = messageRepresentative curve message
                let target =
                    EllipticCurve.add
                        curve
                        (EllipticCurve.scalarMultiply curve z curve.G)
                        (EllipticCurve.scalarMultiply curve sigVal.R pubKey)
                [ BigInteger.One .. curve.N - BigInteger.One ]
                |> List.exists (fun k ->
                    match Modular.modInverse k curve.N with
                    | None -> false
                    | Some _ ->
                        match EllipticCurve.scalarMultiply curve k curve.G with
                        | EllipticCurve.Infinity -> false
                        | EllipticCurve.Point(x1, _) as kPt ->
                            Modular.modPos x1 curve.N = sigVal.R
                            && EllipticCurve.scalarMultiply curve sigVal.S kPt = target)
            | Some w ->
                let z = messageRepresentative curve message
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
