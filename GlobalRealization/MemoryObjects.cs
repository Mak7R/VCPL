using System;

namespace GlobalRealization;

/// /////////////////////////////////////////////// may be a problem with Clone

public class Variable : MemoryObject, IChangeable
{
    protected object? Data;

    public Variable()
    {
        this.Data = null;
    }
    
    public Variable(object? value)
    {
        this.Data = value;
    }

    public override object? Get()
    {
        return Data;
    }

    public void Set(object? newValue)
    {
        this.Data = newValue;
    }

    public override Variable Clone()
    {
        if (this.Data is null) return new Variable(null);
        else if (this.Data is ValueType) return new Variable(Data);
        else if (this.Data is ICloneable clone) return new Variable(clone.Clone());
        else return new Variable(Data);
    }
}

public class Constant : MemoryObject
{
    protected object? Data;

    public Constant()
    {
        this.Data = null;
    }
    
    public Constant(object? value)
    {
        this.Data = value;
    }

    public override object? Get()
    {
        return Data;
    }

    public override Constant Clone()
    {
        return this;
    }
}

public class Function : MemoryObject
{
    private RuntimeContext _context;
    private Instruction[] _program;
    
    public RuntimeContext Context
    {
        get { return _context; }
    }

    public Instruction[] Program
    {
        get { return _program; }
    }
    
    public Function(RuntimeContext packedContext, Instruction[] program)
    {
        this._context = packedContext;
        this._program = program;
    }

    public override Function Clone()
    {
        return this;
    }
    
    public override object Get()
    {
        return new FunctionInstance(((result, args) =>
        {
            RuntimeContext currentContext = this._context.Copy(); // bad (copy context in runtime)
            for (int i = 0; i < args.Length; i++)
            {
                currentContext[i] = args[i].Get();
            }

            for (int i = 0; i < Program.Length; i++)
            {
                try
                {
                    Program[i].Method.Invoke(Program[i].Result, Program[i].Args);
                }
                catch (Return returnCall)
                {
                    if (result.Equals(Pointer.NULL)) { } // check nullptr
                    else if (returnCall.Args.Length == 0) { result.Set(new Variable(null)); }
                    else if (returnCall.Args.Length == 1) { returnCall.Args[0].MoveTo(result); }
                    else { throw new RuntimeException("Incorrect args count"); }
                    break;
                }
            }
        }));
    }
}

public class FunctionInstance : MemoryObject, IExecutable
{
    protected ElementaryFunction Function;
    
    public FunctionInstance(ElementaryFunction value)
    {
        this.Function = value;
    }

    public override object Get()
    {
        return Function;
    }

    public void Invoke(Pointer result, Pointer[] args)
    {
        Function.Invoke(result, args);
    }
    
    public override FunctionInstance Clone()
    {
        return this;
    }
}

internal class Class
{
    
}

internal class ClassInstance
{
    
}