
namespace VCPL
{
    static class Program
    {
        delegate void funcDeleg();
        static void Main(string[] args)
        {
            Dictionary<string, funcDeleg> funcs = new Dictionary<string, funcDeleg>()
            {
                { "init", () => {} },
                { "print", () => {} },
                { "write", () => {} }
            };
            
            string line = "";
            
            string funcName;
            string[]? arguments;
            
            while (true)
            {
                Console.Write(">>> ");
                line = Console.ReadLine() ?? "";
                if (line == "end") break;
                if (line == "") continue;

                
                (funcName, arguments) = StringsToData(line);
                
                Console.WriteLine(funcName);

                if (arguments != null) 
                    foreach (var arg in arguments)
                        Console.WriteLine($"Arg: <{arg}>");

            }
        }

        
        
        static (string, string[]) StringsToData(string line)
        {
            string funcName = "";
            string argsString = "";
            
            int i = 0;
            while (line[i] != ':')
            {
                i++;
                if (i == line.Length) throw new Exception("Incorect input");
            }
                
            funcName = line.Substring(0, i);
            argsString = line.Substring(i + 1);

            string[] argsList = argsString.Split(',');

            bool deleteSpace = true;
            for (i = 0; i < argsList.Length; i++)
            {
                for (int j = 0; j < argsList[i].Length;)
                {
                    if (argsList[i][j] == ' ' && deleteSpace) argsList[i] = argsList[i].Remove(j, 1);
                    else if (argsList[i][j] == '\"')
                    {
                        deleteSpace = !deleteSpace;
                        j++;
                    }
                    else j++;
                }
            }

            if (argsList.Length == 1 && argsList[0] == "") return (funcName, null);
            return (funcName, argsList);
        }
        ////  here will be code editor which will create string code of Program
        // string code = "";
        // MainFunction main = new MainFunction(code);
        // main.Run();
    }
}