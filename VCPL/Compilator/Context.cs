using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using GlobalRealization;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace VCPL;

public class Context
{
    private TempContainer DataContext;
    private TempConstantContainer Constants;
    public FunctionsContainer FunctionsContext { get; private set; }

    private Context()
    {
        
    }
    
    public Context(TempContainer dataContext, TempConstantContainer constants, FunctionsContainer elementaryFunctions)
    {
        this.DataContext = dataContext;
        this.Constants = constants;
        this.FunctionsContext = elementaryFunctions;
    }

    public void PushFunction(string name, ElementaryFunction function)
    {
        this.FunctionsContext.Add(name, function);
        this.Constants.Push(name, function);
    }
    
    public Pointer PushConstant(string? name, object data)
    {
        int position = Constants.Push(name, data); 
        return new Pointer((byte)Contexts.Constant, position);
    }

    public Pointer PushData(string name, object data)
    {
        int position = DataContext.Push(name, data);
        return new Pointer((byte)Contexts.Variable, position);
    }

    public Pointer Peek(string name)
    {
        int pos = Constants.Peek(name);
        if (pos == -1)
        {
            pos = DataContext.Peek(name);
            return new Pointer((byte)Contexts.Variable, pos);
        }
        return new Pointer((byte)Contexts.Constant, pos);
    }

    public Context NewContext()
    {
        Context newContext = new Context();
        newContext.DataContext = new TempContainer(this.DataContext);
        newContext.Constants = new TempConstantContainer(this.Constants);
        newContext.FunctionsContext = new FunctionsContainer(this.FunctionsContext);
        return newContext;
    }

    public PackedContext Pack()
    {
        return new PackedContext() { constants = this.Constants.Pack(), data = this.DataContext.Pack() };
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
                if (retValueId != Pointer.NULL) container[retValueId] = null;
                
                foreach (var arg in argsIds) Console.Write(container[arg]?.ToString());

                return false;
            }
        },
        {
            "read", (container, retValueId, argsIds) =>
            {
                string value = Console.ReadLine();
                if (retValueId != Pointer.NULL) container[retValueId] = value;
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
    public static TempConstantContainer BasicConstants = new TempConstantContainer();

    static BasicContext()
    {
        BasicConstants.Push("NULL", null);
        BasicConstants.Push("true", true);
        BasicConstants.Push("false", false);

        foreach (var elementaryFunction in ElementaryFunctions)
        {
            BasicConstants.Push(elementaryFunction.Key, elementaryFunction.Value);
        }
    }

    public static Context GetBasicContext()
    {
        return new Context(BasicData, BasicConstants, ElementaryFunctions);
    }
}