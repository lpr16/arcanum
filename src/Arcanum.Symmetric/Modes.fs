namespace Arcanum.Symmetric

open System
open Arcanum.Core

module Modes =
    let private BlockSize = 16

    /// Electronic Codebook (ECB) Mode Encryption with PKCS#7 padding.
    /// WARNING: Insecure for structured data as identical plaintext blocks produce identical ciphertext blocks!
    let encryptEcb (key: Key) (plaintext: Plaintext) : Ciphertext =
        let roundKeys = Aes.expandKey128 key.Value
        let padded = Padding.padPkcs7 BlockSize plaintext.Value
        let blockCount = padded.Length / BlockSize
        let cipherBytes = Array.zeroCreate padded.Length

        for i = 0 to blockCount - 1 do
            let block = Array.zeroCreate BlockSize
            Array.Copy(padded, i * BlockSize, block, 0, BlockSize)
            let encBlock = Aes.encryptBlock roundKeys block
            Array.Copy(encBlock, 0, cipherBytes, i * BlockSize, BlockSize)

        Ciphertext cipherBytes

    /// Electronic Codebook (ECB) Mode Decryption with PKCS#7 unpadding.
    let decryptEcb (key: Key) (ciphertext: Ciphertext) : Result<Plaintext, CryptoError> =
        if ciphertext.Length % BlockSize <> 0 || ciphertext.Length = 0 then
            Error (InvalidBlockSize (BlockSize, ciphertext.Length))
        else
            let roundKeys = Aes.expandKey128 key.Value
            let blockCount = ciphertext.Length / BlockSize
            let plainBytes = Array.zeroCreate ciphertext.Length

            for i = 0 to blockCount - 1 do
                let block = Array.zeroCreate BlockSize
                Array.Copy(ciphertext.Value, i * BlockSize, block, 0, BlockSize)
                let decBlock = Aes.decryptBlock roundKeys block
                Array.Copy(decBlock, 0, plainBytes, i * BlockSize, BlockSize)

            match Padding.unpadPkcs7 BlockSize plainBytes with
            | Ok unpadded -> Ok (Plaintext unpadded)
            | Error err -> Error err

    /// Cipher Block Chaining (CBC) Mode Encryption with PKCS#7 padding.
    let encryptCbc (key: Key) (iv: Iv) (plaintext: Plaintext) : Result<Ciphertext, CryptoError> =
        if iv.Length <> BlockSize then
            Error (InvalidIvLength (BlockSize, iv.Length))
        else
            let roundKeys = Aes.expandKey128 key.Value
            let padded = Padding.padPkcs7 BlockSize plaintext.Value
            let blockCount = padded.Length / BlockSize
            let cipherBytes = Array.zeroCreate padded.Length

            let mutable prevBlock = iv.Value
            for i = 0 to blockCount - 1 do
                let block = Array.zeroCreate BlockSize
                Array.Copy(padded, i * BlockSize, block, 0, BlockSize)
                let xored = Bytes.xor block prevBlock
                let encBlock = Aes.encryptBlock roundKeys xored
                Array.Copy(encBlock, 0, cipherBytes, i * BlockSize, BlockSize)
                prevBlock <- encBlock

            Ok (Ciphertext cipherBytes)

    /// Cipher Block Chaining (CBC) Mode Decryption with PKCS#7 unpadding.
    let decryptCbc (key: Key) (iv: Iv) (ciphertext: Ciphertext) : Result<Plaintext, CryptoError> =
        if iv.Length <> BlockSize then
            Error (InvalidIvLength (BlockSize, iv.Length))
        elif ciphertext.Length % BlockSize <> 0 || ciphertext.Length = 0 then
            Error (InvalidBlockSize (BlockSize, ciphertext.Length))
        else
            let roundKeys = Aes.expandKey128 key.Value
            let blockCount = ciphertext.Length / BlockSize
            let plainBytes = Array.zeroCreate ciphertext.Length

            let mutable prevBlock = iv.Value
            for i = 0 to blockCount - 1 do
                let block = Array.zeroCreate BlockSize
                Array.Copy(ciphertext.Value, i * BlockSize, block, 0, BlockSize)
                let decBlock = Aes.decryptBlock roundKeys block
                let xored = Bytes.xor decBlock prevBlock
                Array.Copy(xored, 0, plainBytes, i * BlockSize, BlockSize)
                prevBlock <- block

            match Padding.unpadPkcs7 BlockSize plainBytes with
            | Ok unpadded -> Ok (Plaintext unpadded)
            | Error err -> Error err

    /// Counter (CTR) Mode stream cipher processing.
    /// Encryption and decryption are symmetric (the same function).
    let processCtr (key: Key) (iv: Iv) (data: byte[]) : Result<byte[], CryptoError> =
        if iv.Length <> BlockSize then
            Error (InvalidIvLength (BlockSize, iv.Length))
        else
            let roundKeys = Aes.expandKey128 key.Value
            let result = Array.zeroCreate data.Length
            let counterBlock = Array.copy iv.Value

            let incrementCounter (cb: byte[]) =
                let mutable idx = cb.Length - 1
                let mutable carry = true
                while idx >= 0 && carry do
                    cb.[idx] <- cb.[idx] + 1uy
                    if cb.[idx] <> 0uy then
                        carry <- false
                    idx <- idx - 1

            let blockCount = (data.Length + BlockSize - 1) / BlockSize
            for i = 0 to blockCount - 1 do
                let keystream = Aes.encryptBlock roundKeys counterBlock
                let offset = i * BlockSize
                let chunkLen = min BlockSize (data.Length - offset)
                for j = 0 to chunkLen - 1 do
                    result.[offset + j] <- data.[offset + j] ^^^ keystream.[j]
                incrementCounter counterBlock

            Ok result
