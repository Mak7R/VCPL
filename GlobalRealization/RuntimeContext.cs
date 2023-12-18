using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GlobalRealization;

public class RuntimeContext : IMemory
{
    private MemoryObject[]? container = null;

    public void InitializeContainer(int size)
    {
        container = new MemoryObject[size];
    }
    public void InitializeContainer(List<(string? name, MemoryObject value)> data)
    {
        container = new MemoryObject[data.Count];
        for (int i = 0; i < data.Count; i++) 
            this.container[i] = data[i].value;
    }

    public MemoryObject this[int position]
    {
        get
        {
            if (container == null) throw new RuntimeException("Container was not initialized");
            return this.container[position];
        }
        set
        {
            if (container == null) throw new RuntimeException("Container was not initialized");
            if (this.container?[position] is IChangeable) this.container[position] = value;
            else throw new RuntimeException("Cannot change constant");
        }
    }

    public RuntimeContext Copy()
    {
        RuntimeContext copy = new RuntimeContext();
        if (container == null)
        {
            return copy;
        }
        copy.InitializeContainer(container.Length);
        for (int i = 0; i < container.Length; i++)
        {
            copy.container[i] = (MemoryObject)container[i].Clone();
        }
        return copy;
    }
}