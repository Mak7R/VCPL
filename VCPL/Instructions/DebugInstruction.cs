using GlobalRealization;
using VCPL.CodeConvertion;

namespace VCPL.Instructions;
public class DebugInstruction : Instruction
{
    public readonly CodeLine codeLine;
    public DebugInstruction(CodeLine codeLine, ElementaryFunction function, IPointer[] args) : base(function, args)
    {
        this.codeLine = codeLine;
    }

    public override string ToString()
    {
        return $"{codeLine.FunctionName} in line {codeLine.LineNumber}";
    }
}