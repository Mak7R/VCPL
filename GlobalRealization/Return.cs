using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization
{
    /// <summary>
    /// Realize a return call
    /// </summary>
    public class Return : Exception
    {
        private static IPointer? _returnedArg;

        public Return() { _returnedArg = null; }
        public Return(IPointer arg) { 
            _returnedArg = arg;
        }

        public static IPointer? Get() { return _returnedArg; }
    }
}
