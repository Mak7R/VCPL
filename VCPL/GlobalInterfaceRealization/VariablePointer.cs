using GlobalRealization;
using VCPL.Stacks;

namespace VCPL.GlobalInterfaceRealization;
public readonly struct VariablePointer : IPointer
{
    private readonly RuntimeStack _stack;
    private readonly int _level;
    private readonly int _position;

    public VariablePointer(RuntimeStack stack, int level, int position)
    {
        _stack = stack;
        _level = level;
        _position = position;
    }

    public object? Get()
    {
        return _stack[_level, _position];
    }

    public void Set(object? value)
    {
        _stack[_level, _position] = value;
    }
}
