using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPL.CodeConvertion
{
    public interface ICodeLine
    {
        public string FunctionName { get; init; }

        public List<string> Args { get; init; }
    }
}
