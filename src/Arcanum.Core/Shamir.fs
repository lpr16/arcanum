namespace Arcanum.Core

/// Threshold-2 secret sharing over ToyField (F_17).
/// The polynomial is secret + coefficient * x, so any two shares determine the line and its value at x = 0.
/// F_17 has 17 elements; this is a study of the construction, not a place to keep a secret.
module Shamir =
    /// A share is a point (X, Y) on the line, with both coordinates in ToyField.
    type Share = {
        X: ToyField.Residue
        Y: ToyField.Residue
    }

    /// Evaluates secret + coefficient * x at x. A share at x = 0 is the secret itself and is refused.
    let share (secret: ToyField.Residue) (coefficient: ToyField.Residue) (x: ToyField.Residue) : Result<Share, CryptoError> =
        if x = ToyField.zero then
            Error (ParameterOutOfRange "A share at x = 0 is refused")
        else
            Ok { X = x; Y = ToyField.add secret (ToyField.multiply coefficient x) }

    /// Reconstructs the secret as the value at x = 0 of the Lagrange line through two shares:
    /// secret = (y1 * x2 - y2 * x1) * inverse(x2 - x1), using ToyField.inverse.
    let reconstruct (first: Share) (second: Share) : Result<ToyField.Residue, CryptoError> =
        if first.X = ToyField.zero || second.X = ToyField.zero then
            Error (ParameterOutOfRange "A share at x = 0 is refused")
        elif first.X = second.X then
            Error (ParameterOutOfRange "Two shares with the same x do not determine a line")
        else
            match ToyField.inverse (ToyField.subtract second.X first.X) with
            | None -> Error (NonInvertibleElement "x2 - x1 is not invertible in ToyField")
            | Some inv ->
                let numerator =
                    ToyField.subtract (ToyField.multiply first.Y second.X) (ToyField.multiply second.Y first.X)
                Ok (ToyField.multiply numerator inv)

    /// Evaluates the polynomial with the given coefficients (secret first: c0 + c1*x + c2*x^2 + ...) at x.
    /// Refuses x = 0 and an empty coefficient list. F_17 is not a place to keep a secret.
    let shareWith (coefficients: ToyField.Residue list) (x: ToyField.Residue) : Result<Share, CryptoError> =
        if List.isEmpty coefficients then
            Error (ParameterOutOfRange "Coefficient list must not be empty")
        elif x = ToyField.zero then
            Error (ParameterOutOfRange "A share at x = 0 is refused")
        else
            let y, _ =
                coefficients
                |> List.fold (fun (acc, xPow) c ->
                    let nextAcc = ToyField.add acc (ToyField.multiply c xPow)
                    let nextPow = ToyField.multiply xPow x
                    (nextAcc, nextPow)) (ToyField.zero, ToyField.one)
            Ok { X = x; Y = y }

    /// Evaluates the Lagrange interpolation of the shares at x = 0 using ToyField.inverse.
    /// Refuses an empty list, a share at x = 0, and two shares with the same x.
    let reconstructMany (shares: Share list) : Result<ToyField.Residue, CryptoError> =
        if List.isEmpty shares then
            Error (ParameterOutOfRange "Share list must not be empty")
        elif shares |> List.exists (fun s -> s.X = ToyField.zero) then
            Error (ParameterOutOfRange "A share at x = 0 is refused")
        else
            let xs = shares |> List.map (fun s -> s.X)
            if (List.distinct xs).Length <> xs.Length then
                Error (ParameterOutOfRange "Two shares with the same x are refused")
            else
                let arr = List.toArray shares
                let k = arr.Length
                let rec sumTerms i acc =
                    if i >= k then
                        Ok acc
                    else
                        let rec prodLoop j num den =
                            if j >= k then
                                (num, den)
                            elif i = j then
                                prodLoop (j + 1) num den
                            else
                                let nextNum = ToyField.multiply num (ToyField.negate arr.[j].X)
                                let nextDen = ToyField.multiply den (ToyField.subtract arr.[i].X arr.[j].X)
                                prodLoop (j + 1) nextNum nextDen
                        let num, den = prodLoop 0 ToyField.one ToyField.one
                        match ToyField.inverse den with
                        | None -> Error (NonInvertibleElement "Lagrange denominator is not invertible in ToyField")
                        | Some denInv ->
                            let basis = ToyField.multiply num denInv
                            let term = ToyField.multiply arr.[i].Y basis
                            sumTerms (i + 1) (ToyField.add acc term)
                sumTerms 0 ToyField.zero
