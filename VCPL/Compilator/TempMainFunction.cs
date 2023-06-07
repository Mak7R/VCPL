using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using GlobalRealization;
using CustomLibraries;
using BasicFunctions;

namespace VCPL;

public class TempMainFunction
{
    public DataContainer Container;
    public Dictionary<string, ElementaryFunction> ElementaryFunctions;
    public (ElementaryFunction? method, int retDataId, int[] argsIds)[] Program;
    private static int counter = 0;

    private void AddCommand((ElementaryFunction? method, int retDataId, int[] argsIds) obj)
    {
        this.Program[counter++] = obj;
    }

    private void PackProgram()
    {
        (ElementaryFunction? method, int retDataId, int[] argsIds)[] packedProgram = new (ElementaryFunction? method, int retDataId, int[] argsIds)[counter];
        for (int i = 0; i < counter; i++)
        {
            packedProgram[i] = Program[i];
        }
        Program = packedProgram;
    }

    public TempMainFunction(Dictionary<string, ElementaryFunction> elementaryFunctions, List<CodeLine> codeLines)
    {
        //// this is a process of compilation
        TempContainer container = new TempContainer();
        ElementaryFunctions = elementaryFunctions;

        Program = new (ElementaryFunction function, int retDateId, int[] argsIds)[codeLines.Count];
        
        Compilate(ref container, codeLines);

        (this.Container, _) = container;
        PackProgram();
    }

    public void Compilate(ref TempContainer tempContainer, List<CodeLine> codeLines)
    {
        foreach (var codeLine in codeLines)
        {
            if (codeLine.FunctionName == null || codeLine.FunctionName.Length == 0)
            {
                int retDataId = tempContainer.Peek(codeLine.ReturnData);
                if (retDataId == -1) throw new CompilationException($"Variable was not found: {codeLine.ReturnData}");

                
                switch (codeLine.Args.Count)
                {
                    case 0:
                        throw new CompilationException("No function, no args");
                    case 1:
                        int arg;
                        if (BasicString.isVarable(codeLine.Args[0])) arg = tempContainer.Peek(codeLine.Args[0]);
                        else arg = tempContainer.Push(null, ConstantConvertor(ref tempContainer, codeLine.Args[0]));
                        this.Program.Append(((ref DataContainer container, int retDataId, int[] argsIds) =>
                        {
                            container[retDataId] = argsIds[0];
                            
                        }, retDataId, new int[1]{arg}));
                        break;
                    default: 
                        int[] args = new int[codeLine.Args.Count];
                        for (int i = 0; i < codeLine.Args.Count; i++)
                        {
                            if (BasicString.isVarable(codeLine.Args[i])) args[i] = tempContainer.Peek(codeLine.Args[i]);
                            else args[i] = tempContainer.Push(null, ConstantConvertor(ref tempContainer, codeLine.Args[i]));
                        }
                        this.Program.Append(((ref DataContainer container, int retDataId, int[] argsIds) =>
                        {
                            throw new RuntimeException("No realization of turlples");
                        },retDataId, args));
                        break;
                }
                
            }
            else if (codeLine.FunctionName[0] != '#')
            {
                this.CompilateCodeLine(ref tempContainer, codeLine);
            }
            else
            {
                this.CompilateDirective(ref tempContainer, codeLine);
            }
        }
    }

    private static object? ConstantConvertor(ref TempContainer tempContainer, string arg)
    {
        if (BasicString.isFloat(arg))
        {
            return Convert.ToDouble(arg, new CultureInfo("en-US"));
        }
        else if (BasicString.isNumber(arg))
        {
            return Convert.ToInt32(arg);
        }
        else if (BasicString.isString(arg))
        {
            return arg.Substring(1, arg.Length-2);
        }
        else if(BasicString.isNull(arg))
        {
            return null;
        }
        else
        {
            throw new CompilationException("Type was not detected");
        }
    }
    private void CompilateCodeLine(ref TempContainer tempContainer, CodeLine codeLine)
    {
        int retDataId;
        int[] args;
        ElementaryFunction function;

        if (codeLine.ReturnData == null) retDataId = -1;
        else
        {
            retDataId = tempContainer.Peek(codeLine.ReturnData);
        }

        if (codeLine.Args == null) args = new int[0];
        else
        {
            args = new int[codeLine.Args.Count];
            for (int i = 0; i < codeLine.Args.Count; i++)
            {
                if (BasicString.isVarable(codeLine.Args[i])) args[i] = tempContainer.Peek(codeLine.Args[i]);
                else
                {
                    args[i] = tempContainer.Push(null, ConstantConvertor(ref tempContainer, codeLine.Args[i]));
                }
            }
        }

        try
        {
            function = ElementaryFunctions[codeLine.FunctionName];
        }
        catch (KeyNotFoundException)
        {
            throw new CompilationException("Fiunction {FunctionName} was not declarated");
        }
        this.AddCommand((function, retDataId, args));
    }
    private void CompilateDirective(ref TempContainer tempContainer, CodeLine codeLine)
    {
        switch (codeLine.FunctionName)
        {
            case "#init": // #init(name, start_value, modificator) // modificator (const, static, ref, ...)
                if (BasicString.isVarable(codeLine.Args[0]))
                {
                    if (codeLine.Args.Count == 1)
                    {
                        try
                        {
                            tempContainer.Push(codeLine.Args[0], null);
                        }
                        catch (ArgumentException)
                        {
                            throw new CompilationException("This variable was declarated");
                        }
                    }
                    else if (codeLine.Args.Count == 2) 
                    {
                        try
                        {
                            if (BasicString.isVarable(codeLine.Args[1]))
                            {
                                throw new CompilationException("Variable can be inited only by Constant");
                            }
                            tempContainer.Push(codeLine.Args[0], ConstantConvertor(ref tempContainer, codeLine.Args[1]));
                        }
                        catch (ArgumentException)
                        {
                            throw new CompilationException("This variable was declarated");
                        }
                    }
                    else throw new CompilationException("Has no realization");
                }
                else
                {
                    throw new CompilationException("It isn't posible to init not a variable");
                }
                break;
            case "#import":
                foreach (string arg in codeLine.Args)
                {
                    RuntimeLibConnector.AddToLib(ref ElementaryFunctions, arg);
                }
                break;
        }
    }
    
    public void Run()
    {
        foreach (var command in Program)
        {
            command.method.Invoke(ref this.Container, command.retDataId, command.argsIds);
        }
    }
}