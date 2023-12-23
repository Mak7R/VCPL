using BasicFunctions;
using GlobalRealization;
using GlobalRealization.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using VCPL.CodeConvertion;
using VCPL.Compilator.Contexts;

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

    public readonly static string[] KeyWords = BasicValues.ToArray.Concat(BasicFunctions.ToArray).Concat(Directives.ToArray).ToArray();

    public Function Compilate(List<ICodeLine> codeLines, AbstractContext context, List<string>? args = null)
    {
        List<Instruction> Program = new List<Instruction>();

        // context up
        context = new Context(context); 

        if (args != null) foreach (string arg in args) context.Push(new ContextItem(arg, null, Modificator.Variable));
        CompilateDirectives(codeLines, context);
        CompilateCodeLines(codeLines, Program, context);

        return new Function(context.Pack(), Program.ToArray());
    }

    private void CompilateDirectives(List<ICodeLine> codeLines, AbstractContext context)
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
                            case 0: throw new CompilationException("Incorrect arguments count");
                            case 1:
                                context.Push(new ContextItem(codeLine.Args[0], null, Modificator.Variable));
                                break;
                            case 2:
                                switch(codeLine.Args[1])
                                {
                                    case "var": 
                                        context.Push(new ContextItem(codeLine.Args[0], null, Modificator.Variable));
                                        break;
                                    case "const":
                                        context.Push(new ContextItem(codeLine.Args[0], null, Modificator.Constant));
                                        break;
                                    default:
                                        throw new CompilationException("Incorrect modificator");
                                }
                                break;
                            default: throw new CompilationException("#init has recived more than 2 args");
                        }
                    }
                    else
                    {
                        throw new CompilationException("It isn't posible to init not a variable");
                    }
                    break;
                case Directives.Import:
                    if (codeLine.Args.Count == 0) throw new CompilationException("Incorect args count");
                    string lib = codeLine.Args[0];
                    if (lib.EndsWith(".vcpl"))
                    {
                        if (codeLine.Args.Count != 2) throw new CompilationException("Incorect args count");
                        try
                        {
                            using(StreamReader sr = new StreamReader(lib)) {
                                string code = sr.ReadToEnd();
                                var libCodeLines = CompilerCodeConvertor.Convert(code, codeLine.Args[1]);
                                CompilateDirectives(libCodeLines, context);
                            }
                        }
                        catch
                        {
                            throw new CompilationException("Lib import Error");
                        }
                    }
                    else if (lib.EndsWith(".dll"))
                    {
                        if (codeLine.Args.Count != 1) throw new CompilationException("Incorect args count");
                        CustomLibraryConnector.Import(context, _compilatorAssemblyLoadContext, codeLine.Args[0]);
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
                    context.Push(new ContextItem(codeLine.Args[0], Compilate(funcCodeLines, context, codeLine.Args.GetRange(1, codeLine.Args.Count - 1)), Modificator.Function));
                    
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

    private void CompilateCodeLines(List<ICodeLine> codeLines, List<Instruction> program, AbstractContext context)
    {
        foreach (ICodeLine codeLine in codeLines)
        {
            if (context.PeekObject(codeLine.FunctionName) is Function function)
            {
                Pointer[] args;
                if (codeLine.Args == null || codeLine.Args.Count == 0) args = Array.Empty<Pointer>();
                else
                {
                    args = new Pointer[codeLine.Args.Count];
                    for (int i = 0; i < codeLine.Args.Count; i++)
                        args[i] = BasicString.isVariable(codeLine.Args[i])
                            ? context.Peek(codeLine.Args[i])
                            : context.Push(new ContextItem(null, ConstantConvertor(codeLine.Args[i]), Modificator.Constant));
                }
                program.Add(new Instruction(function.Invoke, args));
            }
            else throw new CompilationException($"Unknown function: {codeLine.FunctionName}");
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
