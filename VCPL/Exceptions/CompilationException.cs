using System;
using VCPL.CodeConvertion;

namespace VCPL.Exceptions;

public class CompilationException : Exception
{
    public CompilationException(string message) : base(message) { }
    public CompilationException(CodeLine line, string message) : base($"Compilation exception in line {line.LineNumber}: {message}.")
    {
    }
}