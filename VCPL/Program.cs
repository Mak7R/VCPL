
using System.Runtime.CompilerServices;

namespace VCPL
{
    static class Program
    {
        static void Main(string[]? args)
        {
            Dictionary<string, funcDeleg> funcs = new Dictionary<string, funcDeleg>()
            {
                { "print", (object? args) => { Console.Write(args);} },
            };

            List<CodeLine> codeLines = new List<CodeLine>();
            
            string line = "";
            
            
            while (true)
            {
                Console.Write(">>> ");
                line = Console.ReadLine() ?? "";
                if (line == "end") break;
                if (line == "") continue;
                if (CodeLineConvertor.IsEmpetyLine(line)) continue;
                
                codeLines.Add(CodeLineConvertor.StringsToData(line));
            }

            TempMainFunction main = new TempMainFunction(funcs, codeLines);
            
            main.Run();
        }
        ////  here will be code editor which will create string code of Program
        // string code = "";
        // MainFunction main = new MainFunction(code);
        // main.Run();
    }
}