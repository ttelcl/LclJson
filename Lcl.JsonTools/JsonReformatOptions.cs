/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcl.JsonTools
{
  /// <summary>
  /// Options affecting the reformat operation
  /// </summary>
  public class JsonReformatOptions
  {
    /// <summary>
    /// Create a new JsonReformatOptions
    /// </summary>
    public JsonReformatOptions()
    {
    }

    /// <summary>
    /// When set to false, JSON sources that are known to parse dates
    /// are rejected. Parsing dates and later formatting dates again
    /// is too error prone to handle reliably for a generic library
    /// library like this, but setting this to true allows you to
    /// bypass the check.
    /// </summary>
    public bool AllowDateHandling { get; set; } = false;

    /// <summary>
    /// A list of rules that allow changing indentation for part of the
    /// output.
    /// </summary>
    public List<JsonIndentRule> IndentRules { get; } = new List<JsonIndentRule>();

    /// <summary>
    /// The number of IndentChar characters to use for indentation. Default 2.
    /// When set to 0, output indentation is disabled for the entire
    /// output (and IndentRules are ignored)
    /// </summary>
    public int Indentation { get; set; } = 2;

    /// <summary>
    /// The character to use for indentation. Default: a space.
    /// </summary>
    public char IndentChar { get; set; } = ' ';
  }
}
