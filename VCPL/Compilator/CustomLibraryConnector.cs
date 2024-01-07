using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace VCPL.Compilator;

public static class CustomLibraryConnector
{
    public static string LibrariesDomain = AppDomain.CurrentDomain.BaseDirectory;
    public const string FileFormat = ".dll";
    public static bool ContainsAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        var loadedAssemblies = context.Assemblies;
        foreach (Assembly assembly in loadedAssemblies)
            if (assembly.FullName == assemblyName.FullName)
                return true;
        return false;
    }
    public static bool ContainsAssembly(AssemblyLoadContext loadContext, string assemblyName)
    {
        var loadedAssemblies = loadContext.Assemblies;
        foreach (Assembly assembly in loadedAssemblies)
            if (assembly.GetName().Name == assemblyName)
                return true;
        return false;
    }
    
    public static void LoadDependencies(AssemblyLoadContext loadContext, AssemblyName assemblyName)
    {
        if (ContainsAssembly(loadContext, assemblyName)) return;
        
        Assembly dependent;
        try { dependent = loadContext.LoadFromAssemblyName(assemblyName); }
        catch
        {
            try { dependent = loadContext.LoadFromAssemblyPath(LibrariesDomain + assemblyName.Name + FileFormat); }
            catch { throw new Exception("Cannot to load lib"); }
        }

        AssemblyName[] dependencies = dependent.GetReferencedAssemblies();
        foreach (var dep in dependencies) LoadDependencies(loadContext, dep);
    }
    public static void Include(CompileStack stack, AssemblyLoadContext loadContext, string assemblyName, string? namespaceName = null)
    {
        Assembly lib;

        if (!ContainsAssembly(loadContext, assemblyName))
        {
            try
            {
                lib = loadContext.LoadFromAssemblyPath(assemblyName + FileFormat);
            }
            catch
            {
                try
                {
                    lib = loadContext.LoadFromAssemblyPath(LibrariesDomain + assemblyName + FileFormat);
                }
                catch
                {
                    throw new Exception("Cannot to load a library");
                }
                throw new Exception("Cannot to load a library");
            }

            AssemblyName[] dependencies = lib.GetReferencedAssemblies();
            foreach (var dependent in dependencies) LoadDependencies(loadContext, dependent);
        }
        else
        {
            lib = loadContext.LoadFromAssemblyName(new AssemblyName(assemblyName));
        }

        var pathPoints = assemblyName.Split("\\");
        string shortAsmName = pathPoints[pathPoints.Length - 1];
        Type Library = lib.GetType(shortAsmName + ".Library")
                               ?? throw new Exception(
                                   "In assembly was not found class Library");

        FieldInfo Items = Library.GetField("Items", BindingFlags.Public | BindingFlags.Static)
                            ?? throw new Exception("Field Items was not found");
        object? value = Items.GetValue(null);
        if (value is ICollection<(string? name, object? value)> items)
        {
            stack.AddConst(namespaceName ?? shortAsmName, $"Namespace: {namespaceName ?? shortAsmName}");
            foreach (var item in items)
                stack.AddConst($"{namespaceName ?? shortAsmName}.{item.name}", item.value);
            stack.Up();
        }
        else throw new Exception($"Cannot convert {value?.GetType().ToString() ?? "null"} to {typeof(ICollection<(string?, object?)>)}");
    }

}