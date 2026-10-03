namespace Arcanum.Hashing

open System
open System.Security.Cryptography
open Arcanum.Core

module SHA256 =
    /// Computes the cryptographic SHA-256 hash using the hardware-accelerated, NIST-certified primitive.
    let hash (message: byte[]) : Digest =
        let bytes = SHA256.HashData(ReadOnlySpan<byte>(message))
        Digest bytes

    /// Computes the SHA-256 hash of an ASCII / UTF-8 string.
    let hashString (text: string) : Digest =
        hash (System.Text.Encoding.UTF8.GetBytes text)
