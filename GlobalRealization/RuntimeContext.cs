using System;
using System.Collections.Generic;
using GlobalRealization.Memory;

namespace GlobalRealization;

public class RuntimeContext : IMemory
{
    private MemoryObject[]? container = null;

    private static int counter = 0;
    public int id { get; set; }
    public void InitializeContainer(int size)
    {
        id = counter++;
        container = new MemoryObject[size];
    }
    public void InitializeContainer(List<(string? name, MemoryObject value)> data)
    {
        id = counter++;
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
}