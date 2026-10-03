namespace Arcanum.Core

module FiniteFields =
    /// Operations in Galois Field GF(2^8) modulo the Rijndael polynomial x^8 + x^4 + x^3 + x + 1 (0x11B).
    module GF256 =
        /// Irreducible polynomial used by Rijndael / AES: 0x11B.
        let IrreduciblePolynomial = 0x11B

        /// Addition in GF(2^8) is bitwise XOR.
        let inline add (a: byte) (b: byte) : byte = a ^^^ b

        /// Multiplication by x (0x02) in GF(2^8).
        let inline xtime (b: byte) : byte =
            let shifted = b <<< 1
            if (b &&& 0x80uy) <> 0uy then
                shifted ^^^ 0x1Buy
            else
                shifted

        /// Multiplies two elements in GF(2^8) using Russian Peasant polynomial multiplication.
        let multiply (aInit: byte) (bInit: byte) : byte =
            let mutable a = aInit
            let mutable b = bInit
            let mutable p = 0uy
            for _ in 0 .. 7 do
                if (b &&& 1uy) <> 0uy then
                    p <- p ^^^ a
                a <- xtime a
                b <- b >>> 1
            p

        /// Multiplicative inverse in GF(2^8) using exponentiation:
        /// By Fermat's Little Theorem in GF(2^8), a^(2^8 - 1) = a^255 = 1.
        /// Thus, a^(-1) = a^254 = a^(11111110_2).
        /// For a = 0, returns 0 by cryptographic convention.
        let inverse (a: byte) : byte =
            if a = 0uy then 0uy
            else
                // Compute a^254 via square and multiply
                // 254 = 128 + 64 + 32 + 16 + 8 + 4 + 2
                let mutable res = 1uy
                let mutable baseVal = a
                let mutable exp = 254
                while exp > 0 do
                    if (exp &&& 1) <> 0 then
                        res <- multiply res baseVal
                    baseVal <- multiply baseVal baseVal
                    exp <- exp >>> 1
                res

        /// Computes the Rijndael S-Box substitution value mathematically:
        /// S(a) = AffineTransformation( inverse(a) ) ^ 0x63.
        let sboxSub (a: byte) : byte =
            let inv = inverse a
            let mutable x = inv
            let mutable s = inv
            for _ in 1 .. 4 do
                x <- (x <<< 1) ||| (x >>> 7) // 8-bit circular rotation
                s <- s ^^^ x
            s ^^^ 0x63uy

        /// Generates the standard 256-byte AES S-Box dynamically from first mathematical principles.
        let generateAesSBox () : byte[] =
            Array.init 256 (fun i -> sboxSub (byte i))

        /// Generates the standard 256-byte AES Inverse S-Box dynamically.
        let generateAesInvSBox () : byte[] =
            let sbox = generateAesSBox ()
            let invSBox = Array.zeroCreate 256
            for i in 0 .. 255 do
                invSBox.[int sbox.[i]] <- byte i
            invSBox

    /// Operations in Galois Field GF(2^128) used in GHASH for AES-GCM authenticated encryption.
    /// Modulo polynomial: x^128 + x^7 + x^2 + x + 1 (represented as 0xE1 in the first byte).
    module GF128 =
        /// Multiplies two 128-bit blocks in GF(2^128) per NIST SP 800-38D.
        let multiply (x: byte[]) (y: byte[]) : byte[] =
            if x.Length <> 16 || y.Length <> 16 then
                invalidArg "blocks" "Both blocks must be 16 bytes (128 bits)"

            let z = Array.zeroCreate 16
            let v = Array.copy x
            let r = 0xE1uy // reduction polynomial representation

            for i in 0 .. 127 do
                let byteIdx = i / 8
                let bitIdx = 7 - (i % 8)
                let bit = (y.[byteIdx] >>> bitIdx) &&& 1uy

                if bit <> 0uy then
                    for j in 0 .. 15 do
                        z.[j] <- z.[j] ^^^ v.[j]

                let lsb = (v.[15] &&& 1uy) <> 0uy

                // Right shift V by 1 bit
                for j = 15 downto 1 do
                    v.[j] <- (v.[j] >>> 1) ||| ((v.[j - 1] &&& 1uy) <<< 7)
                v.[0] <- v.[0] >>> 1

                if lsb then
                    v.[0] <- v.[0] ^^^ r

            z
