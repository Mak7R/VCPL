using BasicFunctions;
using GlobalRealization;
using System;
using System.Collections.Generic;
using System.IO;
using VCPL.CodeConvertion;
using VCPL.Instructions;
using VCPL.Stacks;

namespace VCPL.Еnvironment;

public abstract class AbstractEnvironment
{
    public readonly CodeConvertorsContainer envCodeConvertorsContainer = new CodeConvertorsContainer();
    public Func<string, string[]> SplitCode = (string code) => { throw new Exception("Split code was not implemented"); };  // enviriment exception
    public virtual List<CodeLine> ConvertCode(string code, string convertorName)
    {
        try
        {
            ICodeConvertor codeConvertor = envCodeConvertorsContainer.GetCodeConvertor(convertorName);
            string[] stringCodeLines = SplitCode(code);

            List<CodeLine> codeLines = new List<CodeLine>();
            int i = 0;
            foreach (string codeLine in stringCodeLines)
            {
                i++;
                if (BasicString.IsNoDataString(codeLine)) continue;
                codeLines.Add(codeConvertor.Convert(i, codeLine));
            }
            return codeLines;
        }catch (Exception ex)
        {
            Logger.Log(ex.Message);
            throw;
        }
    }

    public string CurrentDirectory { get; set; } = string.Empty;
    public string BaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
    public virtual string GetFilePath(string name, string format)
    {
        if (File.Exists($"{name}{format}"))
            return $"{name}{format}";
        else if (File.Exists($"{BaseDirectory}{name}{format}"))
            return $"{BaseDirectory}{name}{format}";
        else if (File.Exists($"{CurrentDirectory}{name}{format}"))
            return $"{CurrentDirectory}{name}{format}";
        else
            throw new FileNotFoundException($"File {name}{format} was not found.");
    }

    public readonly ILogger Logger;

    private RuntimeStack? stack;
    public RuntimeStack RuntimeStack
    {
        get { return stack ?? throw new Exception("Stack was not inited"); } // enviroment exception
        set { stack = value; }
    }

    public abstract ElementaryFunction CreateFunction(Instruction[] program, int size);

    public abstract Instruction CreateInstruction(CodeLine codeLine, ElementaryFunction function, IPointer[] args);

    public AbstractEnvironment(ILogger logger)
    {
        Logger = logger;
    }
}