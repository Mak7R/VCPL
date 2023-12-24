using System;

namespace GlobalRealization;

public class Function
{
    private ElementaryFunction _function;

    public Function(ElementaryFunction function)
    {
        _function = function;
    }

    public Function(Instruction[] program, int size, object?[] constants)
    {
        _function = (stack, args) =>
        {
            stack.Up(new object?[size], constants);
            for (int i = 0; i < args.Length; i++) stack.Peek().Variables[i] = stack[args[i]]; 

            for (int i = 0; i < program.Length; i++)
            {
                try
                {
                    program[i].Function.Invoke(stack, program[i].Args);
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
                        stack[args[args.Length - i]] = stack[returnedValue];
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