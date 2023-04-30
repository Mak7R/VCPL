namespace VCPL;

public static class CodeLineConvertor
{
    public static bool IsEmpetyLine(string str)
    {
        foreach (char ch in str) 
            if (ch != ' ') 
                return false;
        return true;
    }
        
    public static CodeLine StringsToData(string line)
    {
        string funcName = "";
        string argsString = "";
            
        int i = 0;
        while (line[i] != ':')
        {
            i++;
            if (i == line.Length) throw new Exception("Incorect input");
        }
                
        funcName = line.Substring(0, i);
        argsString = line.Substring(i + 1);

        string[] argsList = argsString.Split(',');

        bool deleteSpace = true;
        for (i = 0; i < argsList.Length; i++)
        {
            for (int j = 0; j < argsList[i].Length;)
            {
                if (argsList[i][j] == ' ' && deleteSpace) argsList[i] = argsList[i].Remove(j, 1);
                else if (argsList[i][j] == '\"')
                {
                    deleteSpace = !deleteSpace;
                    j++;
                }
                else j++;
            }
        }

        if (argsList.Length == 1 && argsList[0] == "") return new CodeLine(funcName, null);
        return new CodeLine(funcName, argsList);
    }
}

public struct CodeLine
{
    public string FunctionName;
    public string[]? args;

    public CodeLine(string funcName, string[]? args)
    {
        FunctionName = funcName;
        this.args = args;
    }
}