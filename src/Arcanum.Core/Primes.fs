namespace Arcanum.Core

open System
open System.Numerics
open System.Security.Cryptography

module Primes =
    let private smallPrimes = [|
        2I; 3I; 5I; 7I; 11I; 13I; 17I; 19I; 23I; 29I; 31I; 37I; 41I; 43I; 47I; 53I; 59I; 61I; 67I; 71I; 73I; 79I; 83I; 89I; 97I
    |]

    /// Quick trial division against small primes up to 100.
    let private trialDivision (n: BigInteger) : bool option =
        if n < 2I then Some false
        else
            let mutable isDivisible = false
            let mutable isExact = false
            for p in smallPrimes do
                if n = p then isExact <- true
                elif n % p = BigInteger.Zero then isDivisible <- true

            if isExact then Some true
            elif isDivisible then Some false
            else None

    /// Generates a random BigInteger in the inclusive range [minVal, maxVal] (both ends included)
    /// using the CLR RandomNumberGenerator.Fill call.
    let randomBigInteger (minVal: BigInteger) (maxVal: BigInteger) : BigInteger =
        if minVal > maxVal then
            raise (ArgumentException("minVal must be less than or equal to maxVal"))
        elif minVal = maxVal then
            minVal
        else
            let range = maxVal - minVal
            let bitLen = int (range.GetBitLength())
            let bytesNeeded = (bitLen + 7) / 8
            let topByteBits = bitLen % 8
            let buffer: byte[] = Array.zeroCreate bytesNeeded

            let rec loop () =
                RandomNumberGenerator.Fill(Span<byte>(buffer))
                if topByteBits > 0 then
                    let mask = (1uy <<< topByteBits) - 1uy
                    buffer.[bytesNeeded - 1] <- buffer.[bytesNeeded - 1] &&& mask
                let candidate = BigInteger(ReadOnlySpan<byte>(buffer), isUnsigned = true, isBigEndian = false)
                if candidate <= range then
                    minVal + candidate
                else
                    loop ()
            loop ()

    /// Miller-Rabin probabilistic primality test with specified number of rounds.
    /// Draws random bases using RandomNumberGenerator.Fill. This is a probabilistic test, not a deterministic primality proof.
    let isProbablePrime (n: BigInteger) (rounds: int) : bool =
        match trialDivision n with
        | Some result -> result
        | None ->
            // Factor n - 1 = d * 2^s with d odd
            let mutable d = n - BigInteger.One
            let mutable s = 0
            while d % 2I = BigInteger.Zero do
                d <- d / 2I
                s <- s + 1

            let rec testRounds round =
                if round >= rounds then true
                else
                    let a = randomBigInteger 2I (n - 2I)
                    let mutable x = Modular.modPow a d n

                    if x = BigInteger.One || x = n - BigInteger.One then
                        testRounds (round + 1)
                    else
                        let mutable continueRound = false
                        let mutable r = 1
                        while r < s && not continueRound do
                            x <- Modular.modPow x 2I n
                            if x = n - BigInteger.One then
                                continueRound <- true
                            r <- r + 1

                        if continueRound then
                            testRounds (round + 1)
                        else
                            false

            testRounds 0

    /// Generates a random probable prime of the specified bit length.
    let generatePrime (bitLength: int) (rounds: int) : BigInteger =
        if bitLength < 2 then
            raise (ArgumentOutOfRangeException(nameof bitLength, "Bit length must be at least 2"))

        let bytesNeeded = (bitLength + 7) / 8
        let buffer: byte[] = Array.zeroCreate (bytesNeeded + 1)

        let rec findPrime () =
            RandomNumberGenerator.Fill(Span<byte>(buffer))
            buffer.[buffer.Length - 1] <- 0uy // positive sign
            // Mask unused top bits if bitLength is not multiple of 8
            let topByteBits = bitLength % 8
            let topByteIndex = bytesNeeded - 1
            if topByteBits > 0 then
                let mask = (1uy <<< topByteBits) - 1uy
                buffer.[topByteIndex] <- buffer.[topByteIndex] &&& mask
            // Set highest bit to ensure exact bit length
            let highBit = if topByteBits = 0 then 7 else topByteBits - 1
            buffer.[topByteIndex] <- buffer.[topByteIndex] ||| (1uy <<< highBit)
            // Set lowest bit to ensure odd number
            buffer.[0] <- buffer.[0] ||| 1uy

            let candidate = BigInteger(ReadOnlySpan<byte>(buffer))
            if isProbablePrime candidate rounds then
                candidate
            else
                findPrime ()

        findPrime ()

    /// Generates a safe prime p = 2q + 1 where both p and q are primes.
    let generateSafePrime (bitLength: int) (rounds: int) : BigInteger =
        let rec loop () =
            let q = generatePrime (bitLength - 1) rounds
            let p = 2I * q + 1I
            if isProbablePrime p rounds then
                p
            else
                loop ()
        loop ()

    /// Miller-Rabin probable-prime test against a fixed list of bases (no random source).
    /// A true result means probable prime, not proved prime.
    /// Uses Modular.modPow, which delegates non-negative exponents to the CLR BigInteger.ModPow call.
    let millerRabinWithBases (n: BigInteger) (bases: BigInteger list) : bool =
        if n < 2I then false
        elif n = 2I then true
        elif n % 2I = BigInteger.Zero then false
        else
            let mutable d = n - BigInteger.One
            let mutable s = 0
            while d % 2I = BigInteger.Zero do
                d <- d / 2I
                s <- s + 1

            let passesBase (a: BigInteger) : bool =
                let aMod = Modular.modPos a n
                if aMod = BigInteger.Zero then
                    true
                else
                    let mutable x = Modular.modPow aMod d n
                    if x = BigInteger.One || x = n - BigInteger.One then
                        true
                    else
                        let mutable passed = false
                        let mutable r = 1
                        while r < s && not passed do
                            x <- Modular.modPow x 2I n
                            if x = n - BigInteger.One then
                                passed <- true
                            r <- r + 1
                        passed

            bases |> List.forall passesBase

    /// Lucas primality certificate: checks that n > 2, that dividing n - 1 by each listed factor q
    /// as many times as it goes leaves 1, that a^(n - 1) mod n = 1, and that a^((n - 1) / q) mod n <> 1
    /// for every listed q. The listed q must themselves be prime; this function does not check that,
    /// and no factoring routine is included.
    let lucasCertificate (n: BigInteger) (a: BigInteger) (primeFactorsOfNMinus1: BigInteger list) : bool =
        if n <= 2I || List.isEmpty primeFactorsOfNMinus1 || primeFactorsOfNMinus1 |> List.exists (fun q -> q < 2I) then
            false
        else
            let nMinus1 = n - BigInteger.One
            if primeFactorsOfNMinus1 |> List.exists (fun q -> nMinus1 % q <> BigInteger.Zero) then
                false
            else
                let mutable rem = nMinus1
                for q in primeFactorsOfNMinus1 do
                    while rem % q = BigInteger.Zero do
                        rem <- rem / q
                if rem <> BigInteger.One then
                    false
                elif Modular.modPow a nMinus1 n <> BigInteger.One then
                    false
                else
                    primeFactorsOfNMinus1
                    |> List.forall (fun q -> Modular.modPow a (nMinus1 / q) n <> BigInteger.One)
