namespace VCPL;

// syntax realisztion !!!
 
/// <summary>
/// This class has all realisations of Syntax Model. All his methods realize conversion from string to CodeLine
/// </summary>
public static class CodeLineConvertor
{
    /// <summary>
    /// CLite Syntax
    /// </summary>
    /// <param name="line">string line</param>
    /// <returns>new CodeLine which is equal to input line if realize convertion with CLite syntax</returns>
    public static CodeLine SyntaxCLite(string line)
    {
        string? FunctionName = null;
        string? ReturnData = null;
        List<string>? Args = null;

        line = line.Trim();

        int equalsIndex = line.IndexOf('=');
        if (equalsIndex == -1)
        {
            ReturnData = null;
        }
        else
        {
            ReturnData = line.Substring(0, equalsIndex).Trim();
            // if (ReturnGetter == "") -> SyntaxException -> MissedReturnGetterException
            line = line.Substring(equalsIndex + 1).Trim();
        }

        int openParenIndex = line.IndexOf('(');
        int closeParenIndex = line.IndexOf(')');
        if (openParenIndex == -1 && closeParenIndex == -1)
        {
            Args = new List<string> { line };
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
            FunctionName = line.Substring(0, openParenIndex).Trim();
            string argsString = line.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
            Args = GetArgsList(argsString);
        }

        return new CodeLine(FunctionName, Args, ReturnData);
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