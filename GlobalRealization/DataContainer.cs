

using System;
using System.Collections.Generic;

namespace GlobalRealization;
public struct DataContainer
{
    private object?[] _data;
    
    public DataContainer(int size)
    {
        this._data = new object[size];
    }
    
    public void Push(object? value, int index)
    {
        this._data[index] = value;
    }
    
    public object? this[int index]
    {
        get { return this._data[index]; }
        set { this._data[index] = value; }
    }

    public T Get<T>(int index)
    {
        return (T)this._data[index];
    }
}

public class TempContainer
{
    private List<(string? name, object? value)> data;

    public TempContainer()
    {
        this.data = new List<(string? name, object? value)>();
    }

    private static int counter = 0;
    public int Push(string? name, object? value)
    {
        if (name == null) this.data.Add((null, value));
        else
        {
            for (int i = 0; i < this.data.Count; i++) if (this.data[i].name == name) throw new ArgumentException();
            this.data.Add((name, value));
        }
        return counter++;
    }

    public int Peek(string name)
    {
        for (int i = 0; i < this.data.Count; i++)
            if (this.data[i].name == name)
                return i;
        return -1;
    }
    
    public void Deconstruct(out DataContainer container, out int size)
    {
        container = new DataContainer(data.Count);
        size = data.Count;
        for (int i = 0; i < this.data.Count; i++) container.Push(this.data[i].value, i);
    }
}