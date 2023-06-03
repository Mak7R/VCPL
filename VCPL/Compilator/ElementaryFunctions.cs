using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using GlobalRealization;

namespace VCPL;

public static class ElementaryFunctions
{
    private static Dictionary<string, ElementaryFunction> elementaryFunctions = new Dictionary<string, ElementaryFunction>()
    {
        { "print", (ref ProgramStack stack, Reference? ReturnValue, List<ProgramObject>? args) =>
        {
            ReturnValue?.Set(null);
            foreach (ProgramObject arg in args)
            {
                Console.Write(arg.Get()?.ToString());
            }
        } },
        {"read", ((ref ProgramStack stack, Reference? ReturnValue, List<ProgramObject>? args) =>
        {
            string value = Console.ReadLine();
            ReturnValue?.Set(value);
        })},
        {"endl", ((ref ProgramStack stack, Reference? ReturnValue, List<ProgramObject>? args) =>
        {
            ReturnValue?.Set(null);
            Console.WriteLine();
        })},
        { "new", (ref ProgramStack stack, Reference? ReturnValue, List<ProgramObject>? args) =>
        {
            if (args.Count == 0)
            {
                ReturnValue?.Set(null);
            }
            else if (args.Count == 1)
            {
                ReturnValue?.Set(args[0]);
            }
            else
            {
                throw new RuntimeException("new must to get only 0 or 1 args"); // 
            }
        }}
    };

    public static Dictionary<string, ElementaryFunction> Get()
    {
        return elementaryFunctions;
    }
}