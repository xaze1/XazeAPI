// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Reflection;
using EclipsePlugin.API.CustomModules;
using HarmonyLib;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using MEC;
using PlayerStatsSystem;
using XazeAPI.API;
using XazeAPI.API.AudioCore.FakePlayers;

namespace XazeAPI;

public class APILoader : Plugin
{
    public const string PatchGroup = "XAZE-API.Patches";
    public override string Name => "XazeAPI";
    public override string Description => "API Library by xaze_";
    public override string Author => "xaze_";
    public override Version Version => new Version(1, 0, 0);
    public override Version RequiredApiVersion => new(0, 0, 0);
    public override LoadPriority Priority =>  LoadPriority.Highest;

    public static APILoader Singleton { get; private set; }
    public static bool Debug { get; set; } = false;
    public static readonly Assembly APIAssembly = Assembly.GetAssembly(typeof(APILoader));
    public static readonly Harmony Patches = new Harmony("XAZE-API");
    
    public override void Enable()
    {
        Singleton = this;
        Logging.ServerLog("Thank you for using XazeAPI! Version " + Version, ConsoleColor.Magenta);
        AudioManager.Awake(APIAssembly);
        
        Patches.PatchCategory(PatchGroup);

        ReferenceHub.OnPlayerAdded += ctx => Timing.CallDelayed(0.1f, () => SetupPlayer(ctx));
    }

    public override void Disable()
    {
    }

    private static void SetupPlayer(ReferenceHub hub)
    {
        if (hub.Mode == CentralAuth.ClientInstanceMode.Host || hub.Mode == CentralAuth.ClientInstanceMode.DedicatedServer || AudioManager.ActiveFakes.Contains(hub)) return;

        CustomHealthStat healthStat;
        hub.playerStats._dictionarizedTypes[typeof(HealthStat)] = hub.playerStats.StatModules[Array.IndexOf(PlayerStats.DefinedModules, typeof(HealthStat))] = healthStat = new CustomHealthStat { Hub = hub };
        healthStat.CurValue = 100;
    }
}