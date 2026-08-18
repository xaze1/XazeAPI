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
        
        if (empty.Count <= 0)
            return;
        
        // Remove empty Keys
        empty.Do(e => Stacks.Remove(e));
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
        foreach (var stack in stacks.OrderBy(s => s.MaxIntensity))
        {
            if (stack.IsActive)
                intensity = (byte)Mathf.Clamp(intensity + stack.Intensity, 0, Mathf.Min(stack.MaxIntensity, effect.MaxIntensity));
        }

        if (effect.Intensity == intensity)
            return;
        
        if (intensity == 0)
        {
            effect.ServerDisable();
            return;
        }
        
        effect.ServerSetState(intensity);
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
        if (_owner == null)
            return;

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
        
        if (Stacks.TryGetValue(effectType, out var stacks) && stacks.Any(s => !s.CanBeRemoved))
        {
            for (int i = stacks.Count - 1; i >= 0; i--)
            {
                var stack = stacks[i];
                if (!stack.CanBeRemoved)
                    continue;
                stacks.RemoveAt(i);
            }

            return false;
        }
        
        var removedStacks = Stacks.Remove(effectType);
        if (!_owner.TryGetEffect(effectType, out var effect) || !effect.IsEnabled) 
            return removedStacks;
        
        effect.ServerDisable();
        return true;
    }
    
    public void RemoveStacks()
    {
        if (_owner == null)
            return;

        foreach (var kv in Stacks.ToList())
        {
            var effectType = kv.Key;
            var stacks = kv.Value;
            
            if (stacks.Any(s => !s.CanBeRemoved))
            {
                for (int i = stacks.Count - 1; i >= 0; i--)
                {
                    var stack = stacks[i];
                    if (!stack.CanBeRemoved)
                        continue;
                    stacks.RemoveAt(i);
                }
                
                continue;
            }
            
            Stacks.Remove(effectType);
            if (!_owner.TryGetEffect(effectType, out var effect) || !effect.IsEnabled) 
                continue;
        
            effect.ServerDisable();
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