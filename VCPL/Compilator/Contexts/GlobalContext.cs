using GlobalRealization;
using GlobalRealization.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPL.Compilator.Contexts;

public class GlobalContext : AbstractContext
{
    private readonly LocalContext _variableContext = new LocalContext();
    protected override IMemory variableContext { get { return _variableContext; } }
    public GlobalContext(AbstractContext? parentContext)
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

        _variableContext.InitContext(ListToArrayOfValues<object?>(vars));
        constantContext.InitContext(ListToArrayOfValues<object?>(consts));
        functionContext.InitContext(ListToArrayOfValues<Function>(Funcs));

        return null;
    }
}
