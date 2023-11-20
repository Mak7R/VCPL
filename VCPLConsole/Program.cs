using System;
using System.Collections.Generic;
using FileController;
using VCPL;

namespace VCPLConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) Menu.code = FileCodeEditor.ReadCode(args[0]);
            
            Menu.ReadOption();
        }
    }
}