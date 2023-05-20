

using System.Reflection.Metadata;

namespace VCPL
{
    static class Program
    {
        static void Main()
        {
            Dictionary<string, ElementaryFunction> funcs = new Dictionary<string, ElementaryFunction>()
            {
                { "print", (ref ProgramStack stack, List<ProgramObject>? args) =>
                {
                    foreach (ProgramObject arg in args)
                    {
                        if (arg is VCPL.Constant)
                        {
                            Console.Write(arg.Get()?.ToString());
                        }
                        else if (arg is VCPL.Reference)
                        {
                            Console.Write(arg.Get()?.ToString());
                        }
                    }
                } },
                {"set", (ref ProgramStack stack, List<ProgramObject>? args) =>
                {
                    if (args[0] is Reference reference)
                    {
                        stack[reference.index].Value = args[1].Get();
                    }
                    else
                    {
                        throw new Exception("Constant cannot be as variable in stack");
                    }
                }},
                {"read", ((ref ProgramStack stack, List<ProgramObject>? args) =>
                {
                    string value = Console.ReadLine();
                    ((Reference)args[0]).Set(value);
                })},
                {"endl", ((ref ProgramStack stack, List<ProgramObject>? args) => {Console.WriteLine();})}
            };

            Dictionary<string, ElementaryFunction> preCompilationFunctions = new Dictionary<string, ElementaryFunction>();
            
            List<string> codeStrings = CodeEditor.ConsoleReader();
            
            Console.Clear();
            
            List<CodeLine> codeLines = new List<CodeLine>();
            foreach (var line in codeStrings)
            {
                if (CodeLineConvertor.IsEmpetyLine(line)) continue;
                codeLines.Add(CodeLineConvertor.StringToData(line));
            }

            TempMainFunction main = null;
            try
            {
                main = new TempMainFunction(funcs, codeLines);
                main.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            ConsoleKeyInfo cKI;
            while (true)
            {
                cKI = Console.ReadKey(true);
                Console.Clear();
                if (cKI.Key == ConsoleKey.Enter)
                {
                    try
                    {
                        main.Run();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else if (cKI.Key == ConsoleKey.Escape)
                {
                    return;
                }
                else if (cKI.Key == ConsoleKey.Tab)
                {
                    CodeEditor.RedrawAll();
                    Program.Main();
                    return;
                }
            }

        }
    }
}