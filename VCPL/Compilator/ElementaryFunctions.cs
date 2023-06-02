using System.Runtime.CompilerServices;

namespace VCPL;

public delegate void ElementaryFunction(ref ProgramStack stack, List<ProgramObject>? args);

public static class ElementaryFunctions
{
    private static Dictionary<string, ElementaryFunction> elementaryFunctions = new Dictionary<string, ElementaryFunction>()
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

    public static Dictionary<string, ElementaryFunction> Get()
    {
        return elementaryFunctions;
    }
}