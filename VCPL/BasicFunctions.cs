namespace VCPL;

public static class BasicFunctions
{
    private static Dictionary<string, SimpleFunction> functions = new Dictionary<string, SimpleFunction>()
    {
        { "WriteLine", new SimpleFunction((List<Variable> stack, int id) => Console.WriteLine(stack[id].Value)) },
        {
            "ReadLine",
            new SimpleFunction((List<Variable> stack, int id) => { stack[id].Value = Console.ReadLine() ?? ""; })
        }
    };
}