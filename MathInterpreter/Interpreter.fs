﻿namespace MathInterpreter

    // Grammar in BNF:
    // <E>        ::= <Assignment> | <T> <Eopt>
    // <Assignment>::= "Var" <name> "=" <T> <Eopt>
    // <Eopt>     ::= "+" <T> <Eopt> | "-" <T> <Eopt> | <empty>
    // <T>        ::= <P> <Topt>
    // <Topt>     ::= "*" <P> <Topt> | "/" <P> <Topt> | "%" <P> <Topt> | <empty>
    // <P>        ::= <NR> <Popt>
    // <Popt>     ::= "^" <NR> <Popt> | <empty>
    // <NR>       ::= "Num" <value> | "~" <NR> | "(" <E> ")" | "Var" <name> | "sin" <NR> | "cos" <NR>


module Interpreter =

    open System
    open System.Text.RegularExpressions
    let mutable variables : System.Collections.Generic.Dictionary<string, float> option = None
    let getVariables () =
        match variables with
        | Some dict -> dict
        | None -> 
            let dict = System.Collections.Generic.Dictionary<string, float>()
            dict.Add("x", 0.0)
            dict.Add("y", 0.0)
            variables <- Some dict
            dict
    type terminal = 
        Add | Sub | Mul | Function | Div | Lpar | Rpar | Pow | Rem | Neg | Sin | Cos | Num of float | Equals | Var of string

    let str2lst s = [for c in s -> c]
    let isblank c = System.Char.IsWhiteSpace c
    let isdigit c = System.Char.IsDigit c
    let intVal (c:char) = float ((int)c - (int)'0')

    let lexError c = System.Exception($"Lexer error at character: {c}")
    let parseError msg = System.Exception($"Parser error: {msg}")

    let UnaryMinus (input: string) : string =           // This function makes unary minus easier to detect by lexer
        let pattern = @"(?<![\d\)]+\s*)-"
        let replacement = "~"
        Regex.Replace(input, pattern, replacement)

    let MultFix (input: string) : string =              // This function allows lexer to understand mutiplication with brackets such as a(b) = a*(b)
        let pattern = @"(?<=[\d])\("
        let replacement = "*("
        Regex.Replace(input, pattern, replacement)

    let Trig1 (input: string) : string =                // lets lexer read sin/cos
        let pattern = @"cos"
        let replacement = "C"
        Regex.Replace(input, pattern, replacement)
    let Trig2 (input: string) : string =
        let pattern = @"sin"
        let replacement = "S"
        Regex.Replace(input, pattern, replacement)

    let rec scNum (iStr, iVal) =
        match iStr with
        | '.' :: tail -> let (iStr, fracVal, divisor) = scFrac(tail, 0.0, 0.1)
                         (iStr, iVal + fracVal)
        | c :: tail when isdigit c -> scNum(tail, 10.0 * iVal + float (intVal c))
        | _ -> (iStr, iVal)
    and scFrac (iStr, fracVal, divisor) =
      match iStr with
        | c :: tail when isdigit c -> scFrac(tail, fracVal + (float (intVal c)) * divisor, divisor / 10.0)
        | _ -> (iStr, fracVal, divisor)


    let lexer input = 
        let rec scan input =
            match input with
            | [] -> []
            | '+'::tail -> Add :: scan tail
            | '-'::tail -> Sub :: scan tail
            | '*'::tail -> Mul :: scan tail
            | '/'::tail -> Div :: scan tail
            | '('::tail -> Lpar:: scan tail
            | ')'::tail -> Rpar:: scan tail
            | '~'::tail -> Neg:: scan tail
            | '^'::tail -> Pow:: scan tail
            | '%'::tail -> Rem:: scan tail
            | '='::tail -> Equals :: scan tail
            | 'S'::tail -> Sin :: scan tail
            | 'C'::tail -> Cos :: scan tail
            | c :: tail when isblank c -> scan tail
            | c :: tail when isdigit c || c = '.' ->
                let (iStr, iVal) = scNum(c::tail, 0.0) 
                Num iVal :: scan iStr
            | 'x' :: tail -> Var "x" :: scan tail
            | 'y' :: tail -> Var "y" :: scan tail
            | c :: _ -> raise (lexError c)
        scan (str2lst input)

    let parseNeval tList = 
        let rec E tList = 
            match tList with
            | Var varName :: Equals :: tail when varName = "x" || varName = "y" ->
                let (tList, value) = E tail
                getVariables().[varName] <- value
                (tList, value)
            | _ -> (T >> Eopt) tList
        and Eopt (tList, value) = 
            match tList with
            | Add :: tail -> let (tLst, tval) = T tail
                             Eopt (tLst, value + tval)
            | Sub :: tail -> let (tLst, tval) = T tail
                             Eopt (tLst, value - tval)
            | _ -> (tList, value)
        and T tList = (P >> Topt) tList
        and Topt (tList, value) =
            match tList with
            | Mul :: tail -> let (tLst, tval) = P tail
                             Topt (tLst, value * tval)
            | Div :: tail -> let (tLst, tval) = P tail
                             if (tval = 0.0) then
                                raise (parseError "Can't divide by zero")
                             if (floor(value) = value && floor(tval) = tval) then
                                Topt (tLst, floor(value / tval))
                             else
                                Topt (tLst, value / tval)           
            | Rem :: tail -> let (tLst, tval) = P tail
                             if (tval = 0.0) then
                                raise (parseError "Can't divide by zero")
                             Topt (tLst, value % tval)
            | _ -> (tList, value)
        and P tList = (NR >> Popt) tList
        and Popt (tList, value) =
            match tList with
            | Pow :: tail -> let (tLst, tval) = NR tail
                             Popt (tLst, Math.Pow(value, tval))
            | _ -> (tList, value)
        and NR tList =
            match tList with 
            | Num value :: tail -> (tail, value)
            | Var varName :: tail when varName = "x" || varName = "y" ->
                if getVariables().ContainsKey(varName) then
                    (tail, getVariables().[varName])
                else
                    raise (parseError $"Variable '{varName}' not defined.")
            | Neg :: tail -> let (tLst, tval) = NR tail
                             let (tLst, tval) = Popt (tLst, tval)
                             (tLst, -1.0 * tval) 
            | Cos :: tail -> let (tLst, tval) = NR tail
                             let (tLst, tval) = Popt (tLst, tval)
                             (tLst, cos(tval))
            | Sin :: tail -> let (tLst, tval) = NR tail
                             let (tLst, tval) = Popt (tLst, tval)
                             (tLst, sin(tval))         
            | Lpar :: tail -> let (tLst, tval) = E tail
                              match tLst with 
                              | Rpar :: tail -> (tail, tval)
                              | _ -> raise (parseError "Missing closing parenthesis")
            | _ -> raise (parseError "Unexpected token")
        E tList

    let interpret (input: string) =
        let variables = getVariables()
        let input = UnaryMinus input;
        let input = MultFix input;
        let input = Trig1 input;
        let input = Trig2 input;
        let oList = lexer input
        let Out = parseNeval oList
        snd Out
