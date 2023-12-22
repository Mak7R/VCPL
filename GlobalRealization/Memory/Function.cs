

using System.Threading.Tasks.Dataflow;

namespace GlobalRealization.Memory;

public class Function : MemoryObject, IExecutable
{
    private ElementaryFunction _function;

    public Function(ElementaryFunction function)
    {
        _function = function;
    }

    public Function(RuntimeContext context, Instruction[] program)
    {
        _function = (args) =>
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
                    Pointer? returnedValue = Return.Get();
                    if (returnedValue == null)
                    {
                        break;
                    }
                    else
                    {
                        args[args.Length - 1].Set(((Pointer)returnedValue).Get());
                        break;
                    }
                }
            }
        };
    }

    public void Invoke(Pointer[] args)
    {
        this._function.Invoke(args);
    }

    public override Function Clone()
    {
        return this;
    }

    public override ElementaryFunction Get()
    {
        return _function;
    }
}