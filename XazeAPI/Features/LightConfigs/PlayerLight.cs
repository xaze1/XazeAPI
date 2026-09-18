// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.Collections.Generic;
using JetBrains.Annotations;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Visibility;
using UnityEngine;
using XazeAPI.API.Extensions;
using XazeAPI.Features.Helpers;

namespace XazeAPI.Features.LightConfigs;

public class PlayerLight<T> : PlayerLight where T : LightConfigBase
{
    private static readonly List<PlayerLight<T>> _list = new();
    public static IReadOnlyList<PlayerLight<T>> InternalList => _list.AsReadOnly();

    public new T LightConfig => (T)base.LightConfig;
    private readonly bool _destroyOnDeath;
    private readonly Dictionary<Player, float> CurrentIntensity = new();

    private void Update()
    {
        if (Target is not { IsAlive: true })
        {
            if (Light.Intensity == 0)
                return;
            
            Light.Intensity = 0;
            CurrentIntensity.Clear();
            return;
        }

        foreach (var plr in Player.ReadyList)
        {
            float intensityForPlayer = LightConfig.Intensity;

            if (plr.IsAlive)
            {
                if (plr.RoleBase is ICustomVisibilityRole role && !role.VisibilityController.ValidateVisibility(Target.ReferenceHub))
                    intensityForPlayer = 0;
            }
            
            if (CurrentIntensity.TryGetValue(plr, out var lastIntensity) && 
                Mathf.Approximately(lastIntensity, intensityForPlayer))
                continue;
            
            plr.SendFakeSyncVar(Light.Base, 32UL, intensityForPlayer);
            CurrentIntensity[plr] = intensityForPlayer;
        }
    }

    private void OnLeft(PlayerLeftEventArgs args)
    {
        if (args.Player != Target)
            return;
        
        Destroy();
    }
    
    private void OnDeath(PlayerDeathEventArgs args)
    {
        if (args.Player != Target)
            return;
        
        Destroy();
    }

    public override void Destroy()
    {
        StaticUnityMethods.OnUpdate -= Update;
        PlayerEvents.Left -= OnLeft;
        if (_destroyOnDeath)
            PlayerEvents.Death -= OnDeath;
        
        LightConfig.OnThisDestroyed -= Destroy;
        LightConfig.Destroy();
        _list.Remove(this);
        Unregister(this);
    }
    
    private PlayerLight(Player target, T lightConfig, bool destroyOnDeath = false) : base(target, lightConfig)
    {
        _destroyOnDeath = destroyOnDeath;

        lightConfig.OnThisDestroyed += Destroy;
        StaticUnityMethods.OnUpdate += Update;
        PlayerEvents.Left += OnLeft;
        if (_destroyOnDeath)
            PlayerEvents.Death += OnDeath;
        
        _list.Add(this);
        Register(this);
    }
    
    [CanBeNull]
    public static PlayerLight<T> Create(Player Owner, T lightConfig)
    {
        if (Owner.GameObject == null || Owner.IsHost)
            return null;
        
        return new PlayerLight<T>(Owner, lightConfig);
    }
    
    [CanBeNull]
    public static PlayerLight<T> Create(ReferenceHub hub, T lightConfig)
    {
        var Owner = Player.Get(hub);
        if (Owner == null)
            return null;
        
        if (Owner.GameObject == null || Owner.IsHost)
            return null;
        
        return new PlayerLight<T>(Owner, lightConfig);
    }

    public static bool Destroy(Player Owner)
    {
        if (!InternalList.TryGetFirst(f => f.Target == Owner, out var following))
            return false;
        
        following.Destroy();
        return true;
    }

    public static bool Destroy(ReferenceHub hub)
    {
        var Owner = Player.Get(hub);
        if (Owner == null)
            return false;
        
        if (!InternalList.TryGetFirst(f => f.Target == Owner, out var following))
            return false;
        
        following.Destroy();
        return true;
    }

    public static void DestroyAll()
    {
        for (int i = InternalList.Count - 1; i >= 0; i--)
            InternalList[i].Destroy();
    }
}

public abstract class PlayerLight(Player target, LightConfigBase lightConfig)
{
    private static readonly List<PlayerLight> _allInstances = new();
    public static IReadOnlyList<PlayerLight> List => _allInstances.AsReadOnly();

    protected static void Register(PlayerLight instance) => _allInstances.Add(instance);
    protected static void Unregister(PlayerLight instance) => _allInstances.Remove(instance);
    
    public static void DestroyAllGlobal()
    {
        for (int i = _allInstances.Count - 1; i >= 0; i--)
        {
            _allInstances[i].Destroy();
        }
    }
    
    public Player Target { get; protected set; } = target;
    public LightConfigBase LightConfig { get; protected set; } = lightConfig;
    public LightSourceToy Light => LightConfig.Light;
    
    public abstract void Destroy();
}