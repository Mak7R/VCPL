using GlobalRealization;
using System.Collections.Generic;
using VCPL.CodeConvertion;

namespace VCPL.Compilator
{
    public interface ICompilator
    {
        public void ImportAll(List<CodeLine> codeLines, List<string>? importedLibs);
        public void IncludeAll(List<CodeLine> codeLines, CompileStack stack);
        public Function Compilate(List<CodeLine> codeLines, CompileStack context, string[] args);
    }
}
