using System;

namespace GlobalRealization;

public class Function
{
    private ElementaryFunction _function;

    public Function(ElementaryFunction function)
    {
        _function = function;
    }

    public Function(Instruction[] program, int size)
    {
        _function = (stack, args) =>
        {
            stack.Up(size);
            var current = stack.Peek();
            for (int i = 0; i < args.Length; i++) current[i] = args[i].Get(); 

            for (int i = 0; i < program.Length; i++)
            {
                try
                {
                    program[i].Function.Invoke(stack, program[i].Args);
                }
                catch (Return)
                {
                    IPointer? returnedValue = Return.Get();
                    if (returnedValue == null)
                    {
                        break;
                    }
                    else
                    {
                        args[args.Length - i].Set(returnedValue.Get());
                        break;
                    }
                }
            }
            stack.Down();
        };
    }

    public ElementaryFunction Get()
    {
        return _function;
    }
}