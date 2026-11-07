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

    /// Adds a point to itself step by step until it reaches Infinity (returning 1 for Infinity).
    /// This function is for the small toy curves (toy, toy2, and toy3) only.
    let order (curve: EllipticCurve.Curve) (pt: EllipticCurve.ECPoint) : BigInteger =
        if pt = EllipticCurve.Infinity then
            BigInteger.One
        else
            let rec loop (k: BigInteger) (cur: EllipticCurve.ECPoint) =
                if cur = EllipticCurve.Infinity then k
                else loop (k + BigInteger.One) (EllipticCurve.add curve cur pt)
            loop BigInteger.One pt

    /// Pohlig-Hellman discrete logarithm on the small toy curves (toy, toy2, and toy3).
    /// Takes the factorization of the order of basePt as (prime, exponent) pairs; no factoring routine is included.
    /// Verifies that the product of factors equals the order of basePt found by repeated addition, recovers the
    /// base-q digits inside the subgroup of order q using bruteForce, and combines prime-power residues with Modular.crt.
    /// The subgroup work is bounded by the largest prime factor (3 on toy, 19 on toy3), which is why ECDSA requires a group of large prime order.
    let pohligHellman
        (curve: EllipticCurve.Curve)
        (basePt: EllipticCurve.ECPoint)
        (target: EllipticCurve.ECPoint)
        (factors: (BigInteger * int) list)
        : Result<BigInteger, CryptoError> =
        if List.isEmpty factors || factors |> List.exists (fun (q, e) -> q < 2I || e < 1) then
            Error (ParameterOutOfRange "Factorization must contain valid (prime, exponent) pairs")
        else
            let n =
                factors
                |> List.fold (fun acc (q, e) -> acc * BigInteger.Pow(q, e)) BigInteger.One
            let actualOrder = order curve basePt
            if n <> actualOrder then
                Error (ParameterOutOfRange "Factorization product does not equal the order of the base point")
            else
                let solvePrimePower (q: BigInteger, e: int) : Result<BigInteger * BigInteger, CryptoError> =
                    let qe = BigInteger.Pow(q, e)
                    let gSub = EllipticCurve.multiplyUnreduced curve (n / q) basePt
                    let rec digitLoop (j: int) (xPartial: BigInteger) =
                        if j >= e then
                            Ok (xPartial, qe)
                        else
                            let qPowJ = BigInteger.Pow(q, j)
                            let qPowJPlus1 = qPowJ * q
                            let knownPart = EllipticCurve.multiplyUnreduced curve xPartial basePt
                            let diff = EllipticCurve.add curve target (EllipticCurve.negate curve knownPart)
                            let hj = EllipticCurve.multiplyUnreduced curve (n / qPowJPlus1) diff
                            match bruteForce curve gSub hj with
                            | None ->
                                Error (ParameterOutOfRange "Target is not in the subgroup generated by basePt")
                            | Some dj ->
                                digitLoop (j + 1) (xPartial + dj * qPowJ)
                    digitLoop 0 BigInteger.Zero

                let rec collectCongruences remaining acc =
                    match remaining with
                    | [] ->
                        match Modular.crt (List.rev acc) with
                        | Some k -> Ok k
                        | None -> Error (ParameterOutOfRange "Prime-power factors are not pairwise coprime")
                    | factor :: rest ->
                        match solvePrimePower factor with
                        | Ok cong -> collectCongruences rest (cong :: acc)
                        | Error err -> Error err

                collectCongruences factors []
