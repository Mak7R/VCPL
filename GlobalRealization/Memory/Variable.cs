using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization.Memory;

public class Variable : MemoryObject, IChangeable
{
    protected object? Data;

    public Variable()
    {
        Data = null;
    }

    public Variable(object? value)
    {
        Data = value;
    }

    public override object? Get()
    {
        return Data;
    }

    public void Set(object? newValue)
    {
        Data = newValue;
    }

    public override Variable Clone()
    {
        if (Data is ICloneable clone) return new Variable(clone.Clone());
        else return new Variable(Data);
    }
}