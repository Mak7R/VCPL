using System.Globalization;

namespace VCPL;

public class TempMainFunction
{
    public ProgramStack Stack;
    public Dictionary<string, ElementaryFunction> ElementaryFunctions;
    public Dictionary<string, PreProcessorDirective> Directives;
    public (ElementaryFunction method, List<ProgramObject> args)[] Program;
    

    public TempMainFunction(Dictionary<string, ElementaryFunction> elementaryFunctions, Dictionary<string, PreProcessorDirective> directives, List<CodeLine> codeLines)
    {
        //// this is a process of compilation
        Stack = new ProgramStack();
        
        ElementaryFunctions = elementaryFunctions;
        Directives = directives;

        CompileProgram(ref codeLines);
        
        Program = new (ElementaryFunction, List<ProgramObject>)[codeLines.Count];

        for (int i = 0; i < codeLines.Count; i++)
        {
            Program[i] = (ElementaryFunctions[codeLines[i].FunctionName], ArgumentConvertor.ConvertArguments(ref this.Stack, codeLines[i].args));
        }
    }

    public void CompileProgram(ref List<CodeLine> codeLines)
    {
        for (int i = 0; i < codeLines.Count;)
        {
            if (codeLines[i].FunctionName[0] == '#')
            {
                if (codeLines[i].FunctionName == "#init")
                {
                    Directives[codeLines[i].FunctionName].Invoke(new object[2]{this.Stack, codeLines[i].args});
                }
                else
                {
                    Directives[codeLines[i].FunctionName].Invoke(codeLines[i].args);
                }
                codeLines.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
    
    public void Run()
    {
        foreach (var command in Program)
        {
            command.method.Invoke(ref Stack, command.args);
        }
    }
}

public static class ArgumentConvertor
{
    public static List<ProgramObject> ConvertArguments(ref ProgramStack stack, List<string> args)
    {
        List<ProgramObject> ArgumentsList = new List<ProgramObject>();
        for (int i = 0; i < args.Count; i++)
        {
            if (isFloat(args[i]))
            {
                ArgumentsList.Add(new Constant(Convert.ToDouble(args[i], new CultureInfo("en-US"))));
            }
            else if (isNumber(args[i]))
            {
                ArgumentsList.Add(new Constant(Convert.ToInt32(args[i])));
            }
            else if (isString(args[i]))
            {
                ArgumentsList.Add(new Constant(args[i].Substring(1, args[i].Length-2)));
            }
            else
            {
                int index = stack.FindIndex(args[i]);
                if (index != -1) ArgumentsList.Add(new Reference(ref stack, index));
                else
                {
                    throw new Exception("Variable with this name was not found");
                }
            }
        }

        return ArgumentsList;
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
        if (!isNumber(arg) && !isString(arg)) return true;
        return false;
    }
}