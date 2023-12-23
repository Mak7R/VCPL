using GlobalRealization;
using GlobalRealization.Memory;
using System;
using System.Collections.Generic;

namespace VCPL.Compilator.Contexts;

public enum Modificator
{
    Variable,
    Constant,
    Function
}

public record ContextItem(string? name, object? value, Modificator mod);

public abstract class AbstractContext
{
    protected AbstractContext? ParentContext;
    protected List<ContextItem> ContextValue = new List<ContextItem>();

    protected int VarsCount = 0;
    protected int FuncCount = 0;
    protected int ConstCount = 0;

    protected abstract IMemory variableContext { get; }
    protected readonly ConstantContext<Function> functionContext = new ConstantContext<Function>();
    protected readonly ConstantContext<object?> constantContext = new ConstantContext<object?>();

    public Pointer Push(ContextItem item)
    {
        if (item.name != null)
            for (int i = 0; i < ContextValue.Count; i++)
                if (item.name == ContextValue[i].name)
                    throw new CompilationException("Variable with this name was declarated in this context!");
        return AddItem(item);
    }
    public Pointer Peek(string name)
    {
        if (name == null) throw new NullReferenceException("Argument name cannot be null in current context");

        bool wasFound = false;
        Modificator objMod = Modificator.Variable;
        for (int i = 0; i < ContextValue.Count; i++)
        {
            if (ContextValue[i].name == name)
            {
                objMod = ContextValue[i].mod;
                wasFound = true;
                break;
            }
        }

        if (wasFound)
        {
            int counter = 0;
            for (int i = 0; i < ContextValue.Count; i++)
            {
                if (name == ContextValue[i].name)
                {
                    switch (objMod)
                    {
                        case Modificator.Variable: return new Pointer(variableContext, counter);
                        case Modificator.Function: return new Pointer(functionContext, counter);
                        case Modificator.Constant: return new Pointer(constantContext, counter);
                    }
                }
                if (ContextValue[i].mod == objMod)
                {
                    counter++;
                }
            }
            throw new CompilationException("Undefined exception in Peek");
        }
        else
        {
            return ParentContext?.Peek(name) ?? throw new CompilationException($"Variable ({name}) was not found");
        }
    }
    public object? PeekObject(string name)
    {
        if (name == null) throw new NullReferenceException("Argument name cannot be null in current context");
        for (int i = 0; i < ContextValue.Count; i++)
            if (name == ContextValue[i].name)
                return ContextValue[i].value;
        if (ParentContext == null)
        {
            throw new CompilationException($"Variable ({name}) was not found");
        }
        else
        {
            return ParentContext.PeekObject(name);
        }
    }
    private Pointer AddItem(ContextItem item)
    {
        ContextValue.Add(item);
        switch (item.mod)
        {
            case Modificator.Variable: return new Pointer(variableContext, VarsCount++);
            case Modificator.Function: return new Pointer(functionContext, FuncCount++);
            case Modificator.Constant: return new Pointer(constantContext, ConstCount++);
            default:
                throw new CompilationException("Undefined Modificator");

        }
    }

    public abstract UninitedLocalContext Pack();
}