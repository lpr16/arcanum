namespace Arcanum.Core

open System.Numerics

module DiscreteLog =
    /// Discrete logarithm by brute-force repeated addition.
    /// Adds basePt to Infinity step by step until it meets target, returning the step count,
    /// or returns None when it comes back to Infinity first.
    /// This function is for the small toy curves (toy, toy2, and toy3) only; do not call it on secp256k1 or nistP256.
    let bruteForce (curve: EllipticCurve.Curve) (basePt: EllipticCurve.ECPoint) (target: EllipticCurve.ECPoint) : BigInteger option =
        if target = EllipticCurve.Infinity then
            Some BigInteger.Zero
        elif basePt = EllipticCurve.Infinity then
            None
        else
            let rec loop (k: BigInteger) (current: EllipticCurve.ECPoint) =
                if current = target then
                    Some k
                elif current = EllipticCurve.Infinity then
                    None
                else
                    loop (k + BigInteger.One) (EllipticCurve.add curve current basePt)
            loop BigInteger.One basePt
