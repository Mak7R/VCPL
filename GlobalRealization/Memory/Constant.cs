using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization.Memory;

public class Constant : MemoryObject
{
    protected object? Data;

    public Constant()
    {
        Data = null;
    }

    public Constant(object? value)
    {
        Data = value;
    }

    public override object? Get()
    {
        return Data;
    }

    public override Constant Clone()
    {
        return this;
    }
}