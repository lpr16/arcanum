namespace Arcanum.Asymmetric

open System
open System.Numerics
open Arcanum.Core

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
