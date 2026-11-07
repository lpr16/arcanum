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

    [<Fact>]
    let ``Phase 125 - Shamir shareWith and reconstructMany at threshold 3 over ToyField`` () =
        let coeffs = [ residue 5I; residue 3I; residue 2I ]
        let shares =
            [ for x in 1I .. 5I ->
                match Shamir.shareWith coeffs (residue x) with
                | Ok s -> s
                | Error e -> failwith $"Unexpected error: {e}" ]

        let ys = shares |> List.map (fun s -> ToyField.toBigInteger s.Y)
        Assert.Equal<BigInteger list>([ 10I; 2I; 15I; 15I; 2I ], ys)

        let mutable count3 = 0
        for i in 0 .. 4 do
            for j in i + 1 .. 4 do
                for l in j + 1 .. 4 do
                    count3 <- count3 + 1
                    Assert.Equal(Ok (residue 5I), Shamir.reconstructMany [ shares.[i]; shares.[j]; shares.[l] ])
        Assert.Equal(10, count3)

        let mutable count4 = 0
        for omit in 0 .. 4 do
            count4 <- count4 + 1
            let sub4 =
                shares
                |> List.mapi (fun idx s -> idx, s)
                |> List.choose (fun (idx, s) -> if idx = omit then None else Some s)
            Assert.Equal(Ok (residue 5I), Shamir.reconstructMany sub4)
        Assert.Equal(5, count4)

        Assert.Equal(Ok (residue 5I), Shamir.reconstructMany shares)

        Assert.Equal(Ok (residue 1I), Shamir.reconstructMany [ shares.[0]; shares.[1] ])

        let r1 = residue 1I
        let r2 = residue 2I
        let y1Expected = residue 10I
        let y2Expected = residue 2I
        for sPrime in ToyField.residues do
            let matches =
                [ for c1 in ToyField.residues do
                    for c2 in ToyField.residues do
                        match Shamir.shareWith [ sPrime; c1; c2 ] r1, Shamir.shareWith [ sPrime; c1; c2 ] r2 with
                        | Ok sh1, Ok sh2 when sh1.Y = y1Expected && sh2.Y = y2Expected ->
                            yield (ToyField.toBigInteger c1, ToyField.toBigInteger c2)
                        | _ -> () ]
            Assert.Equal(1, matches.Length)
            if sPrime = ToyField.zero then
                Assert.Equal<(BigInteger * BigInteger) list>([ (2I, 8I) ], matches)

        Assert.Equal(Ok (residue 5I), Shamir.reconstructMany [ shareAt 1I; shareAt 2I ])

        Assert.True(Result.isError (Shamir.shareWith [] r1))
        Assert.True(Result.isError (Shamir.shareWith coeffs ToyField.zero))
        Assert.True(Result.isError (Shamir.reconstructMany []))
        Assert.True(Result.isError (Shamir.reconstructMany [ { X = ToyField.zero; Y = secret } ]))
        Assert.True(Result.isError (Shamir.reconstructMany [ shares.[0]; shares.[0] ]))
