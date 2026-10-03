namespace Arcanum.Asymmetric

open System
open System.Globalization
open System.Numerics
open Arcanum.Core

module DiffieHellman =
    /// Classical Diffie-Hellman Domain Parameters (p, g).
    type DhGroup = {
        P: BigInteger
        G: BigInteger
    }

    /// RFC 3526 1536-bit MODP Group 5 (standard safe prime group).
    let rfc3526Group5 : DhGroup =
        let hex =
            "0FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD1" +
            "29024E088A67CC74020BBEA63B139B22514A08798E3404DD" +
            "EF9519B3CD3A431B302B0A6DF25F14374FE1356D6D51C245" +
            "E485B576625E7EC6F44C42E9A637ED6B0BFF5CB6F406B7ED" +
            "EE386BFB5A899FA5AE9F24117C4B1FE649286651ECE65381" +
            "FFFFFFFFFFFFFFFF"
        let p = BigInteger.Parse(hex, NumberStyles.HexNumber)
        { P = p; G = 2I }

    type DhKeyPair = {
        PrivateKey: BigInteger
        PublicKey: BigInteger
    }

    /// Generates a classical Diffie-Hellman key pair.
    let generateKeyPair (group: DhGroup) : DhKeyPair =
        let priv = Primes.randomBigInteger 2I (group.P - 2I)
        let pub = Modular.modPow group.G priv group.P
        { PrivateKey = priv; PublicKey = pub }

    /// Derives the shared secret: S = otherPublicKey ^ myPrivateKey mod P.
    let computeSharedSecret (group: DhGroup) (myPrivateKey: BigInteger) (otherPublicKey: BigInteger) : BigInteger =
        Modular.modPow otherPublicKey myPrivateKey group.P

    /// Elliptic Curve Diffie-Hellman (ECDH) KeyPair.
    type EcdhKeyPair = {
        PrivateKey: BigInteger
        PublicKey: EllipticCurve.ECPoint
    }

    /// Generates an ECDH keypair on the specified curve.
    let generateEcdhKeyPair (curve: EllipticCurve.Curve) : EcdhKeyPair =
        let priv = Primes.randomBigInteger BigInteger.One (curve.N - BigInteger.One)
        let pub = EllipticCurve.scalarMultiply curve priv curve.G
        { PrivateKey = priv; PublicKey = pub }

    /// Computes the shared point on the curve: S = myPrivateKey * otherPublicKey.
    let computeEcdhSharedSecret (curve: EllipticCurve.Curve) (myPrivateKey: BigInteger) (otherPublicKey: EllipticCurve.ECPoint) : EllipticCurve.ECPoint =
        EllipticCurve.scalarMultiply curve myPrivateKey otherPublicKey
