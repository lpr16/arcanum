namespace Arcanum.Cli

open System
open System.Text
open System.Numerics
open Arcanum.Core
open Arcanum.Classical
open Arcanum.Symmetric
open Arcanum.Hashing
open Arcanum.Asymmetric

module Program =
    let printHeader (title: string) =
        Console.ForegroundColor <- ConsoleColor.Cyan
        Console.WriteLine("\n========================================================")
        Console.WriteLine($"  {title}")
        Console.WriteLine("========================================================")
        Console.ResetColor()

    let printSuccess (msg: string) =
        Console.ForegroundColor <- ConsoleColor.Green
        Console.WriteLine($"[+] {msg}")
        Console.ResetColor()

    let printInfo (msg: string) =
        Console.ForegroundColor <- ConsoleColor.Yellow
        Console.WriteLine($"[*] {msg}")
        Console.ResetColor()

    let demoClassical () =
        printHeader "CLASSICAL CRYPTOGRAPHY & CRYPTANALYSIS"

        let original = "CRYPTOGRAPHY IS THE PRACTICE AND STUDY OF TECHNIQUES FOR SECURE COMMUNICATION"
        printInfo $"Original Text: {original}"

        // 1. Caesar Cipher & Chi-Squared Solver
        let caesarShift = 7
        let caesarEncrypted = Substitution.caesarEncrypt caesarShift original
        printInfo $"Caesar Encrypted (shift={caesarShift}): {caesarEncrypted}"
        let (recoveredShift, recoveredText, score) = Cryptanalysis.breakCaesar caesarEncrypted
        printSuccess $"Caesar Cracker: Recovered shift={recoveredShift} (ChiSq={score:F2})"
        printSuccess $"Caesar Cracker Decrypted: {recoveredText}"

        // 2. Vigenere Cipher & IoC
        let vigenereKey = "SECRET"
        match Polyalphabetic.vigenereEncrypt vigenereKey original with
        | Error err -> printfn "Error: %A" err
        | Ok vigEnc ->
            printInfo $"\nVigenere Encrypted (key='{vigenereKey}'): {vigEnc}"
            let ioc = Cryptanalysis.indexOfCoincidence vigEnc
            printInfo $"Ciphertext Index of Coincidence (IoC): {ioc:F4} (Standard English ~0.0667, Random ~0.0385)"
            let (crackedKey, crackedText) = Cryptanalysis.breakVigenereWithKeyLength vigenereKey.Length vigEnc
            printSuccess $"Vigenere Cracker: Recovered key='{crackedKey}'"
            printSuccess $"Vigenere Cracker Decrypted: {crackedText}"

        // 3. Wehrmacht Enigma Simulation
        let enigmaConfig = {
            Enigma.Left = { RotorType = Enigma.RotorI; RingSetting = 0; Position = 0 }
            Enigma.Middle = { RotorType = Enigma.RotorII; RingSetting = 0; Position = 0 }
            Enigma.Right = { RotorType = Enigma.RotorIII; RingSetting = 0; Position = 0 }
            Enigma.Reflector = Enigma.ReflectorB
            Enigma.Plugboard = [ ('A', 'Z'); ('B', 'Y') ]
        }
        let enigmaMessage = "HELLOWORLD"
        let enigmaEncrypted = Enigma.processMessage enigmaConfig enigmaMessage
        let enigmaDecrypted = Enigma.processMessage enigmaConfig enigmaEncrypted
        printInfo $"\nEnigma Input: {enigmaMessage}"
        printInfo $"Enigma Encrypted: {enigmaEncrypted}"
        printSuccess $"Enigma Decrypted (self-reciprocal property): {enigmaDecrypted}"

    let demoSymmetric () =
        printHeader "SYMMETRIC CRYPTOGRAPHY (AES & CHACHA20-POLY1305)"

        // 1. AES-128 CBC Mode
        let key = Key (Bytes.fromHex "000102030405060708090a0b0c0d0e0f")
        let iv = Iv (Bytes.fromHex "00112233445566778899aabbccddeeff")
        let message = Plaintext (Encoding.UTF8.GetBytes "Arcanum: Functional Cryptography Study in F#")

        match Modes.encryptCbc key iv message with
        | Error err -> printfn "CBC Encrypt Error: %A" err
        | Ok ciphertext ->
            let ctHex = Bytes.toHex ciphertext.Value
            printInfo $"AES-128-CBC Ciphertext (Hex): {ctHex}"
            match Modes.decryptCbc key iv ciphertext with
            | Error err -> printfn "CBC Decrypt Error: %A" err
            | Ok decrypted ->
                printSuccess $"AES-128-CBC Decrypted: {Encoding.UTF8.GetString decrypted.Value}"

        // 2. ChaCha20-Poly1305 AEAD
        let chachaKey = Key (Bytes.randomBytes 32)
        let chachaNonce = Nonce (Bytes.randomBytes 12)
        let aad = Encoding.UTF8.GetBytes "header-metadata-authenticated-not-encrypted"

        match ChaCha20Poly1305.encrypt chachaKey chachaNonce aad message with
        | Error err -> printfn "AEAD Encrypt Error: %A" err
        | Ok (ct, tag) ->
            let ctHex = Bytes.toHex ct.Value
            let tagHex = Bytes.toHex tag.Value
            printInfo $"\nChaCha20-Poly1305 Ciphertext (Hex): {ctHex}"
            printInfo $"ChaCha20-Poly1305 Auth Tag (Hex): {tagHex}"
            match ChaCha20Poly1305.decrypt chachaKey chachaNonce aad ct tag with
            | Error err -> printfn "AEAD Decrypt Error: %A" err
            | Ok pt ->
                printSuccess $"ChaCha20-Poly1305 Decrypted & Authenticated: {Encoding.UTF8.GetString pt.Value}"

    let demoAsymmetric () =
        printHeader "ASYMMETRIC CRYPTOGRAPHY (DH, RSA, ECDSA)"

        // 1. Diffie-Hellman Key Exchange (ECDH on secp256k1)
        printInfo "1. Elliptic Curve Diffie-Hellman (secp256k1):"
        let curve = EllipticCurve.secp256k1
        let alice = DiffieHellman.generateEcdhKeyPair curve
        let bob = DiffieHellman.generateEcdhKeyPair curve

        let aliceShared = DiffieHellman.computeEcdhSharedSecret curve alice.PrivateKey bob.PublicKey
        let bobShared = DiffieHellman.computeEcdhSharedSecret curve bob.PrivateKey alice.PublicKey

        match aliceShared, bobShared with
        | EllipticCurve.Point(ax, _), EllipticCurve.Point(bx, _) ->
            if ax = bx then
                let hexX = ax.ToString("X")
                let shortX = if hexX.Length > 32 then hexX.Substring(0, 32) else hexX
                printSuccess $"ECDH Key Exchange Agreement Succeeded! Shared X: {shortX}..."
            else
                printfn "[-] ECDH Key Exchange Failed"
        | _ -> printfn "[-] Point at infinity"

        // 2. RSA Key Generation & CRT Decryption
        printInfo "\n2. RSA (1024-bit key generation):"
        let rsaPair = Rsa.generateKeyPair 1024
        let secretNum = 1234567891011121314151617181920I
        let rsaCipher = Rsa.encryptRaw rsaPair.PublicKey secretNum
        let rsaRecovered = Rsa.decryptCrt rsaPair.PrivateKey rsaCipher
        let nHex = rsaPair.PublicKey.N.ToString("X")
        let shortN = if nHex.Length > 32 then nHex.Substring(0, 32) else nHex
        printInfo $"RSA Modulus N: {shortN}..."
        if rsaRecovered = secretNum then
            printSuccess "RSA Encryption & CRT Decryption Verified!"

        // 3. ECDSA Digital Signature (secp256k1)
        printInfo "\n3. ECDSA Signing & Verification (secp256k1):"
        let ecdsaKey = Ecdsa.generateKeyPair curve
        let doc = Encoding.UTF8.GetBytes "Verifiable Document Signed with F# ECDSA"
        match Ecdsa.sign ecdsaKey doc with
        | Error err -> printfn "Signing error: %A" err
        | Ok sigVal ->
            let rHex = sigVal.R.ToString("X")
            let sHex = sigVal.S.ToString("X")
            let shortR = if rHex.Length > 24 then rHex.Substring(0, 24) else rHex
            let shortS = if sHex.Length > 24 then sHex.Substring(0, 24) else sHex
            printInfo $"Signature R: {shortR}..."
            printInfo $"Signature S: {shortS}..."
            let isValid = Ecdsa.verify curve ecdsaKey.PublicKey doc sigVal
            if isValid then
                printSuccess "ECDSA Signature Verified Successfully with Public Key!"
            else
                printfn "[-] ECDSA Verification Failed"

    let demoAttacks () =
        printHeader "CRYPTANALYSIS & EXPLOIT LAB"

        // 1. Serge Vaudenay's CBC Padding Oracle Attack
        printInfo "1. Executing Serge Vaudenay's CBC Padding Oracle Attack..."
        let secretKey = Key (Bytes.randomBytes 16)
        let iv = Iv (Bytes.randomBytes 16)
        let secretSecret = Plaintext (Encoding.UTF8.GetBytes "SecretPass!")

        let checkPadding (ivBytes: byte[]) (cipherBytes: byte[]) : bool =
            let roundKeys = Aes.expandKey128 secretKey.Value
            let blockCount = cipherBytes.Length / 16
            let plainBytes = Array.zeroCreate cipherBytes.Length
            let mutable prevBlock = ivBytes
            for i = 0 to blockCount - 1 do
                let block = Array.zeroCreate 16
                Array.Copy(cipherBytes, i * 16, block, 0, 16)
                let decBlock = Aes.decryptBlock roundKeys block
                let xored = Bytes.xor decBlock prevBlock
                Array.Copy(xored, 0, plainBytes, i * 16, 16)
                prevBlock <- block

            match Padding.unpadPkcs7 16 plainBytes with
            | Ok _ -> true
            | Error _ -> false

        match Modes.encryptCbc secretKey iv secretSecret with
        | Error err -> printfn "Setup Error: %A" err
        | Ok cbcCipher ->
            let ctHex = Bytes.toHex cbcCipher.Value
            printInfo $"Intercepted Ciphertext (Hex): {ctHex}"
            printInfo "Attacker queries padding oracle without secret key..."

            let recovered =
                let allBlocks = Array.concat [ iv.Value; cbcCipher.Value ]
                let blockCount = cbcCipher.Length / 16
                let recoveredAll = Array.zeroCreate cbcCipher.Length

                for b = 1 to blockCount do
                    let cPrev = Array.sub allBlocks ((b - 1) * 16) 16
                    let cCurrent = Array.sub allBlocks (b * 16) 16
                    let intermediate = Array.zeroCreate 16
                    let recBlock = Array.zeroCreate 16

                    for byteIdx = 15 downto 0 do
                        let padTarget = byte (16 - byteIdx)
                        let cPrime = Array.zeroCreate 16
                        for k = byteIdx + 1 to 15 do
                            cPrime.[k] <- intermediate.[k] ^^^ padTarget

                        let mutable found = false
                        let mutable candidate = 0
                        while candidate < 256 && not found do
                            cPrime.[byteIdx] <- byte candidate
                            if checkPadding cPrime cCurrent then
                                if padTarget = 1uy && byteIdx > 0 then
                                    cPrime.[byteIdx - 1] <- cPrime.[byteIdx - 1] ^^^ 1uy
                                    if checkPadding cPrime cCurrent then found <- true
                                    cPrime.[byteIdx - 1] <- cPrime.[byteIdx - 1] ^^^ 1uy
                                else
                                    found <- true
                            if not found then candidate <- candidate + 1

                        let inter = byte candidate ^^^ padTarget
                        intermediate.[byteIdx] <- inter
                        recBlock.[byteIdx] <- inter ^^^ cPrev.[byteIdx]

                    Array.Copy(recBlock, 0, recoveredAll, (b - 1) * 16, 16)

                match Padding.unpadPkcs7 16 recoveredAll with
                | Ok u -> u
                | Error _ -> recoveredAll

            let recoveredStr = Encoding.UTF8.GetString recovered
            printSuccess $"EXPLOIT SUCCESSFUL! Plaintext Recovered from Oracle: '{recoveredStr}'"

        // 2. ECDSA Nonce Reuse Private Key Recovery
        printInfo "\n2. Executing ECDSA Private Key Recovery via Nonce Reuse..."
        let curve = EllipticCurve.secp256k1
        let signer = Ecdsa.generateKeyPair curve
        let repeatedK = 4242424242424242424242I

        let m1 = Encoding.UTF8.GetBytes "Message 1: transfer $10"
        let m2 = Encoding.UTF8.GetBytes "Message 2: transfer $50"

        match Ecdsa.signWithNonce signer m1 repeatedK, Ecdsa.signWithNonce signer m2 repeatedK with
        | Ok sig1, Ok sig2 ->
            let r1Hex = sig1.R.ToString("X")
            let shortR1 = if r1Hex.Length > 24 then r1Hex.Substring(0, 24) else r1Hex
            printInfo $"Signatures have identical Nonce Point R: {shortR1}..."
            printInfo "Attacker uses linear algebra mod n to extract private key..."

            let hashToInt (msg: byte[]) =
                let d = (SHA256.hash msg).Value
                let buf = Array.zeroCreate (d.Length + 1)
                Array.Copy(d, 0, buf, 0, d.Length)
                Modular.modPos (BigInteger(ReadOnlySpan<byte>(buf))) curve.N

            let z1 = hashToInt m1
            let z2 = hashToInt m2
            let num = Modular.modPos (z1 - z2) curve.N
            let den = Modular.modPos (sig1.S - sig2.S) curve.N

            match Modular.modInverse den curve.N with
            | None -> printfn "Inverse failed"
            | Some invDen ->
                let k = Modular.modPos (num * invDen) curve.N
                let dNum = Modular.modPos (sig1.S * k - z1) curve.N
                match Modular.modInverse sig1.R curve.N with
                | None -> printfn "R inverse failed"
                | Some invR ->
                    let stolenKey = Modular.modPos (dNum * invR) curve.N
                    if stolenKey = signer.PrivateKey then
                        let stolenHex = stolenKey.ToString("X")
                        printSuccess "EXPLOIT SUCCESSFUL! Signer Private Key Compromised!"
                        printSuccess $"Recovered Private Key: {stolenHex}"
                    else
                        printfn "[-] Key mismatch"
        | _ -> printfn "[-] Sign failed"

    [<EntryPoint>]
    let main argv =
        Console.ForegroundColor <- ConsoleColor.Magenta
        Console.WriteLine("""
     /\                                            
    /  \   _ __ ___ __ _ _ __  _   _ _ __ ___      
   / /\ \ | '__/ __/ _` | '_ \| | | | '_ ` _ \     
  / ____ \| | | (_| (_| | | | | |_| | | | | | |    
 /_/    \_\_|  \___\__,_|_| |_|\__,_|_| |_| |_|    
           A Study in Cryptography with F#         
        """)
        Console.ResetColor()

        demoClassical ()
        demoSymmetric ()
        demoAsymmetric ()
        demoAttacks ()

        Console.ForegroundColor <- ConsoleColor.Green
        Console.WriteLine("\n[✓] All Arcanum study demonstrations completed successfully.\n")
        Console.ResetColor()
        0
