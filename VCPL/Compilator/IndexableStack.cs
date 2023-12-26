using System;

namespace VCPL.Compilator;

public abstract class IndexableStack<T>
{
    private T[] _array;
    private int _size;

    private const int _defaultCapacity = 10;

    public IndexableStack()
    {
        _array = new T[_defaultCapacity];
        _size = 0;
    }

    protected T this[int index]
    {
        get { return _array[index]; }
    }

    protected int Count { get { return _size; } }
    public T Peek()
    {
        if (_size == 0)
            throw new InvalidOperationException("Stack is empety");

        return _array[_size - 1];
    }
    protected T Pop()
    {
        if (_size == 0)
            throw new InvalidOperationException("Stack is empety");

        T obj = _array[--_size];
        _array[_size] = default!;     // Free memory quicker. ?????????? think about caching
        return obj;
    }
    protected void Push(T obj)
    {
        if (_size == _array.Length)
        {
            T[] newArray = new T[_array.Length + _defaultCapacity];
            Array.Copy(_array, newArray, _size);
            _array = newArray;
        }
        _array[_size++] = obj;
    }
    protected void Clear() // can be better
    {
        while(_size > 0)
        {
            Pop();
        }
    }
}

