<link rel="stylesheet" href="styles.css">




# UA 
<h3>Як створити власний синтаксис для VCPL</h3>

Це дуже просто! <br>
Вам необхідно лише реалізувати інтерфейс ICodeConvertor.
Та зареєструвати його в оточенні яке буде використано про компіляції
Приклад реалізації інтерфейсу ICodeConvertor базовим синтаксичним конвертором <a href="/Docs/CLite.md">CLite</a>: 
<pre class="code">
using VCPL.Exceptions;
namespace VCPL.CodeConvertion
{
    public class CLiteConvertor : ICodeConvertor
    {
        public CodeLine Convert(int lineNumber, string line)
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
                if (ReturnData == "") throw new SyntaxException(lineNumber, "no variable before '=' symbol");
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
                throw new SyntaxException(lineNumber, "no open parren or no close parren");
            }
            else if (openParenIndex > closeParenIndex)
            {
                throw new SyntaxException(lineNumber, "no open parren");
            }
            else
            {
                FunctionName = line.Substring(0, openParenIndex).Trim();
                string argsString = line.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
                Args = GetArgsList(argsString);
            }
            
            if (ReturnData != null) { Args.Add(ReturnData); }
            return new CodeLine(lineNumber, FunctionName, Args);
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
</pre>
Приклад реєстрації конвертору в оточенні:
<pre class="code">
releaseEnvironment.envCodeConvertorsContainer.AddCodeConvertor("CLite", new CLiteConvertor());
</pre>
</ol>
