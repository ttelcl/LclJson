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
  /// Description of LevelIndentRule
  /// </summary>
  public class LevelIndentRule: JsonIndentRule
  {
    /// <summary>
    /// Create a new LevelIndentRule
    /// </summary>
    public LevelIndentRule(int depth)
      : base()
    {
      Depth = depth;
    }

    /// <summary>
    /// The depth at which to switch from indented to flat
    /// </summary>
    public int Depth { get; set; }

    /// <inheritdoc/>
    public override bool NoIndentInside(JsonReformatter jrf)
    {
      return jrf.Source.Depth >= Depth;
    }
  }
}