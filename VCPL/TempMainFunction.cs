namespace VCPL;

public delegate void funcDeleg(object? args);

public class TempMainFunction
{
    public List<Variable> Stack;
    public Dictionary<string, funcDeleg> Functions;
    public (funcDeleg method, object? args)[] Program;
    

    public TempMainFunction(Dictionary<string, funcDeleg> startFunctions, List<CodeLine> codeLines)
    {
        Stack = new List<Variable>();
        Functions = startFunctions;

        Program = new (funcDeleg, object?)[codeLines.Count];

        for (int i = 0; i < codeLines.Count; i++)
        {
            // arguments conversion
            Program[i] = (Functions[codeLines[i].FunctionName], codeLines[i].args[0]);
        }
    }

    public void Run()
    {
        foreach (var toDo in Program)
        {
            toDo.method.Invoke(toDo.args);
        }
    }
}