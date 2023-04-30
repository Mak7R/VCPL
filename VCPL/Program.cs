

namespace VCPL
{
    static class Program
    {
        static void Main()
        {
            Dictionary<string, ElementaryFunction> funcs = new Dictionary<string, ElementaryFunction>()
            {
                { "print", (ref List<Variable> stack, object? args) => { Console.Write(args); } },
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
                
                codeLines.Add(CodeLineConvertor.StringToData(line));
            }

            TempMainFunction main = new TempMainFunction(funcs, codeLines);
            
            main.Run();
        }
    }
}