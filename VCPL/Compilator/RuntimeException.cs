namespace VCPL;

// all errors which throws in runtime

public class RuntimeException : Exception
{
    public RuntimeException () {}
    public RuntimeException(string messege) : base(messege) {}
}