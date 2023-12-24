using GlobalRealization;
using System.Collections.Generic;
using VCPL.CodeConvertion;

namespace VCPL.Compilator
{
    public interface ICompilator
    {
        public void CompilateAllIncludes(List<ICodeLine> codeLines, CompileStack stack);
        public Function Compilate(List<ICodeLine> codeLines, CompileStack context, string[] args);
    }
}
