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

public struct ConstantPointer : IPointer
{
    private readonly object? _value;

    public ConstantPointer(object? value)
    {
        _value = value;
    }

    public object? Get() { return _value; }
    public void Set(object? value)
    {
        throw new RuntimeException("Cannot to change constant");
    }
}

public struct VariablePointer : IPointer
{
    private readonly RuntimeStack _stack;
    private readonly int _level;
    private readonly int _position;

    public VariablePointer(RuntimeStack stack, int level, int position)
    {
        _stack = stack;
        _level = level;
        _position = position;
    }

    public object? Get()
    {
        return _stack[_level, _position];
    }

    public void Set(object? value)
    {
        _stack[_level, _position] = value;
    }
}