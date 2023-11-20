﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPL.CodeConvertion
{
    public class CLiteConvertor/*<T>*/ : ICodeConvertor /*where T: ICodeLine*/
    {
        public ICodeLine Convert(string line)
        {
            string? FunctionName = "";
            string? ReturnData = null;
            List<string>? Args = null;

            line = line.Trim();

            int equalsIndex = line.IndexOf('=');
            if (equalsIndex == -1)
            {
                ReturnData = null;
            }
            else
            {
                ReturnData = line.Substring(0, equalsIndex).Trim();
                // if (ReturnGetter == "") -> SyntaxException -> MissedReturnGetterException
                line = line.Substring(equalsIndex + 1).Trim();
            }

            int openParenIndex = line.IndexOf('(');
            int closeParenIndex = line.IndexOf(')');
            if (openParenIndex == -1 && closeParenIndex == -1)
            {
                Args = new List<string> { line };
            }
            else if ((openParenIndex == -1) != (closeParenIndex == -1))
            {
                // if <-> else
                // throw new SyntaxException -> new NoOpenParrenExcaption
                // throw new SyntaxException -> new NoCloseParrenExcaption
            }
            else if (openParenIndex > closeParenIndex)
            {
                // throw new SyntaxException -> new NoOpenParrenExcaption
            }
            else
            {
                FunctionName = line.Substring(0, openParenIndex).Trim();
                string argsString = line.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
                Args = GetArgsList(argsString);
            }

            //var constructor = typeof(T).GetConstructor(System.Reflection.BindingFlags.Public, new Type[] { FunctionName.GetType(), Args.GetType(), ReturnData.GetType() });
            //object? obj = constructor?.Invoke(new object[] { FunctionName, Args, ReturnData }) ?? null;
            //if (obj is null) { throw new Exception("Can not create new object with T type"); }

            //ICodeLine codeLine = (ICodeLine)obj;
            //return codeLine;

            return new CodeLine(FunctionName, Args, ReturnData);
        }

        private static List<string> GetArgsList(string argsString)
        {
            List<string> args = new List<string>();

            int startIndex = 0;
            int endIndex = 0;
            bool inQuotes = false;
            char quoteChar = '\0';

            for (int i = 0; i < argsString.Length; i++)
            {
                char currentChar = argsString[i];

                if (!inQuotes && (currentChar == ',' || i == argsString.Length - 1))
                {
                    endIndex = i;
                    if (i == argsString.Length - 1)
                    {
                        endIndex++;
                    }
                    string arg = argsString.Substring(startIndex, endIndex - startIndex).Trim();
                    args.Add(arg);
                    startIndex = i + 1;
                }
                else if (!inQuotes && (currentChar == '\'' || currentChar == '"'))
                {
                    inQuotes = true;
                    quoteChar = currentChar;
                    startIndex = i;
                }
                else if (inQuotes && currentChar == quoteChar)
                {
                    inQuotes = false;
                    if (i == argsString.Length - 1)
                    {
                        endIndex = i + 1;
                        string arg = argsString.Substring(startIndex, endIndex - startIndex).Trim();
                        args.Add(arg);
                    }
                }
            }

            return args;
        }
    }
}