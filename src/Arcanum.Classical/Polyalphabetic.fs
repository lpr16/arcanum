namespace Arcanum.Classical

open System
open Arcanum.Core

/// Historical polyalphabetic mechanisms for study and demonstration.
/// This module is not Arcanum's mathematical core.
module Polyalphabetic =
    let private charToShift (c: char) : int option =
        if c >= 'A' && c <= 'Z' then Some (int c - int 'A')
        elif c >= 'a' && c <= 'z' then Some (int c - int 'a')
        else None

    let private charToIndex (c: char) : int option =
        charToShift c

    let private indexToChar (isUpper: bool) (idx: int) : char =
        let baseChar = if isUpper then 'A' else 'a'
        char (int baseChar + ((idx % 26 + 26) % 26))

    /// Extracts clean alphabetic key shifts.
    let private parseKey (key: string) : int[] =
        key
        |> Seq.choose charToShift
        |> Seq.toArray

    /// Vigenère cipher encryption.
    let vigenereEncrypt (key: string) (text: string) : Result<string, CryptoError> =
        let shifts = parseKey key
        if shifts.Length = 0 then
            Error (ParameterOutOfRange "Key must contain at least one alphabetic character")
        else
            let mutable keyIdx = 0
            let result =
                text
                |> String.map (fun c ->
                    match charToIndex c with
                    | Some idx ->
                        let shift = shifts.[keyIdx % shifts.Length]
                        keyIdx <- keyIdx + 1
                        indexToChar (Char.IsUpper c) (idx + shift)
                    | None -> c
                )
            Ok result

    /// Vigenère cipher decryption.
    let vigenereDecrypt (key: string) (text: string) : Result<string, CryptoError> =
        let shifts = parseKey key
        if shifts.Length = 0 then
            Error (ParameterOutOfRange "Key must contain at least one alphabetic character")
        else
            let mutable keyIdx = 0
            let result =
                text
                |> String.map (fun c ->
                    match charToIndex c with
                    | Some idx ->
                        let shift = shifts.[keyIdx % shifts.Length]
                        keyIdx <- keyIdx + 1
                        indexToChar (Char.IsUpper c) (idx - shift)
                    | None -> c
                )
            Ok result

    /// Beaufort cipher: C = (K - P) mod 26.
    /// Notice Beaufort is self-reciprocal: encrypt and decrypt are the identical operation!
    let beaufort (key: string) (text: string) : Result<string, CryptoError> =
        let shifts = parseKey key
        if shifts.Length = 0 then
            Error (ParameterOutOfRange "Key must contain at least one alphabetic character")
        else
            let mutable keyIdx = 0
            let result =
                text
                |> String.map (fun c ->
                    match charToIndex c with
                    | Some idx ->
                        let k = shifts.[keyIdx % shifts.Length]
                        keyIdx <- keyIdx + 1
                        indexToChar (Char.IsUpper c) (k - idx)
                    | None -> c
                )
            Ok result
