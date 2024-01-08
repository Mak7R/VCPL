using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using VCPL.Compilator.Stacks;
using VCPL.Exceptions;
using VCPL.Еnvironment;

namespace VCPL.Compilator;

public static class CustomLibraryConnector
{
    public const string FileFormat = ".dll";
    public static bool ContainsAssembly(AssemblyLoadContext loadContext, string assemblyName)
    {
        var loadedAssemblies = loadContext.Assemblies;
        foreach (Assembly assembly in loadedAssemblies)
            if (assembly.GetName().Name == assemblyName)
                return true;
        return false;
    }
    
    public static void LoadDependencies(AssemblyLoadContext loadContext, AssemblyName assemblyName, AbstractEnvironment env)
    {
        if (ContainsAssembly(loadContext, assemblyName.FullName)) return;
        
        Assembly dependent;
        try { dependent = loadContext.LoadFromAssemblyName(assemblyName); }
        catch
        {
            try { dependent = loadContext.LoadFromAssemblyPath(env.GetFilePath(assemblyName.Name ?? string.Empty, FileFormat)); }
            catch(Exception e) { throw new Exception(ExceptionsController.CannotLoadLib(assemblyName.Name ?? string.Empty, e.Message)); }
        }

        AssemblyName[] dependencies = dependent.GetReferencedAssemblies();
        foreach (var dep in dependencies) LoadDependencies(loadContext, dep, env);
    }
    public static void Include(CompileStack stack, AbstractEnvironment env, AssemblyLoadContext loadContext, string assemblyName, string? namespaceName = null)
    {
        Assembly lib;
        
        if (!ContainsAssembly(loadContext, assemblyName))
        {
            try
            {
                lib = loadContext.LoadFromAssemblyPath(env.GetFilePath(assemblyName, FileFormat));
            }
            catch (Exception e)
            {
                throw new Exception(ExceptionsController.CannotLoadLib(assemblyName, e.Message));
            }

            AssemblyName[] dependencies = lib.GetReferencedAssemblies();
            foreach (var dependent in dependencies) LoadDependencies(loadContext, dependent, env);
        }
        else
        {
            lib = loadContext.LoadFromAssemblyName(new AssemblyName(assemblyName));
        }

        var pathPoints = assemblyName.Split("\\");
        string shortAsmName = pathPoints[pathPoints.Length - 1];
        Type Library = lib.GetType(shortAsmName + ".Library")
                               ?? throw new Exception(
                                   ExceptionsController.InAssemblyNotFound($"class Library ({shortAsmName + ".Library"})"));

        FieldInfo Items = Library.GetField("Items", BindingFlags.Public | BindingFlags.Static)
                            ?? throw new Exception(ExceptionsController.InAssemblyNotFound("field Items"));
        object? value = Items.GetValue(null);
        if (value is ICollection<(string? name, object? value)> items)
        {
            stack.AddConst(namespaceName ?? shortAsmName, $"Namespace: {namespaceName ?? shortAsmName}");
            foreach (var item in items)
                stack.AddConst($"{namespaceName ?? shortAsmName}.{item.name}", item.value);
            stack.Up();
        }
        else throw new Exception(ExceptionsController.CannotConvert(value?.GetType(), typeof(ICollection<(string? name, object? value)>)));
    }

}