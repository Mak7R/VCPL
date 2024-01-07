using BasicFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPL.Compilator;

namespace VCPL.CodeConvertion;
public class CodeConvertorsContainer
{
    private readonly Dictionary<string, ICodeConvertor> codeConvertors = new Dictionary<string, ICodeConvertor>();

    public void AddCodeConvertor(string name, ICodeConvertor codeConvertor)
    {
        codeConvertors.Add(name, codeConvertor);
    }

    public bool RemoveCodeConvertor(string name)
    {
        return codeConvertors.Remove(name);
    }

    public ICodeConvertor GetCodeConvertor(string name)
    {
        return codeConvertors[name];
    }
}
