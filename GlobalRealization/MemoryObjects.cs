using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalRealization;

public class Variable : MemoryObject, IChangeable, ICopiable
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

    public virtual object Copy()
    {
        throw new System.NotImplementedException();
        if (this.Data is ICopiable copiable) return new Variable(copiable.Copy());
        else
        {
            throw new System.NotImplementedException();
        }
    }
}

public class Constant : MemoryObject, ICopiable
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

    public virtual object Copy()
    {
        throw new System.NotImplementedException();
        if (this.Data is ICopiable copiable) return new Variable(copiable.Copy());
        else
        {
            throw new System.NotImplementedException();
        }
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

    public override object Get()
    {
        return new FunctionInstance(((context, result, args) =>
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// /////////////////////////////////////////// copy context missed
            this._context.ParentContext = context;
            this._context.Push(args);

            for (int i = 0; i < Program.Length; i++)
            {
                if (Program[i].Invoke(_context))
                {
                    if (result == Pointer.NULL) { return false; }

                    if (this._context[result] is IChangeable changeable)
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
}

public class Class
{
    
}

public class ClassInstance
{
    
}