using System;

namespace VCPL.CodeConvertion;

public class SyntaxException : Exception
{
    private readonly int _line;
    private readonly string _message;
    public SyntaxException(int line, string message) 
        : base($"Syntax exception in line {line}: {message}.") 
    { 
        this._line = line;
        this._message = message;
    }
}