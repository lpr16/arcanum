namespace Arcanum.Tests

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
