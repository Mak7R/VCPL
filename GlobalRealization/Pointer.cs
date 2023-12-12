namespace GlobalRealization;

public enum ContextType
{
    Null = 0, 
    Stack = 1,
    Heap = 2
}

public struct Pointer
{
    public static readonly Pointer NULL = new Pointer(ContextType.Null, -1);
    
    private ContextType pointerContextType;
    private int Position;

    public static bool operator ==(Pointer left, Pointer right)
    {
        if (left.pointerContextType == right.pointerContextType && left.Position == right.Position)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public static bool operator !=(Pointer left, Pointer right)
    {
        if (left.pointerContextType != right.pointerContextType || left.Position != right.Position)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public ContextType GetContextType
    {
        get { return this.pointerContextType; }
    }
    
    public int GetPosition
    {
        get { return this.Position; }
    }
    
    public Pointer(ContextType contextType, int position)
    {
        this.pointerContextType = contextType;
        this.Position = position;
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj is Pointer ptr)
        {
            return this == ptr;
        }
        return false;
    }

    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }
}