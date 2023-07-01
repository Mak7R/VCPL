namespace GlobalRealization;

public enum Contexts
{
    Constant = 0,
    Variable = 1
}

public class PackedContext
{
    public ConstantContainer constants;
    public DataContainer data;

    public PackedContext GetCopy()
    {
        return new PackedContext(){constants = this.constants, data = this.data.GetCopy()};
    }
    public object this[Pointer index]
    {
        get
        {
            return index.GetContextType == (byte)Contexts.Constant ? constants[index.GetPosition] : data[index.GetPosition];
        }
        set
        {
            if (index.GetContextType == (byte)Contexts.Constant)
            {
                throw new CompilationException("Cannot to change constant");
            }
            else
            {
                data[index.GetPosition] = value;
            }
        }
    }
}