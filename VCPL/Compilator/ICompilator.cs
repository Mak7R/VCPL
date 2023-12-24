using GlobalRealization;
using System.Collections.Generic;
using VCPL.CodeConvertion;

namespace VCPL.Compilator
{
    public interface ICompilator
    {
        public void ReloadAssemblyLoadContext();
        public Function Compilate(List<ICodeLine> codeLines, CompileStack context, string[] args);
    }
}
