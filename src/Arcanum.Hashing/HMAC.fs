namespace Arcanum.Hashing

open System
open Arcanum.Core

/// An F# construction of HMAC whose hash step calls SHA256.hash.
/// The underlying SHA-256 primitive is the CLR call documented by that module.
module HMAC =
    /// Computes HMAC-SHA256 per RFC 2104.
    let hmacSha256 (key: Key) (message: byte[]) : Tag =
        let blockSize = 64
        let rawKey = key.Value

        // Step 1: Normalize key to blockSize
        let keyPadded = Array.zeroCreate blockSize
        if rawKey.Length > blockSize then
            let hashedKey = SHA256.hash rawKey
            Array.Copy(hashedKey.Value, 0, keyPadded, 0, hashedKey.Length)
        else
            Array.Copy(rawKey, 0, keyPadded, 0, rawKey.Length)

        // Step 2: Compute inner and outer padded keys
        let ipad = Array.create blockSize 0x36uy
        let opad = Array.create blockSize 0x5cuy

        let kIpad = Bytes.xor keyPadded ipad
        let kOpad = Bytes.xor keyPadded opad

        // Step 3: Inner hash = H(kIpad || message)
        let innerData = Array.concat [ kIpad; message ]
        let innerHash = (SHA256.hash innerData).Value

        // Step 4: Outer hash = H(kOpad || innerHash)
        let outerData = Array.concat [ kOpad; innerHash ]
        let outerHash = (SHA256.hash outerData).Value

        Tag outerHash

    /// Verifies HMAC tag in constant time.
    let verifySha256 (key: Key) (message: byte[]) (expectedTag: Tag) : bool =
        let computed = hmacSha256 key message
        Bytes.constantTimeEquals computed.Value expectedTag.Value
