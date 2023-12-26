﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPL.CodeConvertion
{
    public class CLiteConvertor : ICodeConvertor
    {
        /// <summary>
        /// Converts the string line to CodeLine
        /// </summary>
        /// <param name="line">String line which should be converted to CodeLine</param>
        /// <exception cref="SyntaxException">If is any error with line parsing it can throw new SyntaxException</exception>
        /// <returns>New CodeLine which was generetad from line</returns>
        public CodeLine Convert(string line)
        {
            string FunctionName = "";
            string? ReturnData;
            List<string> Args;

            line = line.Trim();

            int equalsIndex = line.IndexOf('=');
            if (equalsIndex == -1)
            {
                ReturnData = null;
            }
            else
            {
                ReturnData = line.Substring(0, equalsIndex).Trim();
                if (ReturnData == "") throw new SyntaxException("Missed return getter");
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
                throw new SyntaxException("No open parren excaption or no close parren excaption");
            }
            else if (openParenIndex > closeParenIndex)
            {
                throw new SyntaxException("No open parren excaption");
            }
            else
            {
                FunctionName = line.Substring(0, openParenIndex).Trim();
                string argsString = line.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
                Args = GetArgsList(argsString);
            }
            
            if (ReturnData != null) { Args.Add(ReturnData); }
            return new CodeLine(FunctionName, Args);
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
