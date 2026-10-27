namespace Arcanum.Core

open System.Numerics

/// One 2-by-2 integer lattice basis.
/// The lattice is a matrix of integers. It is not an encryption scheme.
module Lattice =
    /// A 2-by-2 basis with rows (a, b) and (c, d).
    type Basis = {
        Row0: BigInteger * BigInteger
        Row1: BigInteger * BigInteger
    }

    /// The basis with rows (2, 1) and (1, 2).
    let basis : Basis = { Row0 = (2I, 1I); Row1 = (1I, 2I) }

    /// Determinant of the 2-by-2 basis with rows (a, b) and (c, d): ad - bc.
    let determinant (rows: Basis) : BigInteger =
        let (a, b) = rows.Row0
        let (c, d) = rows.Row1
        a * d - b * c

    /// Gram matrix of the basis: entry (i, j) is the integer dot product of row i and row j.
    let gram (rows: Basis) : BigInteger list list =
        let dot (x1: BigInteger, y1: BigInteger) (x2: BigInteger, y2: BigInteger) = x1 * x2 + y1 * y2
        let all = [ rows.Row0; rows.Row1 ]
        [ for ri in all -> [ for rj in all -> dot ri rj ] ]
