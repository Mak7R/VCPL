namespace VCPL;

public abstract class Function : SimpleFunction
{
    private List<SimpleFunction> pFunctions;
    
    public List<SimpleFunction> Functions {
        get
        {
            return pFunctions;
        }
        protected set
        {
            pFunctions = value;
        }
    }

    public Function AddFunction
    {
        set
        {
            pFunctions.Add(value);
        }
    }

    public List<SimpleFunction> AddFunctions
    {
        set
        {
            pFunctions.AddRange(value);
        }
    }
    
    public Function(RunDelegate runFunction) : base(runFunction)
    {
        pFunctions = new List<SimpleFunction>();
    }

    public override void Run()
    {
        runFunction?.Invoke();
    }
}


/// <summary>
/// Main Function is shoud be only one in the project.
/// The main task of this function is convert string code to List of Functions (This algorithm will a Virtual Complation).
/// </summary>
public class MainFunction : Function
{
    public List<Function> ComilabledProgram;

    /// <summary>
    /// Should be realize text to Functions convertion
    /// </summary>
    public MainFunction(string program) : base((() => {}))
    {
        // here should be algorithm string to Functions convertion
        // in duration of process convertation should be used AddFunctions from import string
        ComilabledProgram = new List<Function>();
    }

    public override void Run()
    {
        foreach (var codeLine in ComilabledProgram)
        {
            codeLine.Run();
        }
    }
}