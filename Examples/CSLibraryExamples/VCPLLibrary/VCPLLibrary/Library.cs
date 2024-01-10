

using GlobalInterface;

namespace VCPLLibrary;

public static class Library
{
    /// <summary>
    /// This is a necessary field.
    /// </summary>
    public readonly static List<(string? name, object? value)> Items = new()
    {
        ("ConstantName", "Some constant"),
        ("MethodName", (ElementaryFunction)((IPointer[] args) => {  })),
    };
}
