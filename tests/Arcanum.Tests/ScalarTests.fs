namespace Arcanum.Tests

open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Core.EllipticCurve

module ScalarTests =
    let enumeratedPoints : ECPoint list =
        Infinity :: [
            for x in 0I .. (toy.P - 1I) do
                for y in 0I .. (toy.P - 1I) do
                    let pt = Point(x, y)
                    if isOnCurve toy pt then
                        yield pt
        ]

    let repeatedAdd (pt: ECPoint) (k: int) : ECPoint =
        let mutable res = Infinity
        for _ in 1 .. k do
            res <- add toy res pt
        res

    let pointOrder (pt: ECPoint) : int =
        if pt = Infinity then 1
        else
            let rec loop acc count =
                if count > 18 then
                    failwith "Order exceeded group bound"
                elif acc = Infinity then
                    count
                else
                    loop (add toy acc pt) (count + 1)
            loop pt 1

    [<Fact>]
    let ``Repeated addition of generator G reaches Infinity at step 18 and not earlier`` () =
        for step in 1 .. 17 do
            Assert.NotEqual(Infinity, repeatedAdd toy.G step)
        Assert.Equal(Infinity, repeatedAdd toy.G 18)
        Assert.Equal(bigint 18, toy.N)
        Assert.Equal(1I, toy.H)

    [<Fact>]
    let ``Partial sums of generator G generate the full enumerated set`` () =
        let partialSums = [ for k in 0 .. 17 -> repeatedAdd toy.G k ]
        Assert.Equal(18, partialSums.Length)
        for pt in enumeratedPoints do
            Assert.Contains(pt, partialSums)
        for sum in partialSums do
            Assert.Contains(sum, enumeratedPoints)

    [<Fact>]
    let ``Point (0, 1) order is three and every point order divides 18`` () =
        Assert.NotEqual(Infinity, repeatedAdd (Point(0I, 1I)) 1)
        Assert.NotEqual(Infinity, repeatedAdd (Point(0I, 1I)) 2)
        Assert.Equal(Infinity, repeatedAdd (Point(0I, 1I)) 3)

        for pt in enumeratedPoints do
            let order = pointOrder pt
            Assert.True(18 % order = 0, $"Order {order} does not divide 18 for {pt}")

    [<Fact>]
    let ``Scalar multiplication agrees with repeated addition for all points and residues 0 to 17`` () =
        for q in enumeratedPoints do
            for k in 0 .. 17 do
                let expected = repeatedAdd q k
                let actual = scalarMultiply toy (bigint k) q
                Assert.Equal(expected, actual)

    [<Fact>]
    let ``Scalar multiplication by 18 yields Infinity via scalar reduction modulo N`` () =
        // scalarMultiply reduces the scalar modulo curve.N (18 mod 18 = 0), yielding Infinity directly.
        // This test case demonstrates scalar reduction, not the proof of point order.
        // The point order proof is established by repeated addition without modular reduction.
        for q in enumeratedPoints do
            Assert.Equal(Infinity, scalarMultiply toy 18I q)
