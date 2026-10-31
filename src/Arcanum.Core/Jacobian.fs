namespace Arcanum.Core

open System.Numerics

module Jacobian =
    /// A Jacobian point (X : Y : Z) stands for the affine point (X / Z^2, Y / Z^3).
    /// Z = 0 stands for Infinity. (X : Y : Z) and (lambda^2 X : lambda^3 Y : lambda Z)
    /// stand for the same point for every non-zero lambda.
    type JacobianPoint = {
        X: BigInteger
        Y: BigInteger
        Z: BigInteger
    }

    /// Maps an affine point to Jacobian coordinates with Z = 1, and maps Infinity to (1 : 1 : 0).
    let ofAffine (pt: EllipticCurve.ECPoint) : JacobianPoint =
        match pt with
        | EllipticCurve.Infinity -> { X = BigInteger.One; Y = BigInteger.One; Z = BigInteger.Zero }
        | EllipticCurve.Point(x, y) -> { X = x; Y = y; Z = BigInteger.One }

    /// Converts a Jacobian point to affine coordinates modulo P using Modular.modInverse on Z.
    let toAffine (curve: EllipticCurve.Curve) (pt: JacobianPoint) : EllipticCurve.ECPoint =
        let p = curve.P
        let z = Modular.modPos pt.Z p
        if z = BigInteger.Zero then
            EllipticCurve.Infinity
        else
            match Modular.modInverse z p with
            | None -> EllipticCurve.Infinity
            | Some zInv ->
                let zInv2 = Modular.modPos (zInv * zInv) p
                let zInv3 = Modular.modPos (zInv2 * zInv) p
                let x = Modular.modPos (pt.X * zInv2) p
                let y = Modular.modPos (pt.Y * zInv3) p
                EllipticCurve.Point(x, y)
