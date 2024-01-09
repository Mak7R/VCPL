using System;
using GlobalRealization;

namespace VCPL.GlobalInterfaceRealization;

/// <summary>
/// Realize a return call
/// </summary>
public sealed class Return : Exception
{
    private static IPointer? _returnedArg;

    public Return() { _returnedArg = null; }
    public Return(IPointer arg)
    {
        _returnedArg = arg;
    }

    public static IPointer? Get() { return _returnedArg; }
}
