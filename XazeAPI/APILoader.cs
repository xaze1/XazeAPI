using System;
using System.Reflection;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using XazeAPI.API;
using XazeAPI.API.AudioCore.FakePlayers;

namespace XazeAPI;

public class APILoader : Plugin
{
    public override string Name => "XazeAPI";
    public override string Description => "API Library by xaze_";
    public override string Author => "xaze_";
    public override Version Version => new Version(1, 0, 0);
    public override Version RequiredApiVersion => new(0, 0, 0);
    public override LoadPriority Priority =>  LoadPriority.Highest;

    public static APILoader Singleton { get; private set; }
    public static bool Debug { get; set; } = false;
    public static readonly Assembly APIAssembly = Assembly.GetAssembly(typeof(APILoader));
    
    public override void Enable()
    {
        Singleton = this;
        Logging.ServerLog("Thank you for using XazeAPI! Version " + Version, ConsoleColor.DarkMagenta);
        AudioManager.Awake(APIAssembly);
    }

    public override void Disable()
    {
    }
}