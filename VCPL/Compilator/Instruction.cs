using GlobalRealization;

namespace VCPL;

public class Instruction
{
    public ElementaryFunction? method;
    public Pointer retDataId;
    public Pointer[] argsIds;

    public void SetMethod(ElementaryFunction function)
    {
        this.method = function;
    }

    public Instruction(ElementaryFunction? method, Pointer retDataId, Pointer[] argsIds)
    {
        this.method = method;
        this.retDataId = retDataId;
        this.argsIds = argsIds;
    }
}