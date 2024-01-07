using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

using BasicFunctions;
using GlobalRealization;
using VCPL.Compilator.GlobalInterfaceRealization;

namespace VCPL.Compilator;

public static class BasicStack
{
    public static CompileStack Get()
    {
        CompileStack basicContext = new CompileStack();
        basicContext.Up();
        basicContext.AddVar("temp");

        basicContext.AddConst("", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Function '=' must to get 2 arguments");
            args[1].Set(args[0].Get());
        }));

        basicContext.AddConst("return", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length == 0) throw new Return();
            else if (args.Length == 1) throw new Return(args[0]);
            else throw new RuntimeException("Incorect arguments count");
        }));

        basicContext.AddConst("pass", (ElementaryFunction)((stack, args) => { }));

        basicContext.AddConst("new", (ElementaryFunction)((stack, args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        }));

        basicContext.AddConst("+", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Plus(arg1, arg2));
        }));

        basicContext.AddConst("-", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Minus(arg1, arg2));
        }));

        basicContext.AddConst("*", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Multiply(arg1, arg2));
        }));

        basicContext.AddConst("/", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Divide(arg1, arg2));
        }));

        basicContext.AddConst("equal", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            args[2].Set(arg1 == null ? arg2 == null : arg1.Equals(arg2));
        }));

        basicContext.AddConst("not", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorect arguments count");
            args[0].Set(!args[0].Get<bool>());
        }));

        basicContext.AddConst(">", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == 1);
        }));

        basicContext.AddConst(">=", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != -1);
        }));

        basicContext.AddConst("<=", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != 1);
        }));

        basicContext.AddConst("<", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == -1);
        }));

        basicContext.AddConst("if", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            if (args[0].Get<bool>())
                args[1].Get<ElementaryFunction>().Invoke(stack, Array.Empty<IPointer>());
            else args[2].Get<ElementaryFunction>().Invoke(stack, Array.Empty<IPointer>());
        }));

        basicContext.AddConst("while", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            while (args[0].Get<bool>()) args[1].Get<ElementaryFunction>().Invoke(stack, Array.Empty<IPointer>());
        }));

        basicContext.AddConst("Sleep", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            var val = args[0].Get<int>();
            if (val < 0) throw new RuntimeException("Argument 1 must be a non-negative value");
            Thread.Sleep(val);
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

        basicContext.AddConst("CreateStopwatch", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = new Stopwatch();
            args[0].Set(stopwatch);
        }));

        basicContext.AddConst("StopwatchStart", (ElementaryFunction)((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = args[0].Get<Stopwatch>();
            stopwatch.Start();
        }));

        basicContext.AddConst("GetDeltaTime", (ElementaryFunction)((stack, args) =>
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