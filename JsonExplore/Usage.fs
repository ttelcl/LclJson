// (c) 2022  ttelcl / ttelcl
module Usage

open CommonTools
open ColorPrint

let usage detailed =
  cp "\foJsonExplore \fg-f\f0 <\fcinputfile\f0>\f0"
  cp "   Reads a JSON or MJSON file and prints the tokens read"
  cp "\fg-f\f0 <\fcinputfile\f0>   The JSON or MJSON file to read."
  cp "\fx\fx\fx\fx                 If the extension is *.mjson, multi-content parsing is enabled"
  cp "\fg-v\fx\fx               \f0Verbose mode"



