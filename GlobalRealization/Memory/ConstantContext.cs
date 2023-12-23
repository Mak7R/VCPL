

namespace GlobalRealization.Memory;
public class ConstantContext<T> : IMemory
{
    private T[] data;

    public void InitContext(T[] data)
    {
        this.data = data;
    }

    public object? this[int index]
    {
        get { return data[index]; }
    }
}