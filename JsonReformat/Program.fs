// (c) 2022  ttelcl / ttelcl

open System

open CommonTools
open ExceptionTool
open Usage

let rec run arglist =
  // For subcommand based apps, split based on subcommand here
  match arglist with
  | "-v" :: rest ->
    verbose <- true
    rest |> run
  | "--help" :: _
  | "-h" :: _
  | [] ->
    usage verbose
    0  // program return status code to the operating system; 0 == "OK"
  | x :: _ when x.StartsWith("-") ->
    arglist |> AppReformat.run
  | _ :: _ ->
    // TODO: actual processing based on command line arguments
    new NotImplementedException("JsonReformat.exe is not yet implemented") |> raise
    1

[<EntryPoint>]
let main args =
  try
    args |> Array.toList |> run
  with
  | ex ->
    ex |> fancyExceptionPrint verbose
    resetColor ()
    1



