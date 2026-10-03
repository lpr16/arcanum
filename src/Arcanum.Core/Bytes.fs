namespace Arcanum.Core

open System
open System.Security.Cryptography

module Bytes =
    /// Constant-time byte array comparison to mitigate timing side-channel attacks.
    let constantTimeEquals (a: byte[]) (b: byte[]) : bool =
        if obj.ReferenceEquals(a, b) then true
        elif isNull a || isNull b then false
        elif a.Length <> b.Length then false
        else
            let mutable diff = 0
            for i in 0 .. a.Length - 1 do
                diff <- diff ||| (int a.[i] ^^^ int b.[i])
            diff = 0

    /// Bitwise XOR of two byte arrays up to the length of the shorter array.
    let xor (a: byte[]) (b: byte[]) : byte[] =
        let len = min a.Length b.Length
        let res: byte[] = Array.zeroCreate len
        for i in 0 .. len - 1 do
            res.[i] <- a.[i] ^^^ b.[i]
        res

    /// Converts a byte array to a lowercase hexadecimal string.
    let toHex (bytes: byte[]) : string =
        Convert.ToHexStringLower(bytes)

    /// Parses a hexadecimal string into a byte array.
    let fromHex (hex: string) : byte[] =
        let cleanHex = hex.Replace(" ", "").Replace("-", "").Replace("0x", "")
        Convert.FromHexString(cleanHex)

    /// Converts a byte array to a Base64 string.
    let toBase64 (bytes: byte[]) : string =
        Convert.ToBase64String(bytes)

    /// Parses a Base64 string into a byte array.
    let fromBase64 (base64: string) : byte[] =
        Convert.FromBase64String(base64)

    /// Generates cryptographically secure random bytes of the specified length.
    let randomBytes (count: int) : byte[] =
        if count < 0 then
            raise (ArgumentOutOfRangeException(nameof count, "Count must be non-negative"))
        let buffer: byte[] = Array.zeroCreate count
        RandomNumberGenerator.Fill(Span<byte>(buffer))
        buffer

    /// Safely clears (zeroizes) sensitive memory buffers.
    let zeroize (buffer: byte[]) : unit =
        if not (isNull buffer) then
            CryptographicOperations.ZeroMemory(Span<byte>(buffer))

    /// Rotates a 32-bit unsigned integer left by n bits.
    let inline rotl32 (value: uint32) (bits: int) : uint32 =
        (value <<< bits) ||| (value >>> (32 - bits))

    /// Rotates a 32-bit unsigned integer right by n bits.
    let inline rotr32 (value: uint32) (bits: int) : uint32 =
        (value >>> bits) ||| (value <<< (32 - bits))

    /// Rotates a 64-bit unsigned integer right by n bits.
    let inline rotr64 (value: uint64) (bits: int) : uint64 =
        (value >>> bits) ||| (value <<< (64 - bits))
