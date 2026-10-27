namespace Arcanum.Tests

open System.Numerics
open Xunit
open Arcanum.Core

module LatticeTests =

    // The basis rows are (2, 1) and (1, 2). The lattice is a matrix of integers, not an encryption scheme.

    [<Fact>]
    let ``Phase 33 - Determinant of the basis is 3`` () =
        // 2 * 2 - 1 * 1 = 3
        Assert.Equal(3I, Lattice.determinant Lattice.basis)

    [<Fact>]
    let ``Phase 33 - Gram matrix of the basis`` () =
        // (2, 1) . (2, 1) = 5, (2, 1) . (1, 2) = 4, (1, 2) . (1, 2) = 5
        let expected: BigInteger list list = [ [ 5I; 4I ]; [ 4I; 5I ] ]
        Assert.Equal<BigInteger list list>(expected, Lattice.gram Lattice.basis)
