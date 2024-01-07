using System.Collections.Generic;
using VCPL.CodeConvertion;
using VCPL.Compilator;

namespace VCPL.CodeConvertion;

public class CodeLine
{
    public int LineNumber { get; init; }
    public string FunctionName { get; set; }
    public List<string> Args { get; set; }

    public CodeLine(int lineNumber, string functionName, List<string> args)
    {
        LineNumber = lineNumber;
        FunctionName = functionName;
        Args = args;
    }

    public CompilationException GenerateException(string message) => new CompilationException(this, message);
}