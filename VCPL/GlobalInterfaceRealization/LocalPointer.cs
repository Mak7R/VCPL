using GlobalRealization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPL.Stacks;

namespace VCPL.GlobalInterfaceRealization;
public readonly struct LocalPointer : IPointer
{
    private readonly RuntimeStack _runtimeStack;
    private readonly int _index;

    public LocalPointer(RuntimeStack runtimeStack, int index)
    {
        _runtimeStack = runtimeStack;
        _index = index;
    }

    public object? Get()
    {
        return _runtimeStack[_runtimeStack.Count - 1, _index];
    }

    public void Set(object? value)
    {
        _runtimeStack[_runtimeStack.Count - 1, _index] = value;
    }
}
