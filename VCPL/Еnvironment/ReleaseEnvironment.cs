using GlobalInterface;
using VCPL.CodeConvertion;
using VCPL.Instructions;
using VCPL.Stacks;
using System;
using System.Threading;
using VCPL.Exceptions;
using VCPL.GlobalInterfaceRealization;

namespace VCPL.Еnvironment
{
    public class ReleaseEnvironment : AbstractEnvironment
    {        
        public ReleaseEnvironment(ILogger logger) : base(logger) { }
        public override ElementaryFunction CreateFunction(Instruction[] program, int size)
        {
            return (args) =>
            {
                RuntimeStack.Push(size, args);
                for (int i = 0; i < program.Length; i++)
                {
                    try
                    {
                        program[i].Function.Invoke(program[i].Args);
                    }
                    catch (Return)
                    {
                        IPointer? returnedValue = Return.Get();
                        if (returnedValue != null)
                        {
                            args[args.Length - 1].Set(returnedValue.Get());
                        }
                        break;
                    }
                    catch (ThreadInterruptedException)
                    {
                        throw;
                    }
                    catch (RuntimeException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.Message);
                        throw program[i].GenerateException(ex);
                    }
                }
                RuntimeStack.Pop();
            };
        }

        public override Instruction CreateInstruction(CodeLine codeLine, ElementaryFunction function, IPointer[] args)
        {
            return new Instruction(function, args);
        }
    }
}
