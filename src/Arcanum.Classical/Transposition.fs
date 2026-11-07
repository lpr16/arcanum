namespace Arcanum.Classical

open System
open Arcanum.Core

/// Historical rail fence transposition cipher. Does not replace the elliptic curve.
module Transposition =

    let private railPattern (rails: int) (length: int) : int[] =
        let pattern = Array.zeroCreate<int> length
        let mutable row = 0
        let mutable dir = 1
        for i in 0 .. length - 1 do
            pattern.[i] <- row
            if row = 0 then
                dir <- 1
            elif row = rails - 1 then
                dir <- -1
            row <- row + dir
        pattern

    /// Encrypts text by writing it in a zigzag down and up the rails and reading the rails top to bottom.
    /// Refuses fewer than 2 rails. Characters are moved, not changed.
    let railFenceEncrypt (rails: int) (text: string) : Result<string, CryptoError> =
        if rails < 2 then
            Error (ParameterOutOfRange "Rail fence requires at least 2 rails")
        elif isNull text || text.Length = 0 then
            Ok ""
        else
            let pattern = railPattern rails text.Length
            let out =
                [| for r in 0 .. rails - 1 do
                    for i in 0 .. text.Length - 1 do
                        if pattern.[i] = r then
                            yield text.[i] |]
            Ok (String(out))

    /// Decrypts text produced by railFenceEncrypt with the same number of rails.
    /// Refuses fewer than 2 rails.
    let railFenceDecrypt (rails: int) (ciphertext: string) : Result<string, CryptoError> =
        if rails < 2 then
            Error (ParameterOutOfRange "Rail fence requires at least 2 rails")
        elif isNull ciphertext || ciphertext.Length = 0 then
            Ok ""
        else
            let n = ciphertext.Length
            let pattern = railPattern rails n
            let out = Array.zeroCreate<char> n
            let mutable pos = 0
            for r in 0 .. rails - 1 do
                for i in 0 .. n - 1 do
                    if pattern.[i] = r then
                        out.[i] <- ciphertext.[pos]
                        pos <- pos + 1
            Ok (String(out))
