namespace Arcanum.Tests

open Xunit
open Arcanum.Classical

module ClassicalLawsTests =

    [<Fact>]
    let ``Phase 28 - Caesar and Atbash hand checks`` () =
        Assert.Equal("N", Substitution.caesarEncrypt 13 "A")
        Assert.Equal("URYYB", Substitution.caesarEncrypt 13 "HELLO")
        Assert.Equal("Z", Substitution.atbash "A")
        Assert.Equal("A", Substitution.atbash "Z")

    [<Fact>]
    let ``Phase 29 - Affine cipher encryption, decryption, and non-coprime rejection`` () =
        // (5 * 0 + 8) mod 26 = 8 -> 'I'
        Assert.Equal(Ok "I", Substitution.affineEncrypt 5 8 "A")
        Assert.Equal(Ok "A", Substitution.affineDecrypt 5 8 "I")

        // 2 is not coprime to 26
        Assert.True(Result.isError (Substitution.affineEncrypt 2 1 "A"))

    [<Fact>]
    let ``Phase 30 - Vigenere tableau encryption, decryption, and non-alphabetic key rejection`` () =
        Assert.Equal(Ok "RIJVS", Polyalphabetic.vigenereEncrypt "KEY" "HELLO")
        Assert.Equal(Ok "HELLO", Polyalphabetic.vigenereDecrypt "KEY" "RIJVS")
        Assert.True(Result.isError (Polyalphabetic.vigenereEncrypt "123" "HELLO"))

    [<Fact>]
    let ``Phase 31 - Playfair square for MONARCHY and same-row pair`` () =
        // The square merges J into I.
        let matrix = Playfair.createMatrix "MONARCHY"
        let row0 = [| for col in 0 .. 4 -> matrix.Grid.[0, col] |]
        Assert.Equal<char>([| 'M'; 'O'; 'N'; 'A'; 'R' |], row0)

        // M and O share row 0, so each letter steps one column right.
        Assert.Equal("ON", Playfair.playfairEncrypt "MONARCHY" "MO")
        Assert.Equal("MO", Playfair.playfairDecrypt "MONARCHY" "ON")


