using System.Reflection;
using GlobalRealization;

namespace CustomLibraries;

/// <summary>
/// This class is an example how to create user libraries
/// </summary>
public static class MethodContainer
{
    /// <summary>
    /// Dictionry is an container for all methods what you want to add
    /// </summary>
    public static Dictionary<string, Action<object?>> lib = new Dictionary<string, Action<object?>>()
    {
        { "MethodName", (object? args) =>
            {
                
            }
        }
    };

    /// <summary>
    /// Method must be in class. It should to give all your methods to VCPL
    /// </summary>
    /// <returns>object[2] where object[0] = list of all dll which you use in your methods, object[1] = Dictionary</returns>
    public static object[] GetAll()
    {

        return new object[2] {new string[] {
            
        },
        lib};
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