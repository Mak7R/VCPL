namespace GlobalRealization;

/// <summary>
/// should be abstract
/// </summary>
public class Variable
{
    private static ulong idCreator = 0;
    public readonly ulong Id;
    public readonly string Name;
    public object? Value;

    public Variable(string name, object? value)
    {
        Id = idCreator++;
        Name = name;
        Value = value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Variable var)
            if (var.Id == Id)
                return true;
        return false;
    }

    public override int GetHashCode() => (int)(this.Id % Int32.MaxValue);
}


//// here will be new system


public abstract class ProgramObject
{
    public abstract object? Get();
}



public class Constant : ProgramObject
{
    protected object? value;
    public override object? Get()
    {
        return this.value;
    }

    public Constant(object? value)
    {
        this.value = value;
    }
}

public class Reference : ProgramObject
{
    public ProgramStack stack;
    public int index;

    public Reference(ref ProgramStack stack, int index)
    {
        this.stack = stack;
        this.index = index;
    }

    public override object? Get()
    {
        return stack[index].Value;
    }

    public void Set(object? value)
    {
        stack[index].Value = value;
    }
}

public class NVariable : ProgramObject
{
    protected object? Value;
    public override object? Get()
    {
        return Value;
    }

    public void Set(object? value)
    {
        this.Value = value;
    }
}