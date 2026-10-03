namespace Arcanum.Tests

open Xunit
open Arcanum.Core

module ToyFieldTests =
    [<Fact>]
    let ``Residues are canonical modulo seventeen`` () =
        for value in 0 .. 16 do
            let residue = ToyField.ofBigInteger (bigint value)
            Assert.Equal(bigint value, ToyField.toBigInteger residue)

        Assert.Equal(0I, ToyField.ofBigInteger 17I |> ToyField.toBigInteger)
        Assert.Equal(16I, ToyField.ofBigInteger -1I |> ToyField.toBigInteger)

    [<Fact>]
    let ``Inverse of three is six`` () =
        let three = ToyField.ofBigInteger 3I
        let six = ToyField.ofBigInteger 6I
        Assert.Equal(Some six, ToyField.inverse three)
        Assert.Equal(ToyField.one, ToyField.multiply three six)

    [<Fact>]
    let ``Zero has no multiplicative inverse`` () =
        Assert.Equal(None, ToyField.inverse ToyField.zero)

    [<Fact>]
    let ``Every nonzero residue has a multiplicative inverse`` () =
        for residue in ToyField.residues |> Array.skip 1 do
            match ToyField.inverse residue with
            | None -> Assert.Fail($"No inverse for {ToyField.toBigInteger residue}")
            | Some inverse ->
                Assert.Equal(ToyField.one, ToyField.multiply residue inverse)

    [<Fact>]
    let ``Every nonzero residue satisfies Fermat's little theorem`` () =
        for value in 1 .. 16 do
            Assert.Equal(1I, Modular.modPow (bigint value) 16I ToyField.prime)
