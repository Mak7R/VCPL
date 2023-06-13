using System;
using System.Collections.Generic;

using BasicFunctions;
using GlobalRealization;

namespace VCPL
{
    static class Program
    {
        static void Main(string[] args)
        {
            List<string> codeStrings;
            if (args.Length > 0) Menu.code = FileCodeEditor.ReadCode(args[0]);
            
            Menu.ReadOption();
        }
    }
}