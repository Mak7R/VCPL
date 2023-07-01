using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BasicFunctions;
using GlobalRealization;
using Pointer = GlobalRealization.Pointer;

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

        return new Function(context.Pack(), packedProgram);
    }

    private static Function Compilate(List<CodeLine> codeLines, Context context, List<string> args)
    {
        List<Instruction> Program = new List<Instruction>();
        
        context = context.NewContext();
        foreach (string arg in args)
        {
            try
            {
                context.PushData(arg, null);
            }
            catch (ArgumentException)
            {
                throw new CompilationException("This variable was declarated");
            }
        }
        
        Compilate(codeLines, Program, context);

        Instruction[] packedProgram = new Instruction[Program.Count];
        for (int i = 0; i < Program.Count; i++)
        {
            packedProgram[i] = Program[i];
        }
        return new Function(context.Pack(), packedProgram);
    }

    private static void Compilate(List<CodeLine> codeLines,
        List<Instruction> program, Context context)
    {
        List<(int, int)> subFunctions = new List<(int, int)>();
        List<string> UndefinedFunctions = new List<string>();
        
        for (int index = 0; index < codeLines.Count; index++)
        {
            CodeLine codeLine = codeLines[index];
            
            if (codeLine.FunctionName != "" && codeLine.FunctionName[0] == '#')
            {
                if (codeLine.FunctionName == "#define")
                {
                    for (int i = index + 1; i < codeLines.Count; i++)
                    {
                        if (codeLines[i].FunctionName == $"#end" && codeLines[i].Args[0] == codeLine.Args[0]) // Compilation errors
                        {
                            subFunctions.Add((index+1, i));
                            
                            context.PushFunction(codeLine.Args[0], null);
                            
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
            List<string> args = codeLines[subFunctions[i].Item1-1].Args;
            // throw exception if args.Count == 0 ^
            if (args.Count == 1) definedFunctions.Add(codeLines[subFunctions[i].Item1-1].Args[0], Compilate(codeLines.GetRange(subFunctions[i].Item1, subFunctions[i].Item2 - subFunctions[i].Item1), context));
            else
            {
                definedFunctions.Add(codeLines[subFunctions[i].Item1-1].Args[0], Compilate(codeLines.GetRange(subFunctions[i].Item1, subFunctions[i].Item2 - subFunctions[i].Item1), context, args.GetRange(1, args.Count-1)));
            }
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
        Pointer retDataId;
        Pointer[] args;
        ElementaryFunction function;

        if (codeLine.ReturnData == null) retDataId = Pointer.NULL;
        else
        {
            retDataId = context.Peek(codeLine.ReturnData);
        }

        if (codeLine.Args == null) args = new Pointer[0];
        else
        {
            args = new Pointer[codeLine.Args.Count];
            for (int i = 0; i < codeLine.Args.Count; i++)
            {
                if (BasicString.isVarable(codeLine.Args[i])) args[i] = context.Peek(codeLine.Args[i]);
                else
                {
                    args[i] = context.PushConstant(null, ConstantConvertor(codeLine.Args[i]));
                }
            }
        }

        try
        {
            function = context.FunctionsContext[codeLine.FunctionName];
            if (function == null)
            {
                retStr = codeLine.FunctionName;
            }
        }
        catch (KeyNotFoundException)
        {
            throw new CompilationException($"Function {codeLine.FunctionName} was not declarated");
        }
        program.Add(new Instruction(function, retDataId, args));
        return retStr;
    }
    private static void CompilateDirective(CodeLine codeLine, Context context)
    {
        switch (codeLine.FunctionName)
        {
            case "#init": // #init(name, !start_value! <- delete it, modificator, type) // modificator (const, static, ref, ...)
                if (BasicString.isVarable(codeLine.Args[0]))
                {
                    if (codeLine.Args.Count == 1)
                    {
                        try
                        {
                            context.PushData(codeLine.Args[0], null);
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
                            context.PushConstant(codeLine.Args[0], ConstantConvertor(codeLine.Args[1]));
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
                    RuntimeLibConnector.AddToLib(ref context, arg);
                }
                break;
        }
    }
    
    private static object ConstantConvertor(string arg)
    {
        if (BasicString.isChar(arg))
        {
            return Convert.ToChar(arg.Substring(1, arg.Length-2));
        }
        else if (BasicString.isDouble(arg))
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
        else
        {
            throw new CompilationException("Type was not detected");
        }
    }
}