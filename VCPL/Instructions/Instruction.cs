using GlobalRealization;
using System;
using VCPL.Exceptions;

namespace VCPL.Instructions;

public class Instruction
{
    public ElementaryFunction Function;
    public IPointer[] Args;

    public Instruction(ElementaryFunction function, IPointer[] args)
    {
        this.Function = function;
        this.Args = args;
    }

    public virtual RuntimeException GenerateException(Exception ex)
    {
        return new RuntimeException($"Runtime exception: {ex.Message}", ex);
    }
}