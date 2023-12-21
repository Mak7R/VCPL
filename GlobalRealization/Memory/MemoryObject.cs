using System;
using System.Reflection.Metadata;

namespace GlobalRealization.Memory;

// may be a problem with Clone

public abstract class MemoryObject : ICloneable
{
    public abstract object? Get();
    public abstract object Clone();

    public T As<T>()
    {
        if (this is T tMemoryObject) return tMemoryObject;
        else throw new RuntimeException($"Impossible to cast from {this?.GetType().ToString() ?? "null"} to {typeof(T).ToString()}");
    }
}