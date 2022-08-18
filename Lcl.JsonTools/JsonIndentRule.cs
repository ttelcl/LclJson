/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Lcl.JsonTools
{

  /// <summary>
  /// Describes a rule for changing from indented to not-indented mode
  /// </summary>
  public abstract class JsonIndentRule
  {
    /// <summary>
    /// Create a new JsonIndentRule
    /// </summary>
    public JsonIndentRule()
    {
    }

    /// <summary>
    /// When returning true, content indentation will be suppressed
    /// for the content of the array or object starting at the current
    /// token or the property value whose name is the current token.
    /// </summary>
    /// <param name="jrf">
    /// The Jsonreformatter whose JsonReader is pointing to the current
    /// input token, which must be a start of an array or object, or
    /// a property name.
    /// </param>
    /// <returns>
    /// True if indentation should be suppressed.
    /// </returns>
    public abstract bool NoIndentInside(JsonReformatter jrf);
  }
}