using System;
using System.Collections.Generic;
using System.Reflection;
using GlobalRealization;

namespace Example;

/// <summary>
/// This class is an example how to create user libraries.
/// This class have to have this name and method GetAll. You can also add mehtod GetNecessaryLibs.
/// </summary>
public static class MethodContainer
{
    /// <summary>
    /// This is a necessary mathod. It returns Elementary Functions to VCPL.
    /// </summary>
    /// <returns>Elementary Functions. Dictionary of method name and method.</returns>
    public static Dictionary<string, ElementaryFunction> GetAll()
    {
        return new Dictionary<string, ElementaryFunction>()
        {
            { "MethodName", (DataContainer container, int retDataId, int[] argsIds) => { return false; } }
        };
    }

    /// <summary>
    /// This is not a necessary mathod. If it is in your class it just load necessary libs to you program.
    /// </summary>
    /// <returns>List of strings which are pathes to necessary libraries.</returns>
    public static string[] GetNecessaryLibs()
    {
        return new string[]
        {
            // libs which you are using
        };
    }
    
    /// <summary>
    /// This function can help you to know which methods is in you code
    /// </summary>
    /// <returns>List of dll which is in your project</returns>
    public static List<string> LibPathes()
    {
        static string MakeStringForPasting(string path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '\\')
                {
                    path = path.Substring(0, i) + "\\" + path.Substring(i);
                    i++;
                }
            }

            return "\"" + path + "\",";
        }
        
        List<string> libs = new List<string>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies) libs.Add(MakeStringForPasting(assembly.Location));

        foreach (var lib in libs)
        {
            Console.WriteLine(lib);
        }
            
        return libs;
    }
}