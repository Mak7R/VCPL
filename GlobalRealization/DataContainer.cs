using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace GlobalRealization;

using System.IO;
using System.Text.Json;


public class DataContainer
{
    private object?[] _data;
    private DataContainer Context;
    private int shift;
    
    public DataContainer(int size)
    {
        this._data = new object[size];
        Context = null;
        shift = 0;
    }

    public void SetContext(DataContainer context)
    {
        if (context == null) return;
        this.Context = context;
        this.shift = context.Size;
    }
    
    public void Push(object? value, int index)
    {
        if (index < shift)
        {
            this.Context.Push(value, index);
        }
        else
        {
            this._data[index - shift] = value;
        }
    }
    
    public object? this[int index]
    {
        get { return (index < shift ? this.Context[index] : this._data[index - shift]); }
        set
        {
            if (index < shift)
            {
                this.Context[index] = value;
            }
            else
            {
                this._data[index - shift] = value;
            }
        }
    }

    public T Get<T>(int index)
    {
        return (T)(index < shift ? this.Context[index] : this._data[index - shift]);
    }
    
    public int Size
    {
        get { return (this.Context?.Size ?? 0) + this._data.Length; }
    }

    public DataContainer GetCopy()
    {
        DataContainer copy = new DataContainer(_data.Length);
        copy.SetContext(this.Context);
        for (int i = 0; i < _data.Length; i++)
        {
            object? obj = this._data[i];
            object? copyObj;

            if (obj == null) copyObj = null;
            else
            {
                string json = JsonSerializer.Serialize(obj);
                copyObj = (object)JsonSerializer.Deserialize<object>(json); /////////
                
            }
            
            copy.Push(copyObj, i + shift);
        }

        return copy;
    }
}

public class TempContainer
{
    private List<(string? name, object? value)> data;
    private TempContainer Context;
    public int Size
    {
        get { return (this.Context?.Size ?? 0) + this.data.Count; }
    }
    public TempContainer()
    {
        this.data = new List<(string? name, object? value)>();
        counter = 0;
        this.Context = null;
    }
    public TempContainer(TempContainer context)
    {
        this.Context = context;
        this.data = new List<(string? name, object? value)>();
        counter = context.Size;
    }
    
    private int counter;
    public int Push(string? name, object? value)
    {
        if (this.Context == null)
        {
            if (name == null) this.data.Add((null, value));
            else
            {
                for (int i = 0; i < this.data.Count; i++) if (this.data[i].name == name) throw new ArgumentException();
                this.data.Add((name, value));
            }
            return counter++;
        }
        else
        {
            if (name == null) this.data.Add((null, value));
            else
            {
                for (int i = 0; i < this.data.Count; i++) if (this.data[i].name == name) throw new ArgumentException();
                for (int i = 0; i < this.data.Count; i++) if (Context.data[i].name == name) throw new ArgumentException();

                this.data.Add((name, value));
            }
            return counter++;
        }
    }

    public int Peek(string name)
    {
        if (this.Context == null)
        {
            for (int i = 0; i < this.data.Count; i++)
                if (this.data[i].name == name)
                    return i;
            return -1;
        }
        else
        {
            for (int i = 0; i < this.data.Count; i++)
                if (this.data[i].name == name)
                    return i + Context.Size;
            for (int i = 0; i < this.Context.data.Count; i++)
                if (this.Context.data[i].name == name)
                    return i;
            return -1;
        }  
    }
    
    public DataContainer Pack()
    {
        DataContainer container = new DataContainer(data.Count);
        for (int i = 0; i < this.data.Count; i++) container.Push(this.data[i].value, i);
        if (this.Context != null) { container.SetContext(this.Context.Pack()); }

        return container;
    }
}