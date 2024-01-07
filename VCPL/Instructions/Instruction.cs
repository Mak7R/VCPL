using GlobalRealization;

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
}