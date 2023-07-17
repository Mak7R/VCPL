﻿using System;

namespace GlobalRealization;

public class CompilationException : Exception
{
    public CompilationException () {}
    public CompilationException(string message) : base(message){}
}
public class RuntimeException : Exception
{
    public RuntimeException () {}
    public RuntimeException(string messege) : base(messege) {}
}

