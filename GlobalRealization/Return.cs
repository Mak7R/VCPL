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
        private static Pointer? _returnedArg;

        public Return() { _returnedArg = null; }
        public Return(Pointer arg) { 
            _returnedArg = arg;
        }

        public static Pointer? Get() { return _returnedArg; }
    }
}
