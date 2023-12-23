using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization.Memory;

public class UninitedLocalContext : IMemory
{
    public object? this[int index]
    {
        get { throw new RuntimeException("Context was not inited"); }
    }

    private int size = 0;
    public int Size { get { return size; } }
    public void InitContext(int size)
    {
        this.size = size;
    }

    public static int counter = 0; // dengerous
    public readonly int Id = counter++;
}