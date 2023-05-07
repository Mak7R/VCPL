

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
            funcs.Add( "import", (ref List<Variable> stack, object? args) => { RuntimeLibConnector.AddToLib(funcs); });

            List<CodeLine> codeLines = new List<CodeLine>();

            List<Variable> delstack = new List<Variable>();

            string line = "";
            while (true)
            {
                Console.Write(">>> ");
                line = Console.ReadLine() ?? "";
                if (line == "end") break;
                if (line == "") continue;
                if (line == "import")
                {
                    funcs["import"].Invoke(ref delstack, null);
                    continue;
                }
                if (CodeLineConvertor.IsEmpetyLine(line)) continue;

                codeLines.Add(CodeLineConvertor.StringToData(line));
            }

            TempMainFunction main = new TempMainFunction(funcs, codeLines);
            try
            {
                main.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

            Console.ReadLine();
        }
    }
}