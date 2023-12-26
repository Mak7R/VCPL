using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace GlobalRealization;

public interface IPointer
{
    public object? Get();
    public void Set(object? value);
    public T Get<T>()
    {
        var obj = Get();
        if (obj is T tObj) return tObj;
        else throw new RuntimeException($"Imposible to cast from {obj?.GetType().ToString() ?? "null"} to {typeof(T)}");
    }
}