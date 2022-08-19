// (c) 2022  ttelcl / ttelcl
module Usage

open CommonTools
open ColorPrint

let usage detailed =
  cp "\foGeneral description\f0"
  cp "   JSON reformatter / prettifier"
  cp "\foJsonReformat \fg-i\f0 <\fcin.json\f0> [\fg-o\f0 <\fcout.json\f0>] [<\fcoptions\f0>]\f0"
  cp "   Reformat JSON files"
  cp "\fg-i\f0 <\fcin.json\f0>       Input file."
  cp "\fg-o\f0 <\fcout.json\f0>      Output file."
  cp "\fg-flat\f0\fx\fx              Remove all indentation and line breaks (same as '-flvl 0')"
  cp "\fg-flvl\f0 <\fclevel\f0>      Switch from indented to flat at the given nesting level"
  cp "\fg-v\f0\fx\fx                 Verbose mode"



