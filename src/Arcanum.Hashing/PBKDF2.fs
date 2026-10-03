namespace Arcanum.Hashing

open System
open Arcanum.Core

module PBKDF2 =
    /// Computes PBKDF2 with HMAC-SHA256 per RFC 2898 / RFC 8018.
    let deriveKey (password: byte[]) (salt: byte[]) (iterations: int) (outputByteLength: int) : Key =
        if iterations <= 0 then
            raise (ArgumentOutOfRangeException(nameof iterations, "Iterations must be positive"))
        if outputByteLength <= 0 then
            raise (ArgumentOutOfRangeException(nameof outputByteLength, "Output length must be positive"))

        let prf (prfKey: Key) (data: byte[]) : byte[] =
            (HMAC.hmacSha256 prfKey data).Value

        let hLen = 32 // SHA-256 output length
        let l = (outputByteLength + hLen - 1) / hLen
        let derived = Array.zeroCreate outputByteLength
        let passKey = Key password

        for i = 1 to l do
            // INT_32_BE(i)
            let iBytes = [|
                byte (i >>> 24)
                byte (i >>> 16)
                byte (i >>> 8)
                byte i
            |]

            let mutable u = prf passKey (Array.append salt iBytes)
            let f = Array.copy u

            for _ = 2 to iterations do
                u <- prf passKey u
                for j = 0 to hLen - 1 do
                    f.[j] <- f.[j] ^^^ u.[j]

            let destOffset = (i - 1) * hLen
            let copyLen = min hLen (outputByteLength - destOffset)
            Array.Copy(f, 0, derived, destOffset, copyLen)

        Key derived
