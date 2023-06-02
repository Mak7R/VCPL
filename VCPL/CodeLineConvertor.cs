namespace VCPL;

/// <summary>
/// This class converts string line of code to parts. Part is a function name, arguments (variables and constants)
/// </summary>
public static class CodeLineConvertor
{
    public static bool IsEmpetyLine(string str)
    {
        foreach (char ch in str) 
            if (ch != ' ') 
                return false;
        return true;
    }
    /// <summary>
    /// Old syntax. Should delete after new syntax will be developed
    /// </summary>
    /// <param name="line">Code line</param>
    /// <returns>Return new Struct Code line as name and parametrs</returns>
    /// <exception cref="Exception"></exception>
    public static CodeLine StringToData(string line)
    {
        string funcName;
        string argsString;
        
        int i = 0;
        while (line[i] != ':')
        {
            i++;
            if (i == line.Length) throw new Exception("Function was not found. Was missed ':' symbol.");
        }
        
        funcName = line.Substring(0, i);
        argsString = line.Substring(i + 1);

        List<string> argsList = new List<string>();

        bool isString = false;
        char stringSymbol = '\0';
        bool isSpecial = false;

        int start = 0;
        for (i = 0; i < argsString.Length; )
        {
            char s = argsString[i];
            if (isSpecial)
            {
                isSpecial = false;
                i++;
                continue;
            }
            else if (!isString)
            {
                if (argsString[i] == '\\') throw new Exception("Incorrect input");
            }

            if (!isString && argsString[i] == ' ') {argsString = argsString.Remove(i, 1); continue;}
            else if (!isString && argsString[i] == ',') { argsList.Add(argsString.Substring(start, i - start));
                start = i + 1;
            }
            else if (!isString && (argsString[i] == '\'' || argsString[i] == '\"'))
            {
                if (i != 0 && argsString[i - 1] != ',') throw new Exception("Incorrect input");
                isString = true;
                stringSymbol = argsString[i];
            }
            else if (isString && !isSpecial && argsString[i] == stringSymbol)
            {
                isString = false;
                stringSymbol = '\0';
            }
            else if (isString && argsString[i] == '\\')
            {
                isSpecial = true;
            }

            i++;
        }

        if (isString) throw new Exception("Incorrect input");
        if (argsString != "") argsList.Add(argsString.Substring(start, i - start));
        
        return new CodeLine(funcName, argsList);
    }

    public static CodeLineNew StringToCodeLine(string line)
    {
        string? FunctionName = null;
        string? ReturnGetter = null;
        List<string>? Args = null;

        line = line.Trim();

        int equalsIndex = line.IndexOf('=');
        if (equalsIndex == -1)
        {
            ReturnGetter = null;
        }
        else
        {
            ReturnGetter = line.Substring(0, equalsIndex).Trim();
            // if (ReturnGetter == "") -> SyntaxException -> MissedReturnGetterException
            line = line.Substring(equalsIndex + 1).Trim();
        }

        int openParenIndex = line.IndexOf('(');
        int closeParenIndex = line.IndexOf(')');
        if (openParenIndex == -1 && closeParenIndex == -1)
        {
            Args = new List<string> { line.Substring(0, line.Length-1) };
        }
        else if ((openParenIndex == -1) != (closeParenIndex == -1))
        {
            // if <-> else
            // throw new SyntaxException -> new NoOpenParrenExcaption
            // throw new SyntaxException -> new NoCloseParrenExcaption
        }
        else if(openParenIndex > closeParenIndex)
        {
            // throw new SyntaxException -> new NoOpenParrenExcaption
        }
        else
        {
            string funcName = line.Substring(0, openParenIndex).Trim();
            FunctionName = funcName;
            string argsString = line.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
            Args = GetArgsList(argsString);
        }

        return new CodeLineNew(FunctionName, Args, ReturnGetter);
    }

    private static List<string> GetArgsList(string argsString)
    {
        List<string> args = new List<string>();

        int startIndex = 0;
        int endIndex = 0;
        bool inQuotes = false;
        char quoteChar = '\0';

        for (int i = 0; i < argsString.Length; i++)
        {
            char currentChar = argsString[i];

            if (!inQuotes && (currentChar == ',' || i == argsString.Length - 1))
            {
                endIndex = i;
                if (i == argsString.Length - 1)
                {
                    endIndex++;
                }
                string arg = argsString.Substring(startIndex, endIndex - startIndex).Trim();
                args.Add(arg);
                startIndex = i + 1;
            }
            else if (!inQuotes && (currentChar == '\'' || currentChar == '"'))
            {
                inQuotes = true;
                quoteChar = currentChar;
                startIndex = i;
            }
            else if (inQuotes && currentChar == quoteChar)
            {
                inQuotes = false;
                endIndex = i+1;
                string arg = argsString.Substring(startIndex, endIndex - startIndex).Trim();
                args.Add(arg);
                startIndex = i + 1;
            }
        }

        return args;
    }
}

public struct CodeLine
{
    public string FunctionName;
    public List<string> args;

    public CodeLine(string funcName, List<string> args)
    {
        FunctionName = funcName;
        this.args = args;
    }
}

public struct CodeLineNew
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
    public string? ReturnGetter;

    public CodeLineNew(string functionName, List<string> args, string? returnGetter = null)
    {
        this.FunctionName = FunctionName;
        this.Args = args;
        this.ReturnGetter = returnGetter;
    }
}