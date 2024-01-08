using System;

namespace BasicFunctions;

public static class BasicMath
{
    public static int Min(int a, int b)
    {
        return a < b ? a : b;
    } 
    public static int Max(int a, int b)
    {
        return a > b ? a : b;
    }

    public static object Plus(object a, object b)
    {
        if (a is int inta && b is int intb) return inta + intb;
        if (a is double doublea && b is double doubleb) return doublea + doubleb;
        
        if (a is int intda && b is double doubleib) return intda + doubleib;
        if (a is double doubleia && b is int intdb) return doubleia + intdb;

        if (a is string stra) return stra + Convert.ToString(b);
        if (b is string strb) return strb + Convert.ToString(a);

        throw new ArgumentException("Unavalible args");
    }
    public static object Minus(object a, object b)
    {
        if (a is int inta && b is int intb) return inta - intb;
        if (a is double doublea && b is double doubleb) return doublea - doubleb;

        if (a is int intda && b is double doubleib) return intda - doubleib;
        if (a is double doubleia && b is int intdb) return doubleia - intdb;

        throw new ArgumentException("Unavalible args");
    }
    public static object Multiply(object a, object b)
    {
        if (a is int inta && b is int intb) return inta * intb;
        if (a is double doublea && b is double doubleb) return doublea * doubleb;

        if (a is int intda && b is double doubleib) return intda * doubleib;
        if (a is double doubleia && b is int intdb) return doubleia * intdb;

        throw new ArgumentException("Unavalible args");
    }
    public static object Divide(object a, object b)
    {
        if (a is int inta && b is int intb) return inta / intb;
        if (a is double doublea && b is double doubleb) return doublea / doubleb;

        if (a is int intda && b is double doubleib) return intda / doubleib;
        if (a is double doubleia && b is int intdb) return doubleia / intdb;

        throw new ArgumentException("Unavalible args");
    }
}