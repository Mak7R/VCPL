using System;
using System.Diagnostics;
using System.Threading;

using BasicFunctions;
using GlobalRealization;
using GlobalRealization.Memory;

namespace VCPL.Compilator.Contexts;

public static class BasicContext
{
    private static readonly GlobalContext basicContext = new GlobalContext(null);

    static BasicContext()
    {
        basicContext.Push(new ContextItem("temp", null, Modificator.Variable));

        basicContext.Push(new ContextItem("", new Function((args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Function '=' must to get 2 arguments");
            args[1].Set(args[0].Get());
        }), Modificator.Function));

        basicContext.Push(new ContextItem("return", new Function((args) =>
        {
            if (args.Length == 0) throw new Return();
            else if (args.Length == 1) throw new Return(args[0]);
            else throw new RuntimeException("Incorect arguments count");
        }), Modificator.Function));

        basicContext.Push(new ContextItem("pass", new Function((args) => { }), Modificator.Function));

        basicContext.Push(new ContextItem("new", new Function((args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        }), Modificator.Function));

        basicContext.Push(new ContextItem("+", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Plus(arg1, arg2));
        }), Modificator.Function));

        basicContext.Push(new ContextItem("-", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Minus(arg1, arg2));
        }), Modificator.Function));

        basicContext.Push(new ContextItem("*", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Multiply(arg1, arg2));
        }), Modificator.Function));

        basicContext.Push(new ContextItem("/", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Divide(arg1, arg2));
        }), Modificator.Function));

        basicContext.Push(new ContextItem("equal", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            args[2].Set(arg1 == null ? arg2 == null : arg1.Equals(arg2));
        }), Modificator.Function));

        basicContext.Push(new ContextItem("not", new Function((args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorect arguments count");
            bool arg = args[0].Get<bool>();
            args[0].Set(!arg);
        }), Modificator.Function));

        basicContext.Push(new ContextItem(">", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == 1);
        }), Modificator.Function));

        basicContext.Push(new ContextItem(">=", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != -1);
        }), Modificator.Function));

        basicContext.Push(new ContextItem("<=", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) != 1);
        }), Modificator.Function));

        basicContext.Push(new ContextItem("<", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            args[2].Set(args[0].Get<IComparable>().CompareTo(args[1].Get()) == -1);
        }), Modificator.Function));

        basicContext.Push(new ContextItem("if", new Function((args) =>
        {
            if (args.Length != 3) throw new RuntimeException("Incorect arguments count");

            if (args[0].Get<bool>())
                args[1].Get<ElementaryFunction>().Invoke(Array.Empty<Pointer>());
            else args[2].Get<ElementaryFunction>().Invoke(Array.Empty<Pointer>());
        }), Modificator.Function));

        basicContext.Push(new ContextItem("while", new Function((args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorect arguments count");

            while (args[0].Get<bool>()) args[1].Get<ElementaryFunction>().Invoke(Array.Empty<Pointer>());
        }), Modificator.Function));

        basicContext.Push(new ContextItem("Sleep", new Function((args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Thread.Sleep(args[0].Get<int>());
        }), Modificator.Function));

        basicContext.Push(new ContextItem("GetFromArray", new Function((args) =>
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
        }), Modificator.Function));

        basicContext.Push(new ContextItem("SetToArray", new Function((args) =>
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
        }), Modificator.Function));

        basicContext.Push(new ContextItem("CreateStopwatch", new Function((args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = new Stopwatch();
            args[0].Set(stopwatch);

        }), Modificator.Function));

        basicContext.Push(new ContextItem("StopwatchStart", new Function((args) =>
        {
            if (args.Length != 1) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = args[0].Get<Stopwatch>();
            stopwatch.Start();
        }), Modificator.Function));

        basicContext.Push(new ContextItem("GetDeltaTime", new Function((args) =>
        {
            if (args.Length != 2) throw new RuntimeException("Incorrect args count");
            Stopwatch stopwatch = args[0].Get<Stopwatch>();
            stopwatch.Stop();
            args[1].Set(stopwatch.ElapsedMilliseconds);
        }), Modificator.Function));

        basicContext.Pack();
    }

    public static AbstractContext Get()
    {
        return basicContext;
    }
}