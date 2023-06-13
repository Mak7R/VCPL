using System;
using System.Collections.Generic;
using GlobalRealization;
using Microsoft.VisualBasic.CompilerServices;

namespace VCPL;

public class Context
{
    public TempContainer dataContext;
    public FunctionsContainer functionsContext;

    public Context(TempContainer dataContext, FunctionsContainer functionsContext)
    {
        this.dataContext = dataContext;
        this.functionsContext = functionsContext;
    }

    public Context NewContext()
    {
        return new Context(new TempContainer(this.dataContext), new FunctionsContainer(this.functionsContext));
    }
}

public static class BasicConteext
{
    public static FunctionsContainer ElementaryFunctions = new FunctionsContainer()
    {
        {"return", (DataContainer container, int reference, int[] args) =>
        {
            if (args.Length > 1) throw new CompilationException("Args count is more than posible");
            return true;
        } },
        {
            "print", (DataContainer container, int retValueId, int[] argsIds) =>
            {
                if (retValueId != -1) container[retValueId] = null;
                
                foreach (int arg in argsIds) Console.Write(container[arg]?.ToString());

                return false;
            }
        },
        {
            "read", (DataContainer container, int retValueId, int[] argsIds) =>
            {
                string value = Console.ReadLine();
                if (retValueId != -1) container[retValueId] = value;
                return false;
            }
        },
        { "endl", (DataContainer container, int retValueId, int[] argsIds) => { 
            Console.WriteLine();
            return false;
        } },
        {
            "new", (DataContainer container, int retValueId, int[] argsIds) =>
            {
                if (argsIds.Length == 0)
                {
                    container[retValueId] = new object(); // here should be alhoritm when types will be in VCPL
                }
                else
                {
                    throw new RuntimeException("new have to get only 0 or 1 args"); // 
                }

                return false;
            }
        },
        {
            "sumInt", (DataContainer container, int ret, int[] args) =>
            {
                if (ret == -1) return false;
                if (args.Length == 0)
                {
                    container[ret] = 0;
                    return false;
                }

                int res = 0;
                foreach (int arg in args)
                {
                    res += (int)container[arg];
                }

                container[ret] = res;
                return false;
            }
        }
    };

    public static TempContainer BasicData = new TempContainer();

    static BasicConteext()
    {
        BasicData.Push("NULL", null);
        BasicData.Push("true", true);
        BasicData.Push("false", false);
    }

    public static Context GetBasicContext()
    {
        return new Context(BasicData, ElementaryFunctions);
    }
}