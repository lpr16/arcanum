namespace Arcanum.Asymmetric

open System
open Arcanum.Core
open Arcanum.Hashing

/// RFC 9380 Hash to Curve with SHA-256 and simplified SWU.
/// CLR boundary: SHA-256 hash calls SHA256.hash (which calls System.Security.Cryptography.SHA256.HashData).
module HashToCurve =

    /// RFC 9380 section 5.3.1 expand_message_xmd with SHA-256 (b_in_bytes = 32, s_in_bytes = 64).
    /// Generates a pseudorandom byte string of length lenInBytes.
    /// Refuses ell > 255, lenInBytes > 65535 (or <= 0), and DST longer than 255 bytes with ParameterOutOfRange.
    let expandMessageXmd (message: byte[]) (dst: byte[]) (lenInBytes: int) : Result<byte[], CryptoError> =
        if isNull dst || dst.Length > 255 then
            Error (ParameterOutOfRange "DST length must be at most 255 bytes")
        elif lenInBytes <= 0 || lenInBytes > 65535 then
            Error (ParameterOutOfRange "len_in_bytes must be in range 1 .. 65535")
        else
            let ell = (lenInBytes + 31) / 32
            if ell > 255 then
                Error (ParameterOutOfRange $"ell ({ell}) exceeds maximum of 255")
            else
                let safeMsg = if isNull message then [||] else message
                let dstPrime = Array.append dst [| byte dst.Length |]
                let zPad = Array.zeroCreate<byte> 64
                let libStr = [| byte (lenInBytes >>> 8); byte (lenInBytes &&& 0xFF) |]
                let msgPrime = Array.concat [ zPad; safeMsg; libStr; [| 0x00uy |]; dstPrime ]
                let b0 = (SHA256.hash msgPrime).Value
                let b1 = (SHA256.hash (Array.concat [ b0; [| 0x01uy |]; dstPrime ])).Value

                let uniformBytes = Array.zeroCreate<byte> lenInBytes
                Array.Copy(b1, 0, uniformBytes, 0, min 32 lenInBytes)

                let mutable prevB = b1
                for i in 2 .. ell do
                    let bi = (SHA256.hash (Array.concat [ Bytes.xor b0 prevB; [| byte i |]; dstPrime ])).Value
                    let offset = (i - 1) * 32
                    let count = min 32 (lenInBytes - offset)
                    Array.Copy(bi, 0, uniformBytes, offset, count)
                    prevB <- bi

                Ok uniformBytes
