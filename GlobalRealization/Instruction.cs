

namespace GlobalRealization;

public class Instruction
{
    public ElementaryFunction? Method;
    public Pointer Result;
    public Pointer[] Args;

    public void SetMethod(ElementaryFunction function)
    {
        this.Method = function;
    }

    public bool Invoke(RuntimeContext context)
    {
        return this.Method.Invoke(context, Result, Args);
    }

    public Instruction(ElementaryFunction? method, Pointer retDataId, Pointer[] argsIds)
    {
        this.Method = method;
        this.Result = retDataId;
        this.Args = argsIds;
    }
}