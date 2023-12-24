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

public static class BasicContext
{
    private static readonly CompileStack basicContext = new CompileStack();

    static BasicContext()
    {
        basicContext.Up();
        basicContext.AddVar("temp");

        basicContext.AddConst("", new Function((stack, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Function '=' must to get 2 arguments");
            stack[args[1]] = stack[args[0]];
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
            var arg1 = stack[args[0]];
            var arg2 = stack[args[1]];
            if (arg1 != null && arg2 != null) stack[args[2]] = BasicMath.Plus(arg1, arg2);
        }));

        basicContext.AddConst("-", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = stack[args[0]];
            var arg2 = stack[args[1]];
            if (arg1 != null && arg2 != null) stack[args[2]] = BasicMath.Minus(arg1, arg2);
        }));

        basicContext.AddConst("*", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = stack[args[0]];
            var arg2 = stack[args[1]];
            if (arg1 != null && arg2 != null) stack[args[2]] = BasicMath.Multiply(arg1, arg2);
        }));

        basicContext.AddConst("/", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = stack[args[0]];
            var arg2 = stack[args[1]];
            if (arg1 != null && arg2 != null) stack[args[2]] = BasicMath.Divide(arg1, arg2);
        }));

        basicContext.AddConst("equal", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = stack[args[0]];
            var arg2 = stack[args[1]];
            stack[args[2]] = arg1 == null ? arg2 == null : arg1.Equals(arg2);
        }));

        basicContext.AddConst("not", new Function((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorect arguments count");
            bool arg = stack.Get<bool>(args[0]);
            stack[args[0]] = !arg;
        }));

        basicContext.AddConst(">", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            stack[args[2]] = stack.Get<IComparable>(args[0]).CompareTo(stack[args[1]]) == 1;
        }));

        basicContext.AddConst(">=", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            stack[args[2]] = stack.Get<IComparable>(args[0]).CompareTo(stack[args[1]]) != -1;
        }));

        basicContext.AddConst("<=", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            stack[args[2]] = stack.Get<IComparable>(args[0]).CompareTo(stack[args[1]]) != 1;
        }));

        basicContext.AddConst("<", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            stack[args[2]] = stack.Get<IComparable>(args[0]).CompareTo(stack[args[1]]) == -1;
        }));

        basicContext.AddConst("if", new Function((stack, args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            if (stack.Get<bool>(args[0]))
                stack.Get<Function>(args[1]).Get().Invoke(stack, Array.Empty<Pointer>());
            else stack.Get<Function>(args[2]).Get().Invoke(stack, Array.Empty<Pointer>());
        }));

        basicContext.AddConst("while", new Function((stack, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            while (stack.Get<bool>(args[0])) stack.Get<Function>(args[1]).Get().Invoke(stack, Array.Empty<Pointer>());
        }));

        basicContext.AddConst("Sleep", new Function((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Thread.Sleep(stack.Get<int>(args[0]));
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
            stack[args[0]] = stopwatch;

        }));

        basicContext.AddConst("StopwatchStart", new Function((stack, args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = stack.Get<Stopwatch>(args[0]);
            stopwatch.Start();
        }));

        basicContext.AddConst("GetDeltaTime", new Function((stack, args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = stack.Get<Stopwatch>(args[0]);
            stopwatch.Stop();
            stack[args[1]] = stopwatch.ElapsedMilliseconds;
        }));

        basicContext.Up();
    }

    public static CompileStack Get()
    {
        return basicContext;
    }
}