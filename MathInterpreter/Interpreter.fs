namespace MathInterpreter

module Interpreter =

    open System

    type terminal = 
        Add | Sub | Mul | Div | Lpar | Rpar | Pow | Rem | Num of float

    let str2lst s = [for c in s -> c]
    let isblank c = System.Char.IsWhiteSpace c
    let isdigit c = System.Char.IsDigit c
    let intVal (c:char) = float ((int)c - (int)'0')

    let lexError c = System.Exception($"Lexer error at character: {c}")
    let parseError msg = System.Exception($"Parser error: {msg}")

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
            | '^'::tail -> Pow:: scan tail
            | '%'::tail -> Rem:: scan tail
            | c :: tail when isblank c -> scan tail
            | c :: tail when isdigit c || c = '.' ->
                let (iStr, iVal) = scNum(c::tail, 0.0) 
                Num iVal :: scan iStr
            | c :: _ -> raise (lexError c)
        scan (str2lst input)

    // Grammar in BNF:
    // <E>        ::= <T> <Eopt>
    // <Eopt>     ::= "+" <T> <Eopt> | "-" <T> <Eopt> | <empty>
    // <T>        ::= <NR> <Topt>
    // <Topt>     ::= "*" <NR> <Topt> | "/" <NR> <Topt> | <empty>
    // <NR>       ::= "Num" <value> | "(" <E> ")"

    let parser tList = 
        let rec E tList = (T >> Eopt) tList         // >> is forward function composition operator: let inline (>>) f g x = g(f x)
        and Eopt tList = 
            match tList with
            | Add :: tail -> (T >> Eopt) tail
            | Sub :: tail -> (T >> Eopt) tail
            | _ -> tList
        and T tList = (NR >> Topt) tList
        and Topt tList =
            match tList with
            | Mul :: tail -> (NR >> Topt) tail
            | Div :: tail -> (NR >> Topt) tail
            | Pow :: tail -> (NR >> Topt) tail
            | Rem :: tail -> (NR >> Topt) tail
            | _ -> tList
        and NR tList =
            match tList with 
            | Num value :: tail -> tail
            | Lpar :: tail -> match E tail with 
                              | Rpar :: tail -> tail
                              | _ -> raise (parseError "Missing closing parenthesis")
            | _ -> raise (parseError "Unexpected token")
        E tList

    let parseNeval tList = 
        let rec E tList = (T >> Eopt) tList
        and Eopt (tList, value) = 
            match tList with
            | Add :: tail -> let (tLst, tval) = T tail
                             Eopt (tLst, value + tval)
            | Sub :: tail -> let (tLst, tval) = T tail
                             Eopt (tLst, value - tval)
            | _ -> (tList, value)
        and T tList = (NR >> Topt) tList
        and Topt (tList, value) =
            match tList with
            | Mul :: tail -> let (tLst, tval) = NR tail
                             Topt (tLst, value * tval)
            | Div :: tail -> let (tLst, tval) = NR tail
                             Topt (tLst, value / tval)
            | Pow :: tail -> let (tLst, tval) = NR tail
                          //   let mutable i = 1
                            // let mutable final = value

                            // while (i < tval) do (
                            //   final <- final * value
                            //   i <- i+1
                            //   )

                             Topt (tLst, Math.Pow(value, tval))
            | Rem :: tail -> let (tLst, tval) = NR tail
                             Topt (tLst, value % tval)
                             //let mutable i = value / tval
                             //Topt (tLst, value-(tval*i))
            | _ -> (tList, value)
        and NR tList =
            match tList with 
            | Num value :: tail -> (tail, value)
            | Lpar :: tail -> let (tLst, tval) = E tail
                              match tLst with 
                              | Rpar :: tail -> (tail, tval)
                              | _ -> raise (parseError "Missing closing parenthesis")
            | _ -> raise (parseError "Unexpected token")
        E tList

    let interpret (input: string) =
        let oList = lexer input
        let Out = parseNeval oList
        snd Out
