using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization.Memory
{
    public interface IChangeable
    {
        public void Set(object? newValue);
    }
}
