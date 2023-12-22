using System;
using System.Collections.Generic;

using GlobalRealization;
using GlobalRealization.Memory;


namespace VCPL.Compilator;

public class Context
{
    private RuntimeContext packedContext = new RuntimeContext();
    private Context? ParentContext;

    private List<(string? name, MemoryObject value)> ContextValue;
    
    public Context()
    {
        ParentContext = null;
        ContextValue = new List<(string? name, MemoryObject value)>();
    }
    
    public Context(Context? parentContext)
    {
        this.ParentContext = parentContext;
        ContextValue = new List<(string? name, MemoryObject value)>();
    }

    public Context NewContext() => new Context(this);

    public RuntimeContext Pack()
    {
        packedContext.InitializeContainer(this.ContextValue);
        return packedContext;
    }

    public Pointer Push(string? name, MemoryObject data)
    {
        if (name != null) 
            for (int i = 0; i < ContextValue.Count; i++) 
                if (ContextValue[i].name == name) 
                    throw new CompilationException("Variable with this name was declarated in this context!");
        int position = ContextValue.Count;
        ContextValue.Add((name, data));
        return new Pointer(packedContext, position);
    }

    public Pointer Peek(string name)
    {
        if (name == null) throw new NullReferenceException("Argument name cannot be null in current context");
        for (int i = 0; i < ContextValue.Count; i++) 
            if (ContextValue[i].name == name) 
                return new Pointer(packedContext, i);
        return this.ParentContext?.Peek(name) ?? throw new CompilationException($"Variable ({name}) was not found");
    }

    public MemoryObject PeekObject(string name)
    {
        if (name == null) throw new NullReferenceException("Argument name cannot be null in current context");
        for (int i = 0; i < ContextValue.Count; i++)
            if (ContextValue[i].name == name)
                return ContextValue[i].value;
        return this.ParentContext?.PeekObject(name) ?? throw new CompilationException($"Variable ({name}) was not found");
    }
}