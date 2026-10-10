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
