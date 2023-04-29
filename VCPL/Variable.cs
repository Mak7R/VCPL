namespace VCPL;

public abstract class Variable
{
    private static ulong idCreator = 0;
    public readonly ulong Id;
    private object Value;

    public Variable(object value)
    {
        Id = idCreator++;
        Value = value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Variable var)
            if (var.Id == Id)
                return true;
        return false;
    }
}