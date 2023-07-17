using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Loader;
using BasicFunctions;
using GlobalRealization;

namespace VCPL;

public static class Compilator
{
    public static AssemblyLoadContext LastCompilationAssemblyLoadContext { get; set; }
    public static Function Compilate(List<CodeLine> codeLines, Context context, List<string> args = null)
    {
        List<Instruction> Program = new List<Instruction>();
        
        context = context.NewContext();
        
        if (args != null) foreach (string arg in args) context.Push(arg, new Variable(null));

        Compilate(codeLines, Program, context);

        Instruction[] packedProgram = new Instruction[Program.Count];
        for (int i = 0; i < Program.Count; i++) packedProgram[i] = Program[i];

        return new Function(context.Pack(), packedProgram);
    }

    
    private static void Compilate(List<CodeLine> codeLines, List<Instruction> program, Context context)
    {
        Dictionary<int, string> UndefinedInstructions = new Dictionary<int, string>();
        List<(string name, int start, int end, Function func)> customFunctions = new List<(string, int start, int end, Function func)>();
        
        for (int i = 0; i < codeLines.Count; i++)
        {
            CodeLine codeLine = codeLines[i];
            
            if (codeLine.FunctionName != "" && codeLine.FunctionName[0] == '#')
            {
                if (codeLine.FunctionName == "#define")
                {
                    for (int j = i + 1; j < codeLines.Count; j++)
                    {
                        if (codeLines[j].FunctionName == $"#end" && codeLines[j].Args[0] == codeLine.Args[0]) // Compilation errors
                        {
                            customFunctions.Add((codeLine.Args[0], i, j, null));
                            context.Push(codeLine.Args[0], null);
                            i = j;
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
                if (CompilateCodeLine(codeLine, program, context)) UndefinedInstructions.Add(program.Count-1, codeLine.FunctionName);
            }
        }

        for (int i = 0; i < customFunctions.Count; i++)
        {
            var customFunction = customFunctions[i];
            List<string> args = codeLines[customFunction.start].Args;
            Function f = Compilate(
                codeLines.GetRange(customFunction.start + 1, customFunction.end - customFunction.start - 1),
                context,
                args.GetRange(1, args.Count - 1)
            );
            
            customFunctions[i] = (customFunction.name, customFunction.start, customFunction.end, f);
            context.Set(customFunction.name, f); //////// what would be better send function or function instance
        }

        foreach (var undefinedInstruction in UndefinedInstructions)
        {
            foreach (var customFunction in customFunctions)
            {
                if (undefinedInstruction.Value == customFunction.name)
                {
                    program[undefinedInstruction.Key].Method =
                        (ElementaryFunction)((FunctionInstance)customFunction.func.Get()).Get();
                    break;
                }

                throw new CompilationException("Object was not found!");
            }
        }
    }

    private static bool CompilateCodeLine(CodeLine codeLine, List<Instruction> program, Context context)
    {
        Pointer[] args;
        if (codeLine.Args == null || codeLine.Args.Count == 0) args = Array.Empty<Pointer>();
        else
        {
            args = new Pointer[codeLine.Args.Count];
            for (int i = 0; i < codeLine.Args.Count; i++) 
                args[i] = BasicString.isVarable(codeLine.Args[i])
                    ? context.Peek(codeLine.Args[i])
                    : context.Push(null, new Constant(ConstantConvertor(codeLine.Args[i])));
        }

        ElementaryFunction function = (ElementaryFunction)context.PeekObject(codeLine.FunctionName)?.Get();
        program.Add(
            new Instruction(
                function,
                codeLine.ReturnData == null ?  Pointer.NULL : context.Peek(codeLine.ReturnData), 
                args)
        );
        return function == null;
    }
    private static void CompilateDirective(CodeLine codeLine, Context context)
    {
        switch (codeLine.FunctionName)
        {
            case "#init":
                if (BasicString.isVarable(codeLine.Args[0]))
                {
                    switch (codeLine.Args.Count)
                    {
                        case 0:
                            throw new CompilationException("Reqired argument missed");
                            break;
                        case 1:
                            context.Push(codeLine.Args[0], new Variable(null));
                            break;
                        case 2:
                            if (BasicString.isVarable(codeLine.Args[1]))
                                context.Push(codeLine.Args[0],
                                    (MemoryObject)context.PeekObject(codeLine.Args[1]).Clone());
                            else context.Push(codeLine.Args[0], new Variable(ConstantConvertor(codeLine.Args[1])));
                            break;
                        case 3:
                            if (codeLine.Args[2] == "const")
                            {
                                if (BasicString.isVarable(codeLine.Args[1]))
                                    context.Push(codeLine.Args[0],
                                        new Constant(((MemoryObject)context.PeekObject(codeLine.Args[1]).Clone()).Get()));
                                else context.Push(codeLine.Args[0], new Constant(ConstantConvertor(codeLine.Args[1])));
                            }
                            else if(codeLine.Args[2] == "var")
                            {
                                if (BasicString.isVarable(codeLine.Args[1]))
                                    context.Push(codeLine.Args[0],
                                        (MemoryObject)context.PeekObject(codeLine.Args[1]).Clone());
                                else context.Push(codeLine.Args[0], new Variable(ConstantConvertor(codeLine.Args[1])));
                            }
                            else
                            {
                                throw new CompilationException("Undefined variable state");
                            }
                            break;
                        default: throw new CompilationException("#init has recived more than 3 args");
                    }
                }
                else
                {
                    throw new CompilationException("It isn't posible to init not a variable");
                }
                break;
            case "#import":
                foreach (string arg in codeLine.Args) CustomLibraryConnector.Import(ref context, LastCompilationAssemblyLoadContext, arg);
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