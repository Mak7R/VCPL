using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using GlobalRealization.Memory;

namespace GlobalRealization;

public struct Pointer
{
    private IMemory memory;
    private int position;

    /// <summary>
    /// Debug info
    /// </summary>
    private string Info { get { return $"{memory?.GetHashCode() ?? -1}[{position}]: {this.Get()?.ToString() ?? "NULL"}"; } }

    public Pointer(IMemory memory, int position)
    {
        this.memory = memory;
        this.position = position;
    }

    public object? Get()
    {
        return memory[position];
    }

    public T Get<T>()
    {
        var obj = Get();
        if (obj is T tObj) return tObj;
        else throw new RuntimeException($"Impossible to cast from {obj?.GetType().ToString() ?? "null"} to {typeof(T).ToString()}");
    }

    public void Set(object? obj)
    {
        try
        {
            ((LocalContext)memory)[position] = obj;
        }
        catch(Exception e)
        {
            throw new RuntimeException(e.Message);
        }
    }

    internal void TrySetContext()
    {
        if (memory is UninitedLocalContext uninited)
        {
            foreach (var con in LocalContext.LocalContexts)
            {
                if (con.Id == uninited.Id)
                {
                    memory = con;
                    return;
                }
            }
            throw new RuntimeException("Contexts exception");
        }
            
    }

    public static readonly Pointer NULL = new Pointer(new UninitedLocalContext(), -1);
}