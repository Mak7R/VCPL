

using GlobalRealization;
using Pointer = GlobalRealization.Pointer;

namespace VCPL;

public class CopyFunction
{
    private PackedContext _context;
    private Instruction[] Program;

    public CopyFunction(PackedContext context, Instruction[] program)
    {
        this._context = context;
        this.Program = program;
    }

    public ElementaryFunction GetMethod()
    {
        return ((PackedContext container, Pointer reference, Pointer[] args) => { 
            this.Run(container, reference, args);
            return false;
        });
    }
    
    public void Run(PackedContext container, Pointer reference, Pointer[] args)
    {
        
        
        this._context.data.SetContext(container.data);
        this._context.constants.SetContext(container.constants);
        for (int i = 0; i < args.Length; i++) 
            this._context.data[this._context.data.GetContext()?.Size ?? 0 + i] = this._context.data[args[i].GetPosition];

        foreach (var command in Program)
        {
            if (command.method.Invoke(this._context, command.retDataId, command.argsIds))
            {
                if (command.argsIds.Length == 0) this._context[reference] = null;
                else this._context[reference] = this._context[command.argsIds[0]];
                return;   
            }
        }
    }
}

public class Function
{
    private PackedContext _context;
    private Instruction[] Program;
    
    public Function(PackedContext packedContext, Instruction[] program)
    {
        this._context = packedContext;
        this.Program = program;
    }

    public CopyFunction GetCopyFunction()
    {
        return new CopyFunction(_context.GetCopy(), Program);
    }
}
