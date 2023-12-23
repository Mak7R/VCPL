using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using VCPL.Compilator.Contexts;

namespace VCPL.Compilator;

public static class CustomLibraryConnector
{
    public static string LibrariesDomain = AppDomain.CurrentDomain.BaseDirectory;
    public const string FileFormat = ".dll";
    private static bool ContainsAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        var loadedAssemblies = context.Assemblies;
        foreach (Assembly assembly in loadedAssemblies)
            if (assembly.FullName == assemblyName.FullName)
                return true;
        return false;
    }
    private static bool ContainsAssembly(AssemblyLoadContext loadContext, string assemblyName)
    {
        var loadedAssemblies = loadContext.Assemblies;
        foreach (Assembly assembly in loadedAssemblies)
            if (assembly.GetName().Name == assemblyName)
                return true;
        return false;
    }
    
    private static void LoadDependencies(AssemblyLoadContext loadContext, AssemblyName assemblyName)
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
    public static void Import(AbstractContext context, AssemblyLoadContext loadContext, string assemblyName)
    {
        throw new NotImplementedException();
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

        //if (Context.GetValue(null) is List<(string? name, object value)> objects)
        //{
        //    context = new Context(context);
        //    foreach (var item in objects)
        //        context.Push(item.name, item.value);
        //}
        //else throw new CompilationException($"Cannot convert {Context.GetType()} to {typeof(List<(string?, MemoryObject)>)}");
    }

}