using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

using BasicFunctions;
using GlobalInterface;
using VCPL.GlobalInterfaceRealization;

namespace VCPL.Stacks;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

public static class BasicStack
{
    public static CompileStack Get()
    {
        CompileStack basicContext = new CompileStack();
        basicContext.Up();
        basicContext.AddVar("temp");

        basicContext.AddConst("", (ElementaryFunction)((args) =>
        {
            args[1].Set(args[0].Get());
        }));

        basicContext.AddConst("return", (ElementaryFunction)((args) =>
        {
            if (args.Length == 0) throw new Return();
            else throw new Return(args[0]);
        }));

        basicContext.AddConst("pass", (ElementaryFunction)((args) => { }));

        basicContext.AddConst("new", (ElementaryFunction)((args) =>
        {
            throw new NotImplementedException("'new' has not implemented");
        }));

        basicContext.AddConst("+", (ElementaryFunction)((args) =>
        {
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();

            args[2].Set(BasicMath.Plus(arg1, arg2));
        }));

        basicContext.AddConst("-", (ElementaryFunction)((args) =>
        {
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Minus(arg1, arg2));
        }));

        basicContext.AddConst("*", (ElementaryFunction)((args) =>
        {
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Multiply(arg1, arg2));
        }));

        basicContext.AddConst("/", (ElementaryFunction)((args) =>
        {
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            if (arg1 != null && arg2 != null) args[2].Set(BasicMath.Divide(arg1, arg2));
        }));

        basicContext.AddConst("equal", (ElementaryFunction)((args) =>
        {
            var arg1 = args[0].Get();
            var arg2 = args[1].Get();
            args[2].Set(arg1 == null ? arg2 == null : arg1.Equals(arg2));
        }));

        basicContext.AddConst("not", (ElementaryFunction)((args) =>
        {
            args[0].Set(!(bool)args[0].Get());
        }));

        basicContext.AddConst(">", (ElementaryFunction)((args) =>
        {
            args[2].Set(((IComparable)args[0].Get()).CompareTo(args[1].Get()) == 1);
        }));

        basicContext.AddConst(">=", (ElementaryFunction)((args) =>
        {
            args[2].Set(((IComparable)args[0].Get()).CompareTo(args[1].Get()) != -1);
        }));

        basicContext.AddConst("<=", (ElementaryFunction)((args) =>
        {
            args[2].Set(((IComparable)args[0].Get()).CompareTo(args[1].Get()) != 1);
        }));

        basicContext.AddConst("<", (ElementaryFunction)((args) =>
        {
            args[2].Set(((IComparable)args[0].Get()).CompareTo(args[1].Get()) == -1);
        }));

        basicContext.AddConst("if", (ElementaryFunction)((args) =>
        {
            if ((bool)args[0].Get())
                ((ElementaryFunction)args[1].Get()).Invoke(Array.Empty<IPointer>());
            else ((ElementaryFunction)args[2].Get()).Invoke(Array.Empty<IPointer>());
        }));

        basicContext.AddConst("while", (ElementaryFunction)((args) =>
        {
            var f = (ElementaryFunction)args[1].Get();
            while ((bool)args[0].Get()) f.Invoke(Array.Empty<IPointer>());
        }));

        basicContext.AddConst("Sleep", (ElementaryFunction)((args) =>
        {
            var val = (int)args[0].Get();
            Thread.Sleep(val);
        }));

        basicContext.AddConst("Array", (ElementaryFunction)((args) =>
        {
            int size = (int)args[0].Get();
            args[1].Set(new object?[size]);
        }));

        basicContext.AddConst("Array.Get", (ElementaryFunction)((args) =>
        {
            object?[] array = (object?[])args[0].Get();
            int pos = (int)args[1].Get();
            args[2].Set(array[pos]);
        }));

        basicContext.AddConst("Array.Set", (ElementaryFunction)((args) =>
        {
            object?[] array = (object?[])args[0].Get();
            int pos = (int)args[1].Get();

            array[pos] = args[2].Get();

        }));

        basicContext.AddConst("CreateStopwatch", (ElementaryFunction)((args) =>
        {
            args[0].Set(new Stopwatch());
        }));

        basicContext.AddConst("StopwatchStart", (ElementaryFunction)((args) =>
        {
            Stopwatch stopwatch = (Stopwatch)args[0].Get();
            stopwatch.Start();
        }));

        basicContext.AddConst("GetDeltaTime", (ElementaryFunction)((args) =>
        {
            Stopwatch stopwatch = (Stopwatch)args[0].Get();
            stopwatch.Stop();
            args[1].Set(stopwatch.ElapsedMilliseconds);
        }));

        basicContext.AddConst("Invoke", (ElementaryFunction)((args) =>
        {
            ElementaryFunction function = (ElementaryFunction)args[0].Get();
            function.Invoke(args.Skip(1).ToArray());
        }));

        basicContext.AddConst("Randint", (ElementaryFunction)((args) =>
        {
            args[2].Set(Random.Shared.Next((int)args[0].Get(), (int)args[1].Get()));
        }));

        basicContext.Up();
        return basicContext;
    }
}

#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.