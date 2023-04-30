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
        argsList.Add(argsString.Substring(start, i - start));
        
        return new CodeLine(funcName, argsList);
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