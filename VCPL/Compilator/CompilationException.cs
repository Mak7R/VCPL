using System;

namespace VCPL;

public class CompilationException : Exception
{
    public CompilationException () {}
    public CompilationException(string message) : base(message){}
}