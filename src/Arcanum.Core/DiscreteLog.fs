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

    /// Least non-negative integer m with m * m >= n, computed with integer arithmetic only.
    let ceilSqrt (n: BigInteger) : BigInteger =
        let rec loop m =
            if m * m >= n then m
            else loop (m + BigInteger.One)
        loop BigInteger.Zero

    /// Shanks baby-step giant-step discrete logarithm on the small toy curves (toy, toy2, and toy3).
    /// Computes m as the least integer with m * m >= order using integer arithmetic, stores j * basePt
    /// for j in 0 .. m - 1, and subtracts m * basePt from target up to m times.
    let babyStepGiantStep
        (curve: EllipticCurve.Curve)
        (basePt: EllipticCurve.ECPoint)
        (target: EllipticCurve.ECPoint)
        (order: BigInteger)
        : BigInteger option =
        if order <= BigInteger.Zero then
            None
        else
            let m = ceilSqrt order
            let mutable babySteps = []
            let mutable cur = EllipticCurve.Infinity
            let mutable j = BigInteger.Zero
            while j < m do
                if not (babySteps |> List.exists (fun (pt, _) -> pt = cur)) then
                    babySteps <- (cur, j) :: babySteps
                cur <- EllipticCurve.add curve cur basePt
                j <- j + BigInteger.One

            let negMBase = EllipticCurve.negate curve cur
            let rec giantLoop (i: BigInteger) (gamma: EllipticCurve.ECPoint) =
                if i >= m then
                    None
                else
                    match babySteps |> List.tryFind (fun (pt, _) -> pt = gamma) with
                    | Some (_, foundJ) ->
                        let candidate = i * m + foundJ
                        if candidate < order then
                            Some candidate
                        else
                            giantLoop (i + BigInteger.One) (EllipticCurve.add curve gamma negMBase)
                    | None ->
                        giantLoop (i + BigInteger.One) (EllipticCurve.add curve gamma negMBase)
            giantLoop BigInteger.Zero target
