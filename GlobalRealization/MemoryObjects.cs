using System;

namespace GlobalRealization;

/// /////////////////////////////////////////////// may be a problem with Copy

public class Variable : MemoryObject, IChangeable
{
    protected object Data;

    public Variable()
    {
        this.Data = null;
    }
    
    public Variable(object value)
    {
        this.Data = value;
    }

    public override object Get()
    {
        return Data;
    }

    public void Set(object newValue)
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
    protected object Data;

    public Constant()
    {
        this.Data = null;
    }
    
    public Constant(object value)
    {
        this.Data = value;
    }

    public override object Get()
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
        return new FunctionInstance(((context, result, args) =>
        {
            RuntimeContext currentContext = this._context.Copy();
            currentContext.ParentContext = context;
            currentContext.Push(args);

            for (int i = 0; i < Program.Length; i++)
            {
                if (Program[i].Method.Invoke(currentContext, Program[i].Result, Program[i].Args))
                {
                    if (result == Pointer.NULL) { return false; }

                    if (currentContext[result] is IChangeable changeable)
                    {
                        if (Program[i].Args.Length == 0) { changeable.Set(null); }
                        else if (Program[i].Args.Length == 1) {  changeable.Set(args[0]); }
                    }
                    else
                    {
                        throw new RuntimeException("Cannot to change constant");
                    }
                    break;
                }
            }

            return false;
        }));
    }
}

public class FunctionInstance : MemoryObject, IExecutable
{
    protected ElementaryFunction Function;

    public FunctionInstance()
    {
        this.Function = null;
    }
    
    public FunctionInstance(ElementaryFunction value)
    {
        this.Function = value;
    }

    public override object Get()
    {
        return Function;
    }

    public bool Invoke(RuntimeContext context, Pointer result, Pointer[] args)
    {
        return this.Function.Invoke(context, result, args);
    }
    
    public override FunctionInstance Clone()
    {
        return this;
    }
}

public class Class
{
    
}

public class ClassInstance
{
    
}