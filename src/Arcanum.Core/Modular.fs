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

    /// Montgomery's trick batch modular inversion: computes the modular inverse of every value in the list modulo m
    /// with one call to modInverse on the product of all values instead of one call per value.
    /// Returns Ok [] for the empty list, or Error NonInvertibleElement if any value is not invertible modulo m.
    let batchInverse (values: BigInteger list) (m: BigInteger) : Result<BigInteger list, CryptoError> =
        match values with
        | [] -> Ok []
        | _ when m <= BigInteger.One ->
            Error (NonInvertibleElement "Modulus must be greater than 1")
        | _ ->
            let arr = values |> List.map (fun v -> modPos v m) |> List.toArray
            let n = arr.Length
            let prefixes: BigInteger[] = Array.zeroCreate n
            prefixes.[0] <- arr.[0]
            for i = 1 to n - 1 do
                prefixes.[i] <- modPos (prefixes.[i - 1] * arr.[i]) m

            match modInverse prefixes.[n - 1] m with
            | None -> Error (NonInvertibleElement "At least one element is not invertible modulo m")
            | Some totalInv ->
                let invs: BigInteger[] = Array.zeroCreate n
                let mutable accInv = totalInv
                for i = n - 1 downto 1 do
                    invs.[i] <- modPos (accInv * prefixes.[i - 1]) m
                    accInv <- modPos (accInv * arr.[i]) m
                invs.[0] <- accInv
                Ok (Array.toList invs)

    /// The secp256k1 field prime 2^256 - 2^32 - 977.
    let secp256k1Prime : BigInteger = (BigInteger.One <<< 256) - (BigInteger.One <<< 32) - 977I

    /// Special-form reduction modulo the secp256k1 prime p = 2^256 - 2^32 - 977 for 0 <= x < p^2,
    /// using 2^256 = 2^32 + 977 = 4294968273 (mod p).
    /// This is a second reduction checked against modPos; no constant-time claim is made.
    let reduceSecp256k1 (xInit: BigInteger) : BigInteger =
        let p = secp256k1Prime
        if xInit < BigInteger.Zero || xInit >= p * p then
            raise (ArgumentOutOfRangeException(nameof xInit, "Input must satisfy 0 <= x < p^2"))
        let two256 = BigInteger.One <<< 256
        let mask256 = two256 - BigInteger.One
        let c = (BigInteger.One <<< 32) + 977I
        let mutable x = xInit
        while x >= two256 do
            let h = x >>> 256
            let l = x &&& mask256
            x <- h * c + l
        while x >= p do
            x <- x - p
        x

    /// The NIST P-256 field prime 2^256 - 2^224 + 2^192 + 2^96 - 1.
    let nistP256Prime : BigInteger =
        (BigInteger.One <<< 256) - (BigInteger.One <<< 224) + (BigInteger.One <<< 192) + (BigInteger.One <<< 96) - BigInteger.One

    /// Solinas reduction modulo the NIST P-256 prime p = 2^256 - 2^224 + 2^192 + 2^96 - 1 for 0 <= A < p^2
    /// per FIPS 186-4 Appendix D.2.3: B = T + 2*S1 + 2*S2 + S3 + S4 - D1 - D2 - D3 - D4 brought into 0 .. p - 1.
    /// Second reduction checked against modPos; no constant-time claim is made.
    let reduceP256 (aInit: BigInteger) : BigInteger =
        let p = nistP256Prime
        if aInit < BigInteger.Zero || aInit >= p * p then
            raise (ArgumentOutOfRangeException(nameof aInit, "Input must satisfy 0 <= A < p^2"))
        let mask32 = (BigInteger.One <<< 32) - BigInteger.One
        let a = Array.init 16 (fun i -> (aInit >>> (32 * i)) &&& mask32)
        let pack8 w7 w6 w5 w4 w3 w2 w1 w0 =
            (w7 <<< 224) + (w6 <<< 192) + (w5 <<< 160) + (w4 <<< 128) +
            (w3 <<< 96)  + (w2 <<< 64)  + (w1 <<< 32)  + w0

        let z = BigInteger.Zero
        let t  = pack8 a.[7]  a.[6]  a.[5]  a.[4]  a.[3]  a.[2]  a.[1]  a.[0]
        let s1 = pack8 a.[15] a.[14] a.[13] a.[12] a.[11] z      z      z
        let s2 = pack8 z      a.[15] a.[14] a.[13] a.[12] z      z      z
        let s3 = pack8 a.[15] a.[14] z      z      z      a.[10] a.[9]  a.[8]
        let s4 = pack8 a.[8]  a.[13] a.[15] a.[14] a.[13] a.[11] a.[10] a.[9]
        let d1 = pack8 a.[10] a.[8]  z      z      z      a.[13] a.[12] a.[11]
        let d2 = pack8 a.[11] a.[9]  z      z      a.[15] a.[14] a.[13] a.[12]
        let d3 = pack8 a.[12] z      a.[10] a.[9]  a.[8]  a.[15] a.[14] a.[13]
        let d4 = pack8 a.[13] z      a.[11] a.[10] a.[9]  z      a.[15] a.[14]

        let mutable b = t + 2I * s1 + 2I * s2 + s3 + s4 - d1 - d2 - d3 - d4
        while b < BigInteger.Zero do
            b <- b + p
        while b >= p do
            b <- b - p
        b
