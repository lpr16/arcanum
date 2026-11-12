namespace Arcanum.Tests

open System
open Xunit
open Arcanum.Hashing

module HkdfTests =

    [<Fact>]
    let ``Phase 139 - RFC 5869 Appendix A.1 test vector with SHA-256`` () =
        let ikm = Convert.FromHexString("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b")
        let salt = Convert.FromHexString("000102030405060708090a0b0c")
        let info = Convert.FromHexString("f0f1f2f3f4f5f6f7f8f9")
        let length = 42

        let expectedPrk = Convert.FromHexString("077709362c2e32df0ddc3f0dc47bba6390b6c73bb50f9c3122ec844ad7c2b3e5")
        let expectedOkm = Convert.FromHexString("3cb25f25faacd57a90434f64d0362f2a2d2d0a90cf1a5a4c5db02d56ecc4c5bf34007208d5b887185865")

        let prk = HKDF.extract salt ikm
        Assert.Equal<byte[]>(expectedPrk, prk)

        let okmResult = HKDF.expand prk info length
        Assert.Equal(Ok expectedOkm, okmResult)
