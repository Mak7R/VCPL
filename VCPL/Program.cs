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

            Function main = null;
            try
            {
                main = Compilator.Compilate(codeLines, BasicConteext.GetBasicContext());
            }
            catch (CompilationException e)
            {
                Console.WriteLine(e.Message);
            }
            
            main.GetCopyFunction().Run(0, new int[0]);
            ConsoleKeyInfo cKI;
            while (true)
            {
                cKI = Console.ReadKey(true);
                Console.Clear();
                if (cKI.Key == ConsoleKey.Enter)
                {
                    try
                    {
                        main?.GetCopyFunction().Run(0, new int[0]);
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