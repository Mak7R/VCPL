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

    private readonly List<ConstantPointer> constants = new List<ConstantPointer>() { new ConstantPointer(null) };
    public void AddVar(string name) { 
        for(int i = 0; i < Count; i++)
            if (this[i].Contains(name)) 
                throw new CompilationException("This variable already exist");
        Peek().Variables.Add(name);
    }
    public IPointer AddConst(string? name, object? value) {
        (ConstantPointer ptr, int i) AddConstant(object? val)
        {
            if (val == null) return (constants[0], 0);
            else
            {
                for (int i = 0; i < constants.Count; i++)
                    if (val.Equals(constants[i].Get()))
                        return (constants[i], i);
            }
            var ptr = new ConstantPointer(val);
            constants.Add(ptr);
            return (ptr, constants.Count - 1);
        }
        var current = Peek();
        if (name != null)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Contains(name))
                    throw new CompilationException("This variable already exist");
            (var ptr, var index) = AddConstant(value);
            current.Constants.Add(name, index);
            return ptr;
        }
        else
        {
            (var ptr, _) = AddConstant(value);
            return ptr;
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
                if (constant.Key == name) return constants[constant.Value];
            }
        }
        throw new CompilationException("Variable was not found");
    }
    public object? PeekVal(string name) {
        for (int i = 0; i < Count; i++)
            if (this[i].Constants.TryGetValue(name, out int ptr)) 
                return constants[ptr].Get();
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
        for(int i = 0; i < Count; i++)
        {
            ContextLevel level = this[i];
            _rtStack.Push(level.Variables.Count);
        }
        return _rtStack;
    }
}