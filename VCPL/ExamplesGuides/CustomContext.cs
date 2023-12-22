using System.Collections.Generic;
using GlobalRealization;
using GlobalRealization.Memory;

// Namespace name must have same name with name of .dll
namespace Example;

/// <summary>
/// This class is an example how to create custom libraries.
/// This class must have this name and field Context.
/// </summary>
public static class CustomContext
{
    /// <summary>
    /// This is a necessary field.
    /// </summary>
    public static List<(string name, MemoryObject value)> Context  = new List<(string name, MemoryObject value)>()
    {
        ("VariableName", new Variable("Some data")),
        ("MethodName", new Function((Pointer[] args) => {  })),
    };
}