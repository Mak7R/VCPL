using GlobalRealization;
using System;
using VCPL.CodeConvertion;
using VCPL.Compilator;
using VCPL.Compilator.GlobalInterfaceRealization;
using VCPL.Instructions;

namespace VCPL.Еnvironment
{
    public class DebugEnvironment : AbstractEnvironment
    {
        public DebugEnvironment(ILogger logger) : base(logger) { }

        public event Action? OnStopped;

        private int level = -1;
        private bool isDbgMod = false;
        private bool isStep = false;

        private RuntimeStack? stack;

        public RuntimeStack RuntimeStack { 
            get { return stack ?? throw new Exception("Stack was not inited"); } // debugger exception
            set { stack = value; }
        } // change to debug exception

        public void Run()
        {
            isDbgMod = false;
            level = -1;
        }

        public void Stop()
        {
            isDbgMod = true;
            level = (stack?.Count ?? throw new Exception("Stack was not inited")); // debugger exception
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
            if (level <= stack?.Count) level++;
            isStep = true;
        }

        public override ElementaryFunction CreateFunction(Instruction[] program, int size)
        {
            return (stack, args) =>
            {
                stack.Push(size, args);
                for (int i = 0; i < program.Length; i++)
                {
                    try
                    {
                        while (isDbgMod && level >= stack.Count && !isStep)
                        {
                            OnStopped?.Invoke();
                        }
                        isStep = false;
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
                    catch(RuntimeException ex)
                    {
                        Logger.Log($"ERROR in {program[i]}: {ex.Message}");
                        throw;
                    }
                }
                stack.Pop();
            };
        }

        public override Instruction CreateInstruction(CodeLine codeLine, ElementaryFunction function, IPointer[] args)
        {
            return new DebugInstruction(codeLine, function, args);
        }
    }
}
