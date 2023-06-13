using System;
using System.Collections.Generic;
using System.IO;

namespace VCPL;

public static class FileCodeEditor
{
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