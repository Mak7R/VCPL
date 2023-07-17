

namespace GlobalRealization;

public class Instruction // may be struct
{
    public ElementaryFunction? Method;
    public Pointer Result;
    public Pointer[] Args;

    public Instruction(ElementaryFunction? method, Pointer retDataId, Pointer[] argsIds)
    {
        this.Method = method;
        this.Result = retDataId;
        this.Args = argsIds;
    }
}