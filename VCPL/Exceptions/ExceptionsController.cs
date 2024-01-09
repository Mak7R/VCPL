using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPL.Exceptions;

public static class ExceptionsController
{
    public static string IncorrectArgumentsCount(int[] correct, int get)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (int i in correct) stringBuilder.Append(i).Append(", ");

        return $"Incorrect arguments count: expected {stringBuilder} arguments but recieved {get}";
    }

    public static string Unknown(string obj, string funtctionName)
    {
        return $"Unknovn {obj}: {funtctionName}";
    }

    public static string KeywordCannotBeVariable(string keyword)
    {
        return $"Keyword {keyword} cannot be a variable";
    }

    public static string LiteralInited()
    {
        return "Literal cannot be initialized";
    }

    public static string ConstantInitedByNotLiteral() {
        return "Constant can only be initialized with a literal";
    }

    public static string VariableAlreadyExist(string name) {
        return $"Variable {name} already exist";
    }

    public static string VariableDoesNotExist(string name)
    {
        return $"Variable {name} doesn't exist";
    }

    public static string CannotLoadLib(string libName, string exMessage)
    {
        return $"Cannot load library {libName}. Load exception: {exMessage}";
    }

    public static string InAssemblyNotFound(string what)
    {
        return $"In assembly was not found {what}";
    }

    public static string CannotConvert(Type? from, Type? to)
    {
        return $"Cannot convert from {from?.ToString() ?? "null"} to {to?.ToString() ?? "null"}";
    }

    public static string CannotChangeConstant()
    {
        return $"Cannot change constant";
    }
}