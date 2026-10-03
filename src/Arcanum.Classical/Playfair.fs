namespace Arcanum.Classical

open System
open Arcanum.Core

/// The historical Playfair mechanism for study and demonstration.
/// This module is not Arcanum's mathematical core.
module Playfair =
    /// Standard 5x5 key matrix structure (merging 'J' into 'I').
    type Matrix = {
        Grid: char[,]
        Positions: Map<char, int * int>
    }

    /// Normalizes key text and builds the 5x5 Playfair grid.
    let createMatrix (key: string) : Matrix =
        let cleanChar (c: char) =
            let u = Char.ToUpperInvariant c
            if u = 'J' then 'I' else u

        let alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ" // 'J' omitted
        let keyChars =
            key
            |> Seq.filter Char.IsLetter
            |> Seq.map cleanChar
            |> Seq.distinct
            |> Seq.toList

        let remainingAlphabet =
            alphabet
            |> Seq.filter (fun c -> not (List.contains c keyChars))
            |> Seq.toList

        let allChars = keyChars @ remainingAlphabet
        let grid = Array2D.zeroCreate 5 5
        let mutable positions = Map.empty

        for idx, c in List.indexed allChars do
            let row = idx / 5
            let col = idx % 5
            grid.[row, col] <- c
            positions <- Map.add c (row, col) positions

        { Grid = grid; Positions = positions }

    /// Prepares plaintext into pairs of characters according to Playfair rules.
    let preparePlaintext (text: string) : (char * char) list =
        let clean =
            text
            |> Seq.filter Char.IsLetter
            |> Seq.map (fun c ->
                let u = Char.ToUpperInvariant c
                if u = 'J' then 'I' else u
            )
            |> Seq.toList

        let rec pairList chars =
            match chars with
            | [] -> []
            | [a] -> [(a, 'X')]
            | a :: b :: rest ->
                if a = b then
                    (a, 'X') :: pairList (b :: rest)
                else
                    (a, b) :: pairList rest

        pairList clean

    /// Encrypts a digraph pair.
    let private encryptPair (matrix: Matrix) (a: char, b: char) : char * char =
        let (r1, c1) = matrix.Positions.[a]
        let (r2, c2) = matrix.Positions.[b]

        if r1 = r2 then
            // Same row -> shift right
            (matrix.Grid.[r1, (c1 + 1) % 5], matrix.Grid.[r2, (c2 + 1) % 5])
        elif c1 = c2 then
            // Same column -> shift down
            (matrix.Grid.[(r1 + 1) % 5, c1], matrix.Grid.[(r2 + 1) % 5, c2])
        else
            // Rectangle -> swap columns
            (matrix.Grid.[r1, c2], matrix.Grid.[r2, c1])

    /// Decrypts a digraph pair.
    let private decryptPair (matrix: Matrix) (a: char, b: char) : char * char =
        let (r1, c1) = matrix.Positions.[a]
        let (r2, c2) = matrix.Positions.[b]

        if r1 = r2 then
            // Same row -> shift left
            (matrix.Grid.[r1, (c1 + 4) % 5], matrix.Grid.[r2, (c2 + 4) % 5])
        elif c1 = c2 then
            // Same column -> shift up
            (matrix.Grid.[(r1 + 4) % 5, c1], matrix.Grid.[(r2 + 4) % 5, c2])
        else
            // Rectangle -> swap columns
            (matrix.Grid.[r1, c2], matrix.Grid.[r2, c1])

    /// Encrypts text using the Playfair cipher.
    let playfairEncrypt (key: string) (text: string) : string =
        let matrix = createMatrix key
        let pairs = preparePlaintext text
        pairs
        |> List.map (encryptPair matrix)
        |> List.collect (fun (c1, c2) -> [c1; c2])
        |> Array.ofList
        |> String

    /// Decrypts Playfair ciphertext.
    let playfairDecrypt (key: string) (ciphertext: string) : string =
        let matrix = createMatrix key
        let clean =
            ciphertext
            |> Seq.filter Char.IsLetter
            |> Seq.map Char.ToUpperInvariant
            |> Seq.toList

        let rec pairList chars =
            match chars with
            | [] -> []
            | a :: b :: rest -> (a, b) :: pairList rest
            | [single] -> [(single, 'X')]

        pairList clean
        |> List.map (decryptPair matrix)
        |> List.collect (fun (c1, c2) -> [c1; c2])
        |> Array.ofList
        |> String
