using BasicFunctions;
using GlobalRealization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using VCPL.CodeConvertion;


namespace VCPL.Compilator;

/// <summary>
/// Compilator Import Include Directives Lines
/// </summary>
public class Compilator_IIDL : ICompilator
{
    private AssemblyLoadContext _compilatorAssemblyLoadContext = new AssemblyLoadContext(null, isCollectible: true);

    private void ReloadAssemblyLoadContext() { 
        _compilatorAssemblyLoadContext.Unload();
        _compilatorAssemblyLoadContext = new AssemblyLoadContext(null, isCollectible: true);
    }

    private List<CodeLine> Import(string fileName, string syntaxName, string namespaceName)
    {
        try
        {
            using StreamReader sr = new StreamReader(fileName + FileFormat);
            string code = sr.ReadToEnd();
            var importCode = CompilerCodeConvertor.Convert(code, syntaxName);
            List<string> currentNames = new List<string>();
            foreach (var line in importCode) { 
                if (line.FunctionName == Directives.Init)
                {
                    currentNames.Add(line.Args[0]);
                }
                else if (line.FunctionName == Directives.Define)
                {
                    foreach (var arg in line.Args) currentNames.Add(arg);
                }
            }
            foreach (var line in importCode)
            {
                if (currentNames.Contains(line.FunctionName))
                {
                    line.FunctionName = $"{namespaceName}.{line.FunctionName}";
                }
                for(int i = 0; i < line.Args.Count; i++)
                {
                    if (currentNames.Contains(line.Args[i]))
                    {
                        line.Args[i] = $"{namespaceName}.{line.Args[i]}";
                    }
                }
            }
            return importCode;
        }
        catch
        {
            throw new CompilationException("Lib import Error");
        }
    }

    private const string FileFormat = ".vcpl";
    public void ImportAll(List<CodeLine> codeLines, List<string>? importedLibs = null)
    {
        importedLibs ??= new List<string>();
        for (int i = 0; i < codeLines.Count; )
        {
            var codeLine = codeLines[i];
            if (codeLine.FunctionName == Directives.Import)
            {
                codeLines.RemoveAt(i);
                if (codeLine.Args.Count < 1) throw new CompilationException("Incorrect args count");
                if (!importedLibs.Contains(codeLine.Args[0])) // can be diferent name but one file // will fixed with enviriment update
                {
                    importedLibs.Add(codeLine.Args[0]);
                    List<CodeLine> importCode;
                    if (codeLine.Args.Count == 2) 
                    {
                        importCode = this.Import(codeLine.Args[0], codeLine.Args[1], codeLine.Args[0]);
                    }
                    else if (codeLine.Args.Count == 3)
                    {
                        importCode = this.Import(codeLine.Args[0], codeLine.Args[1], codeLine.Args[2]);
                    }
                    else
                    {
                        throw new CompilationException("Incorrect args count");
                    }
                    ImportAll(importCode, importedLibs);
                    codeLines.InsertRange(i, importCode);
                    i += importCode.Count;
                }
            }
            else
            {
                i++;
            }
        }
    }

    public void IncludeAll(List<CodeLine> codeLines, CompileStack stack)
    {
        ReloadAssemblyLoadContext();
        List<CodeLine> includes = new List<CodeLine>();
        for (int i = 0; i < codeLines.Count; i++)
        {
            if (codeLines[i].FunctionName == Directives.Include)
            {
                includes.Add(codeLines[i]);
            }
        }
        List<CodeLine> included = new List<CodeLine>();
        foreach (var include in includes)
        {
            codeLines.Remove(include);
            if (!included.Contains(include)) 
            {
                if (include.Args.Count == 1) 
                    CustomLibraryConnector.Include(stack, _compilatorAssemblyLoadContext, include.Args[0]);
                else if (include.Args.Count == 2) 
                    CustomLibraryConnector.Include(stack, _compilatorAssemblyLoadContext, include.Args[0], include.Args[1]);
                else throw new CompilationException("Incorect args count");
                included.Add(include);
            } 
        }
    }

    public static class Directives
    {
        public const string Init = "#init";
        public const string Import = "#import";
        public const string Include = "#include";
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

    public Function Compilate(List<CodeLine> codeLines, CompileStack stack, string[] args)
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
    private void CompilateDirectives(List<CodeLine> codeLines, CompileStack stack)
    {
        List<CodeLine> compiledCodeLines = new List<CodeLine>();
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
                case Directives.Define:
                    List<CodeLine> funcCodeLines = new List<CodeLine>();
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

        foreach (CodeLine compiledCodeLine in compiledCodeLines) codeLines.Remove(compiledCodeLine);
    }

    private void CompilateCodeLines(List<CodeLine> codeLines, List<Instruction> program, CompileStack stack)
    {
        foreach (CodeLine codeLine in codeLines)
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
