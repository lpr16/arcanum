namespace Arcanum.Tests

open System.Numerics
open Xunit
open Arcanum.Core

module ShamirTests =

    // Threshold-2 sharing over ToyField (F_17) with polynomial 5 + 3x. F_17 is not a place to keep a secret.
    let private residue (value: BigInteger) = ToyField.ofBigInteger value
    let private secret = residue 5I
    let private coefficient = residue 3I

    let private shareAt (x: BigInteger) : Shamir.Share =
        match Shamir.share secret coefficient (residue x) with
        | Ok s -> s
        | Error err -> failwith $"Expected a share at x = {x}, got {err}"

    [<Fact>]
    let ``Phase 43 - Shares of secret 5 with coefficient 3 at 1, 2, and 3`` () =
        // 5 + 3 * 1 = 8, 5 + 3 * 2 = 11, 5 + 3 * 3 = 14 (mod 17)
        Assert.Equal(8I, ToyField.toBigInteger (shareAt 1I).Y)
        Assert.Equal(11I, ToyField.toBigInteger (shareAt 2I).Y)
        Assert.Equal(14I, ToyField.toBigInteger (shareAt 3I).Y)

    [<Fact>]
    let ``Phase 43 - Any two of the three shares reconstruct 5`` () =
        let shares = [ shareAt 1I; shareAt 2I; shareAt 3I ]
        for i in 0 .. 2 do
            for j in i + 1 .. 2 do
                Assert.Equal(Ok secret, Shamir.reconstruct shares.[i] shares.[j])
                Assert.Equal(Ok secret, Shamir.reconstruct shares.[j] shares.[i])

    [<Fact>]
    let ``Phase 43 - A share at 0 and two shares at the same x are refused`` () =
        Assert.True(Result.isError (Shamir.share secret coefficient ToyField.zero))

        let atZero: Shamir.Share = { X = ToyField.zero; Y = secret }
        Assert.True(Result.isError (Shamir.reconstruct atZero (shareAt 1I)))

        Assert.True(Result.isError (Shamir.reconstruct (shareAt 1I) (shareAt 1I)))
