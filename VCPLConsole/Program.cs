using System;
using System.Collections.Generic;
using VCPL;
using VCPL.Еnvironment;

namespace VCPLConsole
{
    public static class FileProvider
    {
        public static string ReadCode(string path)
        {
            string code = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    code = sr.ReadToEnd();
                }
                ConsoleLogger.CSLogger.Log("File was successful readed");
            }
            catch (Exception e)
            {
                ConsoleLogger.CSLogger.Log(e.Message);
            }
            return code;
        }

        public static void WriteCode(string path, string code)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(code);
                }
                ConsoleLogger.CSLogger.Log("File was successful writed");
            }
            catch (Exception e)
            {
                 ConsoleLogger.CSLogger.Log(e.Message);
            }
        }
    }
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) { Menu.Code = FileProvider.ReadCode(args[0]); Menu.FilePath = args[0]; }
            Menu.ReadOption();
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
        public readonly static ILogger CSLogger = new ConsoleLogger();
    }
}