namespace GlobalRealization;

public abstract class MemoryObject
{
    public abstract object Get();
}

public interface ICopiable
{
    public object Copy();
}
public interface IChangeable
{
    public void Set(object newValue);
}

public interface IExecutable
{
    public bool Invoke(RuntimeContext context, Pointer result, Pointer[] args);
}