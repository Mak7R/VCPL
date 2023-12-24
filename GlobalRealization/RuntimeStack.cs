using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization;

public readonly struct StackLevel // ????????????????? struct or class
{
    public readonly object?[] Variables;
    public readonly object?[] Constants;
    public StackLevel(object?[] variables, object?[] constants)
    {
        Variables = variables;
        Constants = constants;
    }
}

public class RuntimeStack : IndexableStack<StackLevel>
{
    public void Up(object?[] variables, object?[] constants)
    {
        Push(new StackLevel(variables, constants));
    }

    public void Down()
    {
        Pop();
    }

    public new StackLevel Peek()
    {
        return base.Peek();
    }

    public object? this[Pointer ptr]
    {
        get {
            switch (ptr.MemType)
            {
                case MemoryType.Variable:
                    return this[ptr.Level].Variables[ptr.Position];
                case MemoryType.Constant:
                    return this[ptr.Level].Constants[ptr.Position];
                default:
                    throw new RuntimeException("Undefined memory type");
            }
        }
        set
        {
            if (ptr.MemType == MemoryType.Constant) throw new RuntimeException("Cannot to change constant");
            switch (ptr.MemType)
            {
                case MemoryType.Variable:
                    this[ptr.Level].Variables[ptr.Position] = value;
                    return;
                default:
                    throw new RuntimeException("Undefined memory type");
            }
        }
    }

    public T Get<T>(Pointer ptr) {
        object? value = this[ptr];
        if (value is T tValue) return tValue;
        else throw new RuntimeException($"Imposible to cast {value?.GetType().ToString() ?? "null"} to {typeof(T)}");
    }
}