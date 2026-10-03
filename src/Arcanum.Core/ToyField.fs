namespace Arcanum.Core

open System.Numerics

/// The exact coordinate field for the enumerable toy curve.
module ToyField =
    let prime = 17I

    [<Struct>]
    type Residue = private Residue of BigInteger

    let ofBigInteger (value: BigInteger) : Residue =
        Residue (Modular.modPos value prime)

    let toBigInteger (Residue value) : BigInteger =
        value

    let zero = ofBigInteger BigInteger.Zero
    let one = ofBigInteger BigInteger.One

    let add (Residue left) (Residue right) : Residue =
        ofBigInteger (left + right)

    let subtract (Residue left) (Residue right) : Residue =
        ofBigInteger (left - right)

    let negate (Residue value) : Residue =
        ofBigInteger (-value)

    let multiply (Residue left) (Residue right) : Residue =
        ofBigInteger (left * right)

    let pow (Residue value) (exponent: BigInteger) : Residue =
        ofBigInteger (Modular.modPow value exponent prime)

    let inverse (Residue value) : Residue option =
        Modular.modInverse value prime
        |> Option.map ofBigInteger

    let residues : Residue[] =
        [| for value in 0 .. int prime - 1 -> ofBigInteger (bigint value) |]
