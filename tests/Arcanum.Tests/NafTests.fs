namespace Arcanum.Tests

open System
open System.Numerics
open Xunit
open Arcanum.Core

module NafTests =

    [<Fact>]
    let ``Phase 108 - Non-adjacent form (NAF) representation and laws`` () =
        Assert.Equal<int list>([], EllipticCurve.naf 0I)
        Assert.Equal<int list>([ 1 ], EllipticCurve.naf 1I)
        Assert.Equal<int list>([ -1; 0; 0; 1 ], EllipticCurve.naf 7I)
        Assert.Equal<int list>([ 1; 0; -1; 0; 0; 1 ], EllipticCurve.naf 29I)
        Assert.Equal<int list>([ -1; 0; 0; 0; 0; 0; 0; 0; 1 ], EllipticCurve.naf 255I)
        Assert.Equal<int list>([ 0; 0; 0; 1; 0; -1; 0; 0; 0; 0; 1 ], EllipticCurve.naf 1000I)
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> EllipticCurve.naf -1I |> ignore) |> ignore

        for k in 0I .. 2000I do
            let digits = EllipticCurve.naf k
            let rebuilt =
                digits
                |> List.mapi (fun i d -> BigInteger d * (BigInteger.One <<< i))
                |> List.fold (+) BigInteger.Zero
            Assert.Equal(k, rebuilt)

            for d in digits do
                Assert.True(d = -1 || d = 0 || d = 1)

            for (d1, d2) in List.pairwise digits do
                Assert.True(d1 = 0 || d2 = 0)

            Assert.True(digits.Length <= int (k.GetBitLength()) + 1)
