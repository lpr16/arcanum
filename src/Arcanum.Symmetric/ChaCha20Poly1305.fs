namespace Arcanum.Symmetric

open System
open Arcanum.Core

module ChaCha20Poly1305 =
    let private pad16 (len: int) : byte[] =
        let rem = len % 16
        if rem = 0 then [||]
        else Array.zeroCreate (16 - rem)

    let private len64Le (len: int) : byte[] =
        let u = uint64 len
        let b = Array.zeroCreate 8
        for i = 0 to 7 do
            b.[i] <- byte (u >>> (8 * i))
        b

    let private constructAuthData (aad: byte[]) (ciphertext: byte[]) : byte[] =
        Array.concat [
            aad
            pad16 aad.Length
            ciphertext
            pad16 ciphertext.Length
            len64Le aad.Length
            len64Le ciphertext.Length
        ]

    /// Authenticated Encryption with Associated Data (AEAD) per RFC 8439.
    let encrypt (key: Key) (nonce: Nonce) (aad: byte[]) (plaintext: Plaintext) : Result<Ciphertext * Tag, CryptoError> =
        if key.Length <> 32 then
            Error (InvalidKeyLength (32, key.Length))
        elif nonce.Length <> 12 then
            Error (InvalidNonceLength (12, nonce.Length))
        else
            // 1. One-time Poly1305 key from ChaCha20 block 0
            let polyBlock = ChaCha20.block key.Value 0u nonce.Value
            let otk = Array.sub polyBlock 0 32

            // 2. Encrypt plaintext starting at counter 1
            match ChaCha20.processStream key 1u nonce plaintext.Value with
            | Error err -> Error err
            | Ok cipherBytes ->
                // 3. Construct authentication buffer
                let authData = constructAuthData aad cipherBytes
                let tag = Poly1305.mac otk authData
                Ok (Ciphertext cipherBytes, tag)

    /// Authenticated Decryption with Associated Data (AEAD) per RFC 8439.
    /// Strict constant-time MAC verification before decryption to avoid padding and plaintext oracle leaks!
    let decrypt (key: Key) (nonce: Nonce) (aad: byte[]) (ciphertext: Ciphertext) (tag: Tag) : Result<Plaintext, CryptoError> =
        if key.Length <> 32 then
            Error (InvalidKeyLength (32, key.Length))
        elif nonce.Length <> 12 then
            Error (InvalidNonceLength (12, nonce.Length))
        else
            let polyBlock = ChaCha20.block key.Value 0u nonce.Value
            let otk = Array.sub polyBlock 0 32
            let authData = constructAuthData aad ciphertext.Value
            let expectedTag = Poly1305.mac otk authData

            if not (Bytes.constantTimeEquals tag.Value expectedTag.Value) then
                Error AuthenticationFailed
            else
                match ChaCha20.processStream key 1u nonce ciphertext.Value with
                | Error err -> Error err
                | Ok plainBytes -> Ok (Plaintext plainBytes)
