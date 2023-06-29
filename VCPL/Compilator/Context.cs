using System;
using System.Collections.Generic;
using System.Threading;
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

public static class BasicContext
{
    public static FunctionsContainer ElementaryFunctions = new FunctionsContainer()
    {
        {
            "", (container, reference, args) =>
            {
                container[reference] = container[args[0]];
                return false;
            } 
        },
        {
            "Sleep", (container, reference, args) =>
            {
                if (args.Length != 1) throw new RuntimeException("Incorrect args count");
                Thread.Sleep((int)container[args[0]]);
                return false;
            } 
        },
        {"return", (container, reference, args) =>
        {
            if (args.Length > 1) throw new CompilationException("Args count is more than posible");
            return true;
        } },
        {
            "print", (container, retValueId, argsIds) =>
            {
                if (retValueId != -1) container[retValueId] = null;
                
                foreach (int arg in argsIds) Console.Write(container[arg]?.ToString());

                return false;
            }
        },
        {
            "read", (container, retValueId, argsIds) =>
            {
                string value = Console.ReadLine();
                if (retValueId != -1) container[retValueId] = value;
                return false;
            }
        },
        { "endl", (container, retValueId, argsIds) => { 
            Console.WriteLine();
            return false;
        } },
        {
            "new", (container, retValueId, argsIds) =>
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
        }
    };

    public static TempContainer BasicData = new TempContainer();

    static BasicContext()
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