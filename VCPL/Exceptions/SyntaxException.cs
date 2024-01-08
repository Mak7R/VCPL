using System;

namespace VCPL.Exceptions;

public class SyntaxException : Exception
{
    private readonly int _line;
    private readonly string _message;
    public SyntaxException(int line, string message)
        : base($"Syntax exception in line {line}: {message}.")
    {
        _line = line;
        _message = message;
    }
}