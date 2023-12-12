using System;
using System.Collections.Generic;

namespace GlobalRealization;

public class RuntimeContext
{
    private RuntimeContext? _parentContext;
    public RuntimeContext? ParentContext
    {
        get { return _parentContext; } 
        set {
            this._parentContext = value;
        } 
    }
    public int Size
    {
        get { return (this.ParentContext?.Size ?? 0) + this.container.Length; }
    }

    private MemoryObject[] container;

    public RuntimeContext(RuntimeContext parentContext, int size)
    {
        this._parentContext = parentContext;
        container = new MemoryObject[size];
    }

    public RuntimeContext(RuntimeContext parentContext, List<(string name, MemoryObject value)> data)
    {
        this._parentContext = parentContext;
        container = new MemoryObject[data.Count];

        for (int i = 0; i < data.Count; i++) 
            this.container[i] = data[i].value;
    }
    public void Push(Pointer[] args)
    {
        for (int i = 0; i < args.Length; i++)
            container[i] = this[args[i]];
    }

    public MemoryObject this[Pointer pointer]
    {
        get
        {
            if (pointer == Pointer.NULL) throw new RuntimeException("Null reference error");
            return ((this._parentContext?.Size ?? 0) <= pointer.GetPosition
                ? this.container[pointer.GetPosition - (this._parentContext?.Size ?? 0)]
                : _parentContext[pointer]);
        }
        set
        {
            if (pointer == Pointer.NULL) return;
            if ((this.ParentContext?.Size ?? 0) <= pointer.GetPosition)
            {
                if (this.container[pointer.GetPosition - (this.ParentContext?.Size ?? 0)] is IChangeable)
                    this.container[pointer.GetPosition - (this.ParentContext?.Size ?? 0)] = value;
                else throw new RuntimeException("Cannot to change constant");
            }
            else
            {
                _parentContext[pointer] = value;
            }
        }
    }

    public RuntimeContext Copy()
    {
        RuntimeContext copy = new RuntimeContext(this.ParentContext, container.Length);
        for (int i = 0; i < this.container.Length; i++)
        {
            copy.container[i] = (MemoryObject)this.container[i].Clone();
        }

        return copy;
    }
}