using System;
using System.Collections.Generic;
using System.Threading;
using GlobalRealization;


namespace VCPL;

public class Context
{
    private Context ParentContext;

    public int Size
    {
        get { return (this.ParentContext?.Size ?? 0) + this.DataContext.Count; }
    }

    private List<(string name, MemoryObject value)> DataContext;
    
    private Context()
    {
        ParentContext = null;
        DataContext = new List<(string name, MemoryObject value)>();
    }
    
    public Context(Context parentContext)
    {
        this.ParentContext = parentContext;
        DataContext = new List<(string name, MemoryObject value)>();
    }

    public Context(Context parentContext, List<(string name, MemoryObject value)> dataContext)
    {
        this.ParentContext = parentContext;
        this.DataContext = dataContext;
    }

    public Pointer Push(string name, MemoryObject data)
    {
        if (name != null) 
            for (int i = 0; i < this.DataContext.Count; i++) 
                if (DataContext[i].name == name) 
                    throw new CompilationException("Variable with this name was declarated in this context!");
        int position = (this.ParentContext?.Size ?? 0) + DataContext.Count;
        DataContext.Add((name, data));
        return new Pointer(ContextType.Stack, position);
    }

    public void Push(List<(string name, MemoryObject value)> concateContext)
    {
        foreach (var data in concateContext) this.Push(data.name, data.value);
    }

    public Pointer Peek(string name)
    {
        for (int i = 0; i < DataContext.Count; i++) 
            if (DataContext[i].name == name) 
                return new Pointer(ContextType.Stack, (this.ParentContext?.Size ?? 0) + i);

        return this.ParentContext?.Peek(name) ?? throw new CompilationException("Variable was not found");
    }

    public MemoryObject PeekObject(string name)
    {
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
    
    public Context NewContext(List<(string, MemoryObject)> startData) => new Context(this, startData);

    public RuntimeContext Pack() => new RuntimeContext(this.ParentContext?.Pack(), this.DataContext);
}

public static class BasicContext
{
    public static List<(string name, MemoryObject value)> DeffaultContext = new List<(string name, MemoryObject value)>()
    {
        ("null", new Constant(null)),
        ("true", new Constant(true)),
        ("false", new Constant(false)),
        ("", new FunctionInstance((context, result, args) =>
        {
            context[result] = context[args[0]];
            return false;
        })),
        ("return", new FunctionInstance((context, result, args) =>
        {
            if (args.Length > 1) throw new CompilationException("Args count is more than posible");
            return true;
        })),
        ("new", new FunctionInstance((context, result, args) =>
        {
            throw new RuntimeException("New has not realisation");
        })),
        
        ("equal", new FunctionInstance(((context, result, args) =>
        {
            ((IChangeable)context[result]).Set(context[args[0]].Get().Equals(context[args[1]].Get()));
            return false;
        }))),
        ("disequal", new FunctionInstance(((context, result, args) =>
        {
            ((IChangeable)context[result]).Set(!context[args[0]].Get().Equals(context[args[1]].Get()));
            return false;
        }))),
        (">", new FunctionInstance(((context, result, args) =>
        {
            ((IChangeable)context[result]).Set(((IComparable)context[args[0]].Get()).CompareTo((IComparable)context[args[1]].Get()) == 1);
            return false;
        }))),
        
        ("<", new FunctionInstance(((context, result, args) =>
        {
            ((IChangeable)context[result]).Set(((IComparable)context[args[0]].Get()).CompareTo((IComparable)context[args[1]].Get()) == -1);
            return false;
        }))),
        
        ("if", new FunctionInstance(((context, result, args) =>
        {
            bool res = false;
            if (context[args[0]].Get() is bool trueFalse) res = trueFalse;

            if (res) ((FunctionInstance)((Function)context[args[1]]).Get()).Invoke(context, Pointer.NULL, Array.Empty<Pointer>());
            else ((FunctionInstance)((Function)context[args[2]]).Get()).Invoke(context, Pointer.NULL, Array.Empty<Pointer>());
            return false;
        }))),
        
        ("while", new FunctionInstance(((context, result, args) =>
        {
            throw new NotImplementedException();
            /// realise while
            return false;
        }))),
        
        ("Sleep", new FunctionInstance((context, result, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Thread.Sleep((int)context[args[0]].Get());
            return false;
        })),
    };
}