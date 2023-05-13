

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
                    foreach (var arg in args)
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
                    Console.WriteLine();
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
                }}
                
            };

            Dictionary<string, PreProcessorDirective> PreProcessorDirectives = new Dictionary<string, PreProcessorDirective>()
            {
                {"#init", (object? args) =>
                    {
                        if (args is object[] objArgs)
                        {
                            if (objArgs[0] is ProgramStack pStack)
                                if (objArgs[1] is List<string> pArgs)
                                    foreach (var arg in pArgs)
                                    {
                                        if (ArgumentConvertor.isVarable(arg))
                                        {
                                            pStack.Add(new Variable(arg, null));
                                        }
                                        else
                                        {
                                            throw new Exception("It isn't posible to init constants");
                                        }
                                    }
                        }
                    }
                },
            };
            
            PreProcessorDirectives.Add( "#import", (object? args) => { RuntimeLibConnector.AddToLib(funcs); });
            
            Dictionary<string, ElementaryFunction> preCompilationFunctions = new Dictionary<string, ElementaryFunction>();
            

            List<CodeLine> codeLines = new List<CodeLine>();

            ProgramStack delstack = new ProgramStack();

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

            TempMainFunction main = new TempMainFunction(funcs, PreProcessorDirectives, codeLines);
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