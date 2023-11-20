using System;

namespace VCPL.CodeConvertion;

public class SyntaxException : Exception
{
    public SyntaxException () {}
    public SyntaxException(string message) : base(message){}
}