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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Lcl.JsonTools
{
  /// <summary>
  /// Engine for reformatting JSON on-the-fly
  /// </summary>
  public class JsonReformatter
  {
    private bool _indenting = false;

    /// <summary>
    /// Create a new JsonReformatter
    /// </summary>
    /// <param name="options">
    /// The options affecting the reformatting
    /// </param>
    /// <param name="source">
    /// The JSON input. Some of the options may be changed by this
    /// constructor, based on the options.
    /// </param>
    /// <param name="target">
    /// The JSON output.
    /// </param>
    public JsonReformatter(
      JsonReformatOptions options,
      JsonReader source,
      JsonTextWriter target)
    {
      Options = options;
      Source = source;
      Target = target;
      PropertyNameStack = new Stack<string>();
      if(!Options.AllowDateHandling)
      {
        Source.DateParseHandling = DateParseHandling.None;
      }
      Indenting = Options.Indentation != 0;
    }

    /// <summary>
    /// The reformatting options
    /// </summary>
    public JsonReformatOptions Options { get; }

    /// <summary>
    /// The JSON source
    /// </summary>
    public JsonReader Source { get; }

    /// <summary>
    /// The JSON target
    /// </summary>
    public JsonTextWriter Target { get; }

    /// <summary>
    /// The stack of property names in the current path
    /// </summary>
    public Stack<string> PropertyNameStack { get; }

    /// <summary>
    /// The object nesting level for the current token. Similar to Source.Depth,
    /// but based on objects only, not arrays.
    /// </summary>
    public int ObjectDepth { get; private set; }

    /// <summary>
    /// The array nesting level for the current token. Similar to Source.Depth,
    /// but based on arrays only, not objects.
    /// </summary>
    public int ArrayDepth { get; private set; }

    /// <summary>
    /// True while emitting indented output
    /// </summary>
    public bool Indenting {
      get => _indenting;
      set {
        _indenting = value;
        if(value)
        {
          Target.Formatting = Formatting.Indented;
          Target.IndentChar = Options.IndentChar;
          Target.Indentation = Options.Indentation;
        }
        else
        {
          Target.Formatting = Formatting.None;
        }
      }
    }

    /// <summary>
    /// Indicates that the input supports reading multiple JSON
    /// items (e.g. from an *.mjson file rather than a *.json file)
    /// </summary>
    public bool MultiJson => Source.SupportMultipleContent;

    /// <summary>
    /// Read the next element (including all children) from the reader and
    /// emit them to the writer, returning true if there is more input to be processed.
    /// </summary>
    /// <returns>
    /// true if there may be more elements to process
    /// </returns>
    public bool TranscribeElement()
    {
      if(Source.Read())
      {
        return TranscribeCore();
      }
      else
      {
        return false;
      }
    }

    private bool TranscribeCore()
    {
      switch(Source.TokenType)
      {
        case JsonToken.Integer:
        case JsonToken.Float:
        case JsonToken.String:
        case JsonToken.Boolean:
        case JsonToken.Null:
          Target.WriteValue(Source.Value);
          return true;
        case JsonToken.Date:
          if(Options.AllowDateHandling)
          {
            Target.WriteValue(Source.Value);
            return true;
          }
          else
          {
            throw new NotSupportedException(
              $"Unsupported token type: {Source.TokenType}.");
          }
        case JsonToken.PropertyName:
          return TranscribeProperty();
        case JsonToken.StartObject:
          return TranscibeObject();
        case JsonToken.EndObject:
          throw new InvalidOperationException(
            "Spurious '}' detected");
        case JsonToken.StartArray:
          return TranscibeArray();
        case JsonToken.EndArray:
          throw new InvalidOperationException(
            "Spurious ']' detected");
        default:
          throw new NotSupportedException(
            $"Unsupported token type: {Source.TokenType}");
      }
    }

    private bool TranscribeTerminated(JsonToken terminator)
    {
      while(Source.TokenType != terminator)
      {

        // TBD

        if(!Source.Read())
        {
          return false;
        }
      }
      return true;
    }

    private bool TranscibeObject()
    {
      var indenting = Indenting;
      try
      {
        ObjectDepth++;
        Indenting = NextIndenting();
        Target.WriteStartObject();
        var result = TranscribeTerminated(JsonToken.EndObject);
        Target.WriteEndObject();
        return result;
      }
      finally
      {
        Indenting = indenting;
        ObjectDepth--;
      }
    }

    private bool TranscibeArray()
    {
      var indenting = Indenting;
      try
      {
        ArrayDepth++;
        Indenting = NextIndenting();
        Target.WriteStartArray();
        var result = TranscribeTerminated(JsonToken.EndObject);
        Target.WriteEndArray();
        return result;
      }
      finally
      {
        Indenting = indenting;
        ArrayDepth--;
      }
    }

    private bool TranscribeProperty()
    {
      var indenting = Indenting;
      var propName = (string)Source.Value!;
      try
      {
        PropertyNameStack.Push(propName);
        Indenting = NextIndenting();
        Target.WritePropertyName(propName);
        if(!Source.Read())
        {
          throw new InvalidOperationException(
            $"Unexpected end of input; property name without value");
        }
        var result = TranscribeCore();
        PropertyNameStack.Pop();
        return result;
      }
      finally
      {
        Indenting = indenting;
      }
    }

    private bool NextIndenting()
    {
      if(!Indenting)
      {
        return false;
      }
      switch(Source.TokenType)
      {
        case JsonToken.StartObject:
        case JsonToken.StartArray:
        case JsonToken.PropertyName:
          return !Options.IndentRules.Any(r => r.NoIndentInside(this));
        default:
          return true;
      }
    }
  }
}
