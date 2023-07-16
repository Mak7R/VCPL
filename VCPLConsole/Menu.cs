using System;
using System.Collections.Generic;
using BasicFunctions;
using GlobalRealization;

namespace VCPL;

public static class Menu
{
    public static List<string> code = new List<string>(){""};
    private static Function main = null;
    
    static Context baseContext = new Context(null, BasicContext.DeffaultContext);

    static Menu()
    {
        baseContext.Push("print", new FunctionInstance((context, result, args) =>
        {
            if (result != Pointer.NULL) context[result] = null;

            foreach (var arg in args) Console.Write(context[arg].Get()?.ToString());

            return false;
        }));
        baseContext.Push("read", new FunctionInstance((context, result, args) =>
        {
            string value = Console.ReadLine();
            if (result != Pointer.NULL)
                if (context[result] is IChangeable changeable)
                    changeable.Set(value);
            return false;
        }));

        baseContext.Push("endl", new FunctionInstance((context, result, args) =>
        {
            Console.WriteLine();
            return false;
        }));
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
                codeLines.Add(CodeLineConvertor.SyntaxCLite(line));
            }
        }
        catch(SyntaxException se)
        {
            Console.WriteLine(se.Message);
            Console.ReadKey(true);
            
            ReadOption();
        }

        try
        {
            main = Compilator.Compilate(codeLines, baseContext);
        }
        catch (CompilationException ce)
        {
            Console.WriteLine(ce.Message);
            Console.ReadKey(true);
            
            ReadOption();
        }
        
        Console.WriteLine("Compilation was successfule");

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
            ((IExecutable)main.Get()).Invoke(baseContext.Pack(), Pointer.NULL, Array.Empty<Pointer>());
        }
        catch (RuntimeException re)
        {
            Console.WriteLine(re.Message);
            Console.ReadKey(true);

            ReadOption();
        }
    }
}