namespace Arcanum.Classical

open System

module Enigma =
    type RotorType = RotorI | RotorII | RotorIII | RotorIV | RotorV
    type ReflectorType = ReflectorB | ReflectorC

    type RotorConfig = {
        RotorType: RotorType
        RingSetting: int      // Ringstellung 0..25 (1..26 in German manuals)
        Position: int         // Grundstellung 0..25 ('A'..'Z')
    }

    type MachineConfig = {
        Left: RotorConfig
        Middle: RotorConfig
        Right: RotorConfig
        Reflector: ReflectorType
        Plugboard: (char * char) list
    }

    let private rotorWiring (rt: RotorType) : string * int =
        match rt with
        | RotorI   -> ("EKMFLGDQVZNTOWYHXUSPAIBRCJ", int 'Q' - int 'A')
        | RotorII  -> ("AJDKSIRUXBLHWTMCQGZNPYFVOE", int 'E' - int 'A')
        | RotorIII -> ("BDFHJLCPRTXVZNYEIWGAKMUSQO", int 'V' - int 'A')
        | RotorIV  -> ("ESOVPZJAYQUIRHXLNFTGKDCMWB", int 'J' - int 'A')
        | RotorV   -> ("VZBRGITYUPSDNHLXAWMJQOFECK", int 'Z' - int 'A')

    let private reflectorWiring (ref: ReflectorType) : string =
        match ref with
        | ReflectorB -> "YRUHQSLDPXNGOKMIEBFZCWVJAT"
        | ReflectorC -> "FVPJIAOYEDRZXWGCTKUQSBNMHL"

    let private buildPlugboardMap (pairs: (char * char) list) : Map<int, int> =
        let mutable m = Map.empty
        for (a, b) in pairs do
            let ia = int (Char.ToUpperInvariant a) - int 'A'
            let ib = int (Char.ToUpperInvariant b) - int 'A'
            m <- Map.add ia ib (Map.add ib ia m)
        m

    /// Steps the rotors according to authentic Wehrmacht double-stepping anomaly.
    let stepRotors (left: RotorConfig) (middle: RotorConfig) (right: RotorConfig) : RotorConfig * RotorConfig * RotorConfig =
        let (_, rNotch) = rotorWiring right.RotorType
        let (_, mNotch) = rotorWiring middle.RotorType

        let rightAtNotch = right.Position = rNotch
        let middleAtNotch = middle.Position = mNotch

        // Step rules:
        // Right rotor always steps.
        // Middle rotor steps if right rotor was at notch, OR if middle rotor is at notch (double stepping).
        // Left rotor steps if middle rotor is at notch.
        let newLeftPos =
            if middleAtNotch then (left.Position + 1) % 26
            else left.Position

        let newMiddlePos =
            if rightAtNotch || middleAtNotch then (middle.Position + 1) % 26
            else middle.Position

        let newRightPos = (right.Position + 1) % 26

        let newLeft = { left with Position = newLeftPos }
        let newMiddle = { middle with Position = newMiddlePos }
        let newRight = { right with Position = newRightPos }

        (newLeft, newMiddle, newRight)

    let private passForward (rotor: RotorConfig) (inputPin: int) : int =
        let (wiring, _) = rotorWiring rotor.RotorType
        let offset = ((rotor.Position - rotor.RingSetting) % 26 + 26) % 26
        let shiftedIn = (inputPin + offset) % 26
        let wiredOut = int wiring.[shiftedIn] - int 'A'
        ((wiredOut - offset) % 26 + 26) % 26

    let private passBackward (rotor: RotorConfig) (inputPin: int) : int =
        let (wiring, _) = rotorWiring rotor.RotorType
        let offset = ((rotor.Position - rotor.RingSetting) % 26 + 26) % 26
        let shiftedIn = (inputPin + offset) % 26
        let wiredOut = wiring.IndexOf(char (shiftedIn + int 'A'))
        ((wiredOut - offset) % 26 + 26) % 26

    let private passReflector (ref: ReflectorType) (inputPin: int) : int =
        let wiring = reflectorWiring ref
        int wiring.[inputPin] - int 'A'

    /// Encrypts or decrypts a single character through the Enigma machine.
    let processChar (config: MachineConfig) (c: char) : MachineConfig * char =
        if not (Char.IsLetter c) then
            (config, c)
        else
            // 1. Step rotors BEFORE encrypting key
            let (newLeft, newMiddle, newRight) = stepRotors config.Left config.Middle config.Right
            let newConfig = { config with Left = newLeft; Middle = newMiddle; Right = newRight }

            let plugboardMap = buildPlugboardMap config.Plugboard
            let charIndex = int (Char.ToUpperInvariant c) - int 'A'

            // 2. Input through plugboard
            let afterPlugIn = Map.tryFind charIndex plugboardMap |> Option.defaultValue charIndex

            // 3. Forward through rotors: Right -> Middle -> Left
            let afterR = passForward newRight afterPlugIn
            let afterM = passForward newMiddle afterR
            let afterL = passForward newLeft afterM

            // 4. Reflector
            let afterRef = passReflector config.Reflector afterL

            // 5. Reverse through rotors: Left -> Middle -> Right
            let revL = passBackward newLeft afterRef
            let revM = passBackward newMiddle revL
            let revR = passBackward newRight revM

            // 6. Output through plugboard
            let afterPlugOut = Map.tryFind revR plugboardMap |> Option.defaultValue revR
            let outChar = char (afterPlugOut + int 'A')

            (newConfig, outChar)

    /// Processes an entire message through the Enigma machine.
    let processMessage (config: MachineConfig) (text: string) : string =
        let mutable currentConfig = config
        let result = System.Text.StringBuilder()

        for c in text do
            if Char.IsLetter c then
                let (nextConfig, outC) = processChar currentConfig c
                currentConfig <- nextConfig
                result.Append(outC) |> ignore
            else
                result.Append(c) |> ignore

        result.ToString()
