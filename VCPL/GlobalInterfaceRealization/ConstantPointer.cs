using GlobalInterface;
using VCPL.Exceptions;

namespace VCPL.GlobalInterfaceRealization;
public readonly struct ConstantPointer : IPointer
{
    private readonly object? _value;

    public ConstantPointer(object? value)
    {
        _value = value;
    }

    public readonly object? Get() { return _value; }
    public void Set(object? value)
    {
        throw new RuntimeException(ExceptionsController.CannotChangeConstant());
    }


}
