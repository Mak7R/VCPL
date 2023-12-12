using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

using BasicFunctions;
using GlobalRealization;

namespace VCPL.Compilator;

public static class BasicContext
{
    public readonly static List<(string name, MemoryObject value)> BasicContextList = new List<(string name, MemoryObject value)>()
    {
        ("null", new Constant(null)),
        ("true", new Constant(true)),
        ("false", new Constant(false)),
        ("temp", new Variable(null)),
        
        
        ("", new FunctionInstance((context, result, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Function '=' must to get one argument");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            
            if (res is IChangeable change) change.Set(arg1.Get());
            else throw new RuntimeException("Cannot to change constant");
        })),
        ("return", new FunctionInstance((context, result, args) =>
        {
            throw new Return(args);
        })),
        ("new", new FunctionInstance((context, result, args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        })),
        
        ("+", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(BasicMath.Plus(arg1.Get(), arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        ("-", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(BasicMath.Minus(arg1.Get(), arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        ("*", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(BasicMath.Multiply(arg1.Get(), arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        ("/", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(BasicMath.Divide(arg1.Get(), arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        
        ("equal", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(arg1.Get().Equals(arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        ("disequal", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(!arg1.Get().Equals(arg2.Get()));
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        
        (">", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(((IComparable)arg1.Get()).CompareTo(arg2.Get()) == 1);
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        (">=", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(((IComparable)arg1.Get()).CompareTo(arg2.Get()) != -1);
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        
        ("<=", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(((IComparable)arg1.Get()).CompareTo(arg2.Get()) != 1);
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        ("<", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject res = context[result];
            MemoryObject arg1 = context[args[0]];
            MemoryObject arg2 = context[args[1]];
            
            if (res is IChangeable changeable) changeable.Set(((IComparable)arg1.Get()).CompareTo(arg2.Get()) == -1);
            else throw new RuntimeException("Cannot to change constsnt");
        }))),
        
        ("if", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            MemoryObject arg1 = context[args[0]];
            Function ifTrueF = context[args[1]] is Function func1 ? func1 : throw new RuntimeException("Argument 2 must be a Function");
            Function ifFalseF = context[args[2]] is Function func2 ? func2 : throw new RuntimeException("Argument 3 must be a Function");
            
            if (arg1.Get() is bool isTrue) 
                (isTrue ? ((FunctionInstance)ifTrueF.Get()) : ((FunctionInstance)ifFalseF.Get()))
                    .Invoke(context, Pointer.NULL, Array.Empty<Pointer>());
        }))),
        ("while", new FunctionInstance(((context, result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            MemoryObject arg1 = context[args[0]];
            Function arg2 = context[args[1]] is Function func ? func : throw new RuntimeException("Argument 2 must be a Function");
            
            while ((bool)context[args[0]].Get())
            {
                ((FunctionInstance)arg2.Get()).Invoke(context, Pointer.NULL, Array.Empty<Pointer>());
            }
        }))),

        ("Sleep", new FunctionInstance((context, result, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Thread.Sleep((int)context[args[0]].Get());
        })),
    };
}