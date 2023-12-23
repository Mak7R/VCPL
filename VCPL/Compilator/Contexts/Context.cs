using System.Collections.Generic;
using GlobalRealization.Memory;

namespace VCPL.Compilator.Contexts;

public class Context : AbstractContext
{
    private readonly UninitedLocalContext _variableContext = new UninitedLocalContext();
    protected override IMemory variableContext { get { return _variableContext; } }

    public Context(AbstractContext? parentContext)
    {
        ParentContext = parentContext;
    }

    public override UninitedLocalContext Pack()
    {
        T[] ListToArrayOfValues<T>(List<ContextItem> items)
        {
            T[] array = new T[items.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (T)items[i].value;
            }
            return array;
        }
        List<ContextItem> vars = ContextValue.FindAll(i => i.mod == Modificator.Variable);
        List<ContextItem> consts = ContextValue.FindAll(i => i.mod == Modificator.Constant);
        List<ContextItem> Funcs = ContextValue.FindAll(i => i.mod == Modificator.Function);

        _variableContext.InitContext(vars.Count);
        constantContext.InitContext(ListToArrayOfValues<object?>(consts));
        functionContext.InitContext(ListToArrayOfValues<Function>(Funcs));

        return _variableContext;
    }
}