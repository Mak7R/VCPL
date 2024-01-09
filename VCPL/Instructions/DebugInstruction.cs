using GlobalInterface;
using System;
using VCPL.CodeConvertion;
using VCPL.Exceptions;

namespace VCPL.Instructions;
public class DebugInstruction : Instruction
{
    public readonly CodeLine codeLine;
    public DebugInstruction(CodeLine codeLine, ElementaryFunction function, IPointer[] args) : base(function, args)
    {
        this.codeLine = codeLine;
    }

    public override RuntimeException GenerateException(Exception ex)
    {
        return new RuntimeException($"Runtime exception in {codeLine.FunctionName} in line {codeLine.LineNumber}", ex);
    }
}