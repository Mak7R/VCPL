using System.Globalization;
using System.Linq;
using GlobalRealization;
using CustomLibraries;

namespace VCPL;

public class TempMainFunction
{
    public ProgramStack Stack;
    public Dictionary<string, ElementaryFunction> ElementaryFunctions;
    public (Reference? ReturnData, ElementaryFunction? method, List<ProgramObject> args)[] Program;
    

    public TempMainFunction(Dictionary<string, ElementaryFunction> elementaryFunctions, List<CodeLine> codeLines)
    {
        //// this is a process of compilation
        Stack = new ProgramStack();
        ElementaryFunctions = elementaryFunctions;
        
        Compilate((from cl in codeLines
            where (cl.FunctionName == null ? '\0' : cl.FunctionName[0]) == '#'
                select cl).ToList());
        
        codeLines = (from cl in codeLines
                     where (cl.FunctionName == null ? '\0' : cl.FunctionName[0]) != '#'
                     select cl).ToList();
        
        Program = new (Reference?, ElementaryFunction?, List<ProgramObject>)[codeLines.Count];

        for (int i = 0; i < codeLines.Count; i++)
        {
            Reference reference;
            ProgramObject? obj = ArgumentConvertor.ConvertArgument(ref Stack, codeLines[i].ReturnData);
            if (obj is Reference Refer)
            {
                reference = Refer;
            }
            else if(obj == null)
            {
                reference = null;
            }
            else
            {
                throw new Exception("Compilation error");
            }
            Program[i] = (reference, codeLines[i].FunctionName == null ? null : ElementaryFunctions[codeLines[i].FunctionName], ArgumentConvertor.ConvertArguments(ref this.Stack, codeLines[i].Args));
        }
    }

    public void Compilate(List<CodeLine> codeLines)
    {
        foreach (var codeLine in codeLines)
        {
            switch (codeLine.FunctionName)
            {
                case "#init": // #init(name, start_value, modificator) // modificator (const, static, ref, ...)
                    if (ArgumentConvertor.isVarable(codeLine.Args[0]))
                    {
                        if (codeLine.Args.Count == 1) this.Stack.Add(new Variable(codeLine.Args[0], null));
                        else if (codeLine.Args.Count == 2) this.Stack.Add(new Variable(codeLine.Args[0], codeLine.Args[1]));
                        else
                        {
                            throw new Exception("Has no realization");
                        }
                    }
                    else
                    {
                        throw new Exception("It isn't posible to init not a variable");
                    }
                    break;
                case "#import":
                    foreach (string arg in codeLine.Args)
                    {
                        RuntimeLibConnector.AddToLib(ref ElementaryFunctions, arg);
                    }
                    break;
            }
        }
    }

    
    public void Run()
    {
        foreach (var command in Program)
        {
            command.method.Invoke(ref Stack, command.ReturnData, command.args);
        }
    }
}

public static class ArgumentConvertor
{
    public static List<ProgramObject> ConvertArguments(ref ProgramStack stack, List<string>? args)
    {
        List<ProgramObject> ArgumentsList = new List<ProgramObject>();
        for (int i = 0; i < args.Count; i++)
        {
            ArgumentsList.Add(ConvertArgument(ref stack, args[i]));
        }
        return ArgumentsList;
    }

    public static ProgramObject? ConvertArgument(ref ProgramStack stack, string? arg)
    {
        if (arg == null) return null;
        
        if (isFloat(arg))
        {
            return new Constant(Convert.ToDouble(arg, new CultureInfo("en-US")));
        }
        else if (isNumber(arg))
        {
            return new Constant(Convert.ToInt32(arg));
        }
        else if (isString(arg))
        {
            return new Constant(arg.Substring(1, arg.Length-2));
        }
        else
        {
            int index = stack.FindIndex(arg);
            if (index != -1) return new Reference(ref stack, index);
            else
            {
                throw new Exception("Variable with this name was not found");
            }
        }
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