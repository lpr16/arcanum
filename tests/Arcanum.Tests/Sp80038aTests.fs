namespace Arcanum.Tests

open Xunit
open Arcanum.Core
open Arcanum.Symmetric

module Sp80038aTests =

    // NIST SP 800-38A Appendix F cited example vectors, not a NIST CAVP vector file.

    let private sp80038aKey = Bytes.fromHex "2b7e151628aed2a6abf7158809cf4f3c"

    let private sp80038aPlaintext =
        Bytes.fromHex (
            "6bc1bee22e409f96e93d7e117393172a" +
            "ae2d8a571e03ac9c9eb76fac45af8e51" +
            "30c81c46a35ce411e5fbc1191a0a52ef" +
            "f69f2445df4f9b17ad2b417be66c3710")

    [<Fact>]
    let ``Phase 87 - SP 800-38A section F.1.1 ECB-AES128 and section F.2.1 CBC-AES128`` () =
        let key = Key sp80038aKey
        let pt = Plaintext sp80038aPlaintext

        // SP 800-38A section F.1.1: ECB-AES128.Encrypt
        let expectedEcb64 =
            Bytes.fromHex (
                "3ad77bb40d7a3660a89ecaf32466ef97" +
                "f5d3d58503b9699de785895a96fdbaaf" +
                "43b1cd7f598ece23881b00e3ed030688" +
                "7b0c785e27e8ad3f8223207104725dd4")

        let ecbCt = Modes.encryptEcb key pt
        Assert.Equal(80, ecbCt.Length)
        Assert.Equal<byte>(expectedEcb64, ecbCt.Value.[0 .. 63])

        match Modes.decryptEcb key ecbCt with
        | Ok dec -> Assert.Equal<byte>(sp80038aPlaintext, dec.Value)
        | Error err -> Assert.Fail(sprintf "Unexpected ECB decrypt error: %A" err)

        // SP 800-38A section F.2.1: CBC-AES128.Encrypt
        let iv = Iv (Bytes.fromHex "000102030405060708090a0b0c0d0e0f")
        let expectedCbc64 =
            Bytes.fromHex (
                "7649abac8119b246cee98e9b12e9197d" +
                "5086cb9b507219ee95db113a917678b2" +
                "73bed6b8e3c1743b7116e69e22229516" +
                "3ff1caa1681fac09120eca307586e1a7")

        match Modes.encryptCbc key iv pt with
        | Ok cbcCt ->
            Assert.Equal(80, cbcCt.Length)
            Assert.Equal<byte>(expectedCbc64, cbcCt.Value.[0 .. 63])
            match Modes.decryptCbc key iv cbcCt with
            | Ok dec -> Assert.Equal<byte>(sp80038aPlaintext, dec.Value)
            | Error err -> Assert.Fail(sprintf "Unexpected CBC decrypt error: %A" err)
        | Error err -> Assert.Fail(sprintf "Unexpected CBC encrypt error: %A" err)
