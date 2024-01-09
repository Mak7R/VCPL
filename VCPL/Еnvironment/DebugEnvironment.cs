using GlobalRealization;
using System;
using VCPL.CodeConvertion;
using VCPL.Compilator;
using VCPL.Compilator.GlobalInterfaceRealization;
using VCPL.Instructions;
using VCPL.Compilator.Stacks;
using System.Threading;

namespace VCPL.Еnvironment
{
    public class DebugEnvironment : AbstractEnvironment
    {
        public DebugEnvironment(ILogger logger) : base(logger) { }

        public event Action? OnStopped;

        private int level = -1;
        private bool isDbgMod = false;
        private bool isStep = false;

        public void Run()
        {
            isDbgMod = false;
            level = -1;
        }

        public void Stop()
        {
            isDbgMod = true;
            level = this.RuntimeStack.Count; // debugger exception
        }

        public void GoUp()
        {
            level--;
            isStep = true;
        }

        public void GoThrough()
        {
            isStep = true;
        }

        public void GoDown()
        {
            if (level <= RuntimeStack.Count) level++;
            isStep = true;
        }

        public override ElementaryFunction CreateFunction(Instruction[] program, int size)
        {
            return (args) =>
            {
                RuntimeStack.Push(size, args);
                for (int i = 0; i < program.Length; i++)
                {
                    try
                    {
                        while (isDbgMod && level >= RuntimeStack.Count && !isStep)
                        {
                            OnStopped?.Invoke();
                        }
                        isStep = false;
                        program[i].Function.Invoke(program[i].Args);
                        Thread.Sleep(0);
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
                    catch (ThreadInterruptedException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        var e = program[i].GenerateException(ex);
                        Logger.Log($"{e.Message}: {e.InnerException?.Message}");
                        throw e;
                    }
                }
                RuntimeStack.Pop();
            };
        }

        public override Instruction CreateInstruction(CodeLine codeLine, ElementaryFunction function, IPointer[] args)
        {
            return new DebugInstruction(codeLine, function, args);
        }
    }
}
