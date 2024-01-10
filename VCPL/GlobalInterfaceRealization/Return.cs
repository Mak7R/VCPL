using System;
using GlobalInterface;

namespace VCPL.GlobalInterfaceRealization;

/// <summary>
/// Realize a return call
/// </summary>
public sealed class Return : Exception
{
    private readonly IPointer? _returnedArg;

    public Return() { _returnedArg = null; }
    public Return(IPointer arg)
    {
        _returnedArg = arg;
    }

    public IPointer? Get() { return _returnedArg; }
}
