namespace Arcanum.Asymmetric

open System
open System.Numerics
open Arcanum.Core

/// DER encoding and decoding for ECDSA signatures per RFC 3279 section 2.2.3 and SEC 1 section C.8.
/// Ecdsa-Sig-Value ::= SEQUENCE { r INTEGER, s INTEGER }
module EcdsaDer =

    let private encodeInteger (value: BigInteger) : byte[] =
        let raw =
            if value = BigInteger.Zero then
                [| 0x00uy |]
            else
                value.ToByteArray(isUnsigned = true, isBigEndian = true)
        let body =
            if (raw.[0] &&& 0x80uy) <> 0uy then
                Array.append [| 0x00uy |] raw
            else
                raw
        Array.concat [
            [| 0x02uy; byte body.Length |]
            body
        ]

    /// Encodes an ECDSA signature (r, s) into DER format.
    let encode (sigVal: EcdsaSignature) : byte[] =
        let rBytes = encodeInteger sigVal.R
        let sBytes = encodeInteger sigVal.S
        let body = Array.append rBytes sBytes
        Array.concat [
            [| 0x30uy; byte body.Length |]
            body
        ]
