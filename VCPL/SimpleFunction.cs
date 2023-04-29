namespace VCPL;

/// <summary>
/// Simple Function is abstract object.
/// It generete an Id for every functions and can check equals another object to it or not.
/// It also initialized Stack with empety List.
/// (In future stack will have max size and shoald be initialized when intialized an Function)
/// It have a one method Run. This method will realize an algorithm of Function.
/// </summary>
public class SimpleFunction
{
    protected List<Variable> Stack;
    private static ulong idCreator = 0;
    public readonly ulong Id;

    public readonly RunDelegate runFunction;
    
    public SimpleFunction(RunDelegate runFunction)
    {
        Stack = new List<Variable>();
        Id = idCreator++;
        this.runFunction = runFunction;
    }

    public virtual void Run()
    {
        // runFunction?.Invoke(Stack, id);
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is SimpleFunction sFunc)
            if (sFunc.Id == Id)
                return true;
        return false;
    }
}