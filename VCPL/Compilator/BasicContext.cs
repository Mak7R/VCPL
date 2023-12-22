using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

using BasicFunctions;
using GlobalRealization;
using GlobalRealization.Memory;

namespace VCPL.Compilator;

public static class BasicContext
{
    private static readonly Context basicContext = new Context();

    static BasicContext()
    {
        basicContext.Push("temp", new Variable(null));

        basicContext.Push("", new Function((args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Function '=' must to get 2 arguments");
            args[1].Set(args[0].Get());
        }));

        basicContext.Push("return", new Function((args) =>
        {
            if (args.Length == 0) throw new Return();
            else if (args.Length == 1) throw new Return(args[0]);
            else throw new RuntimeException("Incorect arguments count");
        }));

        basicContext.Push("pass", new Function((args) => { }));

        basicContext.Push("new", new Function((args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        }));

        basicContext.Push("+", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Plus(arg1, arg2));
        }));

        basicContext.Push("-", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Minus(arg1, arg2));
        }));

        basicContext.Push("*", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Multiply(arg1, arg2));
        }));

        basicContext.Push("/", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Divide(arg1, arg2));
        }));

        basicContext.Push("equal", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            args[2].Set(arg1 == null ? arg2 == null : arg1.Equals(arg2));
        }));

        basicContext.Push("not", new Function((args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorect arguments count");
            bool arg = args[0].Get<bool>();
            args[0].Set(!arg);
        }));

        basicContext.Push(">", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == 1);
        }));

        basicContext.Push(">=", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != -1);
        }));

        basicContext.Push("<=", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != 1);
        }));

        basicContext.Push("<", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == -1);
        }));

        basicContext.Push("if", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            if (args[0].Get<bool>())
                args[1].Get<ElementaryFunction>().Invoke(Array.Empty<Pointer>());
            else args[2].Get<ElementaryFunction>().Invoke(Array.Empty<Pointer>());
        }));

        basicContext.Push("while", new Function((args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            while (args[0].Get<bool>()) args[1].Get<ElementaryFunction>().Invoke(Array.Empty<Pointer>());
        }));

        basicContext.Push("Sleep", new Function((args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Thread.Sleep(args[0].Get<int>());
        }));

        basicContext.Push("GetFromArray", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorrect args count");
            try
            {
                args[2].Set(args[0].Get<object?[]>()[args[1].Get<int>()]);
            }
            catch (IndexOutOfRangeException)
            {
                throw new RuntimeException("Index out of range");
            }
        }));

        basicContext.Push("SetToArray", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorrect args count");
            try
            {
                args[0].Get<object?[]>()[args[1].Get<int>()] = args[2].Get();
            }
            catch (IndexOutOfRangeException)
            {
                throw new RuntimeException("Index out of range");
            }
        }));

        basicContext.Push("CreateStopwatch", new Function((args) => {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = new Stopwatch();
            args[0].Set(stopwatch);
            
        }));

        basicContext.Push("StopwatchStart", new Function((args) => {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = args[0].Get<Stopwatch>();
            stopwatch.Start();
        }));

        basicContext.Push("GetDeltaTime", new Function((args) => {
            if (args.Length != 2) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = args[0].Get<Stopwatch>();
            stopwatch.Stop();
            args[1].Set(stopwatch.ElapsedMilliseconds);
        }));


        basicContext.Pack();
    }

    public static Context Get()
    {
        return basicContext;
    }
}