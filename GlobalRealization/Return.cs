using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalRealization
{
    public class Return : Exception
    {
        public readonly Pointer[] Args;
        public Return(Pointer[] args) {
            if (args.Length > 1)
            {
                throw new RuntimeException("Incorrect arguments count");
            }
            this.Args = args;
        }
    }
}
