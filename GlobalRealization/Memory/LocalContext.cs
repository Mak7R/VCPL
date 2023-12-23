using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization.Memory;
public class LocalContext : IMemory
{
    private object?[] data;

    public readonly int Id = -1;

    public static readonly Stack<LocalContext> LocalContexts = new Stack<LocalContext>();
    public LocalContext() {  }

    public LocalContext(int id, int size)
    {
        this.Id = id;
        this.data = new object?[size];
    }

    public void InitContext(object?[] data)
    {
        this.data = data;
    }

    public object? this[int index]
    {
        get
        {
            return this.data[index];
        }

        set
        {
            this.data[index] = value;
        }
    }
}
