using System;

namespace VCPL.Exceptions;

public class RuntimeException : Exception
{
    public RuntimeException() { }
    public RuntimeException(string messege) : base(messege) { }
    public RuntimeException(string message, Exception ex) : base(message, ex) { }
}

