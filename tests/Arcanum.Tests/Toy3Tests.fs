namespace Arcanum.Tests

open Xunit
open Arcanum.Core
open Arcanum.Core.EllipticCurve

module Toy3Tests =
    // EllipticCurve.toy3 is y^2 = x^3 + 2x + 2 over F_17. Its group has prime order 19,
    // so every point other than Infinity generates it. This curve is not a secure group.
    let enumeratedPoints : ECPoint list =
        Infinity :: [
            for x in 0I .. (toy3.P - 1I) do
                for y in 0I .. (toy3.P - 1I) do
                    let pt = Point(x, y)
                    if isOnCurve toy3 pt then
                        yield pt
        ]

    let repeatedAdd (pt: ECPoint) (k: int) : ECPoint =
        let mutable res = Infinity
        for _ in 1 .. k do
            res <- add toy3 res pt
        res

    [<Fact>]
    let ``Phase 53 - toy3 is non-singular and enumeration finds 19 points`` () =
        // 4 * 2^3 + 27 * 2^2 = 32 + 108 = 140 = 4 (mod 17), which is not 0.
        let disc = Modular.modPos (4I * Modular.modPow toy3.A 3I toy3.P + 27I * Modular.modPow toy3.B 2I toy3.P) toy3.P
        Assert.Equal(4I, disc)
        Assert.NotEqual(0I, disc)
        Assert.Equal(19, enumeratedPoints.Length)
        Assert.Contains(Infinity, enumeratedPoints)

    [<Fact>]
    let ``Phase 53 - Every non-Infinity point on toy3 has order 19 by repeated addition`` () =
        let nonIdentity = enumeratedPoints |> List.filter (fun pt -> pt <> Infinity)
        Assert.Equal(18, nonIdentity.Length)
        for pt in nonIdentity do
            for step in 1 .. 18 do
                Assert.NotEqual(Infinity, repeatedAdd pt step)
            Assert.Equal(Infinity, repeatedAdd pt 19)

    [<Fact>]
    let ``Phase 53 - Tangent doubling, chord addition, and j-invariant on toy3`` () =
        // double (5, 1): slope = (3 * 5^2 + 2) * (2 * 1)^(-1) = 77 * 9 = 13 (mod 17)
        // x = 13^2 - 10 = 6 (mod 17), y = 13 * (5 - 6) - 1 = 3 (mod 17)
        Assert.Equal(Point(6I, 3I), double toy3 (Point(5I, 1I)))

        // add (5, 1) (6, 3): slope = (3 - 1) * (6 - 5)^(-1) = 2 (mod 17)
        // x = 4 - 5 - 6 = 10 (mod 17), y = 2 * (5 - 10) - 1 = 6 (mod 17)
        Assert.Equal(Point(10I, 6I), add toy3 (Point(5I, 1I)) (Point(6I, 3I)))

        // jInvariant: Delta = -16 * 4 = 4, (4a)^3 = 512 = 2, 4^(-1) = 13, -1728 = 6,
        // so j = 6 * 2 * 13 = 3 (mod 17).
        Assert.Equal(Some 3I, jInvariant toy3)
