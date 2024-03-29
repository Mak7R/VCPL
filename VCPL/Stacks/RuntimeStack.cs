﻿using GlobalInterface;
using System;

namespace VCPL.Stacks;

public class RuntimeStack
{
    private int _size;
    private object?[][] _array;

    private const int _defaultCapacity = 32;
    public RuntimeStack()
    {
        _array = new object?[_defaultCapacity][];
        _size = 0;
    }

    public object? this[int level, int position]
    {
        get => _array[level][position];
        set => _array[level][position] = value;
    }
    public int Count { get { return _size; } }

    public object?[] Peek()
    {
        if (_size == 0)
            throw new InvalidOperationException("Stack is empty");

        return _array[_size - 1];
    }
    public void Pop()
    {
        _size--;
    }
    public void Push(int size)
    {
        if (_size == _array.Length)
        {
            object?[][] newArray = new object?[_array.Length + _defaultCapacity][];
            Array.Copy(_array, newArray, _size);
            _array = newArray;
            _array[_size++] = new object?[size];
        }
        else
        {
            if (_array[_size] != null && _array[_size].Length >= size)
            {
                for (int i = 0; i < _array[_size].Length; i++) _array[_size][i] = null;
            }
            else
            {
                _array[_size] = new object?[size];
            }
            _size++;
        }
    }
    public void Push(int size, IPointer[] args)
    {
        if (_size == _array.Length)
        {
            object?[][] newArray = new object?[_array.Length + _defaultCapacity][];
            Array.Copy(_array, newArray, _size);
            _array = newArray;
        }
        else if (_array[_size] != null && _array[_size].Length >= size)
        {
            int i = 0;
            for (; i < args.Length; i++)
            {
                _array[_size][i] = args[i].Get();
            }
            for (; i < _array[_size].Length; i++)
            {

                _array[_size][i] = null;
            }
            _size++;
            return;
        }
        _array[_size] = new object?[size];
        for (int i = 0; i < args.Length; i++) _array[_size][i] = args[i].Get();
        _size++;
    }
    public void Clear()
    {
        _array = new object?[_defaultCapacity][];
        _size = 0;
    }
}