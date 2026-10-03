namespace Arcanum.Symmetric

open System
open Arcanum.Core
open Arcanum.Core.FiniteFields

module Aes =
    let private SBox = GF256.generateAesSBox ()
    let private InvSBox = GF256.generateAesInvSBox ()

    let private Rcon : uint32[] = [|
        0x01000000u; 0x02000000u; 0x04000000u; 0x08000000u; 0x10000000u
        0x20000000u; 0x40000000u; 0x80000000u; 0x1B000000u; 0x36000000u
    |]

    let private subWord (w: uint32) : uint32 =
        let b0 = uint32 SBox.[int ((w >>> 24) &&& 0xFFu)]
        let b1 = uint32 SBox.[int ((w >>> 16) &&& 0xFFu)]
        let b2 = uint32 SBox.[int ((w >>> 8) &&& 0xFFu)]
        let b3 = uint32 SBox.[int (w &&& 0xFFu)]
        (b0 <<< 24) ||| (b1 <<< 16) ||| (b2 <<< 8) ||| b3

    let private rotWord (w: uint32) : uint32 =
        (w <<< 8) ||| (w >>> 24)

    /// Expands a 128-bit key into 44 round key words (11 round keys).
    let expandKey128 (keyBytes: byte[]) : uint32[] =
        if keyBytes.Length <> 16 then
            raise (ArgumentException("AES-128 requires a 16-byte key"))

        let w = Array.zeroCreate 44
        for i = 0 to 3 do
            let idx = i * 4
            w.[i] <- (uint32 keyBytes.[idx] <<< 24) |||
                     (uint32 keyBytes.[idx + 1] <<< 16) |||
                     (uint32 keyBytes.[idx + 2] <<< 8) |||
                     (uint32 keyBytes.[idx + 3])

        for i = 4 to 43 do
            let mutable temp = w.[i - 1]
            if i % 4 = 0 then
                temp <- subWord (rotWord temp) ^^^ Rcon.[(i / 4) - 1]
            w.[i] <- w.[i - 4] ^^^ temp

        w

    let private subBytes (state: byte[]) : unit =
        for i = 0 to 15 do
            state.[i] <- SBox.[int state.[i]]

    let private invSubBytes (state: byte[]) : unit =
        for i = 0 to 15 do
            state.[i] <- InvSBox.[int state.[i]]

    let private shiftRows (state: byte[]) : unit =
        // Row 1: shift left 1
        let temp = state.[1]
        state.[1] <- state.[5]
        state.[5] <- state.[9]
        state.[9] <- state.[13]
        state.[13] <- temp

        // Row 2: shift left 2
        let t1 = state.[2]
        let t2 = state.[6]
        state.[2] <- state.[10]
        state.[6] <- state.[14]
        state.[10] <- t1
        state.[14] <- t2

        // Row 3: shift left 3 (shift right 1)
        let t3 = state.[15]
        state.[15] <- state.[11]
        state.[11] <- state.[7]
        state.[7] <- state.[3]
        state.[3] <- t3

    let private invShiftRows (state: byte[]) : unit =
        // Row 1: shift right 1
        let temp = state.[13]
        state.[13] <- state.[9]
        state.[9] <- state.[5]
        state.[5] <- state.[1]
        state.[1] <- temp

        // Row 2: shift right 2
        let t1 = state.[10]
        let t2 = state.[14]
        state.[10] <- state.[2]
        state.[14] <- state.[6]
        state.[2] <- t1
        state.[6] <- t2

        // Row 3: shift right 3 (shift left 1)
        let t3 = state.[3]
        state.[3] <- state.[7]
        state.[7] <- state.[11]
        state.[11] <- state.[15]
        state.[15] <- t3

    let private mixColumns (state: byte[]) : unit =
        for c = 0 to 3 do
            let i = c * 4
            let s0 = state.[i]
            let s1 = state.[i + 1]
            let s2 = state.[i + 2]
            let s3 = state.[i + 3]

            state.[i]     <- GF256.xtime s0 ^^^ (GF256.xtime s1 ^^^ s1) ^^^ s2 ^^^ s3
            state.[i + 1] <- s0 ^^^ GF256.xtime s1 ^^^ (GF256.xtime s2 ^^^ s2) ^^^ s3
            state.[i + 2] <- s0 ^^^ s1 ^^^ GF256.xtime s2 ^^^ (GF256.xtime s3 ^^^ s3)
            state.[i + 3] <- (GF256.xtime s0 ^^^ s0) ^^^ s1 ^^^ s2 ^^^ GF256.xtime s3

    let private invMixColumns (state: byte[]) : unit =
        for c = 0 to 3 do
            let i = c * 4
            let s0 = state.[i]
            let s1 = state.[i + 1]
            let s2 = state.[i + 2]
            let s3 = state.[i + 3]

            state.[i]     <- GF256.multiply 0x0Euy s0 ^^^ GF256.multiply 0x0Buy s1 ^^^ GF256.multiply 0x0Duy s2 ^^^ GF256.multiply 0x09uy s3
            state.[i + 1] <- GF256.multiply 0x09uy s0 ^^^ GF256.multiply 0x0Euy s1 ^^^ GF256.multiply 0x0Buy s2 ^^^ GF256.multiply 0x0Duy s3
            state.[i + 2] <- GF256.multiply 0x0Duy s0 ^^^ GF256.multiply 0x09uy s1 ^^^ GF256.multiply 0x0Euy s2 ^^^ GF256.multiply 0x0Buy s3
            state.[i + 3] <- GF256.multiply 0x0Buy s0 ^^^ GF256.multiply 0x0Duy s1 ^^^ GF256.multiply 0x09uy s2 ^^^ GF256.multiply 0x0Euy s3

    let private addRoundKey (state: byte[]) (roundKeys: uint32[]) (round: int) : unit =
        let kOffset = round * 4
        for c = 0 to 3 do
            let w = roundKeys.[kOffset + c]
            let i = c * 4
            state.[i]     <- state.[i]     ^^^ byte (w >>> 24)
            state.[i + 1] <- state.[i + 1] ^^^ byte (w >>> 16)
            state.[i + 2] <- state.[i + 2] ^^^ byte (w >>> 8)
            state.[i + 3] <- state.[i + 3] ^^^ byte w

    /// Encrypts a single 16-byte block with AES-128.
    let encryptBlock (roundKeys: uint32[]) (input: byte[]) : byte[] =
        if input.Length <> 16 then
            raise (ArgumentException("AES block must be 16 bytes"))

        let state = Array.copy input
        addRoundKey state roundKeys 0

        for round = 1 to 9 do
            subBytes state
            shiftRows state
            mixColumns state
            addRoundKey state roundKeys round

        // Final round (no mixColumns)
        subBytes state
        shiftRows state
        addRoundKey state roundKeys 10

        state

    /// Decrypts a single 16-byte block with AES-128.
    let decryptBlock (roundKeys: uint32[]) (input: byte[]) : byte[] =
        if input.Length <> 16 then
            raise (ArgumentException("AES block must be 16 bytes"))

        let state = Array.copy input
        addRoundKey state roundKeys 10

        for round = 9 downto 1 do
            invShiftRows state
            invSubBytes state
            addRoundKey state roundKeys round
            invMixColumns state

        // Final round (no invMixColumns)
        invShiftRows state
        invSubBytes state
        addRoundKey state roundKeys 0

        state
