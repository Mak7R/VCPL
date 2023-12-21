using System;

namespace BasicFunctions;

public static class BasicString
{
    public static bool IsNoDataString(string str)
    {
        foreach (char ch in str) 
            if (ch != ' ') 
                return false;
        return true;
    }
    public static (string left, string? right)? SplitBy(string str, char separator)
    {
        if (str.Length == 0) return null;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == separator)
            {
                return (str.Substring(0, i), str.Substring(i + 1));
            }
        }
        return (str, null);
    }

    public static bool isChar(string arg)
    {
        if (arg.Length == 3 && arg[0] == '\'' && arg[2] == '\'') return true;
        return false;
    }
    
    public static bool isString(string arg)
    {
        if (arg[0] == '\"') return true;
        return false;
    }

    public static bool isCorrectString(string arg)
    {
        if (arg[0] == '\"')
            if (arg[arg.Length-1] == '\"' )
                return true;
        return false;
    }

    public static bool isNumber(string arg)
    {
        if (isNumber(arg[0])) return true;
        if (arg[0] == '-' && isNumber(arg[1])) return true;
        return false;
    }

    public static bool isNumber(char symbol)
    {
        switch (symbol)
        {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return true;
            default:
                return false;
        }
    }

    public static bool isCorrectNumber(string arg)
    {
        for (int i = 0; i < arg.Length; i++)
        {
            if (i == 0 && arg[i] == '-') continue;
            if (!isNumber(arg[i])) return false;
        }
        return true;
    }

    public static bool isDouble(string arg)
    {
        if (isNumber(arg))
        {
            for (int i = 1; i < arg.Length; i++)
            {
                if (arg[i] == '.' && i != arg.Length - 1) // '.' - it should be value which user can set because '.' == ',' and '.' != ','
                {
                    return true;
                } 
            }
        }

        return false;
    }

    public static bool isVariable(string arg)
    {
        if (arg == string.Empty) return false;
        if (arg == "true") return false;
        if (arg == "false") return false;
        if (arg == "null") return false;
        if (isNumber(arg)) return false;
        if (isSpcialSymbol(arg[0])) return false;
        return true;
    }

    public static bool isSpcialSymbol(char symbol)
    {
        switch (symbol)
        {
            case '\'':
            case '"':
            case '\\':
            case '.':
            case ',':
                return true;
            default: 
                return false;
        }
    }

    public static string GetNameFromPath(string path)
    {
        if (path == String.Empty) return path;
        for (int i = path.Length - 1; i >= 0; i--)
        {
            if (path[i] == '\\') return path.Substring(i + 1);
        }
        return path;
    }
}