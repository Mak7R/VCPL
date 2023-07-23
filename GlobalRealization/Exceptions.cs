using System;

namespace GlobalRealization;

public class RuntimeException : Exception
{
    public RuntimeException () {}
    public RuntimeException(string messege) : base(messege) {}
}

