module AppTokens

open System
open System.IO

open Newtonsoft.Json

open ColorPrint
open CommonTools

type private JsonFile = {
  FileName: string
  MultiJson: bool
}

type private TokensOptions = {
  InFiles: JsonFile list
}

let runTokens args =
  let rec parseMore o args =
    match args with
    | "-v" :: rest ->
      verbose <- true
      rest |> parseMore o
    | [] ->
      if o.InFiles |> List.isEmpty then
        failwith "No input file(s) specified"
      o
    | "-f" :: fnm :: rest ->
      let fnm = fnm |> Path.GetFullPath
      let ext = Path.GetExtension(fnm).ToLower();
      let jf =
        match ext with
        | ".json" ->
          {FileName = fnm; MultiJson = false}
        | ".mjson" ->
          {FileName = fnm; MultiJson = true}
        | x ->
          failwithf $"unrecognized file extension '{x}'"
      rest |> parseMore {o with InFiles = jf :: o.InFiles}
    | x :: _ ->
      failwith $"Unrecognized argument '{x}'"
  let o = args |> parseMore {
    InFiles = []
  }
  for jf in o.InFiles do
    cp $"Processing \fg{jf.FileName |> Path.GetFileName}"
    use tr = File.OpenText(jf.FileName)
    use jr = new JsonTextReader(tr)
    let ili = jr :> IJsonLineInfo
    jr.SupportMultipleContent <- jf.MultiJson
    while jr.Read() do
      let fakeindent = new String(' ', jr.Depth)
      let clr =
        match jr.TokenType with
        | JsonToken.StartObject -> "\fg"
        | JsonToken.PropertyName -> "\fb"
        | JsonToken.StartArray -> "\fc"
        | JsonToken.EndObject -> "\fG"
        | JsonToken.EndArray -> "\fC"
        | _ -> ""
      cpx $"%4d{ili.LineNumber}:%03d{ili.LinePosition} \fy%2d{jr.Depth} {fakeindent}{clr}{jr.TokenType}\f0"
      if jr.Value <> null then
        cp $" {jr.Value}"
      else
        cp " \fo-"
      ()
    ()
  0
