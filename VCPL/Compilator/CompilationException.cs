using System;

namespace VCPL.Compilator;

public class CompilationException : Exception
{
    public CompilationException () {}
    public CompilationException(string message) : base(message){}
}