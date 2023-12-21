using System;
using System.Collections.Generic;

using GlobalRealization;
using GlobalRealization.Memory;


namespace VCPL.Compilator;

public class Context
{
    private RuntimeContext packedContext = new RuntimeContext();
    private Context? ParentContext;

    private List<(string? name, MemoryObject value)> DataContext;
    
    private Context()
    {
        ParentContext = null;
        DataContext = new List<(string? name, MemoryObject value)>();
    }
    
    public Context(Context? parentContext)
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
            for (int i = 0; i < DataContext.Count; i++) 
                if (DataContext[i].name == name) 
                    throw new CompilationException("Variable with this name was declarated in this context!");
        int position = DataContext.Count;
        DataContext.Add((name, data));
        return new Pointer(packedContext, position);
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
                return new Pointer(packedContext, i);
        return this.ParentContext?.Peek(name) ?? throw new CompilationException($"Variable ({name}) was not found");
    }

    public MemoryObject PeekObject(string name)
    {
        if (name == null) throw new NullReferenceException("Argument name cannot be null in current context");
        for (int i = 0; i < DataContext.Count; i++)
            if (DataContext[i].name == name)
                return DataContext[i].value;
        return this.ParentContext?.PeekObject(name) ?? throw new CompilationException($"Variable ({name}) was not found");
    }

    public Context NewContext() => new Context(this);
    
    public Context NewContext(List<(string?, MemoryObject)> startData) => new Context(this, startData);

    public RuntimeContext Pack() {
        packedContext.InitializeContainer(this.DataContext);
        return packedContext;
    }
}