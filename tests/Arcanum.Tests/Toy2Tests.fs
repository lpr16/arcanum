namespace Arcanum.Tests

open Xunit
open Arcanum.Core
open Arcanum.Core.EllipticCurve

module Toy2Tests =
    // EllipticCurve.toy2 is y^2 = x^3 + x over F_17. Its group of 16 points is not cyclic,
    // G = (1, 6) does not generate it, and toy2 is not a secure group.
    let enumeratedPoints : ECPoint list =
        Infinity :: [
            for x in 0I .. (toy2.P - 1I) do
                for y in 0I .. (toy2.P - 1I) do
                    let pt = Point(x, y)
                    if isOnCurve toy2 pt then
                        yield pt
        ]

    let repeatedAdd (pt: ECPoint) (k: int) : ECPoint =
        let mutable res = Infinity
        for _ in 1 .. k do
            res <- add toy2 res pt
        res

    [<Fact>]
    let ``Phase 38 - toy2 is non-singular and enumeration finds 16 points`` () =
        // 4 * 1^3 + 27 * 0^2 = 4, which is not 0 modulo 17.
        Assert.Equal(4I, Modular.modPos (4I * Modular.modPow toy2.A 3I toy2.P + 27I * Modular.modPow toy2.B 2I toy2.P) toy2.P)
        Assert.Equal(16, enumeratedPoints.Length)
        Assert.Contains(Infinity, enumeratedPoints)

    [<Fact>]
    let ``Phase 38 - G has order 4 and (0, 0) has order 2`` () =
        for step in 1 .. 3 do
            Assert.NotEqual(Infinity, repeatedAdd toy2.G step)
        Assert.Equal(Infinity, repeatedAdd toy2.G 4)

        Assert.NotEqual(Infinity, repeatedAdd (Point(0I, 0I)) 1)
        Assert.Equal(Infinity, repeatedAdd (Point(0I, 0I)) 2)

    [<Fact>]
    let ``Phase 38 - The subgroup of G has 4 points, not 16`` () =
        let subgroup = [ for k in 0 .. 3 -> repeatedAdd toy2.G k ] |> List.distinct
        Assert.Equal(4, subgroup.Length)
        Assert.NotEqual(enumeratedPoints.Length, subgroup.Length)

    [<Fact>]
    let ``Phase 38 - Chord addition on toy2 matches manual slope derivation`` () =
        // (1, 6) + (6, 1): slope = (1 - 6) * (6 - 1)^(-1) = -5 * 5^(-1) = -1 = 16 (mod 17)
        // x = 16^2 - 1 - 6 = 249 = 11 (mod 17)
        // y = 16 * (1 - 11) - 6 = -166 = 4 (mod 17)
        Assert.Equal(Point(11I, 4I), add toy2 (Point(1I, 6I)) (Point(6I, 1I)))

    [<Fact>]
    let ``Phase 38 - j-invariant of toy2 is 1728 modulo 17`` () =
        // y^2 = x^3 + x has j = 1728, and 1728 = 11 (mod 17).
        // By hand: Delta = -16 * 4 = 4, (4a)^3 = 64 = 13, 4^(-1) = 13, -1728 = 6 (mod 17),
        // so j = 6 * 13 * 13 = 11 (mod 17).
        Assert.Equal(Some 11I, jInvariant toy2)
