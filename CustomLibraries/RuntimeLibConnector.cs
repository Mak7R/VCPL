using System.IO;
using System.Reflection;

using GlobalRealization;

namespace CustomLibraries;

public static class RuntimeLibConnector
{
    public static void AddToLib(Dictionary<string, ElementaryFunction> lib)
    {
        Dictionary<string, ElementaryFunction> addLib = AddLib() ?? new Dictionary<string, ElementaryFunction>();
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
    public static Dictionary<string, ElementaryFunction>? AddLib()
    {
        Dictionary<string, ElementaryFunction> mylib = new Dictionary<string, ElementaryFunction>();

        Console.Write("Write lib name with dll: ");
        string? lib = Console.ReadLine();
        Assembly asm;
        try
        {
            asm = Assembly.LoadFrom(lib ?? ".dll");
            LoadAllDependenciesRecursively(lib ?? ".dll");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.ReadLine();
            throw;
        }
        
        Type[] types = asm.GetTypes();
        
        int i = 0;
        bool wasFound = false;
        foreach (Type t in types)
        {
            if (t.Name == "MethodContainer") { wasFound = true; }
            if (!wasFound) i++;
        }

        string[] addlibs;
        if (wasFound)
        {
            var MethodContainer = types[i];
            MethodInfo? GetAll = MethodContainer.GetMethod("GetAll", BindingFlags.Public | BindingFlags.Static);

            object? methodsObj = GetAll?.Invoke(null, null);
            Dictionary<string, Action<object?>> dict;
            if (methodsObj is object[] data)
            {
                if (data[0] is string[] libs) addlibs = libs;
                else return null;

                if (data[1] is Dictionary<string, Action<object?>> funcs) dict = funcs;
                else return null;
            }
            else return null;
            //addlibs = ((string[], Dictionary<string, Action<object?>>)methodsObj);
            
            try
            {
                foreach(var func in dict) 
                {
                    // now it is not working // it should be changed after creating whole basic system
                    mylib.Add(func.Key, (ref ProgramStack stack, Reference? ReturnValue, List<ProgramObject>? args) =>
                    {
                        object? argsToSend = null;
                        if (args != null)
                        {
                            argsToSend = (from arg in args select arg.Get()).ToList();
                        }
                        func.Value.Invoke(argsToSend); // should be syntax updated
                        
                    });
                }
            }
            catch
            { 
                return null;
            }
        }
        else
        {
            return null;
        }

        foreach (string addLib in addlibs)
        {
            // перевірка, чи бібліотека вже завантажена
            if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.Location == addLib))
            {
                continue;
            }

            // завантаження бібліотеки
            Assembly.LoadFrom(addLib);
        }
        
        Console.WriteLine($"Succesful {asm.FullName} added!"); 
        return mylib;
    }
}