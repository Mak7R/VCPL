using System;

namespace VCPL;

// all errors which throws when compilation goes wrong

public class CompilationException : Exception
{
    public CompilationException(string message) : base(message){}
}