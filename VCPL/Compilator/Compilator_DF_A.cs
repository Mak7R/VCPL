using BasicFunctions;
using GlobalRealization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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

    private void ReloadAssemblyLoadContext() { 
        _compilatorAssemblyLoadContext.Unload();
        _compilatorAssemblyLoadContext = new AssemblyLoadContext(null, isCollectible: true);
    }

    public void CompilateAllIncludes(List<ICodeLine> codeLines, CompileStack stack) { 
        ReloadAssemblyLoadContext();
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

    public Function Compilate(List<ICodeLine> codeLines, CompileStack stack, string[] args)
    {
        List<Instruction> Program = new List<Instruction>();

        stack.Up();
        try
        {
            foreach (string arg in args) stack.AddVar(arg);

            CompilateDirectives(codeLines, stack);
            CompilateCodeLines(codeLines, Program, stack);

            return new Function(Program.ToArray(), stack.Down());
        }catch
        {
            stack.Down(); // can to write stack trace
            throw;
        }
    }
    private void CompilateDirectives(List<ICodeLine> codeLines, CompileStack stack)
    {
        List<ICodeLine> compiledCodeLines = new List<ICodeLine>();
        for (int i = 0; i < codeLines.Count; i++)
        {
            var codeLine = codeLines[i];
            if (!codeLine.FunctionName.StartsWith('#')) continue;
            switch (codeLine.FunctionName)
            {
                case Directives.Init:
                    if (codeLine.Args.Count == 0) throw new CompilationException("Incorrect args count");
                    if (isKeyWord(codeLine.Args[0])) throw new CompilationException("Key word cannot be a variable name");
                    if (BasicString.isVariable(codeLine.Args[0]))
                    {
                        switch (codeLine.Args.Count)
                        {
                            case 1:
                                stack.AddVar(codeLine.Args[0]);
                                break;
                            case 2:
                                if (!BasicString.isVariable(codeLine.Args[1]))
                                {
                                    stack.AddConst(codeLine.Args[0], ConstantConvertor(codeLine.Args[1]));
                                }
                                else
                                {
                                    throw new CompilationException("Constant can be inited only with value constan");
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
                                CompilateDirectives(libCodeLines, stack);
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
                        CustomLibraryConnector.Import(stack, _compilatorAssemblyLoadContext, codeLine.Args[0]);
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
                    stack.AddConst(codeLine.Args[0], 
                        Compilate(funcCodeLines, stack, codeLine.Args.GetRange(1, codeLine.Args.Count - 1).ToArray()));
                    
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

    private void CompilateCodeLines(List<ICodeLine> codeLines, List<Instruction> program, CompileStack stack)
    {
        foreach (ICodeLine codeLine in codeLines)
        {
            if (stack.PeekVal(codeLine.FunctionName) is Function function)
            {
                if (function.Get() is ElementaryFunction elementaryFunction)
                {
                    IPointer[] args;
                    if (codeLine.Args == null || codeLine.Args.Count == 0) args = Array.Empty<IPointer>();
                    else
                    {
                        args = new IPointer[codeLine.Args.Count];
                        for (int i = 0; i < codeLine.Args.Count; i++)
                        {
                            if (BasicString.isVariable(codeLine.Args[i]))
                            {
                                args[i] = stack.PeekPtr(codeLine.Args[i]);
                            }
                            else
                            {
                                args[i] = stack.AddConst(null, ConstantConvertor(codeLine.Args[i]));
                            }
                        }
                    }
                    program.Add(new Instruction(elementaryFunction, args));
                }
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
