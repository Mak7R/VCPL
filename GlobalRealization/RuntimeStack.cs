using System;

namespace GlobalRealization;

public class RuntimeStack : IndexableStack<object?[]>
{
    public object?[] Constants = Array.Empty<object?>();
    public void Up(int size)
    {
        Push(new object?[size]);
    }

    public void Down()
    {
        Pop();
    }

    public object? this[int level, int position] 
    { 
        get
        {
            return this[level][position];
        }
        set
        {
            this[level][position] = value;
        }
    }

    public new void Clear()
    {
        Constants = Array.Empty<object?>();
        base.Clear();
    }
}


public class RuntimeStack2
{
    private object?[][] _array;
    private int _size;

    private const int _defaultCapacity = 10;

    public RuntimeStack2()
    {
        _array = new object?[_defaultCapacity][];
        _size = 0;
    }

    protected object?[] this[int index]
    {
        get { return _array[index]; }
    }
    public object? this[int level, int position]
    {
        get
        {
            return this[level][position];
        }
        set
        {
            this[level][position] = value;
        }
    }

    public int Count { get { return _size; } }
    public object? Peek()
    {
        if (_size == 0)
            throw new InvalidOperationException("Stack is empety");

        return _array[_size - 1];
    }
    protected object?[] Pop()
    {
        if (_size == 0)
            throw new InvalidOperationException("Stack is empety");

        object?[] obj = _array[--_size];
        _array[_size] = default!;     // Free memory quicker. ?????????? think about caching
        return obj;
    }
    protected void Push(object?[] obj)
    {
        if (_size == _array.Length)
        {
            object?[][] newArray = new object?[_array.Length + _defaultCapacity][];
            Array.Copy(_array, newArray, _size);
            _array = newArray;
        }
        _array[_size++] = obj;
    }
    public void Clear()
    {
        Constants = Array.Empty<object?>();
        _array = new object?[_defaultCapacity][];
    }

    public object?[] Constants = Array.Empty<object?>();
    public void Up(int size)
    {
        Push(new object?[size]);
    }

    public void Down()
    {
        Pop();
    }
}