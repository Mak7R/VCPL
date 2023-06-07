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
    
    public static bool isString(string arg)
    {
        if (arg[0] == '\'' || arg[0] == '\"') return true;
        return false;
    }

    public static bool isCorrectString(string arg)
    {
        if (arg[0] == '\'' || arg[0] == '\"')
            if ( arg[arg.Length-1] == '\'' || arg[arg.Length-1] == '\"' )
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

    public static bool isFloat(string arg)
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

    public static bool isVarable(string arg)
    {
        if (!isNumber(arg) && !isString(arg) && !isNull(arg)) return true;
        return false;
    }

    public const string NULL = "null";
    public static bool isNull(string arg)
    {
        if (arg == NULL) return true;
        else return false;
    }
}