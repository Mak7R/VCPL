using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GlobalRealization;

namespace VCPL;

public static class RuntimeLibConnector
{
    public static void AddToLib(ref FunctionsContainer lib, string pathToLib)
    {
        Dictionary<string, ElementaryFunction> addLib = AddLib(pathToLib);
        foreach (var method in addLib)
        {
            lib.Add(method.Key, method.Value);
        }
    }
    public static List<string> LoadAllDependenciesRecursively(string pathToAssembly)
    {
        var loadedAssemblies = new List<string>();
        var stack = new Stack<string>();
        stack.Push(pathToAssembly);
        
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var assemblyName = new AssemblyName(args.Name);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName.Name}.dll");

            if (!File.Exists(path))
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", $"{assemblyName.Name}.dll");
            }

            if (!File.Exists(path))
            {
                return null;
            }

            return Assembly.LoadFrom(path);
        };
        
        while (stack.Count > 0)
        {
            var assemblyPath = stack.Pop();
            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            var assembly = Assembly.Load(assemblyName);
            
            loadedAssemblies.Add(assemblyPath);
            
            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                var referencedAssemblyPath = Path.Combine(
                    Path.GetDirectoryName(assemblyPath),
                    referencedAssemblyName.Name + ".dll"
                );
                
                if (File.Exists(referencedAssemblyPath) && !loadedAssemblies.Contains(referencedAssemblyPath))
                {
                    stack.Push(referencedAssemblyPath);
                }
            }
        }
        
        return loadedAssemblies;
    }
    public static Dictionary<string, ElementaryFunction> AddLib(string pathToLib)
    {
        Dictionary<string, ElementaryFunction> mylib = new Dictionary<string, ElementaryFunction>();
        
        Assembly asm = Assembly.LoadFrom(pathToLib);;
        LoadAllDependenciesRecursively(pathToLib);
        
        Type[] types = asm.GetTypes();
        
        int index = -1;
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].Name == "MethodContainer")
            {
                index = i;
                break;
            }
        }

        string[] addlibs;
        if (index != -1)
        {
            var MethodContainer = types[index];
            MethodInfo? GetAll = MethodContainer.GetMethod("GetAll", BindingFlags.Public | BindingFlags.Static);

            MethodInfo? GetNecessary = MethodContainer.GetMethod("GetNecessaryLibs", BindingFlags.Public | BindingFlags.Static);
            
            if (GetAll == null) throw new Exception("Get all was not found"); // new exception system panding (panding == очікується)
            if (GetNecessary != null)
            {
                object? libs = GetNecessary.Invoke(null, null);
                if (libs is string[] strings)
                {
                    AddNecessaryLibs(strings); 
                }
                else
                {
                    throw new Exception("incorrect output"); // new exception system panding
                }
            }
            object? methodsObj = GetAll?.Invoke(null, null);
            Dictionary<string, ElementaryFunction> dict;
            if (methodsObj is Dictionary<string, ElementaryFunction> funcs)
            {
                dict = funcs;
            }
            else throw new Exception("incorrect return"); // new exception system panding
        }
        else
        {
            throw new Exception("MethodContainer was not found"); // new exception system panding
        }
        
        return mylib;
    }

    public static void AddNecessaryLibs(string[] libs)
    {
        foreach (string lib in libs)
        {
           if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.Location == lib))
            {
                continue;
            }
            Assembly.LoadFrom(lib);
        }
    }
}