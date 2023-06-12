using System.Collections.Generic;
using GlobalRealization;

namespace VCPL;

public class FunctionsContainer : Dictionary<string, ElementaryFunction>
{
    public FunctionsContainer Context;
    public ElementaryFunction this[string key]
    {
        get
        {
            if (this.ContainsKey(key))
            {
                return base[key];
            }
            else if (this.Context != null)
            {
                return this.Context[key];
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        set
        {
            if (this.ContainsKey(key))
            {
                this[key] = value;
            }
            else if (this.Context != null)
            {
                this.Context[key] = value;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }

    public FunctionsContainer()
    {
        this.Context = null;
    }

    public FunctionsContainer(FunctionsContainer functionsContext)
    {
        this.Context = functionsContext;
    }

    public FunctionsContainer(Dictionary<string, ElementaryFunction> data) : base(data)
    {
        this.Context = null;
    }

    public FunctionsContainer(FunctionsContainer functionsContext, Dictionary<string, ElementaryFunction> data) : base(data)
    {
        this.Context = functionsContext;
    }
}
