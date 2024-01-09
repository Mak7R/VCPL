using GlobalRealization;
using System;
using VCPL.CodeConvertion;
using VCPL.Compilator;
using VCPL.Instructions;
using VCPL.Stacks;
using System.Threading;
using VCPL.GlobalInterfaceRealization;

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
            level = this.RuntimeStack.Count;
        }

        private void Step()
        {
            if (level > RuntimeStack.Count)
            {
                level = RuntimeStack.Count;
            }
            isStep = true;
        }

        public void GoUp()
        {
            level--;
            Step();
        }

        public void GoThrough()
        {
            Step();
        }

        public void GoDown()
        {
            level++;
            Step();
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
                            Thread.Sleep(0);
                        }
                        isStep = false;
                        program[i].Function.Invoke(program[i].Args);
                        Thread.Sleep(0); // it stops program when thread was intrrupted but makes program very slow
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
