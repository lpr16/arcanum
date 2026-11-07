namespace Arcanum.Classical

open System
open System.Numerics
open Arcanum.Core

/// Historical 2-by-2 Hill letter cipher modulo 26. Does not replace the elliptic curve.
module Hill =

    let private mod26 (n: int) : int =
        int (Modular.modPos (BigInteger n) 26I)

    let private validateText (text: string) : Result<int[], CryptoError> =
        if isNull text || text.Length % 2 <> 0 then
            Error (ParameterOutOfRange "Hill cipher text must have an even number of letters A-Z")
        elif text |> Seq.exists (fun ch -> ch < 'A' || ch > 'Z') then
            Error (ParameterOutOfRange "Hill cipher text must contain only uppercase letters A-Z")
        else
            Ok [| for ch in text -> int ch - int 'A' |]

    let private applyMatrix (m00: int, m01: int, m10: int, m11: int) (nums: int[]) : string =
        let out = Array.zeroCreate<char> nums.Length
        for i in 0 .. 2 .. nums.Length - 1 do
            let p1 = nums.[i]
            let p2 = nums.[i + 1]
            let c1 = mod26 (m00 * p1 + m01 * p2)
            let c2 = mod26 (m10 * p1 + m11 * p2)
            out.[i] <- char (int 'A' + c1)
            out.[i + 1] <- char (int 'A' + c2)
        String(out)

    /// Encrypts uppercase letter pairs with the 2x2 matrix (a, b, c, d) modulo 26.
    /// Refuses a key whose determinant is not invertible modulo 26, and text that is not an even number of letters A to Z.
    let encrypt ((a, b, c, d): int * int * int * int) (text: string) : Result<string, CryptoError> =
        let det = BigInteger (a * d - b * c)
        match Modular.modInverse det 26I with
        | None -> Error (NonInvertibleElement "Hill key matrix determinant is not invertible modulo 26")
        | Some _ ->
            match validateText text with
            | Error err -> Error err
            | Ok nums -> Ok (applyMatrix (mod26 a, mod26 b, mod26 c, mod26 d) nums)

    /// Decrypts uppercase letter pairs using the inverse matrix det^(-1) * (d, -b, -c, a) modulo 26.
    /// Refuses a key whose determinant is not invertible modulo 26, and text that is not an even number of letters A to Z.
    let decrypt ((a, b, c, d): int * int * int * int) (text: string) : Result<string, CryptoError> =
        let det = BigInteger (a * d - b * c)
        match Modular.modInverse det 26I with
        | None -> Error (NonInvertibleElement "Hill key matrix determinant is not invertible modulo 26")
        | Some detInvBig ->
            match validateText text with
            | Error err -> Error err
            | Ok nums ->
                let detInv = int detInvBig
                let invMatrix =
                    (mod26 (detInv * d),
                     mod26 (detInv * -b),
                     mod26 (detInv * -c),
                     mod26 (detInv * a))
                Ok (applyMatrix invMatrix nums)
