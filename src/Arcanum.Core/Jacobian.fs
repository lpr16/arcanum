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

    /// Point doubling in Jacobian coordinates for y^2 = x^3 + a*x + b modulo P.
    /// If Z1 = 0 or Y1 = 0, returns (1 : 1 : 0). Otherwise:
    /// S = 4 * X1 * Y1^2, M = 3 * X1^2 + a * Z1^4,
    /// X3 = M^2 - 2 * S, Y3 = M * (S - X3) - 8 * Y1^4, Z3 = 2 * Y1 * Z1 (all modulo P).
    /// Second implementation of the group law checked against EllipticCurve.add; no constant-time claim is made.
    let double (curve: EllipticCurve.Curve) (pt: JacobianPoint) : JacobianPoint =
        let p = curve.P
        let x1 = Modular.modPos pt.X p
        let y1 = Modular.modPos pt.Y p
        let z1 = Modular.modPos pt.Z p
        if z1 = BigInteger.Zero || y1 = BigInteger.Zero then
            { X = BigInteger.One; Y = BigInteger.One; Z = BigInteger.Zero }
        else
            let y1Sq = Modular.modPos (y1 * y1) p
            let y1Fourth = Modular.modPos (y1Sq * y1Sq) p
            let z1Sq = Modular.modPos (z1 * z1) p
            let z1Fourth = Modular.modPos (z1Sq * z1Sq) p
            let s = Modular.modPos (4I * x1 * y1Sq) p
            let m = Modular.modPos (3I * x1 * x1 + curve.A * z1Fourth) p
            let x3 = Modular.modPos (m * m - 2I * s) p
            let y3 = Modular.modPos (m * (s - x3) - 8I * y1Fourth) p
            let z3 = Modular.modPos (2I * y1 * z1) p
            { X = x3; Y = y3; Z = z3 }

    /// Point addition in Jacobian coordinates for y^2 = x^3 + a*x + b modulo P.
    /// If either Z is 0, returns the other point. Otherwise:
    /// U1 = X1 * Z2^2, U2 = X2 * Z1^2, S1 = Y1 * Z2^3, S2 = Y2 * Z1^3.
    /// If U1 = U2: when S1 = S2 returns double p1, otherwise returns (1 : 1 : 0).
    /// Otherwise H = U2 - U1, R = S2 - S1,
    /// X3 = R^2 - H^3 - 2 * U1 * H^2, Y3 = R * (U1 * H^2 - X3) - S1 * H^3, Z3 = H * Z1 * Z2 (all modulo P).
    /// Second implementation of the group law checked against EllipticCurve.add; no constant-time claim is made.
    let add (curve: EllipticCurve.Curve) (p1: JacobianPoint) (p2: JacobianPoint) : JacobianPoint =
        let p = curve.P
        let z1 = Modular.modPos p1.Z p
        let z2 = Modular.modPos p2.Z p
        if z1 = BigInteger.Zero then p2
        elif z2 = BigInteger.Zero then p1
        else
            let z1Sq = Modular.modPos (z1 * z1) p
            let z1Cube = Modular.modPos (z1Sq * z1) p
            let z2Sq = Modular.modPos (z2 * z2) p
            let z2Cube = Modular.modPos (z2Sq * z2) p
            let u1 = Modular.modPos (p1.X * z2Sq) p
            let u2 = Modular.modPos (p2.X * z1Sq) p
            let s1 = Modular.modPos (p1.Y * z2Cube) p
            let s2 = Modular.modPos (p2.Y * z1Cube) p
            if u1 = u2 then
                if s1 = s2 then
                    double curve p1
                else
                    { X = BigInteger.One; Y = BigInteger.One; Z = BigInteger.Zero }
            else
                let h = Modular.modPos (u2 - u1) p
                let r = Modular.modPos (s2 - s1) p
                let hSq = Modular.modPos (h * h) p
                let hCube = Modular.modPos (hSq * h) p
                let u1hSq = Modular.modPos (u1 * hSq) p
                let x3 = Modular.modPos (r * r - hCube - 2I * u1hSq) p
                let y3 = Modular.modPos (r * (u1hSq - x3) - s1 * hCube) p
                let z3 = Modular.modPos (h * z1 * z2) p
                { X = x3; Y = y3; Z = z3 }

    /// Mixed Jacobian-affine point addition for y^2 = x^3 + a*x + b modulo P, where p2 has Z2 = 1.
    /// If p2 is Infinity, returns p1. If Z1 = 0, returns ofAffine p2. Otherwise:
    /// U1 = X1, S1 = Y1, U2 = x2 * Z1^2, S2 = y2 * Z1^3.
    /// If U1 = U2: when S1 = S2 returns double p1, otherwise returns (1 : 1 : 0).
    /// Otherwise H = U2 - U1, R = S2 - S1,
    /// X3 = R^2 - H^3 - 2 * U1 * H^2, Y3 = R * (U1 * H^2 - X3) - S1 * H^3, Z3 = H * Z1 (all modulo P).
    /// No constant-time claim is made.
    let addMixed (curve: EllipticCurve.Curve) (p1: JacobianPoint) (p2: EllipticCurve.ECPoint) : JacobianPoint =
        match p2 with
        | EllipticCurve.Infinity -> p1
        | EllipticCurve.Point(x2, y2) ->
            let p = curve.P
            let z1 = Modular.modPos p1.Z p
            if z1 = BigInteger.Zero then
                ofAffine p2
            else
                let z1Sq = Modular.modPos (z1 * z1) p
                let z1Cube = Modular.modPos (z1Sq * z1) p
                let u1 = Modular.modPos p1.X p
                let u2 = Modular.modPos (x2 * z1Sq) p
                let s1 = Modular.modPos p1.Y p
                let s2 = Modular.modPos (y2 * z1Cube) p
                if u1 = u2 then
                    if s1 = s2 then
                        double curve p1
                    else
                        { X = BigInteger.One; Y = BigInteger.One; Z = BigInteger.Zero }
                else
                    let h = Modular.modPos (u2 - u1) p
                    let r = Modular.modPos (s2 - s1) p
                    let hSq = Modular.modPos (h * h) p
                    let hCube = Modular.modPos (hSq * h) p
                    let u1hSq = Modular.modPos (u1 * hSq) p
                    let x3 = Modular.modPos (r * r - hCube - 2I * u1hSq) p
                    let y3 = Modular.modPos (r * (u1hSq - x3) - s1 * hCube) p
                    let z3 = Modular.modPos (h * z1) p
                    { X = x3; Y = y3; Z = z3 }

    /// Scalar multiplication in Jacobian coordinates: reduces k modulo N, walks the bits of k
    /// from the top with Jacobian.double and Jacobian.addMixed of the affine input, and converts
    /// to affine once at the end with toAffine. No constant-time claim is made.
    let scalarMultiply (curve: EllipticCurve.Curve) (k: BigInteger) (pt: EllipticCurve.ECPoint) : EllipticCurve.ECPoint =
        let s = Modular.modPos k curve.N
        if s = BigInteger.Zero || pt = EllipticCurve.Infinity then
            EllipticCurve.Infinity
        else
            let rec bits acc n =
                if n = BigInteger.Zero then acc
                else bits (int (n &&& BigInteger.One) :: acc) (n >>> 1)
            let mutable acc = ofAffine EllipticCurve.Infinity
            for b in bits [] s do
                acc <- double curve acc
                if b = 1 then
                    acc <- addMixed curve acc pt
            toAffine curve acc

    /// Point doubling in Jacobian coordinates when a = -3 modulo P, using
    /// M = 3 * (X1 - Z1^2) * (X1 + Z1^2) modulo P. Refuses any curve with modPos (A + 3) P <> 0.
    /// No constant-time claim is made.
    let doubleAMinus3 (curve: EllipticCurve.Curve) (pt: JacobianPoint) : Result<JacobianPoint, CryptoError> =
        let p = curve.P
        if Modular.modPos (curve.A + 3I) p <> BigInteger.Zero then
            Error (ParameterOutOfRange "doubleAMinus3 requires a = -3 modulo P.")
        else
            let x1 = Modular.modPos pt.X p
            let y1 = Modular.modPos pt.Y p
            let z1 = Modular.modPos pt.Z p
            if z1 = BigInteger.Zero || y1 = BigInteger.Zero then
                Ok { X = BigInteger.One; Y = BigInteger.One; Z = BigInteger.Zero }
            else
                let y1Sq = Modular.modPos (y1 * y1) p
                let y1Fourth = Modular.modPos (y1Sq * y1Sq) p
                let z1Sq = Modular.modPos (z1 * z1) p
                let s = Modular.modPos (4I * x1 * y1Sq) p
                let m = Modular.modPos (3I * (x1 - z1Sq) * (x1 + z1Sq)) p
                let x3 = Modular.modPos (m * m - 2I * s) p
                let y3 = Modular.modPos (m * (s - x3) - 8I * y1Fourth) p
                let z3 = Modular.modPos (2I * y1 * z1) p
                Ok { X = x3; Y = y3; Z = z3 }
