(*** hide ***)
#I "../../bin"

(**

Using Chessie for Railway-oriented programming
==============================================

Railway-oriented programming was introduced by Scott Wlaschin.

Resources:

* Railway Oriented Programming - A functional approach to error handling 
    * [Slide deck](http://www.slideshare.net/ScottWlaschin/railway-oriented-programming)
    * [Video](https://vimeo.com/97344498)

*)
#r "Chessie.dll"

open Chessie.ErrorHandling

type Request = 
    { Name : string
      EMail : string }

let validateInput input = 
    if input.Name = "" then fail "Name must not be blank"
    elif input.EMail = "" then fail "Email must not be blank"
    else succeed input // happy path

let validate1 input = 
    if input.Name = "" then fail "Name must not be blank"
    else succeed input

let validate2 input = 
    if input.Name.Length > 50 then fail "Name must not be longer than 50 chars"
    else succeed input

let validate3 input = 
    if input.EMail = "" then fail "Email must not be blank"
    else succeed input

let combinedValidation = 
    // connect the two-tracks together
    validate1
    >> bind validate2
    >> bind validate3

{ Name = ""; EMail = "" }
|> combinedValidation

// [fsi:val it : Chessie.ErrorHandling.Result<Request,string> =
//  Failure ["Name must not be blank"]]
    
//
//
//[<Test>]
//let ``should find empty mail``() = 
//    { Name = "Scott"
//      EMail = "" }
//    |> combinedValidation
//    |> shouldEqual (Failure [ "Email must not be blank" ])
//
//[<Test>]
//let ``should find long name``() = 
//    { Name = "ScottScottScottScottScottScottScottScottScottScottScottScottScottScottScottScottScottScottScott"
//      EMail = "" }
//    |> combinedValidation
//    |> shouldEqual (Failure [ "Name must not be longer than 50 chars" ])
//
//[<Test>]
//let ``should not complain on valid data``() = 
//    let scott = 
//        { Name = "Scott"
//          EMail = "scott@chessie.com" }
//    scott
//    |> combinedValidation
//    |> returnOrFail
//    |> shouldEqual scott
//
//let canonicalizeEmail input = { input with EMail = input.EMail.Trim().ToLower() }
//
//let usecase = 
//    combinedValidation
//    >> (lift canonicalizeEmail)
//
//[<Test>]
//let ``should canonicalize valid data``() = 
//    { Name = "Scott"
//      EMail = "SCOTT@CHESSIE.com" }
//    |> usecase
//    |> returnOrFail
//    |> shouldEqual { Name = "Scott"
//                     EMail = "scott@chessie.com" }
//
//[<Test>]
//let ``should not canonicalize invalid data``() = 
//    { Name = ""
//      EMail = "SCOTT@CHESSIE.com" }
//    |> usecase
//    |> shouldEqual (Failure [ "Name must not be blank" ])
//
//// a dead-end function    
//let updateDatabase input =
//   ()   // dummy dead-end function for now
//
//
//let log logF twoTrackInput = 
//    let success(x,msgs) = logF "DEBUG. Success so far."; Success(x,msgs)
//    let failure msgs = logF <| sprintf "ERROR. %A" msgs; Failure(msgs)
//    either success failure twoTrackInput 
//
//let usecase2 logF = 
//    usecase
//    >> (successTee updateDatabase)
//    >> (log logF)
//
//[<Test>]
//let ``should log valid data``() = 
//    let logF s =
//        if s <> "DEBUG. Success so far." then
//            failwithf "unexpected log: %s"  s
//
//    { Name = "Scott"
//      EMail = "SCOTT@CHESSIE.com" }
//    |> usecase2 logF
//    |> returnOrFail
//    |> shouldEqual { Name = "Scott"
//                     EMail = "scott@chessie.com" }
//
//
//[<Test>]
//let ``should log invalid data``() = 
//    let logF s =
//        if s <> """ERROR. ["Email must not be blank"]""" then
//            failwithf "unexpected log: %s"  s
//
//    { Name = "Scott"
//      EMail = "" }
//    |> usecase2 logF
//    |> ignore
