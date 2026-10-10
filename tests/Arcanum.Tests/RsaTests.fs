namespace Arcanum.Tests

open System
open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Asymmetric

module RsaTests =

    // Small textbook RSA key: p = 61, q = 53, n = 3233, e = 17, d = 2753.
    // This key is not a secure modulus; it is for hand-verifiable testing only.
    let smallPubKey: Rsa.RsaPublicKey = { N = 3233I; E = 17I }

    [<Fact>]
    let ``Phase 22 - Textbook RSA encryption and modular power decryption`` () =
        let ciphertext = Rsa.encryptRaw smallPubKey 65I
        Assert.Equal(2790I, ciphertext)

        let decrypted = Modular.modPow 2790I 2753I 3233I
        Assert.Equal(65I, decrypted)

    [<Fact>]
    let ``Phase 22 - encryptRaw refuses messages greater than or equal to modulus`` () =
        Assert.Throws<ArgumentOutOfRangeException>(fun () ->
            Rsa.encryptRaw smallPubKey 3233I |> ignore
        ) |> ignore

        Assert.Throws<ArgumentOutOfRangeException>(fun () ->
            Rsa.encryptRaw smallPubKey 3234I |> ignore
        ) |> ignore

    let smallPrivKey: Rsa.RsaPrivateKey = {
        N = 3233I
        E = 17I
        D = 2753I
        P = 61I
        Q = 53I
        Dp = 53I
        Dq = 49I
        QInv = 38I
    }

    [<Fact>]
    let ``Phase 23 - RSA decryption by the Chinese Remainder Theorem`` () =
        let decryptedCrt = Rsa.decryptCrt smallPrivKey 2790I
        Assert.Equal(65I, decryptedCrt)
        Assert.Equal(Modular.modPow 2790I 2753I 3233I, decryptedCrt)

