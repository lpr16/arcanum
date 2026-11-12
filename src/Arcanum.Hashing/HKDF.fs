namespace Arcanum.Hashing

open System
open Arcanum.Core

/// RFC 5869 HMAC-based Extract-and-Expand Key Derivation Function (HKDF) with SHA-256.
/// CLR boundary: HMAC is HMAC.hmacSha256, the F# HMAC construction built on SHA256.hash
/// (which calls System.Security.Cryptography.SHA256.HashData).
module HKDF =

    let private hashLen = 32

    /// RFC 5869 section 2.2 HKDF-Extract(salt, IKM) -> PRK.
    /// An empty or null salt defaults to 32 zero octets.
    let extract (salt: byte[]) (ikm: byte[]) : byte[] =
        let effectiveSalt =
            if isNull salt || salt.Length = 0 then
                Array.zeroCreate<byte> hashLen
            else
                salt
        let safeIkm = if isNull ikm then [||] else ikm
        (HMAC.hmacSha256 (Key effectiveSalt) safeIkm).Value

    /// RFC 5869 section 2.3 HKDF-Expand(PRK, info, L) -> OKM.
    /// Refuses lengths L > 255 * HashLen (8160 octets) or L < 0 with ParameterOutOfRange.
    /// Returns an empty array when L = 0.
    let expand (prk: byte[]) (info: byte[]) (length: int) : Result<byte[], CryptoError> =
        if length < 0 || length > 255 * hashLen then
            Error (ParameterOutOfRange $"Requested output length {length} exceeds maximum {255 * hashLen} octets")
        elif length = 0 then
            Ok [||]
        else
            let safeInfo = if isNull info then [||] else info
            let n = (length + hashLen - 1) / hashLen
            let okm = Array.zeroCreate<byte> length
            let mutable t = [||]
            let mutable offset = 0
            for i in 1 .. n do
                let stepData = Array.concat [ t; safeInfo; [| byte i |] ]
                t <- (HMAC.hmacSha256 (Key prk) stepData).Value
                let bytesToCopy = min hashLen (length - offset)
                Array.Copy(t, 0, okm, offset, bytesToCopy)
                offset <- offset + bytesToCopy
            Ok okm
