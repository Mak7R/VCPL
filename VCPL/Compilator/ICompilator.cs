using GlobalRealization.Memory;
using System.Collections.Generic;
using VCPL.CodeConvertion;
using VCPL.Compilator.Contexts;

namespace VCPL.Compilator
{
    public interface ICompilator
    {
        public void ReloadAssemblyLoadContext();
        public Function Compilate(List<ICodeLine> codeLines, AbstractContext context, List<string>? args = null);
    }
}
