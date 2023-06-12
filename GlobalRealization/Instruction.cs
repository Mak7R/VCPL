namespace GlobalRealization;

public class Instruction
{
    public ElementaryFunction? method;
    public int retDataId;
    public int[] argsIds;

    public void SetMethod(ElementaryFunction function)
    {
        this.method = function;
    }

    public Instruction(ElementaryFunction? method, int retDataId, int[] argsIds)
    {
        this.method = method;
        this.retDataId = retDataId;
        this.argsIds = argsIds;
    }
}