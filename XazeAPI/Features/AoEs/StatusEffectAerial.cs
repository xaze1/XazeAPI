// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using XazeAPI.API.EffectStacks;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.AoEs;

public class StatusEffectAerial<T> : AerialEffect where T : StatusEffectBase
{
    private readonly string _guid;
    public float Duration { get; set; }
    public bool CanStack { get; set; } = false;
    public double StackCooldown { get; set; } = 1.0;
    public int StackIntensity { get; set; } = 1;

    public int Intensity
    {
        get => _stack.Intensity;
        set => _stack.Intensity = value;
    }
    
    private readonly EffectStack _stack;
    private readonly Dictionary<Player, double> _lastStacked = new();

    public override bool OnEnter(Player player)
    {
        base.OnEnter(player);
        _lastStacked[player] = NetworkTime.time;
        if (player.TryGetEffectStack<T>(_guid, out var stack))
        {
            stack.Duration = 0;
            return true;
        }
        player.AddEffect<T>(_stack.Clone());
        return true;
    }

    public override bool OnStay(Player player)
    {
        if (!CanStack)
            return false;

        if (_lastStacked[player] + StackCooldown >= NetworkTime.time)
            return false;
        
        _lastStacked[player] = NetworkTime.time;
        if (player.TryGetEffectStack<T>(_guid, out var stack))
        {
            stack.Intensity += StackIntensity;
            return true;
        }
        player.AddEffect<T>(_stack.Clone());
        return true;
    }

    public override bool OnExit(Player player)
    {
        base.OnExit(player);
        var stack = player.GetEffectStack<T>(_guid);
        stack?.Duration = Duration;
        return true;
    }

    public StatusEffectAerial(Vector3 sourcePos, float duration, int intensity = 1, byte maxIntensity = 255) : base(sourcePos)
    {
        _guid = Guid.NewGuid().ToString();
        _stack = new EffectStack(_guid) { Intensity = intensity, MaxIntensity = maxIntensity };
        Duration = duration;
    }
}