using System;
using System.Collections.Generic;
using System.Text;
using BasicFunctions;
using GlobalRealization;
using VCPL;
using VCPL.CodeConvertion;
using VCPL.Compilator;
using VCPL.Exceptions;
using VCPL.Stacks;
using VCPL.Еnvironment;

namespace VCPLConsole;

public static class Menu
{
    public static string Code { get; set; } = string.Empty;
    public static string FilePath { get; set; } = string.Empty; 

    private static CompileStack GetBasicStack()
    {
        CompileStack basicStack = BasicStack.Get();
        basicStack.AddConst("print", (ElementaryFunction)((args) =>
        {
            foreach (var arg in args) Console.Write(arg.Get()?.ToString());
        }));
        basicStack.AddConst("read", (ElementaryFunction)((args) =>
        {
            string? value = Console.ReadLine();
            if (args.Length == 0) return;
            else if (args.Length == 1) args[0].Set(value);
            else throw new RuntimeException("Incorrect arguments count");
        }));
        basicStack.AddConst("endl", (ElementaryFunction)((args) =>
        {
            Console.WriteLine();
        }));
        basicStack.Up();
        return basicStack;
    }

    public static void Draw()
    {
        Console.Clear();
        Console.WriteLine("1. Read file");
        Console.WriteLine("2. Save to file");
        Console.WriteLine("3. Edit code");
        Console.WriteLine("4. Run Code");
        Console.WriteLine("0. Exit");
    }

    public static void ReadOption()
    {
        while (true)
        {
            Draw();
            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.KeyChar)
            {
                case '1':
                    Console.Write("Write path to file to read: ");
                    FilePath = Console.ReadLine() ?? string.Empty;
                    Code = FileProvider.ReadCode(FilePath);
                    Console.ReadKey(true);
                    break;
                case '2':
                    Console.Write("Write path to file to save: ");
                    FilePath = Console.ReadLine() ?? "";
                    FileProvider.WriteCode(FilePath, Code);
                    Console.ReadKey(true);
                    break;
                case '3':
                    Console.Clear();
                    CodeEditor.SetCode(Code.Split('\n').ToList());
                    var codeLines = CodeEditor.ConsoleReader();

                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var line in codeLines)
                    {
                        stringBuilder.AppendLine(line);
                    }
                    Code = stringBuilder.ToString();

                    Console.Clear();
                    Console.WriteLine("Code was successful edit");
                    Console.ReadKey(true);
                    break;
                case '4':
                    RunCode();
                    break;
                case '0':
                    return;
                default:
                    break;
            }
        }
    }

    private static readonly ICodeConvertor _codeConvertor = new CLiteConvertor();
    static Menu()
    {
        releaseEnvironment = new ReleaseEnvironment(ConsoleLogger.CSLogger);
        debugEnvironment = new DebugEnvironment(ConsoleLogger.CSLogger);

        environment = releaseEnvironment;

        releaseEnvironment.SplitCode = (string code) => { return code.Split("\r\n"); };
        releaseEnvironment.envCodeConvertorsContainer.AddCodeConvertor("CLite", _codeConvertor);

        debugEnvironment.SplitCode = (string code) => { return code.Split("\r\n"); };
        debugEnvironment.envCodeConvertorsContainer.AddCodeConvertor("CLite", _codeConvertor);
    }

    private static AbstractEnvironment environment;
    private static readonly ReleaseEnvironment releaseEnvironment;
    private static readonly DebugEnvironment debugEnvironment;
    private static string ChosenSyntax = "CLite";
    public static void RunCode()
    {
        Console.Clear();
        ICompilator compilator = new Compilator_IIDL(environment);
        CompileStack cStack = GetBasicStack();
        RuntimeStack rtStack = cStack.Pack();
        environment.RuntimeStack = rtStack;
        
        try
        {
            ElementaryFunction main = compilator.CompilateMain(cStack, Code, ChosenSyntax, Array.Empty<string>());
            GC.Collect();
            GC.WaitForPendingFinalizers();
            main.Invoke(Array.Empty<IPointer>());
        }
        catch (SyntaxException se)
        {
            ConsoleLogger.CSLogger.Log(se.Message);
        }
        catch (CompilationException ce)
        {
            ConsoleLogger.CSLogger.Log(ce.Message);
        }
        catch (RuntimeException re)
        {
            ConsoleLogger.CSLogger.Log(re.Message);
        }
    }
}