namespace VCPL;

public abstract class Variable
{
    private static ulong idCreator = 0;
    public readonly ulong Id;
    public readonly string Name;
    public object Value;

    public Variable(string name, object value)
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