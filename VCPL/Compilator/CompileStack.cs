using GlobalRealization;
using System.Collections.Generic;

namespace VCPL.Compilator;

public struct ContextLevel {
    public readonly List<string> Variables;
    public readonly Dictionary<string, int> Constants;

    public ContextLevel()
    {
        Variables = new List<string>();
        Constants = new Dictionary<string, int>();
    }

    public bool Contains(string name)
    {
        return Variables.Contains(name) || Constants.ContainsKey(name);
    }
}

public class CompileStack : IndexableStack<ContextLevel>
{
    private readonly RuntimeStack _rtStack = new RuntimeStack();

    private readonly List<object?> constants = new List<object?>() { null };
    public void AddVar(string name) { 
        for(int i = 0; i < Count; i++)
            if (this[i].Contains(name)) 
                throw new CompilationException("This variable already exist");
        Peek().Variables.Add(name);
    }
    public IPointer AddConst(string? name, object? value) {
        var current = Peek();
        if (name != null)
        {
            for(int i = 0; i < Count; i++)
                if (this[i].Contains(name))
                    throw new CompilationException("This variable already exist");
            constants.Add(value);
            current.Constants.Add(name, constants.Count - 1);
            return new ConstantPointer(_rtStack, constants.Count - 1);
        }
        else
        {
            if (value == null) return new ConstantPointer(_rtStack, 0);
            else
            {
                for(int i = 0; i < constants.Count; i++)
                {
                    if (value.Equals(constants[i]))
                    {
                        return new ConstantPointer(_rtStack, i);
                    }
                }
            }
            constants.Add(value);
            return new ConstantPointer(_rtStack, constants.Count - 1);
        }
    }
    public IPointer PeekPtr(string name) {
        for (int lvl = 0; lvl < Count; lvl++)
        {
            for (int pos = 0; pos < this[lvl].Variables.Count; pos++) { 
                if (this[lvl].Variables[pos] == name)
                {
                    return new VariablePointer(_rtStack, lvl, pos);
                }
            }
            foreach(var constant in this[lvl].Constants)
            {
                if (constant.Key == name) return new ConstantPointer(_rtStack, constant.Value);
            }
        }
        throw new CompilationException("Variable was not found");
    }
    public object? PeekVal(string name) {
        for (int i = 0; i < Count; i++)
            if (this[i].Constants.TryGetValue(name, out int ptr)) 
                return constants[ptr];
        throw new CompilationException("Variable was not found");   
    }
    public void Up() { Push(new ContextLevel()); }

    public int Down()
    {
        return Pop().Variables.Count;
    }

    public RuntimeStack Pack()
    {
        _rtStack.Clear();
        _rtStack.Constants = constants.ToArray();
        for(int i = 0; i < Count; i++)
        {
            ContextLevel level = this[i];
            _rtStack.Push(level.Variables.Count);
        }
        return _rtStack;
    }
}