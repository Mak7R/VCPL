using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using GlobalRealization;
using Pointer = System.Reflection.Pointer;

namespace VCPL;

public static class CustomLibraryConnector
{
    public static List<string> LoadAllDependenciesRecursively(string pathToAssembly) // useless
    {
        var loadedAssemblies = new List<string>();
        var stack = new Stack<string>();
        stack.Push(pathToAssembly);

        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var assemblyName = new AssemblyName(args.Name);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName.Name}.dll");

            if (!File.Exists(path)) return null; // maybe throw exception

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

    public static void Import(Context context, string pathToLib) // useless
    {
        Assembly asm = Assembly.LoadFrom(pathToLib);
        LoadAllDependenciesRecursively(pathToLib);

        Type MethodContainer = asm.GetType("CustomContext")
                               ?? throw new CompilationException(
                                   "In assembly was not found class CustomContext"); ///////////////////////

        MethodInfo GetAll = MethodContainer.GetMethod("GetAll", BindingFlags.Public | BindingFlags.Static)
                            ?? throw new CompilationException("Get all was not found");

        MethodInfo? GetNecessary =
            MethodContainer.GetMethod("GetNecessaryLibs", BindingFlags.Public | BindingFlags.Static);


        if (GetNecessary != null)
        {
            object libs = GetNecessary.Invoke(null, null);
            if (libs is string[] pathes)
                foreach (string lib in pathes)
                    if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.Location == lib))
                        Assembly.LoadFrom(lib);
                    else throw new CompilationException($"Cannot convert {libs.GetType()} to {typeof(string[])}");
        }

        if (GetAll.Invoke(null, null) is Dictionary<string, MemoryObject> objects)
        {
            context = context.NewContext();
            foreach (var memoryObject in objects) context.Push(memoryObject.Key, memoryObject.Value);
        }
        else
            throw new CompilationException(
                $"Cannot convert {context.GetType()} to {typeof(Dictionary<string, MemoryObject>)}");
    }

    public static AssemblyLoadContext NewImport(ref Context context, string assemblyName)
    {
        AssemblyLoadContext loadContext = new AssemblyLoadContext($"Context {assemblyName}", true);
        Assembly lib = loadContext.LoadFromAssemblyPath(AppDomain.CurrentDomain.BaseDirectory + assemblyName + ".dll"); // 

        void LoadDependinces(AssemblyName dep)
        {
            var loadedAssemblies = loadContext.Assemblies;
            foreach (Assembly assembly in loadedAssemblies)
                if (assembly.FullName == dep.FullName)
                    return; 
            
            Assembly ldep = null;
            try
            {
                ldep = loadContext.LoadFromAssemblyName(dep);
            }
            catch
            {
                try { ldep = loadContext.LoadFromAssemblyPath(AppDomain.CurrentDomain.BaseDirectory + dep.Name + ".dll"); }
                catch { throw new CompilationException("Cannot to load lib"); }
            }

            AssemblyName[] deps = ldep.GetReferencedAssemblies();
            foreach (var d in deps) LoadDependinces(d);
        }
        
        AssemblyName[] dependencies = lib.GetReferencedAssemblies();
        foreach (var dependency in dependencies) LoadDependinces(dependency);
        
        Type MethodContainer = lib.GetType(assemblyName + ".CustomContext")
                               ?? throw new CompilationException(
                                   "In assembly was not found class CustomContext"); ///////////////////////

        MethodInfo GetAll = MethodContainer.GetMethod("GetAll", BindingFlags.Public | BindingFlags.Static)
                            ?? throw new CompilationException("Get all was not found");

        if (GetAll.Invoke(null, null) is List<(string name, MemoryObject value)> objects)
            foreach (var memoryObject in objects)
                context.Push(memoryObject.name, memoryObject.value);
        else
            throw new CompilationException(
                $"Cannot convert {context.GetType()} to {typeof(Dictionary<string, MemoryObject>)}");
        
        return loadContext;
    }

}