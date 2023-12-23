

namespace GlobalRealization.Memory;

public class Function
{
    private readonly ElementaryFunction _function;

    public Function(ElementaryFunction function)
    {
        _function = function;
    }

    public Function(UninitedLocalContext context, Instruction[] program)
    {
        _function = (args) =>
        {
            LocalContext localContext = new LocalContext(context.Id, context.Size);
            LocalContext.LocalContexts.Push(localContext);

            for (int i = 0; i < args.Length; i++) localContext[i] = args[i].Get();

            for (int i = 0; i < program.Length; i++)
            {
                try
                {
                    for(int j = 0; j < program[i].Args.Length; j++) {
                        program[i].Args[j].TrySetContext(); //// dengerous
                    }
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
            LocalContext.LocalContexts.Pop();
        };
    }

    public void Invoke(Pointer[] args) { this._function.Invoke(args); }
}