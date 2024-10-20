namespace MathInterpreter

module Interpreter =

    open System

    type terminal = 
        Add | Sub | Mul | Div | Lpar | Rpar | Pow | Rem | Num of int

    let str2lst s = [for c in s -> c]
    let isblank c = System.Char.IsWhiteSpace c
    let isdigit c = System.Char.IsDigit c
    let lexError = System.Exception("Lexer error")
    let intVal (c:char) = (int)((int)c - (int)'0')
    let parseError = System.Exception("Parser error")

    let rec scInt(iStr, iVal) = 
        match iStr with
        c :: tail when isdigit c -> scInt(tail, 10*iVal+(intVal c))
        | _ -> (iStr, iVal)

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
            | c :: tail when isdigit c -> let (iStr, iVal) = scInt(tail, intVal c) 
                                          Num iVal :: scan iStr
            | _ -> raise lexError
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
                              | _ -> raise parseError
            | _ -> raise parseError
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
                             let mutable i = 1
                             let mutable final = value

                             while (i < tval) do (
                               final <- final * value
                               i <- i+1
                               )

                             Topt (tLst,final )
            | Rem :: tail -> let (tLst, tval) = NR tail
                             let mutable i = value / tval
                             Topt (tLst, value-(tval*i))
            | _ -> (tList, value)
        and NR tList =
            match tList with 
            | Num value :: tail -> (tail, value)
            | Lpar :: tail -> let (tLst, tval) = E tail
                              match tLst with 
                              | Rpar :: tail -> (tail, tval)
                              | _ -> raise parseError
            | _ -> raise parseError
        E tList

    let interpret (input: string) =
        let oList = lexer input
        let Out = parseNeval oList
        snd Out
