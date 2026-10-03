namespace Arcanum.Classical

open System

/// Historical cipher-analysis techniques for study and demonstration.
/// This module is not Arcanum's mathematical core.
module Cryptanalysis =
    /// Standard English letter probabilities (A to Z).
    let englishFrequencies : float[] = [|
        0.08167; 0.01492; 0.02782; 0.04253; 0.12702; 0.02228; 0.02015; // A-G
        0.06094; 0.06966; 0.00153; 0.00772; 0.04025; 0.02406; 0.06749; // H-N
        0.07507; 0.01929; 0.00095; 0.05987; 0.06327; 0.09056; 0.02758; // O-U
        0.00978; 0.02360; 0.00150; 0.01974; 0.00074                    // V-Z
    |]

    /// Counts letter occurrences (A-Z) in a given string.
    let countFrequencies (text: string) : int[] =
        let counts = Array.zeroCreate 26
        for c in text do
            if c >= 'A' && c <= 'Z' then
                let idx = int c - int 'A'
                counts.[idx] <- counts.[idx] + 1
            elif c >= 'a' && c <= 'z' then
                let idx = int c - int 'a'
                counts.[idx] <- counts.[idx] + 1
        counts

    /// Calculates Chi-Squared (χ²) statistic against standard English letter frequencies.
    /// Lower score represents a closer fit to natural English.
    let chiSquared (text: string) : float =
        let counts = countFrequencies text
        let totalLetters = counts |> Array.sum
        if totalLetters = 0 then Double.PositiveInfinity
        else
            let n = float totalLetters
            let mutable chiSq = 0.0
            for i in 0 .. 25 do
                let expected = n * englishFrequencies.[i]
                let observed = float counts.[i]
                let diff = observed - expected
                chiSq <- chiSq + (diff * diff) / expected
            chiSq

    /// Computes the Index of Coincidence (IoC / Friedman test).
    /// Natural English text typically scores ~0.0667.
    /// Polyalphabetic or random text typically scores ~0.0385.
    let indexOfCoincidence (text: string) : float =
        let counts = countFrequencies text
        let n = counts |> Array.sum
        if n <= 1 then 0.0
        else
            let sumNumerator =
                counts
                |> Array.sumBy (fun c -> int64 c * int64 (c - 1))
            float sumNumerator / float (int64 n * int64 (n - 1))

    /// Automatically breaks Caesar cipher by finding the shift that minimizes the Chi-Squared statistic.
    let breakCaesar (ciphertext: string) : int * string * float =
        let shifts =
            [0 .. 25]
            |> List.map (fun shift ->
                let candidate = Substitution.caesarDecrypt shift ciphertext
                let score = chiSquared candidate
                (shift, candidate, score)
            )
        shifts |> List.minBy (fun (_, _, score) -> score)

    /// Estimates the most probable Vigenère key length by testing candidate lengths from 1 to maxKeyLen
    /// and finding which length maximizes the average Index of Coincidence of its cosets.
    let estimateVigenereKeyLength (ciphertext: string) (maxKeyLen: int) : (int * float) list =
        let letters =
            ciphertext
            |> Seq.filter Char.IsLetter
            |> Seq.map Char.ToUpperInvariant
            |> Seq.toArray

        [1 .. maxKeyLen]
        |> List.map (fun klen ->
            let slices = Array.init klen (fun i ->
                letters
                |> Array.mapi (fun idx c -> if idx % klen = i then Some c else None)
                |> Array.choose id
                |> String
            )
            let avgIoC =
                slices
                |> Array.map indexOfCoincidence
                |> Array.average
            (klen, avgIoC)
        )
        |> List.sortByDescending snd

    /// Automatically cracks a Vigenère cipher given a known or estimated key length.
    let breakVigenereWithKeyLength (klen: int) (ciphertext: string) : string * string =
        let letters =
            ciphertext
            |> Seq.filter Char.IsLetter
            |> Seq.map Char.ToUpperInvariant
            |> Seq.toArray

        let keyChars =
            Array.init klen (fun i ->
                let slice =
                    letters
                    |> Array.mapi (fun idx c -> if idx % klen = i then Some c else None)
                    |> Array.choose id
                    |> String
                let (bestShift, _, _) = breakCaesar slice
                char (int 'A' + bestShift)
            )
        let key = String(keyChars)
        let decrypted =
            match Polyalphabetic.vigenereDecrypt key ciphertext with
            | Ok text -> text
            | Error _ -> ""
        (key, decrypted)
