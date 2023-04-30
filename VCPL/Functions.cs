namespace VCPL;

public abstract class Function
{
    private List<ElementaryFunction> pFunctions;
    
    public List<ElementaryFunction> Functions {
        get
        {
            return pFunctions;
        }
        protected set
        {
            pFunctions = value;
        }
    }

    public ElementaryFunction AddFunction
    {
        set
        {
            pFunctions.Add(value);
        }
    }

    public List<ElementaryFunction> AddFunctions
    {
        set
        {
            pFunctions.AddRange(value);
        }
    }
    
    public Function(ElementaryFunction runFunction)
    {
        pFunctions = new List<ElementaryFunction>();
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
    public MainFunction(string program) : base(null)
    {
        // here should be algorithm string to Functions convertion
        // in duration of process convertation should be used AddFunctions from import string
        ComilabledProgram = new List<Function>();
    }
}