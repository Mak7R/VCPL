

namespace GlobalRealization.Memory;

public class Function : MemoryObject
{
    private FunctionInstance functionInstance;

    public Function(RuntimeContext context, Instruction[] program)
    {
        functionInstance = new FunctionInstance((args) =>
        {
            for (int i = 0; i < args.Length; i++) context[i].As<IChangeable>().Set(args[i].Get());

            for (int i = 0; i < program.Length; i++)
            {
                try
                {
                    program[i].Function.Invoke(program[i].Args);
                }
                catch (Return)
                {
                    break;
                }
            }
        });
    }

    public override Function Clone()
    {
        return this;
    }

    public override FunctionInstance Get()
    {
        return functionInstance;
    }
}