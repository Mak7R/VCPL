namespace VCPL;

public struct CodeLine
{
    /// <summary>
    /// Name of the function
    /// </summary>
    public string? FunctionName;
    /// <summary>
    /// List of arguments
    /// </summary>
    public List<string>? Args;
    /// <summary>
    /// value which get the result of function
    /// </summary>
    public string? ReturnData;

    public CodeLine(string? functionName, List<string>? args, string? returnData = null)
    {
        this.FunctionName = functionName;
        this.Args = args;
        this.ReturnData = returnData;
    }
}