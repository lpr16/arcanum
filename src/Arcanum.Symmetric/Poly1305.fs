namespace Arcanum.Symmetric

open System
open System.Numerics
open Arcanum.Core

module Poly1305 =
    let private P = (BigInteger.One <<< 130) - 5I

    /// Clamps r according to RFC 8439.
    let clampR (r: byte[]) : byte[] =
        let clamped = Array.copy r
        clamped.[3]  <- clamped.[3]  &&& 15uy
        clamped.[7]  <- clamped.[7]  &&& 15uy
        clamped.[11] <- clamped.[11] &&& 15uy
        clamped.[15] <- clamped.[15] &&& 15uy
        clamped.[4]  <- clamped.[4]  &&& 252uy
        clamped.[8]  <- clamped.[8]  &&& 252uy
        clamped.[12] <- clamped.[12] &&& 252uy
        clamped

    /// Computes Poly1305 MAC tag (16 bytes) per RFC 8439.
    let mac (key: byte[]) (message: byte[]) : Tag =
        if key.Length <> 32 then
            raise (ArgumentException("Poly1305 key must be 32 bytes"))

        let rBytes = clampR (Array.sub key 0 16)
        let sBytes = Array.sub key 16 16

        // Little-endian BigInteger conversions
        let rBuffer = Array.zeroCreate 17 // +1 for positive sign
        Array.Copy(rBytes, 0, rBuffer, 0, 16)
        let r = BigInteger(ReadOnlySpan<byte>(rBuffer))

        let sBuffer = Array.zeroCreate 17
        Array.Copy(sBytes, 0, sBuffer, 0, 16)
        let s = BigInteger(ReadOnlySpan<byte>(sBuffer))

        let mutable a = BigInteger.Zero
        let chunkCount = (message.Length + 15) / 16

        for i = 0 to chunkCount - 1 do
            let offset = i * 16
            let chunkLen = min 16 (message.Length - offset)
            // Block with appended 0x01 byte
            let blockBuffer = Array.zeroCreate (chunkLen + 2)
            Array.Copy(message, offset, blockBuffer, 0, chunkLen)
            blockBuffer.[chunkLen] <- 0x01uy // 0x01 appended per spec
            let n = BigInteger(ReadOnlySpan<byte>(blockBuffer))

            a <- ((a + n) * r) % P

        a <- (a + s) % (BigInteger.One <<< 128)

        let tagBytes = Array.zeroCreate 16
        let rawTag = a.ToByteArray()
        let copyLen = min 16 rawTag.Length
        Array.Copy(rawTag, 0, tagBytes, 0, copyLen)

        Tag tagBytes

    /// Verifies a Poly1305 tag in constant time.
    let verify (key: byte[]) (message: byte[]) (expectedTag: Tag) : bool =
        let computed = mac key message
        Bytes.constantTimeEquals computed.Value expectedTag.Value
