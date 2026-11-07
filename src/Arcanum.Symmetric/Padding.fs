namespace Arcanum.Symmetric

open System
open Arcanum.Core

module Padding =
    /// Applies PKCS#7 padding to align data to the specified blockSize (typically 16 for AES).
    let padPkcs7 (blockSize: int) (data: byte[]) : byte[] =
        if blockSize <= 0 || blockSize > 255 then
            raise (ArgumentOutOfRangeException(nameof blockSize, "Block size must be between 1 and 255"))

        let padLen = blockSize - (data.Length % blockSize)
        let padByte = byte padLen
        let padded = Array.zeroCreate (data.Length + padLen)
        Array.Copy(data, 0, padded, 0, data.Length)
        for i = data.Length to padded.Length - 1 do
            padded.[i] <- padByte
        padded

    /// Strips PKCS#7 padding by checking length and the last byte, then looping over the last padLen bytes
    /// accumulating the bitwise OR of (int data.[i] ^^^ padLen). No constant-time claim is made.
    let unpadPkcs7 (blockSize: int) (data: byte[]) : Result<byte[], CryptoError> =
        if data.Length = 0 || data.Length % blockSize <> 0 then
            Error InvalidPadding
        else
            let padLen = int data.[data.Length - 1]
            if padLen <= 0 || padLen > blockSize || padLen > data.Length then
                Error InvalidPadding
            else
                // Accumulate the bitwise OR of (int data.[i] ^^^ padLen) across the last padLen bytes; no constant-time claim is made.
                let mutable diff = 0
                for i = data.Length - padLen to data.Length - 1 do
                    diff <- diff ||| (int data.[i] ^^^ padLen)

                if diff <> 0 then
                    Error InvalidPadding
                else
                    let unpaddedLen = data.Length - padLen
                    let unpadded = Array.zeroCreate unpaddedLen
                    Array.Copy(data, 0, unpadded, 0, unpaddedLen)
                    Ok unpadded
