namespace Arcanum.Classical

open System
open Arcanum.Core

/// Historical substitution mechanisms for study and demonstration.
/// This module is not Arcanum's mathematical core.
module Substitution =
    /// Normalizes a character to uppercase A-Z index [0..25].
    let private charToIndex (c: char) : int option =
        if c >= 'A' && c <= 'Z' then Some (int c - int 'A')
        elif c >= 'a' && c <= 'z' then Some (int c - int 'a')
        else None

    /// Converts an index [0..25] back to a character preserving case.
    let private indexToChar (isUpper: bool) (idx: int) : char =
        let baseChar = if isUpper then 'A' else 'a'
        char (int baseChar + ((idx % 26 + 26) % 26))

    /// Caesar cipher encryption with shift k (0..25).
    let caesarEncrypt (shift: int) (text: string) : string =
        let k = ((shift % 26) + 26) % 26
        text
        |> String.map (fun c ->
            match charToIndex c with
            | Some idx -> indexToChar (Char.IsUpper c) (idx + k)
            | None -> c
        )

    /// Caesar cipher decryption with shift k.
    let caesarDecrypt (shift: int) (text: string) : string =
        caesarEncrypt (-shift) text

    /// Atbash cipher (A <-> Z, B <-> Y, ...). Self-reciprocal.
    let atbash (text: string) : string =
        text
        |> String.map (fun c ->
            match charToIndex c with
            | Some idx -> indexToChar (Char.IsUpper c) (25 - idx)
            | None -> c
        )

    /// Affine cipher encryption: E(x) = (a * x + b) mod 26.
    /// Requires gcd(a, 26) = 1.
    let affineEncrypt (a: int) (b: int) (text: string) : Result<string, CryptoError> =
        let (g, _, _) = Modular.xgcd (bigint a) 26I
        if g <> 1I then
            Error (NonInvertibleElement $"Multiplier 'a' = {a} is not coprime to 26 (gcd = {g})")
        else
            let enc =
                text
                |> String.map (fun c ->
                    match charToIndex c with
                    | Some idx ->
                        let newIdx = (a * idx + b) % 26
                        indexToChar (Char.IsUpper c) newIdx
                    | None -> c
                )
            Ok enc

    /// Affine cipher decryption: D(y) = a^(-1) * (y - b) mod 26.
    let affineDecrypt (a: int) (b: int) (text: string) : Result<string, CryptoError> =
        match Modular.modInverse (bigint a) 26I with
        | None ->
            Error (NonInvertibleElement $"Multiplier 'a' = {a} has no modular inverse modulo 26")
        | Some invA ->
            let aInv = int invA
            let dec =
                text
                |> String.map (fun c ->
                    match charToIndex c with
                    | Some idx ->
                        let newIdx = (aInv * (idx - b)) % 26
                        indexToChar (Char.IsUpper c) newIdx
                    | None -> c
                )
            Ok dec
