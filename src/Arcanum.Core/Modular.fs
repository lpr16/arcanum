namespace Arcanum.Core

open System
open System.Numerics

module Modular =
    /// Always returns a non-negative remainder in the range [0, m - 1].
    let inline modPos (a: BigInteger) (m: BigInteger) : BigInteger =
        let r = a % m
        if r < BigInteger.Zero then r + m else r

    /// Extended Euclidean Algorithm.
    /// Returns (gcd, x, y) such that a * x + b * y = gcd(a, b).
    let rec xgcd (a: BigInteger) (b: BigInteger) : BigInteger * BigInteger * BigInteger =
        if b = BigInteger.Zero then
            (a, BigInteger.One, BigInteger.Zero)
        else
            let q = a / b
            let r = a % b
            let (g, x1, y1) = xgcd b r
            (g, y1, x1 - q * y1)

    /// Computes the modular multiplicative inverse of `a` modulo `m`.
    /// Returns `Some inv` such that (a * inv) % m = 1, or `None` if gcd(a, m) <> 1.
    let modInverse (a: BigInteger) (m: BigInteger) : BigInteger option =
        if m <= BigInteger.One then None
        else
            let (g, x, _) = xgcd (modPos a m) m
            if g = BigInteger.One then
                Some (modPos x m)
            else
                None

    /// Fast modular exponentiation (baseVal^exponent mod modulus).
    /// A non-negative exponent delegates to BigInteger.ModPow.
    let modPow (baseVal: BigInteger) (exponent: BigInteger) (modulus: BigInteger) : BigInteger =
        if modulus <= BigInteger.Zero then
            raise (ArgumentOutOfRangeException(nameof modulus, "Modulus must be positive"))
        if exponent < BigInteger.Zero then
            match modInverse baseVal modulus with
            | Some inv -> BigInteger.ModPow(inv, -exponent, modulus)
            | None -> raise (InvalidOperationException("Modular inverse does not exist for negative exponent"))
        else
            BigInteger.ModPow(modPos baseVal modulus, exponent, modulus)

    /// Solves a system of simultaneous congruences using the Chinese Remainder Theorem:
    /// x = a_i (mod m_i) where all m_i are pairwise coprime.
    let crt (congruences: (BigInteger * BigInteger) list) : BigInteger option =
        match congruences with
        | [] -> None
        | _ ->
            let totalM = congruences |> List.fold (fun acc (_, m) -> acc * m) BigInteger.One
            let rec loop items acc =
                match items with
                | [] -> Some (modPos acc totalM)
                | (a, m) :: rest ->
                    let mPrime = totalM / m
                    match modInverse mPrime m with
                    | None -> None // Moduli were not coprime!
                    | Some inv ->
                        let term = modPos (a * mPrime * inv) totalM
                        loop rest (acc + term)
            loop congruences BigInteger.Zero

    /// Computes the Jacobi symbol (a/n) for an integer a and odd positive integer n.
    /// When n is prime, this matches the Legendre symbol.
    let jacobi (aInit: BigInteger) (nInit: BigInteger) : int =
        if nInit <= BigInteger.Zero || nInit % 2I = BigInteger.Zero then
            raise (ArgumentException("Jacobi symbol requires an odd positive integer n"))
        let mutable a = modPos aInit nInit
        let mutable n = nInit
        let mutable t = 1

        while a <> BigInteger.Zero do
            while a % 2I = BigInteger.Zero do
                a <- a / 2I
                let nMod8 = int (n % 8I)
                if nMod8 = 3 || nMod8 = 5 then
                    t <- -t
            let temp = a
            a <- n
            n <- temp
            if (a % 4I = 3I) && (n % 4I = 3I) then
                t <- -t
            a <- modPos a n

        if n = BigInteger.One then t else 0

    /// Tonelli-Shanks algorithm to find a quadratic residue square root:
    /// Solves r^2 = n (mod p) for odd prime p.
    /// Returns Some r where 0 <= r < p, or None if no square root exists.
    let modSqrt (nVal: BigInteger) (p: BigInteger) : BigInteger option =
        let n = modPos nVal p
        if n = BigInteger.Zero then Some BigInteger.Zero
        elif p = 2I then Some n
        elif jacobi n p <> 1 then None // Not a quadratic residue
        elif p % 4I = 3I then
            // Fast path for p = 3 mod 4: r = n^((p + 1) / 4) mod p
            Some (modPow n ((p + 1I) / 4I) p)
        else
            // General Tonelli-Shanks algorithm
            // Factor p - 1 as Q * 2^S with Q odd
            let mutable q = p - 1I
            let mutable s = 0
            while q % 2I = BigInteger.Zero do
                q <- q / 2I
                s <- s + 1

            // Find a quadratic non-residue z modulo p
            let mutable z = 2I
            while jacobi z p <> -1 do
                z <- z + 1I

            let mutable m = s
            let mutable c = modPow z q p
            let mutable t = modPow n q p
            let mutable r = modPow n ((q + 1I) / 2I) p

            let mutable found = false
            let mutable result = None

            while not found do
                if t = BigInteger.Zero then
                    result <- Some BigInteger.Zero
                    found <- true
                elif t = BigInteger.One then
                    result <- Some r
                    found <- true
                else
                    // Find lowest i such that t^(2^i) = 1 mod p
                    let mutable i = 0
                    let mutable temp = t
                    let mutable foundI = false
                    while i < m && not foundI do
                        if temp = BigInteger.One then
                            foundI <- true
                        else
                            i <- i + 1
                            temp <- modPow temp 2I p

                    if i = m then
                        found <- true // No solution
                    else
                        let bExponent = BigInteger.Pow(2I, m - i - 1)
                        let b = modPow c bExponent p
                        m <- i
                        c <- modPow b 2I p
                        t <- modPos (t * c) p
                        r <- modPos (r * b) p

            result
