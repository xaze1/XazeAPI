// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using CommandSystem;
using CustomPlayerEffects;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using UnityEngine;
using XazeAPI.API.EffectStacks;
using XazeAPI.API.Extensions;
using XazeAPI.API.Helpers;
using XazeAPI.API.Stats;

namespace XazeAPI.Features;

public class XazePlayer
{
    public static event Action<XazePlayer> OnCreation;
    
    public static Dictionary<string, XazePlayer> Dictionary { get; } = new();
    public static List<XazePlayer> List { get; } = new();

    public Player Player { get; private init; }
    [CanBeNull] public ReferenceHub ReferenceHub => Player.ReferenceHub;
    [CanBeNull] public GameObject GameObject => Player.GameObject;
    public EffectStackManager EffectStacks => EffectStackManager.TryGet(Player, out var manager) ? manager : GameObject?.AddComponent<EffectStackManager>();

    public string Username
    {
        get => Player.DisplayName;
        set => Player.DisplayName = value;
    }

    public string CustomInfo
    {
        get => Player.CustomInfo;
        set => Player.CustomInfo = value;
    }
    
    public string UserId => Player.UserId;
    public bool IsDestroyed => Player.IsDestroyed;
    public bool IsPlayer => Player.IsPlayer;

    public uint Kills
    {
        get => (uint)(GetStat<PlayerKillStat>()?.Value ?? 0);
        set
        {
            var kills = GetStat<PlayerKillStat>();
            if (kills == null)
                return;
            kills.Value = (int)value;
            kills.LastTeam = Player.Team;
        }
    }
    public uint Deaths
    {
        get => (uint)(GetStat<PlayerDeathStat>()?.Value ?? 0);
        set
        {
            var deaths = GetStat<PlayerDeathStat>();
            if (deaths == null)
                return;
            deaths.Value = (int)value;
            deaths.LastTeam = Player.Team;
        }
    }
    public RoleTypeId LastRole { get; internal set; } = RoleTypeId.None;

    public RoleTypeId Role
    {
        get => Player.Role;
        set => Player.SetRole(value);
    }
    
    public int UnitId
    {
        get => Player.UnitId;
        set
        {
            if (Player.RoleBase is not HumanRole { UsesUnitNames: true } human)
                return;
            human.UnitNameId = (byte)value;
        }
    }
    
    public bool IsDisguised => Disguise != RoleTypeId.None;
    public RoleTypeId Disguise
    {
        get => DisguiseHelper.TryGetDisguise(Player, out var disguise) ? disguise.Disguise : RoleTypeId.None;
        set
        {
            if (value == Disguise)
                return;

            if (value == RoleTypeId.None)
            {
                Player.ResetAppearance();
                return;
            }
            Player.ChangeAppearance(value);
        }
    }
    
    public int ScpChance
    {
        get
        {
            using ScpTicketsLoader scpTicketsLoader = new ScpTicketsLoader();
            return scpTicketsLoader.GetTickets(ReferenceHub, ScpPlayerPicker.DefaultTickets);
        }
        set
        {
            using ScpTicketsLoader scpTickets = new ScpTicketsLoader();
            scpTickets.ModifyTickets(ReferenceHub, value);
        }
    }
    
    public PlayerBaseStat[] Stats { get; private set; }
    private Dictionary<Type, object> CustomData { get; } = new();
    
    public Dictionary<RoleTypeId, int> GetSCPPreferences()
    {
        return ScpSpawnPreferences.Preferences.TryGetValue(Player.Connection.connectionId, out var preferences)? preferences.Preferences : [];
    }

    public int GetCombinedPreference(RoleTypeId scp, List<RoleTypeId> otherScps = null)
    {
        return ScpSpawner.GetCombinedPreferencePoints(ReferenceHub, scp, otherScps?? []);
    }

    public bool CanDamage(XazePlayer xPlr) => CanDamage(xPlr.Player);
    public bool CanDamage(Player plr)
    {
        if (HitboxIdentity.IsDamageable(ReferenceHub, plr.ReferenceHub)) 
            return false;

        return true;
    }

    [CanBeNull]
    public T GetStat<T>() where T : PlayerBaseStat
    {
        if (!Stats.TryGetFirst(s => s is T, out var stat))
            return null;
        return (T)stat;
    }

    public T AddData<T>() where T : class
    {
        var data = Activator.CreateInstance<T>();
        CustomData.Add(typeof(T), data);
        return data;
    }

    [CanBeNull]
    public T GetData<T>() where T : class
    {
        if (!CustomData.TryGetValue(typeof(T), out var data))
            return null;
        return (T)data;
    }

    public bool TryGetData<T>(out T data) where T : class
    {
        data = null;
        if (!CustomData.TryGetValue(typeof(T), out var smth))
            return false;
        data = (T)smth;
        return true;
    }

    public void AddEffect(Type effectType, EffectStack stack) => EffectStacks.AddStack(effectType, stack);
    public void AddEffect<T>(EffectStack stack) where T : StatusEffectBase => EffectStacks.AddStack<T>(stack);
    public void AddEffect<T>(int intensity, float duration = 0.0f) where T : StatusEffectBase => EffectStacks.AddStack<T>(new EffectStack{ Intensity = intensity, Duration =  duration });
    public void AddEffect<T>(Func<int> intensityCalc, float duration = 0.0f) where T : StatusEffectBase => EffectStacks.AddStack<T>(new EffectStack(intensityCalc) { Duration =  duration });

    public void RemoveEffect(Type effectType, EffectStack stack) => EffectStacks.RemoveStack(effectType, stack);
    public void RemoveEffect<T>(EffectStack stack) where T : StatusEffectBase => EffectStacks.RemoveStack<T>(stack);
    public void RemoveEffect(Type effectType) => EffectStacks.RemoveStacks(effectType);
    public void RemoveEffect<T>() where T : StatusEffectBase => EffectStacks.RemoveStacks<T>();
    public void RemoveEffects() => EffectStacks.RemoveStacks();

    private static XazePlayer CreateWrapper(Player plr)
    {
        var xPlayer = new XazePlayer
        {
            Player = plr,
            Stats = [
                PlayerKillStat.Get(plr),
                PlayerDeathStat.Get(plr)
            ]
        };

        OnCreation.InvokeSafely(xPlayer);
        
        List.Add(xPlayer);
        Dictionary.Add(plr.UserId, xPlayer);
        return xPlayer;
    }

    [CanBeNull] public static XazePlayer Get(ReferenceHub hub) => Get(Player.Get(hub));
    [CanBeNull]
    public static XazePlayer Get(Player plr)
    {
        if (plr == null || plr.IsDestroyed || plr.IsHost)
            return null;
        
        if (Dictionary.TryGetValue(plr.UserId, out var xPlayer))
            return xPlayer;

        return CreateWrapper(plr);
    }

    public static bool TryGet(ReferenceHub hub, out XazePlayer xPlayer) => TryGet(Player.Get(hub), out xPlayer);
    public static bool TryGet(Player plr, out XazePlayer xPlayer)
    {
        xPlayer = null;
        if (plr.IsDestroyed || plr.IsHost)
            return false;
        
        if (!Dictionary.TryGetValue(plr.UserId, out var x))
        {
            x = CreateWrapper(plr);
        }
        
        xPlayer = x;
        return true;
    }

    public static bool TryGet(ICommandSender sender, out XazePlayer xPlayer)
    {
        xPlayer = null;
        if (!Player.TryGet(sender, out var plr))
            return false;

        return TryGet(plr, out xPlayer);
    }
}