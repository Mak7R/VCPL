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
        
        
        ("", new FunctionInstance((result, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Function '=' must to get one argument");
            result.Set(args[0].Get().Get());
        })),
        ("return", new FunctionInstance((result, args) =>
        {
            throw new Return(args);
        })),
        ("new", new FunctionInstance((result, args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        })),
        
        ("+", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
            result.Set(new Variable(BasicMath.Plus(args[0].Get().Get(), args[1].Get().Get())));
        }))),
        ("-", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(BasicMath.Minus(args[0].Get().Get(), args[1].Get().Get())));
        }))),
        ("*", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(BasicMath.Multiply(args[0].Get().Get(), args[1].Get().Get())));
        }))),
        ("/", new FunctionInstance(((result, args) =>
        {
           if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(BasicMath.Divide(args[0].Get().Get(), args[1].Get().Get())));
        }))),
        
        ("equal", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(args[0].Get().Get().Equals(args[1].Get().Get())));
        }))),
        ("disequal", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(!args[0].Get().Get().Equals(args[1].Get().Get())));
        }))),
        
        (">", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(((IComparable)args[0].Get().Get()).CompareTo(args[1].Get().Get()) == 1));
        }))),
        (">=", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(((IComparable)args[0].Get().Get()).CompareTo(args[1].Get().Get()) != -1));
        }))),
        
        ("<=", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(((IComparable)args[0].Get().Get()).CompareTo(args[1].Get().Get()) != 1));
        }))),
        ("<", new FunctionInstance(((result, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            result.Set(new Variable(((IComparable)args[0].Get().Get()).CompareTo(args[1].Get().Get()) == -1));
        }))),
        
        //("if", new FunctionInstance(((result, args) =>
        //{
        //    if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

        //    MemoryObject arg1 = context[args[0]];
        //    Function ifTrueF = context[args[1]] is Function func1 ? func1 : throw new RuntimeException("Argument 2 must be a Function");
        //    Function ifFalseF = context[args[2]] is Function func2 ? func2 : throw new RuntimeException("Argument 3 must be a Function");
            
        //    if (arg1.Get() is bool isTrue) 
        //        (isTrue ? ((FunctionInstance)ifTrueF.Get()) : ((FunctionInstance)ifFalseF.Get()))
        //            .Invoke(context, Pointer.NULL, Array.Empty<Pointer>());
        //}))),
        //("while", new FunctionInstance(((result, args) =>
        //{
        //    if (args.Length != 2) throw new RuntimeException("Incorect arguments count");
            
        //    MemoryObject arg1 = context[args[0]];
        //    Function arg2 = context[args[1]] is Function func ? func : throw new RuntimeException("Argument 2 must be a Function");
            
        //    while ((bool)context[args[0]].Get())
        //    {
        //        ((FunctionInstance)arg2.Get()).Invoke(context, Pointer.NULL, Array.Empty<Pointer>());
        //    }
        //}))),

        ("Sleep", new FunctionInstance((result, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Thread.Sleep((int)args[0].Get().Get());
        })),
    };
}