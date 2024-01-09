using GlobalInterface;
using System.Collections.Generic;
using VCPL.CodeConvertion;
using VCPL.Stacks;

namespace VCPL.Compilator
{
    public interface ICompilator
    {
        public ElementaryFunction CompilateMain(CompileStack stack, string code, string convertorName, string name = "Main");
    }
}
