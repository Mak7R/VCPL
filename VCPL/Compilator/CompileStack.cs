using GlobalRealization;
using System.Collections.Generic;

namespace VCPL.Compilator;

public record class Constant
{
    public string? Name;
    public object? Value;

    public Constant(string? name, object? value)
    {
        Name = name;
        Value = value;
    }
}
public class Constants : List<Constant>
{
    public object?[] GetArray()
    {
        object?[] array = new object?[this.Count];

        for (int i = 0; i < this.Count; i++)
        {
            array[i] = this[i].Value;
        }

        return array;
    }
}

public struct ContextLevel {
    public readonly List<string> Variables;
    public readonly Constants Constants;

    public ContextLevel()
    {
        Variables = new List<string>();
        Constants = new Constants();
    }
}

public class CompileStack : IndexableStack<ContextLevel>
{
    public void AddVar(string name) { 
        for(int i = 0; i < Count; i++)
            if (this[i].Variables.Contains(name) && this[i].Constants.Find(i => i.Name == name) != null) 
                throw new CompilationException("This variable already exist");
        Peek().Variables.Add(name);
    }
    public Pointer AddConst(string? name, object? value) {
        if (name != null)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Variables.Contains(name) || this[i].Constants.Find(i => i.Name == name) != null)
                    throw new CompilationException("This variable already exist");
        }
        var current = Peek();
        current.Constants.Add(new Constant(name, value));
        return new Pointer(MemoryType.Constant, current.Constants.Count - 1, Count - 1);
    }
    public Pointer PeekPtr(string name) {
        for (int i = 0; i < Count; i++)
        {
            for (int j = 0; j < this[i].Variables.Count; j++) { 
                if (this[i].Variables[j] == name)
                {
                    return new Pointer(MemoryType.Variable, j, i);
                }
            }
            for(int j = 0; j < this[i].Constants.Count; j++)
            {
                if (this[i].Constants[j].Name == name)
                {
                    return new Pointer(MemoryType.Constant, j, i);
                }
            }
        }
        throw new CompilationException("Variable was not found");
    }
    public object? PeekVal(string name) {
        for (int i = 0; i < Count; i++)
        {
            Constant? item = this[i].Constants.Find(i => i.Name == name);
            if (item != null) return item.Value;
        }
        throw new CompilationException("Variable was not found");   
    }
    public void Up() { Push(new ContextLevel()); }

    public (int size, object?[] consts) Down()
    {
        var contextLevel = Pop();
        return (contextLevel.Variables.Count, contextLevel.Constants.GetArray());
    }

    public RuntimeStack ToRuntimeStack()
    {
        RuntimeStack stack = new RuntimeStack();
        for(int i = 0; i < Count; i++)
        {
            ContextLevel level = this[i];
            stack.Up(new object?[level.Variables.Count], level.Constants.GetArray());
        }
        return stack;
    }
}