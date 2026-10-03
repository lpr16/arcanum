namespace Arcanum.Hashing

open System
open System.Security.Cryptography
open Arcanum.Core

/// Adapts the CLR SHA-256 primitive for Arcanum.
/// The primitive is System.Security.Cryptography.SHA256.HashData;
/// this repository does not implement the SHA-256 compression function.
module SHA256 =
    /// Computes a SHA-256 digest by calling the CLR primitive.
    let hash (message: byte[]) : Digest =
        let bytes = SHA256.HashData(ReadOnlySpan<byte>(message))
        Digest bytes

    /// Computes the SHA-256 hash of an ASCII / UTF-8 string.
    let hashString (text: string) : Digest =
        hash (System.Text.Encoding.UTF8.GetBytes text)
