

namespace GlobalRealization;

public class Instruction // may be struct
{
    public ElementaryFunction Function { get; set; }
    public Pointer[] Args { get; set; }

    public Instruction(ElementaryFunction function, Pointer[] args)
    {
        this.Function = function;
        this.Args = args;
    }
}

