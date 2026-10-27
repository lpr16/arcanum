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
