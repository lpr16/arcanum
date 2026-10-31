namespace Arcanum.Core

open System
open System.Numerics

module Sec1 =
    let private fieldByteLength (curve: EllipticCurve.Curve) : int =
        (int (curve.P.GetBitLength()) + 7) / 8

    let private fieldElementToBytes (m: int) (p: BigInteger) (v: BigInteger) : byte[] =
        let normalized = Modular.modPos v p
        let raw =
            if normalized = BigInteger.Zero then
                [||]
            else
                normalized.ToByteArray(isUnsigned = true, isBigEndian = true)
        let out = Array.zeroCreate<byte> m
        Array.blit raw 0 out (m - raw.Length) raw.Length
        out

    /// Encodes an elliptic curve point to octets per SEC 1 section 2.3.3.
    /// Infinity is 0x00. Uncompressed is 0x04 || X || Y. Compressed is 0x02 || X (even y) or 0x03 || X (odd y).
    let encodePoint (curve: EllipticCurve.Curve) (compressed: bool) (pt: EllipticCurve.ECPoint) : byte[] =
        let m = fieldByteLength curve
        if compressed then
            match EllipticCurve.compress curve pt with
            | EllipticCurve.CompressedInfinity -> [| 0x00uy |]
            | EllipticCurve.Compressed(x, yBit) ->
                let prefix = if yBit = 0 then 0x02uy else 0x03uy
                Array.append [| prefix |] (fieldElementToBytes m curve.P x)
        else
            match pt with
            | EllipticCurve.Infinity -> [| 0x00uy |]
            | EllipticCurve.Point(x, y) ->
                Array.concat [
                    [| 0x04uy |]
                    fieldElementToBytes m curve.P x
                    fieldElementToBytes m curve.P y
                ]

    /// Decodes an octet string to an elliptic curve point per SEC 1 section 2.3.4.
    /// Uses EllipticCurve.decompress for compressed points and EllipticCurve.isOnCurve for uncompressed points.
    let decodePoint (curve: EllipticCurve.Curve) (bytes: byte[]) : Result<EllipticCurve.ECPoint, CryptoError> =
        let m = fieldByteLength curve
        if isNull bytes || bytes.Length = 0 then
            Error (ParameterOutOfRange "SEC 1 point encoding cannot be empty")
        else
            match bytes.[0] with
            | 0x00uy ->
                if bytes.Length = 1 then
                    Ok EllipticCurve.Infinity
                else
                    Error (ParameterOutOfRange "SEC 1 infinity encoding must be 1 byte")
            | 0x02uy | 0x03uy ->
                if bytes.Length <> 1 + m then
                    Error (ParameterOutOfRange "Invalid SEC 1 compressed point length")
                else
                    let x = BigInteger(ReadOnlySpan<byte>(bytes, 1, m), isUnsigned = true, isBigEndian = true)
                    if x < BigInteger.Zero || x >= curve.P then
                        Error (ParameterOutOfRange "Compressed x coordinate is out of field range")
                    else
                        let yBit = if bytes.[0] = 0x02uy then 0 else 1
                        match EllipticCurve.decompress curve (EllipticCurve.Compressed(x, yBit)) with
                        | Some pt when EllipticCurve.isOnCurve curve pt -> Ok pt
                        | _ -> Error (ParameterOutOfRange "Compressed x coordinate has no square root on the curve")
            | 0x04uy ->
                if bytes.Length <> 1 + 2 * m then
                    Error (ParameterOutOfRange "Invalid SEC 1 uncompressed point length")
                else
                    let x = BigInteger(ReadOnlySpan<byte>(bytes, 1, m), isUnsigned = true, isBigEndian = true)
                    let y = BigInteger(ReadOnlySpan<byte>(bytes, 1 + m, m), isUnsigned = true, isBigEndian = true)
                    let pt = EllipticCurve.Point(x, y)
                    if EllipticCurve.isOnCurve curve pt then
                        Ok pt
                    else
                        Error (ParameterOutOfRange "Decoded uncompressed point is not on the curve")
            | _ ->
                Error (ParameterOutOfRange "Unknown SEC 1 point prefix")
