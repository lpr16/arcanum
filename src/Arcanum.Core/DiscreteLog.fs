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

    /// Deterministic Pollard's rho discrete logarithm on the small toy curves (toy, toy2, and toy3) only.
    /// Requires prime order n of basePt for modular division. Rho needs on the order of sqrt n steps,
    /// which is why N near 2^256 is out of reach; no such run is made here.
    let pollardRho
        (curve: EllipticCurve.Curve)
        (basePt: EllipticCurve.ECPoint)
        (target: EllipticCurve.ECPoint)
        (n: BigInteger)
        : BigInteger option =
        if target = EllipticCurve.Infinity then
            Some BigInteger.Zero
        elif n <= BigInteger.One || basePt = EllipticCurve.Infinity then
            None
        else
            let step (xPt: EllipticCurve.ECPoint, a: BigInteger, b: BigInteger) =
                let cls =
                    match xPt with
                    | EllipticCurve.Infinity -> 0
                    | EllipticCurve.Point(x, _) -> int (Modular.modPos x 3I)
                match cls with
                | 0 ->
                    (EllipticCurve.add curve xPt basePt, Modular.modPos (a + BigInteger.One) n, b)
                | 1 ->
                    (EllipticCurve.double curve xPt, Modular.modPos (2I * a) n, Modular.modPos (2I * b) n)
                | _ ->
                    (EllipticCurve.add curve xPt target, a, Modular.modPos (b + BigInteger.One) n)

            let rec attemptLoop (t: BigInteger) =
                if t >= n then
                    None
                else
                    let startPt = EllipticCurve.add curve (EllipticCurve.multiplyUnreduced curve t basePt) target
                    let startState = (startPt, Modular.modPos t n, BigInteger.One)
                    let rec floyd tortoise hare =
                        let (xt, at, bt) = step tortoise
                        let (xh, ah, bh) = step (step hare)
                        if xt = xh then
                            (at, bt, ah, bh)
                        else
                            floyd (xt, at, bt) (xh, ah, bh)
                    let (at, bt, ah, bh) = floyd startState startState
                    let bDiff = Modular.modPos (bh - bt) n
                    if bDiff = BigInteger.Zero then
                        attemptLoop (t + BigInteger.One)
                    else
                        match Modular.modInverse bDiff n with
                        | None -> attemptLoop (t + BigInteger.One)
                        | Some bDiffInv ->
                            let k = Modular.modPos ((at - ah) * bDiffInv) n
                            if EllipticCurve.multiplyUnreduced curve k basePt = target then
                                Some k
                            else
                                attemptLoop (t + BigInteger.One)
            attemptLoop BigInteger.Zero

    /// Pollard's kangaroo discrete logarithm for k in lo .. hi on the small toy curves (toy, toy2, and toy3) only.
    /// Uses jump sizes [1; 2; 4] indexed by x mod 3 (with Infinity at index 0), sets the trap after 2 tame jumps
    /// from hi * basePt, and walks the wild kangaroo from target while dw <= (hi - lo) + dt.
    /// This walk was checked to meet the trap for every tested case on toy3.
    let kangaroo
        (curve: EllipticCurve.Curve)
        (basePt: EllipticCurve.ECPoint)
        (target: EllipticCurve.ECPoint)
        (lo: BigInteger)
        (hi: BigInteger)
        : BigInteger option =
        if hi < lo then
            None
        else
            let jumps = [| 1I; 2I; 4I |]
            let p1 = basePt
            let p2 = EllipticCurve.double curve basePt
            let p4 = EllipticCurve.double curve p2
            let jumpPts = [| p1; p2; p4 |]
            let step (pt: EllipticCurve.ECPoint, dist: BigInteger) =
                let idx =
                    match pt with
                    | EllipticCurve.Infinity -> 0
                    | EllipticCurve.Point(x, _) -> int (Modular.modPos x 3I)
                (EllipticCurve.add curve pt jumpPts.[idx], dist + jumps.[idx])

            let tameStart = (EllipticCurve.scalarMultiply curve hi basePt, BigInteger.Zero)
            let (trapPt, dt) = tameStart |> step |> step
            let bound = (hi - lo) + dt
            let rec wildLoop (wPt: EllipticCurve.ECPoint, dw: BigInteger) =
                if dw > bound then
                    None
                elif wPt = trapPt then
                    Some (Modular.modPos (hi + dt - dw) curve.N)
                else
                    wildLoop (step (wPt, dw))
            wildLoop (target, BigInteger.Zero)
