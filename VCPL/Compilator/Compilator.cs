using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BasicFunctions;
using CustomLibraries;
using GlobalRealization;

namespace VCPL;

public static class Compilator
{

    public static Function Compilate(List<CodeLine> codeLines, Context context)
    {
        List<Instruction> Program = new List<Instruction>();
        
        context = context.NewContext();
        Compilate(codeLines, Program, context);

        Instruction[] packedProgram = new Instruction[Program.Count];
        for (int i = 0; i < Program.Count; i++)
        {
            packedProgram[i] = Program[i];
        }
        
        return new Function(context.dataContext.Pack(), packedProgram);
    }

    private static void Compilate(List<CodeLine> codeLines,
        List<Instruction> program, Context context)
    {
        List<(int, int)> subFunctions = new List<(int, int)>();
        List<string> UndefinedFunctions = new List<string>();
        
        for (int index = 0; index < codeLines.Count; index++)
        {
            CodeLine codeLine = codeLines[index];
            
            if (codeLine.FunctionName == null || codeLine.FunctionName.Length == 0)
            {
                int retDataId = context.dataContext.Peek(codeLine.ReturnData);
                if (retDataId == -1) throw new CompilationException($"Variable was not found: {codeLine.ReturnData}");


                switch (codeLine.Args.Count)
                {
                    case 0:
                        throw new CompilationException("No function, no args");
                    case 1:
                        int arg;
                        if (BasicString.isVarable(codeLine.Args[0])) arg = context.dataContext.Peek(codeLine.Args[0]);
                        else arg = context.dataContext.Push(null, ConstantConvertor(codeLine.Args[0]));
                        program.Append(new Instruction(
                            (ref DataContainer container, int retDataId, int[] argsIds) =>
                            {
                                container[retDataId] = argsIds[0];
                            }, retDataId, new int[1] { arg }));
                        break;
                    default:
                        int[] args = new int[codeLine.Args.Count];
                        for (int i = 0; i < codeLine.Args.Count; i++)
                        {
                            if (BasicString.isVarable(codeLine.Args[i]))
                                args[i] = context.dataContext.Peek(codeLine.Args[i]);
                            else args[i] = context.dataContext.Push(null, ConstantConvertor(codeLine.Args[i]));
                        }

                        program.Append(new Instruction(
                            (ref DataContainer container, int retDataId, int[] argsIds) =>
                            {
                                throw new RuntimeException("No realization of turlples");
                            }, retDataId, args));
                        break;
                }
            }
            else if (codeLine.FunctionName[0] == '#')
            {
                if (codeLine.FunctionName == "#define")
                {
                    for (int i = index + 1; i < codeLines.Count; i++)
                    {
                        if (codeLines[i].FunctionName == $"#end" && codeLines[i].Args[0] == codeLine.Args[0]) // Compilation errors
                        {
                            subFunctions.Add((index+1, i));
                            
                            context.functionsContext.Add(codeLine.Args[0], null);
                            
                            index = i;
                            break;
                        }
                    }
                }
                else
                {
                    CompilateDirective(codeLine, context);
                }
            }
            else
            {
                string userFunc = CompilateCodeLine(codeLine, program, context);
                if (userFunc != string.Empty) UndefinedFunctions.Add(userFunc);
            }
        }

        Dictionary<string, Function> definedFunctions = new Dictionary<string, Function>();
        
        for (int i = 0; i < subFunctions.Count; i++)
        {
            definedFunctions.Add(codeLines[subFunctions[i].Item1-1].Args[0], Compilate(codeLines.GetRange(subFunctions[i].Item1, subFunctions[i].Item2 - subFunctions[i].Item1), context));
        }

        int j = 0;
        foreach (string undefinedFunction in UndefinedFunctions)
        {
            while (j < program.Count)
            {   
                if (program[j].method == null)
                {
                    program[j].SetMethod(definedFunctions[undefinedFunction].GetCopyFunction().GetMethod());
                    break;
                }
                j++;
            }
            // if problem make exception
        }
    }

    private static string CompilateCodeLine(CodeLine codeLine,
        List<Instruction> program, Context context)
    {
        string retStr = string.Empty;
        int retDataId;
        int[] args;
        ElementaryFunction function;

        if (codeLine.ReturnData == null) retDataId = -1;
        else
        {
            retDataId = context.dataContext.Peek(codeLine.ReturnData);
        }

        if (codeLine.Args == null) args = new int[0];
        else
        {
            args = new int[codeLine.Args.Count];
            for (int i = 0; i < codeLine.Args.Count; i++)
            {
                if (BasicString.isVarable(codeLine.Args[i])) args[i] = context.dataContext.Peek(codeLine.Args[i]);
                else
                {
                    args[i] = context.dataContext.Push(null, ConstantConvertor(codeLine.Args[i]));
                }
            }
        }

        try
        {
            function = context.functionsContext[codeLine.FunctionName];
            if (function == null)
            {
                retStr = codeLine.FunctionName;
            }
        }
        catch (KeyNotFoundException)
        {
            throw new CompilationException("Fiunction {FunctionName} was not declarated");
        }
        program.Add(new Instruction(function, retDataId, args));
        return retStr;
    }
    private static void CompilateDirective(CodeLine codeLine, Context context)
    {
        switch (codeLine.FunctionName)
        {
            case "#init": // #init(name, start_value, modificator) // modificator (const, static, ref, ...)
                if (BasicString.isVarable(codeLine.Args[0]))
                {
                    if (codeLine.Args.Count == 1)
                    {
                        try
                        {
                            context.dataContext.Push(codeLine.Args[0], null);
                        }
                        catch (ArgumentException)
                        {
                            throw new CompilationException("This variable was declarated");
                        }
                    }
                    else if (codeLine.Args.Count == 2) 
                    {
                        try
                        {
                            if (BasicString.isVarable(codeLine.Args[1]))
                            {
                                throw new CompilationException("Variable can be inited only by Constant");
                            }
                            context.dataContext.Push(codeLine.Args[0], ConstantConvertor(codeLine.Args[1]));
                        }
                        catch (ArgumentException)
                        {
                            throw new CompilationException("This variable was declarated");
                        }
                    }
                    else throw new CompilationException("Has no realization");
                }
                else
                {
                    throw new CompilationException("It isn't posible to init not a variable");
                }
                break;
            case "#import":
                foreach (string arg in codeLine.Args)
                {
                    RuntimeLibConnector.AddToLib(ref context.functionsContext, arg);
                }
                break;
        }
    }
    
    private static object? ConstantConvertor(string arg)
    {
        if (BasicString.isFloat(arg))
        {
            return Convert.ToDouble(arg, new CultureInfo("en-US"));
        }
        else if (BasicString.isNumber(arg))
        {
            return Convert.ToInt32(arg);
        }
        else if (BasicString.isString(arg))
        {
            return arg.Substring(1, arg.Length-2);
        }
        else if(BasicString.isNull(arg))
        {
            return null;
        }
        else
        {
            throw new CompilationException("Type was not detected");
        }
    }
}