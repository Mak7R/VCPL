using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using BasicFunctions;
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
        ("temp", new Variable(null)),
        
        
        ("", new FunctionInstance((context, result, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Function '=' must to get one argument");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            
            if (res is IChangeable change) change.Set(arg1.Get());
            else throw new RuntimeException("Cannot to change constsnt");

            return false;
        })),
        ("return", new FunctionInstance((context, result, args) =>
        {
            if (args.Length > 1) throw new RuntimeException($"return recieves 0 or 1 arguments but recieved {args.Length} args");
            return true;
        })),
        ("new", new FunctionInstance((context, result, args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        })),
        
        ("+", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(BasicMath.Plus(arg1.Get(), arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        ("-", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(BasicMath.Minus(arg1.Get(), arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        ("*", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(BasicMath.Multiply(arg1.Get(), arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        ("/", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(BasicMath.Divide(arg1.Get(), arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        
        ("equal", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(arg1.Get().Equals(arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        ("disequal", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(!arg1.Get().Equals(arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        
        (">", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(((IComparable)arg1.Get()).CompareTo(arg2.Get()) == 1);
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        (">=", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(((IComparable)arg1.Get()).CompareTo(arg2.Get()) != -1);
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        
        ("<=", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(((IComparable)arg1.Get()).CompareTo(arg2.Get()) != 1);
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        ("<", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(((IComparable)arg1.Get()).CompareTo(arg2.Get()) == -1);
            else throw new RuntimeException("Cannot to change constsnt");
            
            return false;
        }))),
        
        ("if", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            MemoryObject arg1 = context[args[0]];
            Function ifTrueF = context[args[1]] is Function func1 ? func1 : throw new RuntimeException("Argument 2 must be a Function");
            Function ifFalseF = context[args[2]] is Function func2 ? func2 : throw new RuntimeException("Argument 3 must be a Function");
            
            if (arg1.Get() is bool isTrue) 
                (isTrue ? ((FunctionInstance)ifTrueF.Get()) : ((FunctionInstance)ifFalseF.Get()))
                    .Invoke(context, Pointer.NULL, Array.Empty<Pointer>());
            
            return false;
        }))),
        ("while", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject arg1 = context[args[0]];
            Function arg2 = context[args[1]] is Function func ? func : throw new RuntimeException("Argument 2 must be a Function");
            
            while ((bool)context[args[0]].Get())
            {
                ((FunctionInstance)arg2.Get()).Invoke(context, Pointer.NULL, Array.Empty<Pointer>());
            }
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