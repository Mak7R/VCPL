using System;
using System.Collections.Generic;

using BasicFunctions;
using GlobalRealization;

namespace VCPL
{
    static class Program
    {
        static void Main()
        {
            List<string> codeStrings = CodeEditor.ConsoleReader();
            
            Console.Clear();
            
            List<CodeLine> codeLines = new List<CodeLine>();
            foreach (var line in codeStrings)
            {
                if (BasicString.IsNoDataString(line)) continue;
                codeLines.Add(CodeLineConvertor.SyntaxCLite(line));
            }

            TempMainFunction main = null;
            try
            { 
                main = new TempMainFunction(ElementaryFunctions.Get(), codeLines);
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