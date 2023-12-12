using System;
using System.Collections.Generic;

using GlobalRealization;


namespace VCPL.Compilator;

public class Context
{
    private Context? ParentContext;

    public int Size
    {
        get { return (this.ParentContext?.Size ?? 0) + this.DataContext.Count; }
    }

    private List<(string? name, MemoryObject value)> DataContext;
    
    private Context()
    {
        ParentContext = null;
        DataContext = new List<(string? name, MemoryObject value)>();
    }
    
    public Context(Context parentContext)
    {
        this.ParentContext = parentContext;
        DataContext = new List<(string? name, MemoryObject value)>();
    }

    public Context(Context? parentContext, List<(string? name, MemoryObject value)> dataContext)
    {
        this.ParentContext = parentContext;
        this.DataContext = dataContext;
    }

    public Pointer Push(string? name, MemoryObject data)
    {
        if (name != null) 
            for (int i = 0; i < this.DataContext.Count; i++) 
                if (DataContext[i].name == name) 
                    throw new CompilationException("Variable with this name was declarated in this context!");
        int position = (this.ParentContext?.Size ?? 0) + DataContext.Count;
        DataContext.Add((name, data));
        return new Pointer(ContextType.Stack, position);
    }

    public void Push(List<(string? name, MemoryObject value)> concateContext)
    {
        foreach (var data in concateContext) this.Push(data.name, data.value);
    }

    public Pointer Peek(string name)
    {
        if (name == null) throw new NullReferenceException("Argument name cannot be null in current context");
        for (int i = 0; i < DataContext.Count; i++) 
            if (DataContext[i].name == name) 
                return new Pointer(ContextType.Stack, (this.ParentContext?.Size ?? 0) + i);

        return this.ParentContext?.Peek(name) ?? throw new CompilationException("Variable was not found");
    }

    public MemoryObject PeekObject(string name)
    {
        if (name == null) throw new NullReferenceException("Argument name cannot be null in current context");
        for (int i = 0; i < DataContext.Count; i++)
            if (DataContext[i].name == name)
                return DataContext[i].value;
        return this.ParentContext?.PeekObject(name) ?? throw new CompilationException("Variable was not found");
    }

    public void Set(string name, object value)
    {
        for (int i = 0; i < DataContext.Count; i++)
            if (DataContext[i].name == name)
            {
                if (DataContext[i].value is not MemoryObject) DataContext[i] = (DataContext[i].name, (MemoryObject)value);
                else throw new CompilationException("Cannot to change constant");
            }
        this.ParentContext?.Set(name, value);
    }

    public Context NewContext() => new Context(this);
    
    public Context NewContext(List<(string?, MemoryObject)> startData) => new Context(this, startData);

    public RuntimeContext Pack() => new RuntimeContext(this.ParentContext?.Pack(), this.DataContext);
}