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

    /// Generates a random BigInteger in the range [min, max].
    let randomBigInteger (minVal: BigInteger) (maxVal: BigInteger) : BigInteger =
        if minVal >= maxVal then
            raise (ArgumentException("minVal must be strictly less than maxVal"))
        let range = maxVal - minVal
        let bytesNeeded = range.ToByteArray().Length
        let buffer: byte[] = Array.zeroCreate (bytesNeeded + 1) // +1 ensures positive sign

        let rec loop () =
            RandomNumberGenerator.Fill(Span<byte>(buffer))
            buffer.[buffer.Length - 1] <- 0uy // enforce positive
            let candidate = BigInteger(ReadOnlySpan<byte>(buffer))
            if candidate < range then
                minVal + candidate
            else
                loop ()
        loop ()

    /// Miller-Rabin probabilistic primality test with specified number of rounds.
    /// The probability of a composite passing k independent rounds is at most 4^(-k).
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
