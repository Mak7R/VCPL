﻿using System;

namespace GlobalRealization;

public abstract class MemoryObject : ICloneable
{
    public abstract object? Get();
    public abstract object Clone();
}

public interface IChangeable
{
    public void Set(object? newValue);
}

public interface IExecutable
{
    public void Invoke(Pointer result, Pointer[] args);
}