using System;
using VCPL.CodeConvertion;

namespace VCPL.Compilator;

public class CompilationException : Exception
{
    public CompilationException(string message) : base(message) { }
    public CompilationException(CodeLine line, string message) : base($"Compilation exception in line {line.LineNumber}: {message}.") 
    {
    }
}