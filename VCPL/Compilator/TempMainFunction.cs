

using GlobalRealization;

namespace VCPL;

public class CopyFunction
{
    private DataContainer _container;
    private Instruction[] Program;

    public CopyFunction(DataContainer container, Instruction[] program)
    {
        this._container = container;
        this.Program = program;
    }

    public ElementaryFunction GetMethod()
    {
        return ((ref DataContainer container, int reference, int[] args) => { this.Run(reference, args); });
    }
    
    public void Run(int reference, int[] args)
    {
        foreach (var command in Program)
        {
            command.method.Invoke(ref this._container, command.retDataId, command.argsIds);
        }
    }
}

public class Function
{
    private DataContainer Container;
    private Instruction[] Program;
    
    public Function(DataContainer container, Instruction[] program)
    {
        this.Container = container;
        this.Program = program;
    }

    public CopyFunction GetCopyFunction()
    {
        return new CopyFunction(Container.GetCopy(), Program);
    }
}
