using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPL.CodeConvertion
{
    public interface ICodeConvertor
    {
        public ICodeLine Convert(string line);
    }
}
