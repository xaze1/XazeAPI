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
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using MEC;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerStatsSystem;
using XazeAPI.API;
using XazeAPI.API.AudioCore.FakePlayers;
using XazeAPI.API.AudioCore.Speakers;
using XazeAPI.API.Events;
using XazeAPI.API.Events.Handler;
using XazeAPI.API.Helpers;

namespace XazeAPI;

public class APILoader : Plugin
{
    public const string PatchGroup = "XAZE-API.Patches";
    public override string Name => "XazeAPI";
    public override string Description => "API Library by xaze_";
    public override string Author => "xaze_";
    public override Version Version => new(1, 1);
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
    public override LoadPriority Priority =>  LoadPriority.Highest;

    public static APILoader Singleton { get; private set; }
    public static bool Debug { get; set; } = false;
    public static readonly Assembly APIAssembly = Assembly.GetAssembly(typeof(APILoader));
    public static readonly Harmony Patches = new("XAZE-API");

    public void Setup()
    {
        if (Singleton != null)
        {
            return;
        }
        
        Singleton = this;
        Logging.ServerLog(ConsoleColor.Magenta, "Thank you for using XazeAPI! Version", Version);
        CustomSSSSync.Init();
        XazeHandlerManager.InitializeEvents();
        
        Patches.PatchCategory(PatchGroup);

        SpeakerLoader.OnTrackSelected += (speaker, track) =>
        {
            Logging.Debug("Track selected: " + track);
        };
        
        SpeakerLoader.OnTrackSelecting += (speaker) =>
        {
            Logging.Debug("Selecting Track: " + speaker);
        };
        
        ReferenceHub.OnPlayerAdded += ctx => Timing.CallDelayed(0.1f, () => SetupPlayer(ctx));
        FpcServerPositionDistributor.RoleSyncEvent += DisguiseHelper.OnRoleSyncEvent;
        PlayerEvents.Left += HintHelper.RemoveHub;
        
        XazeHandlerManager.InternalInvoke();
    }
    
    public override void Enable()
    {
        Setup();
    }

    public override void Disable()
    {
        Setup();
    }

    private static void SetupPlayer(ReferenceHub hub)
    {
        if (hub.Mode == CentralAuth.ClientInstanceMode.Host || hub.Mode == CentralAuth.ClientInstanceMode.DedicatedServer || FakeManager.ActiveFakes.Contains(hub)) return;

        CustomHealthStat healthStat;
        hub.playerStats._dictionarizedTypes[typeof(HealthStat)] = hub.playerStats.StatModules[Array.IndexOf(PlayerStats.DefinedModules, typeof(HealthStat))] = healthStat = new CustomHealthStat { Hub = hub };
        healthStat.CurValue = 100;
    }
}