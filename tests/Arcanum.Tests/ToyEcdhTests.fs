namespace Arcanum.Tests

open System.Numerics
open Xunit
open Arcanum.Core
open Arcanum.Asymmetric

module ToyEcdhTests =

    // EllipticCurve.toy is a pedagogical curve over F_17; toy is not a secure group.
    [<Fact>]
    let ``Phase 25 - ECDH shared secret equality and repeated addition on toy curve`` () =
        let curve = EllipticCurve.toy
        let privA = 3I
        let privB = 5I

        let pubA = EllipticCurve.scalarMultiply curve privA curve.G
        let pubB = EllipticCurve.scalarMultiply curve privB curve.G

        let sharedA = DiffieHellman.computeEcdhSharedSecret curve privA pubB
        let sharedB = DiffieHellman.computeEcdhSharedSecret curve privB pubA

        // 1. The two shared points are equal
        Assert.Equal(sharedA, sharedB)

        // 2. That point equals scalarMultiply toy (3 * 5) toy.G
        let expectedScalar = EllipticCurve.scalarMultiply curve (3I * 5I) curve.G
        Assert.Equal(expectedScalar, sharedA)

        // 3. It also equals repeated addition of G, fifteen times
        let repeatedAdd =
            [ 1 .. 15 ]
            |> List.fold (fun acc _ -> EllipticCurve.add curve acc curve.G) EllipticCurve.Infinity
        Assert.Equal(repeatedAdd, sharedA)

    [<Fact>]
    let ``Phase 107 - ECDH agreement on every pair of private keys on prime-order toy3`` () =
        let curve = EllipticCurve.toy3
        for d1 in 1I .. 18I do
            let pub1 = EllipticCurve.scalarMultiply curve d1 curve.G
            for d2 in 1I .. 18I do
                let pub2 = EllipticCurve.scalarMultiply curve d2 curve.G
                let s1 = DiffieHellman.computeEcdhSharedSecret curve d1 pub2
                let s2 = DiffieHellman.computeEcdhSharedSecret curve d2 pub1
                let expected = EllipticCurve.scalarMultiply curve (Modular.modPos (d1 * d2) 19I) curve.G
                Assert.NotEqual(EllipticCurve.Infinity, expected)
                Assert.Equal(expected, s1)
                Assert.Equal(expected, s2)
                Assert.Equal(Ok expected, DiffieHellman.computeEcdhSharedSecretChecked curve d1 pub2)
                Assert.Equal(Ok expected, DiffieHellman.computeEcdhSharedSecretChecked curve d2 pub1)
