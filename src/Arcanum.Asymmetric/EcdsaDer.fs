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

    let private parseInteger (bytes: byte[]) (offset: int) : Result<BigInteger * int, CryptoError> =
        if offset + 2 > bytes.Length then
            Error (ParameterOutOfRange "Integer header extends beyond buffer")
        elif bytes.[offset] <> 0x02uy then
            Error (ParameterOutOfRange "Expected integer tag 0x02")
        else
            let len = int bytes.[offset + 1]
            if len <= 0 then
                Error (ParameterOutOfRange "Integer length must be positive")
            elif offset + 2 + len > bytes.Length then
                Error (ParameterOutOfRange "Integer content extends beyond buffer")
            else
                let intSpan = ReadOnlySpan<byte>(bytes, offset + 2, len)
                if (intSpan.[0] &&& 0x80uy) <> 0uy then
                    Error (ParameterOutOfRange "Integer has negative sign bit in DER")
                elif intSpan.[0] = 0x00uy && len > 1 && (intSpan.[1] &&& 0x80uy) = 0uy then
                    Error (ParameterOutOfRange "Integer has redundant leading zero byte (not minimal DER)")
                else
                    let v = BigInteger(intSpan, isUnsigned = true, isBigEndian = true)
                    Ok (v, offset + 2 + len)

    /// Decodes an ECDSA signature (r, s) from strict DER format.
    /// Accepts only minimal, non-negative, non-zero r and s in a SEQUENCE.
    let decode (bytes: byte[]) : Result<EcdsaSignature, CryptoError> =
        if isNull bytes || bytes.Length < 6 then
            Error (ParameterOutOfRange "DER signature is too short")
        elif bytes.[0] <> 0x30uy then
            Error (ParameterOutOfRange "Expected sequence tag 0x30")
        elif int bytes.[1] <> bytes.Length - 2 then
            Error (ParameterOutOfRange "Sequence length does not match body length")
        else
            match parseInteger bytes 2 with
            | Error err -> Error err
            | Ok (r, offsetR) ->
                if r <= BigInteger.Zero then
                    Error (ParameterOutOfRange "Signature r must be positive")
                else
                    match parseInteger bytes offsetR with
                    | Error err -> Error err
                    | Ok (s, offsetS) ->
                        if s <= BigInteger.Zero then
                            Error (ParameterOutOfRange "Signature s must be positive")
                        elif offsetS <> bytes.Length then
                            Error (ParameterOutOfRange "Unexpected trailing bytes in signature")
                        else
                            Ok { R = r; S = s }
