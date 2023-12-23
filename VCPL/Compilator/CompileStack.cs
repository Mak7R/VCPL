using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPL.Compilator.Contexts;

namespace VCPL.Compilator;

public struct NewCon
{
    public List<ContextItem> ContextItems { get; set; }

    public NewCon()
    {
        ContextItems = new List<ContextItem>();
    }
}

public struct Consts
{
    public List<ContextItem> ContextItems { get; set; }
    public Consts()
    {
        ContextItems = new List<ContextItem>();
    }
}

public class CompileStack : Stack<(NewCon vars, Consts consts)>
{
    public void AddVar(string name) { this.Peek().vars.ContextItems.Add(name); }
    public void AddConst(string name) { }

    public void PeakPtr(string name) {  }
    public void PeakVal(string name) {  }

    public void Up() { this.Push((new NewCon(), new Consts())); }

    public (int size, object?[] consts) Down() { 
        var contextLevel = this.Pop();

        return (contextLevel.vars.ContextItems.Count, contextLevel.consts.ContextItems.ToArray());
    }
}
