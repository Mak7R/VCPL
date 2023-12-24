using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace GlobalRealization;

public enum MemoryType
{
    Constant,
    Variable
}

public record Pointer
{
    public MemoryType MemType;
    public int Position;
    public int Level;

    public Pointer(MemoryType type, int position, int level){
        MemType = type;
        Position = position;
        Level = level;
    }

    private static readonly Pointer nullptr = new Pointer(MemoryType.Constant, -1, -1);
    public static Pointer NULL { get => nullptr; }
}