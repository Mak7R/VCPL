using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization.Memory;

public class FunctionInstance : MemoryObject, IExecutable
{
    private ElementaryFunction Function;

    public FunctionInstance(ElementaryFunction value)
    {
        Function = value;
    }

    public override ElementaryFunction Get()
    {
        return Function;
    }

    public void Invoke(Pointer[] args)
    {
        Function.Invoke(args);
    }

    public override FunctionInstance Clone()
    {
        return this;
    }
}