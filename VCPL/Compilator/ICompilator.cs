using GlobalRealization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPL.CodeConvertion;

namespace VCPL
{
    public interface ICompilator
    {
        public Function Compilate(List<ICodeLine> codeLines, Context context, List<string> args);
    }
}
