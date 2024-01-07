using GlobalRealization;
using System.Collections.Generic;

namespace TestIncludesLib
{
    public static class Library
    {
        private const double PI = 3.14;
        public static List<(string?, object?)> Items = new()
        {
            ("PI", PI),
            ("GetCircleLength", (ElementaryFunction)((args) =>
            {
                if (args.Length != 2) throw new RuntimeException("Incorrect arguments count");
                int radius = args[0].Get<int>();
                args[1].Set(2 * PI * radius);
            }))
        };
    }
}
