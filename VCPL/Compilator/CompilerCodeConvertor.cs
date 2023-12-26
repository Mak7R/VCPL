using BasicFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPL.CodeConvertion;

namespace VCPL.Compilator
{
    public static class CompilerCodeConvertor
    {
        private static Dictionary<string, ICodeConvertor> codeConvertors = new Dictionary<string, ICodeConvertor>();

        public static void AddCodeConvertor(string name, ICodeConvertor codeConvertor)
        {
            codeConvertors.Add(name, codeConvertor);
        }

        public static bool RemoveCodeConvertor(string name)
        {
            return codeConvertors.Remove(name);
        }

        public delegate string[] ToStringCodeLines(string code);
        public static ToStringCodeLines SplitCode = (string code) => { throw new CompilationException("Split code was not implemented"); };

        public static List<CodeLine> Convert(string code, string convertorName)
        {
            ICodeConvertor codeConvertor = codeConvertors[convertorName];

            string[] stringCodeLines = SplitCode(code);

            List<CodeLine> codeLines = new List<CodeLine>();
            foreach(string codeLine in stringCodeLines)
            {
                if (BasicString.IsNoDataString(codeLine)) continue;
                codeLines.Add(codeConvertor.Convert(codeLine));
            }
            return codeLines;
        }
    }
}
