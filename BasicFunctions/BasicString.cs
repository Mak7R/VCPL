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
}