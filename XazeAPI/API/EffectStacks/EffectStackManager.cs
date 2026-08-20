// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using JetBrains.Annotations;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using PlayerRoles;
using UnityEngine;

namespace XazeAPI.API.EffectStacks;

using Extensions;

public class EffectStackManager : MonoBehaviour
{
    public static readonly List<Type> BlacklistedEffects = [
        typeof(Scp1853),
        typeof(Scp1576),
    ];
    
    [ThreadStatic]
    public static bool IsInternalCall;
    
    public static Dictionary<Player, EffectStackManager> List { get; } = new();
    
    private Player _owner;
    public Dictionary<Type, List<EffectStack>> Stacks { get; } = new();

    private void Awake()
    {
        _owner = Player.Get(gameObject);
        if (_owner == null)
            return;
        
        PlayerEvents.ChangedRole += OnRoleChanged;
        List[_owner] = this;
    }

    private void OnDestroy()
    {
        if (_owner == null)
            return;
        
        PlayerEvents.ChangedRole -= OnRoleChanged;
        List.Remove(_owner);
    }

    private void Update()
    {
        if (_owner == null)
            return;

        var empty = ListPool<Type>.Shared.Rent();
        foreach (var pair in Stacks)
        {
            var stacks = pair.Value;
            for (int i = stacks.Count - 1; i >= 0; i--)
            {
                var stack = stacks[i];
                stack.RefreshTime(Time.deltaTime);
                
                if (stack.Duration == 0 || stack.TimeLeft > 0 || !stack.CanBeRemoved)
                    continue;
                stacks.RemoveAt(i);
            }
            
            UpdateIntensity(pair.Key, stacks);
            if (stacks.Count > 0)
                continue;
            empty.Add(pair.Key);
        }

        // Remove empty Keys
        if (empty.Count > 0)
        {
            empty.Do(e => Stacks.Remove(e));
        }
        
        ListPool<Type>.Shared.Return(empty);
    }

    private void OnRoleChanged(PlayerChangedRoleEventArgs args)
    {
        if (_owner == null || args.Player != _owner)
            return;
        
        if (args.OldRole == RoleTypeId.None || args.OldRole.GetTeam() == Team.Dead || args.NewRole.Team != Team.Dead)
            return;
        
        Stacks.Clear();
        _owner.DisableAllEffects();
    }

    private void UpdateIntensity(Type effectType, List<EffectStack> stacks)
    {
        if (_owner == null || !_owner.TryGetEffect(effectType, out var effect))
            return;

        byte intensity = 0;
        stacks.Sort((a, b) => a.MaxIntensity.CompareTo(b.MaxIntensity));
        foreach (var stack in stacks)
        {
            if (stack.IsActive)
                intensity = (byte)Mathf.Clamp(intensity + stack.Intensity, 0, Mathf.Min(stack.MaxIntensity, effect.MaxIntensity));
        }

        if (effect.Intensity == intensity)
            return;

        try
        {
            IsInternalCall = true;
            effect.ServerSetState(intensity);
        }
        finally
        {
            IsInternalCall = false;
        }
    }

    public void AddStack<T>(byte intensity, float duration) where T : StatusEffectBase
    {
        if (_owner == null)
            return;

        AddStack<T>(new EffectStack
        {
            Intensity =  intensity, 
            Duration = duration
        });
    }

    public void AddStack<T>(EffectStack stack) where T : StatusEffectBase
    {
        if (_owner == null)
            return;

        AddStack(typeof(T), stack);
    }

    public void AddStack(Type effectType, EffectStack stack)
    {
        if (_owner == null || BlacklistedEffects.Contains(effectType))
            return;

        if (typeof(CokeBase).IsAssignableFrom(effectType))
        {
            if (Stacks.Keys.Any(t => t != effectType && typeof(CokeBase).IsAssignableFrom(t)))
            {
                try
                {
                    IsInternalCall = true;
                    _owner.EnableEffect(effectType, stack.Intensity, stack.Duration);
                }
                finally
                {
                    IsInternalCall = false;
                }
                return;
            }
        }

        if (!Stacks.TryGetValue(effectType, out var stacks))
        {
            stacks = new();
            Stacks.Add(effectType, stacks);
        }
        
        stacks.Add(stack);
        UpdateIntensity(effectType, stacks);
    }
    
    public bool RemoveStack<T>(EffectStack stack) where T : StatusEffectBase
    {
        if (_owner == null)
            return false;

        return RemoveStack(typeof(T), stack);
    }
    
    public bool RemoveStack(Type effectType, EffectStack stack)
    {
        if (_owner == null || !stack.CanBeRemoved)
            return false;
        
        if (!Stacks.TryGetValue(effectType, out var stacks))
            return false;
        
        var outcome = stacks.Remove(stack);
        if (outcome)
            UpdateIntensity(effectType, stacks);
        return outcome;
    }
    
    public bool RemoveStacks<T>() where T : StatusEffectBase
    {
        if (_owner == null)
            return false;

        return RemoveStacks(typeof(T));
    }
    
    public bool RemoveStacks(Type effectType)
    {
        if (_owner == null)
            return false;

        if (Stacks.TryGetValue(effectType, out var stacks))
        {
            var hasLockedStacks = false;
            for (int i = stacks.Count - 1; i >= 0; i--)
            {
                if (stacks[i].CanBeRemoved)
                    stacks.RemoveAt(i);
                else
                    hasLockedStacks = true;
            }

            if (hasLockedStacks)
            {
                UpdateIntensity(effectType, stacks);
                return false;
            }
        }
        
        var removedStacks = Stacks.Remove(effectType);
        if (!_owner.TryGetEffect(effectType, out var effect) || !effect.IsEnabled) 
            return removedStacks;
        
        try
        {
            IsInternalCall = true;
            effect.ServerDisable();
        }
        finally
        {
            IsInternalCall = false;
        }
        return true;
    }
    
    public void RemoveStacks()
    {
        if (_owner == null)
            return;

        var keys = ListPool<Type>.Shared.Rent(Stacks.Keys);
        try
        {
            foreach (var key in keys)
                RemoveStacks(key);
        }
        finally
        {
            ListPool<Type>.Shared.Return(keys);
        }
    }

    public static bool TryGet([CanBeNull] Player plr, out EffectStackManager manager)
    {
        if (plr != null)
            return List.TryGetValue(plr, out manager);
        
        manager = null;
        return false;

    }

    [CanBeNull]
    public static EffectStackManager Get([CanBeNull] Player plr)
    {
        if (plr == null)
            return null;
        
        return List.TryGetValue(plr, out var manager)? manager : null;
    }

    public static bool TryGet([CanBeNull] ReferenceHub hub, out EffectStackManager manager) => TryGet(Player.Get(hub), out manager);

    [CanBeNull]
    public static EffectStackManager Get(ReferenceHub hub) => Get(Player.Get(hub));
}