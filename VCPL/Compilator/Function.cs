

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
        return ((DataContainer container, int reference, int[] args) => { 
            this.Run(container, reference, args);
            return false;
        });
    }
    
    public void Run(DataContainer container, int reference, int[] args)
    {
        this._container.SetContext(container);
        for (int i = 0; i < args.Length; i++) 
            this._container[this._container.Shift + i] = this._container[args[i]];

        foreach (var command in Program)
        {
            if (command.method.Invoke(this._container, command.retDataId, command.argsIds))
            {
                if (command.argsIds.Length == 0) this._container[reference] = null;
                else this._container[reference] = this._container[command.argsIds[0]];
                return;   
            }
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
