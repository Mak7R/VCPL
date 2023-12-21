using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization.Memory;
public class VCPLArray : Variable
{
    public VCPLArray() { }

    public VCPLArray(int capacity) : base(new object?[capacity]) {
                
    }

    public object? Get(int index) { 
        if (Data is object?[] arr)
        {
            return arr[index];
        }
        else
        {
            throw new RuntimeException("List was not inited");
        }
    }

    public void Set(int index, object? value)
    {
        if (Data is object?[] arr)
        {
            arr[index] = value;
        }
        else
        {
            throw new RuntimeException("List was not inited");
        }
    }

    public override VCPLArray Clone()
    {
        if (Data is object?[] arr)
        {
            VCPLArray array = new VCPLArray(arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] is ICloneable cloneable)
                {
                    array.Set(i, cloneable.Clone());
                }
                else
                {
                    array.Set(i, arr[i]);
                }
            }
            return array;
        }
        else
        {
            throw new RuntimeException("Incorrect type of data");
        }
    }
}
