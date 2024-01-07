using System;
using System.Collections.Generic;
using BasicFunctions;
using FileController;
using GlobalRealization;
using VCPL;
using VCPL.CodeConvertion;
using VCPL.Compilator;
using VCPL.Compilator.Stacks;

namespace VCPLConsole;

public static class Menu
{
    public static List<string> code = new List<string>(){""};
    private static Function main = null;
    private static ICodeConvertor _codeConvertor = new CLiteConvertor();
    private static CompileStack cStack;

    private static CompileStack GetBasicStack()
    {
        CompileStack basicStack = BasicStack.Get();
        basicStack.AddConst("print", new Function((stack, args) =>
        {
            foreach (var arg in args) Console.Write(arg.Get()?.ToString());
        }));
        basicStack.AddConst("read", new Function((stack, args) =>
        {
            string? value = Console.ReadLine();
            if (args.Length == 0) return;
            else if (args.Length == 1) args[0].Set(value);
            else throw new RuntimeException("Incorrect arguments count");
        }));
        basicStack.AddConst("endl", new Function((stack, args) =>
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
            string filePath;

            switch (key.KeyChar)
            {
                case '1':
                    Console.Write("Write path to file to read: ");
                    filePath = Console.ReadLine();
                    List<string> readedData = FileCodeEditor.ReadCode(filePath);
                    if (readedData != null)
                    {
                        Console.WriteLine("File was successful readed");
                        code = readedData;
                    }
                    else
                    {
                        Console.WriteLine("File was not successful readed");
                    }
                    Console.ReadKey(true);
                    break;
                case '2':
                    Console.Write("Write path to file to save: ");
                    filePath = Console.ReadLine();
                    bool ok = FileCodeEditor.WriteCode(filePath, code);
                    if (ok)
                    {
                        Console.WriteLine("File was successful writed");
                    }
                    else
                    {
                        Console.WriteLine("File was not successful writed");
                    }
                    Console.ReadKey(true);
                    break;
                case '3':
                    Console.Clear();
                    CodeEditor.SetCode(code);
                    code = CodeEditor.ConsoleReader();
                    Console.Clear();
                    Console.WriteLine("Code was successful edit");
                    Console.ReadKey(true);
                    break;
                case '4':
                    CompilateCode();
                    break;
                case '0':
                    return;
                default:
                    break;
            }
        }
    }

    public static void CompilateCode()
    {
        
        
        Console.Clear();
        List<CodeLine> codeLines = new List<CodeLine>();
        try
        {
            foreach (var line in code)
            {
                if (BasicString.IsNoDataString(line)) continue;
                codeLines.Add(_codeConvertor.Convert(line));
            }
        }
        catch(SyntaxException se)
        {
            Console.WriteLine(se.Message);
            Console.ReadKey(true);
            
            ReadOption();
        }

        ICompilator compilator = new Compilator_IIDL();
        cStack = GetBasicStack();
        try
        {
            compilator.ImportAll(codeLines, null);
            compilator.IncludeAll(codeLines, cStack); // should to change to CompilateMain
            main = compilator.Compilate(codeLines, cStack, Array.Empty<string>()); // can put args here
        }
        catch (CompilationException ce)
        {
            Console.WriteLine(ce.Message);
            Console.ReadKey(true);
            
            ReadOption();
        }
        
        Console.WriteLine("Compilation was successful");

        while (true)
        {
            
            Console.WriteLine("Tab - run, esc - exit to menu");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            Console.Clear();

            switch (keyInfo.Key)
            {
                case ConsoleKey.Tab:
                    try
                    {
                        RunCode(); 
                        Console.ReadKey(true);
                        Console.Clear();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
        
    }

    public static void RunCode()
    {
        try
        {
            main.Get().Invoke(cStack.Pack(), Array.Empty<IPointer>());
        }
        catch (RuntimeException re)
        {
            Console.WriteLine(re.Message);
            Console.ReadKey(true);

            ReadOption();
        }
    }
}