using System.Reflection;

namespace XazeAPI;

public class MainStaticVars
{
    public static bool Debug { get; set; } = false;
    public static readonly Assembly APIAssembly = Assembly.GetAssembly(typeof(MainStaticVars));
}