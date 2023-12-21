using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

using BasicFunctions;
using GlobalRealization;
using GlobalRealization.Memory;

namespace VCPL.Compilator;

public static class BasicContext
{
    public readonly static List<(string? name, MemoryObject value)> BasicContextList = new List<(string? name, MemoryObject value)>()
    {
        ("temp", new Variable(null)),
        
        ("", new FunctionInstance((args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Function '=' must to get 2 arguments");
            args[1].Set(args[0].Get());
        })),
        ("return", new FunctionInstance((args) =>
        {
            throw new Return();
        })),
        ("new", new FunctionInstance((args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        })),
        
        ("+", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Plus(arg1, arg2));
        }))),
        ("-", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Minus(arg1, arg2));
        }))),
        ("*", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Multiply(arg1, arg2));
        }))),
        ("/", new FunctionInstance(((args) =>
        {
           if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Divide(arg1, arg2));
        }))),
        
        ("equal", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            args[2].Set(arg1 == null ? arg2 == null : arg1.Equals(arg2));
        }))),
        ("not", new FunctionInstance(((args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorect arguments count");
            bool arg = args[0].Get<bool>();
            args[0].Set(!arg);
        }))),
        
        (">", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == 1);
        }))),
        (">=", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != -1);
        }))),
        
        ("<=", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != 1);
        }))),
        ("<", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == -1);
        }))),

        ("if", new FunctionInstance(((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            if (args[0].Get<bool>())
                args[1].Get<FunctionInstance>().Invoke(Array.Empty<Pointer>());
            else args[2].Get<FunctionInstance>().Invoke(Array.Empty<Pointer>());
        }))),
        ("while", new FunctionInstance(((args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            while (args[0].Get<bool>()) args[1].Get<FunctionInstance>().Invoke(Array.Empty<Pointer>());
        }))),

        ("Sleep", new FunctionInstance((args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Thread.Sleep(args[0].Get<int>());
        })),

        ("GetFromArray", new FunctionInstance((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorrect args count");
            try
            {
                args[2].Set(args[0].Get<object?[]>()[args[1].Get<int>()]);
            }
            catch(IndexOutOfRangeException) {
                throw new RuntimeException("Index out of range");
            }
        })),

        ("SetToArray", new FunctionInstance((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorrect args count");
            try
            {
                args[0].Get<object?[]>()[args[1].Get<int>()] = args[2].Get();
            }
            catch(IndexOutOfRangeException)
            {
                throw new RuntimeException("Index out of range");
            }
        })),
    };
}