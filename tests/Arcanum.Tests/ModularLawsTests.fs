namespace Arcanum.Tests

open System
open System.Numerics
open Xunit
open Arcanum.Core

module ModularLawsTests =
    [<Fact>]
    let ``Extended Euclidean algorithm satisfies Bezout identity`` () =
        let (g1, x1, y1) = Modular.xgcd 240I 46I
        Assert.Equal(2I, g1)
        Assert.Equal(-9I, x1)
        Assert.Equal(47I, y1)
        Assert.Equal(2I, 240I * x1 + 46I * y1)

        let (g2, x2, y2) = Modular.xgcd 15I 25I
        Assert.Equal(5I, g2)
        Assert.Equal(2I, x2)
        Assert.Equal(-1I, y2)
        Assert.Equal(5I, 15I * x2 + 25I * y2)

        let (g3, x3, y3) = Modular.xgcd 17I 0I
        Assert.Equal(17I, g3)
        Assert.Equal(1I, x3)
        Assert.Equal(0I, y3)
        Assert.Equal(17I, 17I * x3 + 0I * y3)

    [<Fact>]
    let ``Chinese remainder theorem solves simultaneous congruences and rejects non-coprime moduli`` () =
        let congruences = [ (2I, 3I); (3I, 5I); (2I, 7I) ]
        let result = Modular.crt congruences
        Assert.Equal(Some 23I, result)

        match result with
        | Some x ->
            for (a, m) in congruences do
                Assert.Equal(a, Modular.modPos x m)
        | None -> Assert.Fail("Expected CRT solution")

        // Inconsistent/non-coprime even moduli return None
        let nonCoprime = [ (1I, 2I); (0I, 4I) ]
        Assert.Equal(None, Modular.crt nonCoprime)

    [<Fact>]
    let ``Jacobi symbol and modular square roots on toy prime 17`` () =
        Assert.Equal(1, Modular.jacobi 2I 17I)
        Assert.Equal(-1, Modular.jacobi 3I 17I)

        match Modular.modSqrt 2I 17I with
        | Some r ->
            Assert.True(r >= 0I && r < 17I)
            Assert.Equal(2I, Modular.modPos (r * r) 17I)
        | None -> Assert.Fail("Expected modular square root for 2 mod 17")

        Assert.Equal(None, Modular.modSqrt 3I 17I)
        Assert.Equal(Some 0I, Modular.modSqrt 0I 17I)

    [<Fact>]
    let ``Phase 101 - Montgomery batch inversion modulo 17`` () =
        let inputs = [ 1I .. 16I ]
        let expected = [ 1I; 9I; 6I; 13I; 7I; 3I; 5I; 15I; 2I; 12I; 14I; 10I; 4I; 11I; 8I; 16I ]
        match Modular.batchInverse inputs 17I with
        | Ok actual ->
            Assert.Equal<BigInteger list>(expected, actual)
            for (x, inv) in List.zip inputs actual do
                Assert.Equal(Some inv, Modular.modInverse x 17I)
        | Error err -> Assert.Fail(sprintf "Unexpected batchInverse error: %A" err)

        Assert.Equal(Ok ([] : BigInteger list), Modular.batchInverse [] 17I)
        Assert.True(Result.isError (Modular.batchInverse [ 3I; 0I; 5I ] 17I))
        Assert.True(Result.isError (Modular.batchInverse [ 3I; 17I ] 17I))

    [<Fact>]
    let ``Phase 102 - Special-form reduction modulo the secp256k1 prime`` () =
        let p = EllipticCurve.secp256k1.P
        Assert.Equal(p, Modular.secp256k1Prime)
        let gx, gy =
            match EllipticCurve.secp256k1.G with
            | EllipticCurve.Point(x, y) -> x, y
            | EllipticCurve.Infinity -> failwith "Unexpected Infinity"

        let two256 = BigInteger.One <<< 256
        let candidates = [
            0I
            1I
            p - 1I
            p
            p + 1I
            two256 - 1I
            two256
            (p - 1I) * (p - 1I)
            gx * gy
            gx * gx
            p * p - 1I
        ]
        for x in candidates do
            Assert.Equal(Modular.modPos x p, Modular.reduceSecp256k1 x)

        Assert.Equal(4294968273I, Modular.reduceSecp256k1 two256)
        Assert.Equal(1I, Modular.reduceSecp256k1 ((p - 1I) * (p - 1I)))
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> Modular.reduceSecp256k1 (p * p) |> ignore) |> ignore
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> Modular.reduceSecp256k1 -1I |> ignore) |> ignore
