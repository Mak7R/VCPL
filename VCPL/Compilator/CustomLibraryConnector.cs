﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using GlobalRealization.Memory;

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
            catch { throw new CompilationException("Cannot to load lib"); }
        }

        AssemblyName[] dependencies = dependent.GetReferencedAssemblies();
        foreach (var dep in dependencies) LoadDependencies(loadContext, dep);
    }
    public static void Import(ref Context context, AssemblyLoadContext loadContext, string assemblyName)
    {
        Assembly lib;
        if (!ContainsAssembly(loadContext, assemblyName))
        {
            try
            {
                lib = loadContext.LoadFromAssemblyPath(LibrariesDomain + assemblyName + FileFormat);
            }
            catch
            {
                throw new CompilationException("Cannot to load a library");
            }

            AssemblyName[] dependencies = lib.GetReferencedAssemblies();
            foreach (var dependent in dependencies) LoadDependencies(loadContext, dependent);
        }
        else
        {
            lib = loadContext.LoadFromAssemblyName(new AssemblyName(assemblyName));
        }
        
        Type MethodContainer = lib.GetType(assemblyName + ".CustomContext")
                               ?? throw new CompilationException(
                                   "In assembly was not found class CustomContext");

        FieldInfo Context = MethodContainer.GetField("Context", BindingFlags.Public | BindingFlags.Static)
                            ?? throw new CompilationException("Field Context was not found");

        if (Context.GetValue(null) is List<(string? name, MemoryObject value)> objects) context.Push(objects);
        else
            throw new CompilationException(
                $"Cannot convert {context.GetType()} to {typeof(Dictionary<string, MemoryObject>)}");
    }

}