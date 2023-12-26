using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

using BasicFunctions;
using GlobalRealization;

namespace VCPL.Compilator;

public static class BasicStack
{
    public static CompileStack Get()
    {
        CompileStack basicContext = new CompileStack();
        basicContext.Up();
        basicContext.AddVar("temp");

        basicContext.AddConst("", new Function((stack, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Function '=' must to get 2 arguments");
            args[1].Set(args[0].Get());
        }));

        basicContext.AddConst("return", new Function((stack, args) =>
        {
            if (args.Length == 0) throw new Return();
            else if (args.Length == 1) throw new Return(args[0]);
            else throw new RuntimeException("Incorect arguments count");
        }));

        basicContext.AddConst("pass", new Function((stack, args) => { }));

        basicContext.AddConst("new", new Function((stack, args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        }));

        basicContext.AddConst("+", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Plus(arg1, arg2));
        }));

        basicContext.AddConst("-", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Minus(arg1, arg2));
        }));

        basicContext.AddConst("*", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Multiply(arg1, arg2));
        }));

        basicContext.AddConst("/", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Divide(arg1, arg2));
        }));

        basicContext.AddConst("equal", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            args[2].Set(arg1 == null ? arg2 == null : arg1.Equals(arg2));
        }));

        basicContext.AddConst("not", new Function((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorect arguments count");
            args[0].Set(!args[0].Get<bool>());
        }));

        basicContext.AddConst(">", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == 1);
        }));

        basicContext.AddConst(">=", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != -1);
        }));

        basicContext.AddConst("<=", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != 1);
        }));

        basicContext.AddConst("<", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == -1);
        }));

        basicContext.AddConst("if", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            if (args[0].Get<bool>())
                args[1].Get<Function>().Get().Invoke(stack, Array.Empty<IPointer>());
            else args[2].Get<Function>().Get().Invoke(stack, Array.Empty<IPointer>());
        }));

        basicContext.AddConst("while", new Function((stack, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            while (args[0].Get<bool>()) args[1].Get<Function>().Get().Invoke(stack, Array.Empty<IPointer>());
        }));

        basicContext.AddConst("Sleep", new Function((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Thread.Sleep(args[0].Get<int>());
        }));

        //basicContext.AddConst("GetFromArray", new Function((stack, args) =>
        //{
        //    if (args.Length != 3) throw new RuntimeException("Incorrect args count");
        //    try
        //    {
        //        args[2].Set(args[0].Get<object?[]>()[args[1].Get<int>()]);
        //    }
        //    catch (IndexOutOfRangeException)
        //    {
        //        throw new RuntimeException("Index out of range");
        //    }
        //}));

        //basicContext.AddConst("SetToArray", new Function((stack, args) =>
        //{
        //    if (args.Length != 3) throw new RuntimeException("Incorrect args count");
        //    try
        //    {
        //        args[0].Get<object?[]>()[args[1].Get<int>()] = args[2].Get();
        //    }
        //    catch (IndexOutOfRangeException)
        //    {
        //        throw new RuntimeException("Index out of range");
        //    }
        //}));

        basicContext.AddConst("CreateStopwatch", new Function((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = new Stopwatch();
            args[0].Set(stopwatch);
        }));

        basicContext.AddConst("StopwatchStart", new Function((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = args[0].Get<Stopwatch>();
            stopwatch.Start();
        }));

        basicContext.AddConst("GetDeltaTime", new Function((stack, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = args[0].Get<Stopwatch>();
            stopwatch.Stop();
            args[1].Set(stopwatch.ElapsedMilliseconds);
        }));

        basicContext.Up();
        return basicContext;
    }
}