using GlobalRealization;
using VCPL.CodeConvertion;
using VCPL.Compilator.GlobalInterfaceRealization;
using VCPL.Instructions;

namespace VCPL.Еnvironment
{
    public class ReleaseEnvironment : AbstractEnvironment
    {        
        public ReleaseEnvironment(ILogger logger) : base(logger) { }
        public override ElementaryFunction CreateFunction(Instruction[] program, int size)
        {
            return (stack, args) =>
            {
                stack.Push(size, args);
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
                            args[args.Length - 1].Set(returnedValue.Get());
                            break;
                        }
                    }
                    catch (RuntimeException ex)
                    {
                        Logger.Log(ex.Message);
                        throw;
                    }
                }
                stack.Pop();
            };
        }

        public override Instruction CreateInstruction(CodeLine codeLine, ElementaryFunction function, IPointer[] args)
        {
            return new Instruction(function, args);
        }
    }
}
