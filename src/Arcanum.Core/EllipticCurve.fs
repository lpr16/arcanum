namespace Arcanum.Core

open System
open System.Numerics

module EllipticCurve =
    [<CustomEquality; NoComparison>]
    type ECPoint =
        | Infinity
        | Point of x: BigInteger * y: BigInteger
        with
            override this.Equals(other) =
                match other with
                | :? ECPoint as o ->
                    match this, o with
                    | Infinity, Infinity -> true
                    | Point(x1, y1), Point(x2, y2) -> x1 = x2 && y1 = y2
                    | _ -> false
                | _ -> false

            override this.GetHashCode() =
                match this with
                | Infinity -> 0
                | Point(x, y) -> HashCode.Combine(x, y)

    /// Weierstrass form: y^2 = x^3 + a*x + b (mod p)
    type Curve = {
        P: BigInteger  // Field prime modulus
        A: BigInteger  // Coefficient a
        B: BigInteger  // Coefficient b
        G: ECPoint     // Base generator point
        N: BigInteger  // Order of G
        H: BigInteger  // Cofactor
    }

    /// Verifies if a point lies on the elliptic curve.
    let isOnCurve (curve: Curve) (pt: ECPoint) : bool =
        match pt with
        | Infinity -> true
        | Point(x, y) ->
            let left = Modular.modPow y 2I curve.P
            let right =
                Modular.modPos (
                    Modular.modPow x 3I curve.P +
                    Modular.modPos (curve.A * x) curve.P +
                    curve.B
                ) curve.P
            left = right

    /// Point negation: -(x, y) = (x, -y mod p)
    let negate (curve: Curve) (pt: ECPoint) : ECPoint =
        match pt with
        | Infinity -> Infinity
        | Point(x, y) -> Point(x, Modular.modPos (-y) curve.P)

    /// Point addition on Weierstrass curve: P + Q
    let add (curve: Curve) (p1: ECPoint) (p2: ECPoint) : ECPoint =
        match p1, p2 with
        | Infinity, q -> q
        | p, Infinity -> p
        | Point(x1, y1), Point(x2, y2) ->
            if x1 = x2 then
                if Modular.modPos (y1 + y2) curve.P = BigInteger.Zero then
                    Infinity // P + (-P) = O
                else
                    // Point doubling: lambda = (3*x1^2 + a) / (2*y1) mod p
                    let num = Modular.modPos (3I * Modular.modPow x1 2I curve.P + curve.A) curve.P
                    let denom = Modular.modPos (2I * y1) curve.P
                    match Modular.modInverse denom curve.P with
                    | None -> Infinity
                    | Some invDenom ->
                        let lambda = Modular.modPos (num * invDenom) curve.P
                        let x3 = Modular.modPos (Modular.modPow lambda 2I curve.P - 2I * x1) curve.P
                        let y3 = Modular.modPos (lambda * (x1 - x3) - y1) curve.P
                        Point(x3, y3)
            else
                // Distinct points: lambda = (y2 - y1) / (x2 - x1) mod p
                let num = Modular.modPos (y2 - y1) curve.P
                let denom = Modular.modPos (x2 - x1) curve.P
                match Modular.modInverse denom curve.P with
                | None -> Infinity
                | Some invDenom ->
                    let lambda = Modular.modPos (num * invDenom) curve.P
                    let x3 = Modular.modPos (Modular.modPow lambda 2I curve.P - x1 - x2) curve.P
                    let y3 = Modular.modPos (lambda * (x1 - x3) - y1) curve.P
                    Point(x3, y3)

    /// Point doubling: 2 * P
    let double (curve: Curve) (pt: ECPoint) : ECPoint =
        add curve pt pt

    /// Scalar multiplication using double-and-add algorithm: k * P.
    /// The scalar is reduced modulo Curve.N, and for toy, secp256k1, and nistP256 that N is the order of G.
    /// On toy this is the order of the whole group, so k * Q and (k mod N) * Q are the same point for every Q on the curve.
    let scalarMultiply (curve: Curve) (kVal: BigInteger) (pt: ECPoint) : ECPoint =
        let k = Modular.modPos kVal curve.N
        if k = BigInteger.Zero || pt = Infinity then
            Infinity
        else
            let mutable res = Infinity
            let mutable addend = pt
            let mutable scalar = k

            while scalar > BigInteger.Zero do
                if (scalar &&& BigInteger.One) = BigInteger.One then
                    res <- add curve res addend
                addend <- double curve addend
                scalar <- scalar >>> 1

            res

    /// Standard curve parameters for secp256k1 (Bitcoin, Ethereum).
    let secp256k1 : Curve =
        let p = BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F", Globalization.NumberStyles.HexNumber)
        let a = BigInteger.Zero
        let b = 7I
        let gx = BigInteger.Parse("079BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", Globalization.NumberStyles.HexNumber)
        let gy = BigInteger.Parse("0483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", Globalization.NumberStyles.HexNumber)
        let n = BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141", Globalization.NumberStyles.HexNumber)
        let h = BigInteger.One
        {
            P = p
            A = a
            B = b
            G = Point(gx, gy)
            N = n
            H = h
        }

    /// Standard curve parameters for NIST P-256 (secp256r1).
    let nistP256 : Curve =
        let p = BigInteger.Parse("0FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFF", Globalization.NumberStyles.HexNumber)
        let a = Modular.modPos -3I p
        let b = BigInteger.Parse("05AC635D8AA3A93E7B3EBBD55769886BC651D06B0CC53B0F63BCE3C3E27D2604B", Globalization.NumberStyles.HexNumber)
        let gx = BigInteger.Parse("06B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", Globalization.NumberStyles.HexNumber)
        let gy = BigInteger.Parse("04FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", Globalization.NumberStyles.HexNumber)
        let n = BigInteger.Parse("0FFFFFFFF00000000FFFFFFFFFFFFFFFFBCE6FAADA7179E84F3B9CAC2FC632551", Globalization.NumberStyles.HexNumber)
        let h = BigInteger.One
        {
            P = p
            A = a
            B = b
            G = Point(gx, gy)
            N = n
            H = h
        }

    /// Toy curve y^2 = x^3 + 1 over F_17.
    /// This toy curve is not a secure group.
    let toy : Curve =
        {
            P = ToyField.prime
            A = 0I
            B = 1I
            G = Point(6I, 8I)
            N = 18I
            H = 1I
        }
