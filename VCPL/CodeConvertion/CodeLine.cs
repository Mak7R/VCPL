using System.Collections.Generic;
using VCPL.CodeConvertion;

namespace VCPL.CodeConvertion;

public struct CodeLine : ICodeLine
{
    /// <summary>
    /// Name of the function
    /// </summary>
    public string FunctionName { get; set; }
    /// <summary>
    /// List of arguments
    /// </summary>
    public List<string> Args { get; set; }
    /// <summary>
    /// value which get the result of function
    /// </summary>
    public string? ReturnData { get; set; }

    public CodeLine(string functionName, List<string> args, string? returnData = null)
    {
        this.FunctionName = functionName;
        this.Args = args;
        this.ReturnData = returnData;
    }
}