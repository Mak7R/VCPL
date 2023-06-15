using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VCPL;

public static class FileCodeEditor
{
    public static bool ReadCodeS(string path, out string code)
    {
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                code = sr.ReadToEnd();
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            code = string.Empty;
            return false;
        }
    }

    public static bool WriteCodeS(string path, string code)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(code);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }

        return true;
    }
    public static List<string> ReadCode(string path)
    {
        List<string> lines = new List<string>();
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
        return lines;
    }

    public static bool WriteCode(string path, List<string> lines)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (string line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }
}