using System.Collections.Generic;
using VCPL.CodeConvertion;

namespace VCPL.CodeConvertion;

public class CodeLine
{
    /// <summary>
    /// Name of the function
    /// </summary>
    public string FunctionName { get; set; }
    /// <summary>
    /// List of arguments
    /// </summary>
    public List<string> Args { get; set; }

    public CodeLine(string functionName, List<string> args)
    {
        FunctionName = functionName;
        Args = args;
    }
}