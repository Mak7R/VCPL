using System;

namespace VCPL;

public class SyntaxException : Exception
{
    public SyntaxException () {}
    public SyntaxException(string message) : base(message){}
}