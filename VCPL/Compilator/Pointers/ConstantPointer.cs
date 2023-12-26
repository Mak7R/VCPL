using GlobalRealization;

namespace VCPL.Compilator.Pointers;
public struct ConstantPointer : IPointer
{
    private readonly object? _value;

    public ConstantPointer(object? value)
    {
        _value = value;
    }

    public object? Get() { return _value; }
    public void Set(object? value)
    {
        throw new RuntimeException("Cannot to change constant");
    }
}
