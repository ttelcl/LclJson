module AppReformat

open System
open System.IO

open Newtonsoft.Json

open Lcl.JsonTools

open ColorPrint
open CommonTools


type private ReformatOptions = {
  Inputs: string list  // support for mutiple inputs is not yet implemented
  Output: string
  FlatLevel: int
  RfOp: JsonReformatOptions
}

let run args =
  let rec parseMore o args =
    match args with
    | "-v" :: rest ->
      verbose <- true
      rest |> parseMore o
    | [] ->
      let inputs = o.Inputs |> List.rev
      if inputs |> List.isEmpty then
        failwith "No input files specified"
      if (inputs |> List.length) > 1 then
        failwith "Support for multiple inputs is NYI"
      let o =
        if o.Output |> String.IsNullOrEmpty then
          if (inputs |> List.length) > 1 then
            failwith "Cannot derive output name when there are multiple inputs"
          let inm = inputs.Head
          let ext = ".out" + (inm |> Path.GetExtension)
          {o with Output = Path.ChangeExtension(inm, ext)}
        else
          o
      {o with Inputs = inputs}
    | "-flat" :: rest ->
      // Take "'-flat' is the same as '-flvl 0'" literal
      "-flvl" :: "0" :: rest |> parseMore o
    | "-flvl" :: lvltxt :: rest ->
      let ok, level = lvltxt |> Int32.TryParse
      if ok |> not then
        failwith $"Expecting a number after '-flvl' but got '{lvltxt}'"
      rest |> parseMore {o with FlatLevel = level}
    | "-i" :: fnm :: rest ->
      rest |> parseMore {o with Inputs = fnm :: o.Inputs}
    | "-o" :: fnm :: rest ->
      rest |> parseMore {o with Output = fnm}
    | x :: _ ->
      failwith $"Unrecognized argument {x}"
  let o = args |> parseMore {
    Inputs = []
    Output = null
    FlatLevel = Int32.MaxValue
    RfOp = new JsonReformatOptions()
  }

  if o.FlatLevel > 0 && o.FlatLevel < Int32.MaxValue then
    o.RfOp.IndentRules.Add(new LevelIndentRule(o.FlatLevel))

  // For now prepare inputs and outputs here in this app code;
  // this may become library code in the future
  let onm = o.Output |> Path.GetFullPath
  do
    use owr = onm |> startFile
    use jtw = new JsonTextWriter(owr)
    if o.FlatLevel <= 0 then
      jtw.Formatting <- Formatting.None
      o.RfOp.Indentation <- 0
    else
      jtw.Indentation <- o.RfOp.Indentation
      jtw.IndentChar <- o.RfOp.IndentChar
      jtw.Formatting <- Formatting.Indented

    let inm = o.Inputs.Head // the only element, for now
    use rdr = File.OpenText(inm)
    use jrd = new JsonTextReader(rdr)
    let multiInput = inm.EndsWith(".mjson", StringComparison.InvariantCultureIgnoreCase)
    if multiInput then
      jrd.SupportMultipleContent <- true

    let trx = new JsonReformatter(o.RfOp, jrd, jtw)

    while trx.TranscribeElement() do
      () // nothing; the reformatter did already everything


    ()
  onm |> finishFile
        
  0