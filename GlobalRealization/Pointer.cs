using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using GlobalRealization.Memory;

namespace GlobalRealization;

public struct Pointer
{
    private IMemory? memory;
    private int position;

    private string Info { get { return $"{(this.memory as RuntimeContext)?.id}[{position}]: {this.Get()?.ToString() ?? "NULL"}" ; } }
    
    public Pointer(IMemory? memory, int position)
    {
        this.memory = memory;
        this.position = position;
    }

    public object? Get()
    {
        if (memory == null) throw new RuntimeException("Null Reference Exception");
        return memory[position].Get();
    }

    public T Get<T>()
    {
        var obj = Get();
        if (obj is T tObj) return tObj;
        else throw new RuntimeException($"Impossible to cast from {obj?.GetType().ToString() ?? "null"} to {typeof(T).ToString()}");
    }

    public void Set(object? obj)
    {
        if (memory == null) throw new RuntimeException("Null Reference Exception");
        try
        {
            memory[position].As<IChangeable>().Set(obj);
        }
        catch (RuntimeException)
        {
            throw new RuntimeException("Cannot to change constant");
        }
    }

    public static readonly Pointer NULL = new Pointer(null, -1);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Pointer pointer)
        {
            return this.memory == pointer.memory && this.position == pointer.position;
        }
        return false;
    }
}