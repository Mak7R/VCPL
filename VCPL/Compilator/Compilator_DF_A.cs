using BasicFunctions;
using GlobalRealization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using VCPL.CodeConvertion;

namespace VCPL.Compilator;

/// <summary>
/// Compilator Directives First version A
/// </summary>
public class Compilator_DF_A : ICompilator
{
    private AssemblyLoadContext _compilatorAssemblyLoadContext = new AssemblyLoadContext(null, isCollectible: true);

    public void ReloadAssemblyLoadContext() { 
        _compilatorAssemblyLoadContext.Unload();
        _compilatorAssemblyLoadContext = new AssemblyLoadContext(null, isCollectible: true);
    }

    public static class Directives
    {
        public const string Init = "#init";
        public const string Import = "#import";
        public const string Define = "#define";
        public const string End = "#end";
        public const string Class = "#class";
        
        private static readonly string[] list = new string[] { Init, Import, Define, End, Class };
        public static string[] ToArray { get { return list; } }
    }

    public static class BasicValues
    {
        public const string True = "true";
        public const string False = "false";
        public const string Null = "null";

        private static readonly string[] list = new string[] { True, False, Null };
        public static string[] ToArray { get { return list; } }
    }

    public static class BasicFunctions
    {
        public const string If = "if";
        public const string While = "while";

        private static readonly string[] list = new string[] { If, While };
        public static string[] ToArray { get { return list; } }
    }

    public readonly static string[] KeyWords = (string[])BasicValues.ToArray.Concat(BasicFunctions.ToArray).Concat(Directives.ToArray);

    public Function Compilate(List<ICodeLine> codeLines, Context context, List<string>? args = null)
    {
        List<Instruction> Program = new List<Instruction>();

        // context up
        context = context.NewContext(); 

        if (args != null) foreach (string arg in args) context.Push(arg, new Variable(null));
        CompilateDirectives(codeLines, context);
        CompilateCodeLines(codeLines, Program, context);

        return new Function(context.Pack(), Program.ToArray());
    }

    private void CompilateDirectives(List<ICodeLine> codeLines, Context context)
    {
        List<ICodeLine> compiledCodeLines = new List<ICodeLine>();
        for (int i = 0; i < codeLines.Count; i++)
        {
            var codeLine = codeLines[i];
            if (!codeLine.FunctionName.StartsWith('#')) continue;
            switch (codeLine.FunctionName)
            {
                case Directives.Init:
                    if (BasicString.isVariable(codeLine.Args[0]))
                    {
                        switch (codeLine.Args.Count)
                        {
                            case 0: throw new CompilationException("Reqired argument missed");
                            case 1:
                                context.Push(codeLine.Args[0], new Variable(null));
                                break;
                            case 2:
                                if (BasicString.isVariable(codeLine.Args[1]) && !this.isKeyWord(codeLine.Args[1]))
                                    context.Push(codeLine.Args[0], (MemoryObject)context.PeekObject(codeLine.Args[1]).Clone());
                                else context.Push(codeLine.Args[0], new Variable(ConstantConvertor(codeLine.Args[1])));
                                break;
                            case 3:
                                if (codeLine.Args[2] == "const")
                                {
                                    if (BasicString.isVariable(codeLine.Args[1]))
                                        context.Push(codeLine.Args[0],
                                            new Constant(((MemoryObject)context.PeekObject(codeLine.Args[1]).Clone()).Get()));
                                    else context.Push(codeLine.Args[0], new Constant(ConstantConvertor(codeLine.Args[1])));
                                }
                                else if (codeLine.Args[2] == "var")
                                {
                                    if (BasicString.isVariable(codeLine.Args[1]))
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
                case Directives.Import:
                    if (codeLine.Args.Count != 1) throw new CompilationException("Incorect args count");

                    string lib = codeLine.Args[0];
                    if (lib.EndsWith(".vcpl"))
                    {
                        throw new NotImplementedException();
                    }
                    else if (lib.EndsWith(".dll"))
                    {
                        CustomLibraryConnector.Import(ref context, _compilatorAssemblyLoadContext, codeLine.Args[0]);
                    }
                    else throw new CompilationException("Incorect name of library. Now supports .dll and .vcpl libraries only");
                    break;
                case Directives.Define:
                    List<ICodeLine> funcCodeLines = new List<ICodeLine>();
                    int j = i + 1;
                    for (; (codeLines[j].FunctionName != Directives.End || codeLines[j].Args[0] != codeLine.Args[0]); j++)
                    {
                        funcCodeLines.Add(codeLines[j]);
                        compiledCodeLines.Add(codeLines[j]);
                    }
                    context.Push(codeLine.Args[0], Compilate(funcCodeLines, context, codeLine.Args.GetRange(1, codeLine.Args.Count - 1)));
                    
                    // adding to delete directive '#end'
                    compiledCodeLines.Add(codeLines[j]);
                    i = j;
                    break;
                default: throw new CompilationException($"Unknown directive: {codeLine.FunctionName}");
            }
            compiledCodeLines.Add(codeLine);
        }

        foreach (ICodeLine compiledCodeLine in compiledCodeLines) codeLines.Remove(compiledCodeLine);
    }

    private void CompilateCodeLines(List<ICodeLine> codeLines, List<Instruction> program, Context context)
    {
        foreach (ICodeLine codeLine in codeLines)
        {
            MemoryObject memoryObject = context.PeekObject(codeLine.FunctionName);
            FunctionInstance? functionInstance = null;
            if (memoryObject is FunctionInstance) functionInstance = (FunctionInstance)memoryObject;
            if (memoryObject is Function Func && Func.Get() is FunctionInstance func) functionInstance = func;

            if (functionInstance == null) throw new CompilationException($"Unknown function: {codeLine.FunctionName}");
            else
            {
                if (functionInstance.Get() is ElementaryFunction function)
                {
                    Pointer[] args;
                    if (codeLine.Args == null || codeLine.Args.Count == 0) args = Array.Empty<Pointer>();
                    else
                    {
                        args = new Pointer[codeLine.Args.Count];
                        for (int i = 0; i < codeLine.Args.Count; i++)
                            args[i] = BasicString.isVariable(codeLine.Args[i])
                                ? context.Peek(codeLine.Args[i])
                                : context.Push(null, new Constant(ConstantConvertor(codeLine.Args[i])));
                    }
                    program.Add(
                        new Instruction(
                            function,
                            codeLine.ReturnData == null ? Pointer.NULL : context.Peek(codeLine.ReturnData),
                            args)
                    );
                }
            }
        }
    }

    private bool isKeyWord(string arg)
    {
        foreach (var keyWord in KeyWords)
            if (keyWord == arg) 
                return true;
        return false;
    }

    private static object? ConstantConvertor(string arg)
    {
        if (arg == "true") return true;
        else if (arg == "false") return false;
        else if (arg == "null") return null;
        else if (BasicString.isChar(arg)) return Convert.ToChar(arg.Substring(1, arg.Length - 2));
        else if (BasicString.isDouble(arg)) return Convert.ToDouble(arg, new CultureInfo("en-US"));
        else if (BasicString.isNumber(arg)) return Convert.ToInt32(arg);
        else if (BasicString.isString(arg)) return arg.Substring(1, arg.Length - 2);
        else throw new CompilationException("Type was not detected");
    }
}
