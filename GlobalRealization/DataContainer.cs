using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace GlobalRealization;

using System.IO;
using System.Text.Json;


public class DataContainer
{
    private object[] _data;
    private DataContainer Context;
    private int shift;
    public int Shift
    {
        get { return this.shift; }
    }
    
    public DataContainer(int size)
    {
        this._data = new object[size];
        Context = null;
        shift = 0;
    }

    public void SetContext(DataContainer context)
    {
        this.Context = context;
        this.shift = context?.Size ?? 0;
    }

    public object this[int index]
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

    public int Size
    {
        get { return (this.Context?.Size ?? 0) + this._data.Length; }
    }

    public DataContainer GetCopy()
    {
        DataContainer copy = new DataContainer(_data.Length);
        
        for (int i = 0; i < _data.Length; i++)
        {
            copy[i] = Copy(this._data[i]);
        }
        copy.SetContext(this.Context);

        return copy;
    }

    public static object Copy(object item)
    {
        if (item == null) return null;
        if (item is byte byteItem) return byteItem;
        if (item is char charItem) return charItem;
        if (item is bool boolItem) return boolItem;
        if (item is int intItem) return intItem;
        if (item is double doubleItem) return doubleItem;
        if (item is string stringItem) return new string(stringItem);
        return item; // it should be error // imposible to init not based types
    }
}

public class TempContainer
{
    private List<(string? name, object value)> data;
    private TempContainer Context;
    
    private int counter;
    
    public int Size
    {
        get { return (this.Context?.Size ?? 0) + this.data.Count; }
    }
    public TempContainer()
    {
        this.data = new List<(string? name, object value)>();
        counter = 0;
        this.Context = null;
    }
    public TempContainer(TempContainer context)
    {
        this.data = new List<(string? name, object value)>();
        counter = context.Size;
        this.Context = context;
    }
    
    public int Push(string? name, object value)
    {
        if (name == null)
        {
            this.data.Add((null, value)); // throw new Exception() <-> Constant Context 
            return counter++;
        }
        
        for (int i = 0; i < this.data.Count; i++) 
            if (this.data[i].name == name) 
                throw new ArgumentException();
        
        if (this.Context != null) 
            for (int i = 0; i < this.Context.data.Count; i++) 
                if (Context.data[i].name == name) 
                    throw new ArgumentException();

        this.data.Add((name, value));
        return counter++;
    }

    public int Peek(string name)
    {
        for (int i = 0; i < this.data.Count; i++)
            if (this.data[i].name == name)
                return i + this.Context?.Size ?? 0;

        return this.Context?.Peek(name) ?? -1;
    }
    
    public DataContainer Pack()
    {
        DataContainer container = new DataContainer(data.Count);
        
        for (int i = 0; i < this.data.Count; i++) 
            container[i] = this.data[i].value;
        
        if (this.Context != null) 
            container.SetContext(this.Context.Pack());

        return container;
    }
}