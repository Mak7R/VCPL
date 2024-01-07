using BasicFunctions;
using GlobalRealization;
using System;
using System.Collections.Generic;
using System.IO;
using VCPL.CodeConvertion;
using VCPL.Instructions;

namespace VCPL.Еnvironment;

public abstract class AbstractEnvironment
{
    public readonly CodeConvertorsContainer envCodeConvertorsContainer = new CodeConvertorsContainer();
    public Func<string, string[]> SplitCode = (string code) => { throw new Exception("Split code was not implemented"); };  // enviriment exception
    public List<CodeLine> ConvertCode(string code, string convertorName)
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
    public string GetFilePath(string name, string format)
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

    public abstract ElementaryFunction CreateFunction(Instruction[] program, int size);

    public abstract Instruction CreateInstruction(CodeLine codeLine, ElementaryFunction function, IPointer[] args);

    public AbstractEnvironment(ILogger logger)
    {
        Logger = logger;
    }
}