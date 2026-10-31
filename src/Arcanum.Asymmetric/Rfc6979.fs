namespace Arcanum.Asymmetric

open System
open System.Numerics
open Arcanum.Core
open Arcanum.Hashing

module Rfc6979 =
    let private qBitLength (q: BigInteger) : int =
        int (q.GetBitLength())

    let private rByteLength (q: BigInteger) : int =
        (qBitLength q + 7) / 8

    /// RFC 6979 section 2.3.2 bits2int: reads octets as a big-endian unsigned integer,
    /// then shifts right by 8 * octets.Length - qlen when that difference is positive.
    let bits2int (q: BigInteger) (octets: byte[]) : BigInteger =
        let qlen = qBitLength q
        let blen = 8 * octets.Length
        let v = BigInteger(ReadOnlySpan<byte>(octets), isUnsigned = true, isBigEndian = true)
        if blen > qlen then
            v >>> (blen - qlen)
        else
            v

    /// RFC 6979 section 2.3.3 int2octets: encodes a non-negative integer less than q as
    /// big-endian octets of length rlen / 8 = ceil(qlen / 8).
    let int2octets (q: BigInteger) (value: BigInteger) : byte[] =
        let rolen = rByteLength q
        let normalized = Modular.modPos value q
        let raw =
            if normalized = BigInteger.Zero then
                [||]
            else
                normalized.ToByteArray(isUnsigned = true, isBigEndian = true)
        let out = Array.zeroCreate<byte> rolen
        Array.blit raw 0 out (rolen - raw.Length) raw.Length
        out

    /// RFC 6979 section 2.3.4 bits2octets: int2octets q (bits2int q octets mod q).
    let bits2octets (q: BigInteger) (octets: byte[]) : byte[] =
        let z1 = bits2int q octets
        let z2 = Modular.modPos z1 q
        int2octets q z2

    /// RFC 6979 section 3.2 deterministic nonce generation (steps a through h) with SHA-256.
    /// CLR boundaries: the message digest is SHA256.hash (which calls System.Security.Cryptography.SHA256.HashData),
    /// and HMAC_K is HMAC.hmacSha256 (the F# HMAC construction built on SHA256.hash).
    let generateK (q: BigInteger) (privateKey: BigInteger) (message: byte[]) : BigInteger =
        let qlen = qBitLength q
        let h1 = (SHA256.hash message).Value
        let xOctets = int2octets q privateKey
        let h1Octets = bits2octets q h1

        let mutable v = Array.create 32 0x01uy
        let mutable k = Array.create 32 0x00uy

        k <- (HMAC.hmacSha256 (Key k) (Array.concat [ v; [| 0x00uy |]; xOctets; h1Octets ])).Value
        v <- (HMAC.hmacSha256 (Key k) v).Value
        k <- (HMAC.hmacSha256 (Key k) (Array.concat [ v; [| 0x01uy |]; xOctets; h1Octets ])).Value
        v <- (HMAC.hmacSha256 (Key k) v).Value

        let rec loop () =
            let mutable t = [||]
            while 8 * t.Length < qlen do
                v <- (HMAC.hmacSha256 (Key k) v).Value
                t <- Array.append t v
            let candidate = bits2int q t
            if candidate >= BigInteger.One && candidate < q then
                candidate
            else
                k <- (HMAC.hmacSha256 (Key k) (Array.append v [| 0x00uy |])).Value
                v <- (HMAC.hmacSha256 (Key k) v).Value
                loop ()
        loop ()
