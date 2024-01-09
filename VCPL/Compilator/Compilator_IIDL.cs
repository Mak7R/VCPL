using BasicFunctions;
using GlobalRealization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using VCPL.CodeConvertion;
using VCPL.Еnvironment;
using VCPL.Instructions;
using VCPL.Compilator.Stacks;
using VCPL.Exceptions;

namespace VCPL.Compilator;

/// <summary>
/// Compilator Import Include Directives Lines
/// </summary>
public class Compilator_IIDL : ICompilator
{
    private readonly AbstractEnvironment _environment;
    private AssemblyLoadContext _compilatorAssemblyLoadContext = new AssemblyLoadContext(null, isCollectible: true);

    public Compilator_IIDL(AbstractEnvironment environment)
    {
        _environment = environment;
    }

    private void ReloadAssemblyLoadContext() { 
        _compilatorAssemblyLoadContext.Unload();
        _compilatorAssemblyLoadContext = new AssemblyLoadContext(null, isCollectible: true);
    }

    public List<CodeLine> Import(string fileName, string syntaxName, string namespaceName)
    {
        using StreamReader sr = new StreamReader(fileName + FileFormat);
        string code = sr.ReadToEnd();
        var importCode = _environment.ConvertCode(code, syntaxName);
        List<string> currentNames = new List<string>();
        foreach (var line in importCode)
        {
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
            for (int i = 0; i < line.Args.Count; i++)
            {
                if (currentNames.Contains(line.Args[i]))
                {
                    line.Args[i] = $"{namespaceName}.{line.Args[i]}";
                }
            }
        }
        return importCode;
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
                if (codeLine.Args.Count < 1) throw codeLine.GenerateException(ExceptionsController.IncorrectArgumentsCount(new int[] {2, 3}, codeLine.Args.Count));
                if (!importedLibs.Contains(codeLine.Args[0])) // can be diferent name but one file // will fixed with enviriment update
                {
                    importedLibs.Add(codeLine.Args[0]);
                    List<CodeLine> importCode;
                    if (codeLine.Args.Count == 2) 
                    {
                        try
                        {
                            importCode = Import(codeLine.Args[0], codeLine.Args[1], codeLine.Args[0]);
                        }
                        catch (Exception e)
                        {
                            throw codeLine.GenerateException(e.Message);
                        }
                    }
                    else if (codeLine.Args.Count == 3)
                    {
                        try
                        {
                            importCode = Import(codeLine.Args[0], codeLine.Args[1], codeLine.Args[2]);
                        }
                        catch (Exception e)
                        {
                            throw codeLine.GenerateException(e.Message);
                        }
                    }
                    else
                    {
                        throw codeLine.GenerateException(ExceptionsController.IncorrectArgumentsCount(new[] {2, 3}, codeLine.Args.Count ));
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

    public void IncludeAll(List<CodeLine> codeLines, CompileStack stack, AbstractEnvironment environment)
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
                {
                    try
                    {
                        CustomLibraryConnector.Include(stack, environment, _compilatorAssemblyLoadContext, include.Args[0]);
                    }
                    catch (Exception e)
                    {
                        throw include.GenerateException(e.Message);
                    }
                }
                else if (include.Args.Count == 2)
                {
                    try
                    {
                        CustomLibraryConnector.Include(stack, environment, _compilatorAssemblyLoadContext, include.Args[0], include.Args[1]);
                    }
                    catch(Exception e)
                    {
                        throw include.GenerateException(e.Message);
                    }
                }
                else throw include.GenerateException(ExceptionsController.IncorrectArgumentsCount(new[] {1, 2}, include.Args.Count));
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

    public ElementaryFunction CompilateMain(CompileStack stack, string code, string convertorName, string[] args)
    {
        List<CodeLine> codeLines = _environment.ConvertCode(code, convertorName);
        ImportAll(codeLines, null);
        IncludeAll(codeLines, stack, _environment);
        return Compilate(new CodeLine(0, "Main", args.ToList()), codeLines, stack, args);
    }

    public ElementaryFunction Compilate(CodeLine function, List<CodeLine> codeLines, CompileStack stack, string[] args)
    {
        List<Instruction> Program = new List<Instruction>();

        stack.Up();
        try
        {
            foreach (string arg in args) {
                try
                {
                    stack.AddVar(arg);
                }
                catch (Exception e)
                {
                    throw function.GenerateException($"in {(function.FunctionName == "Main" ? function.FunctionName : function.Args[0])}: {e.Message}");
                }
            }

            CompilateDirectives(codeLines, stack);
            CompilateCodeLines(codeLines, Program, stack);

            return _environment.CreateFunction(Program.ToArray(), stack.Down());
        }
        catch(Exception e)
        {
            stack.Down(); // can to write stack trace
            _environment.Logger.Log($"ERROR: in {(function.FunctionName == "Main" ? function.FunctionName : function.Args[0])}: {e.Message}");
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
                    if (codeLine.Args.Count == 0) throw codeLine.GenerateException(ExceptionsController.IncorrectArgumentsCount(new[] { 1, 2 }, codeLine.Args.Count));
                    if (isKeyWord(codeLine.Args[0])) throw codeLine.GenerateException(ExceptionsController.KeywordCannotBeVariable(codeLine.Args[0]));
                    if (BasicString.isVariable(codeLine.Args[0]))
                    {
                        switch (codeLine.Args.Count)
                        {
                            case 1:
                                try 
                                {
                                    stack.AddVar(codeLine.Args[0]);
                                }
                                catch (Exception e)
                                {
                                    throw codeLine.GenerateException(e.Message);
                                }
                                
                                break;
                            case 2:
                                if (!BasicString.isVariable(codeLine.Args[1]))
                                {
                                    try
                                    {
                                        stack.AddConst(codeLine.Args[0], ConstantConvertor(codeLine, 1));
                                    }
                                    catch (Exception e)
                                    {
                                        throw codeLine.GenerateException(e.Message);
                                    }
                                }
                                else
                                {
                                    throw codeLine.GenerateException(ExceptionsController.ConstantInitedByNotLiteral());
                                }
                                break;
                            default: throw codeLine.GenerateException(ExceptionsController.IncorrectArgumentsCount(new[] {1, 2}, codeLine.Args.Count));
                        }
                    }
                    else
                    {
                        throw codeLine.GenerateException(ExceptionsController.LiteralInited());
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
                    var f = Compilate(codeLine, funcCodeLines, stack, codeLine.Args.GetRange(1, codeLine.Args.Count - 1).ToArray());
                    try
                    {
                        stack.AddConst(codeLine.Args[0], f);
                    }
                    catch(Exception e)
                    {
                        throw codeLine.GenerateException(e.Message);
                    }
                    // adding to delete directive '#end'
                    compiledCodeLines.Add(codeLines[j]);
                    i = j;
                    break;
                default: throw codeLine.GenerateException(ExceptionsController.Unknown("directive", codeLine.FunctionName));
            }
            compiledCodeLines.Add(codeLine);
        }

        foreach (CodeLine compiledCodeLine in compiledCodeLines) codeLines.Remove(compiledCodeLine);
    }

    private void CompilateCodeLines(List<CodeLine> codeLines, List<Instruction> program, CompileStack stack)
    {
        foreach (CodeLine codeLine in codeLines)
        {
            object? f;
            try
            {
                f = stack.PeekVal(codeLine.FunctionName);
            }
            catch (Exception e)
            {
                throw codeLine.GenerateException(e.Message);
            }
            
            if (f is ElementaryFunction function)
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
                            try
                            {
                                args[i] = stack.PeekPtr(codeLine.Args[i]);
                            }
                            catch (Exception e)
                            {
                                throw codeLine.GenerateException(e.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                args[i] = stack.AddConst(null, ConstantConvertor(codeLine, i));
                            }catch (Exception e)
                            {
                                throw codeLine.GenerateException(e.Message);
                            }
                            
                        }
                    }
                }
                program.Add(_environment.CreateInstruction(codeLine, function, args));
            }
            else throw codeLine.GenerateException(ExceptionsController.Unknown("function", codeLine.FunctionName));
        }
    }

    private bool isKeyWord(string arg)
    {
        foreach (var keyWord in KeyWords)
            if (keyWord == arg) 
                return true;
        return false;
    }

    private static object? ConstantConvertor(CodeLine line, int pos)
    {
        string arg = line.Args[pos];
        if (arg == "true") return true;
        else if (arg == "false") return false;
        else if (arg == "null") return null;
        else if (BasicString.isChar(arg)) return Convert.ToChar(arg.Substring(1, arg.Length - 2));
        else if (BasicString.isDouble(arg)) return Convert.ToDouble(arg, new CultureInfo("en-US"));
        else if (BasicString.isNumber(arg)) return Convert.ToInt32(arg);
        else if (BasicString.isString(arg)) return arg.Substring(1, arg.Length - 2);
        else throw line.GenerateException(ExceptionsController.Unknown("type of variable", arg));
    }
}
