using System.Collections.Generic;
using GlobalRealization;

/// <summary>
/// Namespace name must have same name with name of .dll
/// </summary>
namespace Example;

/// <summary>
/// This class is an example how to create custom libraries.
/// This class must have this name and field Context.
/// </summary>
public static class Library
{
    /// <summary>
    /// This is a necessary field.
    /// </summary>
    public readonly static List<(string? name, object? value)> Items = new()
    {
        ("ConstantName", "Some constant"),
        ("MethodName", new Function((RuntimeStack stack, IPointer[] args) => {  })),
    };
}