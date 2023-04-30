namespace VCPL;

public class TempMainFunction
{
    public List<Variable> Stack;
    public Dictionary<string, ElementaryFunction> Functions;
    public (ElementaryFunction method, object? args)[] Program;
    

    public TempMainFunction(Dictionary<string, ElementaryFunction> startFunctions, List<CodeLine> codeLines)
    {
        Stack = new List<Variable>();
        Functions = startFunctions;

        Program = new (ElementaryFunction, object?)[codeLines.Count];

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
            toDo.method.Invoke(ref Stack, toDo.args);
        }
    }
}