namespace Arcanum.Symmetric

open System
open Arcanum.Core

module ChaCha20 =
    let private C0 = 0x61707865u
    let private C1 = 0x3320646eu
    let private C2 = 0x79622d32u
    let private C3 = 0x6b206574u

    let inline private quarterRound (state: uint32[]) (a: int) (b: int) (c: int) (d: int) : unit =
        state.[a] <- state.[a] + state.[b]
        state.[d] <- Bytes.rotl32 (state.[d] ^^^ state.[a]) 16
        state.[c] <- state.[c] + state.[d]
        state.[b] <- Bytes.rotl32 (state.[b] ^^^ state.[c]) 12
        state.[a] <- state.[a] + state.[b]
        state.[d] <- Bytes.rotl32 (state.[d] ^^^ state.[a]) 8
        state.[c] <- state.[c] + state.[d]
        state.[b] <- Bytes.rotl32 (state.[b] ^^^ state.[c]) 7

    /// Converts 4 bytes little-endian to uint32.
    let inline private readUint32Le (b: byte[]) (offset: int) : uint32 =
        uint32 b.[offset] |||
        (uint32 b.[offset + 1] <<< 8) |||
        (uint32 b.[offset + 2] <<< 16) |||
        (uint32 b.[offset + 3] <<< 24)

    /// Writes uint32 little-endian to byte buffer.
    let inline private writeUint32Le (v: uint32) (b: byte[]) (offset: int) : unit =
        b.[offset]     <- byte v
        b.[offset + 1] <- byte (v >>> 8)
        b.[offset + 2] <- byte (v >>> 16)
        b.[offset + 3] <- byte (v >>> 24)

    /// Generates a single 64-byte ChaCha20 keystream block.
    let block (key: byte[]) (counter: uint32) (nonce: byte[]) : byte[] =
        if key.Length <> 32 then
            raise (ArgumentException("ChaCha20 key must be 32 bytes"))
        if nonce.Length <> 12 then
            raise (ArgumentException("ChaCha20 nonce must be 12 bytes"))

        let state = [|
            C0; C1; C2; C3
            readUint32Le key 0;  readUint32Le key 4;  readUint32Le key 8;  readUint32Le key 12
            readUint32Le key 16; readUint32Le key 20; readUint32Le key 24; readUint32Le key 28
            counter
            readUint32Le nonce 0; readUint32Le nonce 4; readUint32Le nonce 8
        |]

        let working = Array.copy state

        for _ = 1 to 10 do
            // Column rounds
            quarterRound working 0 4 8 12
            quarterRound working 1 5 9 13
            quarterRound working 2 6 10 14
            quarterRound working 3 7 11 15

            // Diagonal rounds
            quarterRound working 0 5 10 15
            quarterRound working 1 6 11 12
            quarterRound working 2 7 8 13
            quarterRound working 3 4 9 14

        let output = Array.zeroCreate 64
        for i = 0 to 15 do
            let sum = working.[i] + state.[i]
            writeUint32Le sum output (i * 4)

        output

    /// Processes data with ChaCha20 stream cipher (encryption & decryption are identical).
    let processStream (key: Key) (counter: uint32) (nonce: Nonce) (data: byte[]) : Result<byte[], CryptoError> =
        if key.Length <> 32 then
            Error (InvalidKeyLength (32, key.Length))
        elif nonce.Length <> 12 then
            Error (InvalidNonceLength (12, nonce.Length))
        else
            let result = Array.zeroCreate data.Length
            let blockCount = (data.Length + 63) / 64
            let mutable currentCounter = counter

            for i = 0 to blockCount - 1 do
                let keystream = block key.Value currentCounter nonce.Value
                let offset = i * 64
                let chunkLen = min 64 (data.Length - offset)
                for j = 0 to chunkLen - 1 do
                    result.[offset + j] <- data.[offset + j] ^^^ keystream.[j]
                currentCounter <- currentCounter + 1u

            Ok result
