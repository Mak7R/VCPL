using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace GlobalRealization;

public struct Pointer
{
    private IMemory? memory;
    private int position;
    
    public Pointer(IMemory? memory, int position)
    {
        this.memory = memory;
        this.position = position;
    }

    public MemoryObject Get()
    {
        if (memory == null) throw new RuntimeException("Null reference");
        return memory[position];
    }

    public T Get<T>() where T: MemoryObject
    {
        if (memory == null) throw new RuntimeException("Null reference");
        if (memory[position] is T memObj)
        {
            return memObj;
        }
        else
        {
            throw new RuntimeException($"Impossible to cast from {memory[position].GetType()} to {typeof(T)}");
        }
    }

    public void Set(MemoryObject memoryObject)
    {
        if (memory == null) throw new RuntimeException("Null reference");
        memory[position] = memoryObject;
    }

    public void Set(object? obj)
    {
        if (this.Get() is IChangeable changeable) changeable.Set(obj); 
    }

    public void MoveTo(Pointer pointer)
    {
        pointer.Set(Get());
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