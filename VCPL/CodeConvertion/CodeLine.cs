using System.Collections.Generic;
using VCPL.CodeConvertion;

namespace VCPL.CodeConvertion;

public struct CodeLine : ICodeLine
{
    /// <summary>
    /// Name of the function
    /// </summary>
    public string FunctionName { get; init; }
    /// <summary>
    /// List of arguments
    /// </summary>
    public List<string> Args { get; init; }

    public CodeLine(string functionName, List<string> args)
    {
        this.FunctionName = functionName;
        this.Args = args;
    }
}