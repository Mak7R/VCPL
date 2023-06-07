using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using GlobalRealization;

namespace GlobalRealization;

public static class ElementaryFunctions
{
    private static Dictionary<string, ElementaryFunction> elementaryFunctions = new Dictionary<string, ElementaryFunction>()
    {
        { "print", (ref DataContainer container, int retValueId, int[] argsIds) =>
        {
            if (retValueId != -1) container[retValueId] = null;
            foreach (int arg in argsIds)
            {
                Console.Write(container[arg]?.ToString());
            }
        } },
        {"read", (ref DataContainer container, int retValueId, int[] argsIds) =>
        {
            string value = Console.ReadLine();
            if (retValueId != -1) container[retValueId] = value;
        }},
        {"endl", (ref DataContainer container, int retValueId, int[] argsIds) =>
        {
            Console.WriteLine();
        }},
        { "new", (ref DataContainer container, int retValueId, int[] argsIds) =>
        {
            if (argsIds.Length == 0)
            {
                container[retValueId] = new object() { };
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