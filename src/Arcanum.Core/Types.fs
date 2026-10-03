namespace Arcanum.Core

open System.Numerics

module Units =
    [<Measure>] type bit
    [<Measure>] type byteUnit

/// Strongly-typed wrappers to avoid accidental confusion between sensitive byte sequences.
[<Struct>]
type Plaintext = Plaintext of byte[]
with
    member this.Value = match this with Plaintext b -> b
    member this.Length = this.Value.Length

[<Struct>]
type Ciphertext = Ciphertext of byte[]
with
    member this.Value = match this with Ciphertext b -> b
    member this.Length = this.Value.Length

[<Struct>]
type Key = Key of byte[]
with
    member this.Value = match this with Key b -> b
    member this.Length = this.Value.Length

[<Struct>]
type Nonce = Nonce of byte[]
with
    member this.Value = match this with Nonce b -> b
    member this.Length = this.Value.Length

[<Struct>]
type Iv = Iv of byte[]
with
    member this.Value = match this with Iv b -> b
    member this.Length = this.Value.Length

[<Struct>]
type Tag = Tag of byte[]
with
    member this.Value = match this with Tag b -> b
    member this.Length = this.Value.Length

[<Struct>]
type Digest = Digest of byte[]
with
    member this.Value = match this with Digest b -> b
    member this.Length = this.Value.Length

/// ECDSA / Digital Signature representation (r, s).
type EcdsaSignature = {
    R: BigInteger
    S: BigInteger
}

/// Comprehensive, strongly-typed errors for cryptographic operations.
type CryptoError =
    | InvalidKeyLength of expected: int * actual: int
    | InvalidNonceLength of expected: int * actual: int
    | InvalidIvLength of expected: int * actual: int
    | InvalidTagLength of expected: int * actual: int
    | InvalidBlockSize of expected: int * actual: int
    | DecryptionFailed of message: string
    | AuthenticationFailed
    | InvalidPadding
    | NonInvertibleElement of string
    | ParameterOutOfRange of string
    | KeyGenerationFailed of string
    | CorruptedState of string

module CryptoResult =
    let map f r = Result.map f r
    let bind f r = Result.bind f r
    let mapError f r = Result.mapError f r
