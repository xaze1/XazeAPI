// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using UnityEngine;
using XazeAPI.API.EffectStacks;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.AoEs;

public class StatusEffectAerial<T> : AerialEffect where T : StatusEffectBase
{
    private readonly string _guid;
    public float Duration { get; set; }

    public int Intensity
    {
        get => _stack.Intensity;
        set => _stack.Intensity = value;
    }
    
    private readonly EffectStack _stack;
    private readonly Type StatusEffect;

    public override bool OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.TryGetEffectStack(StatusEffect, _guid, out var stack))
        {
            stack.Duration = 0;
            return true;
        }
        player.AddEffect(StatusEffect, _stack.Clone());
        return true;
    }

    public override bool OnExit(Player player)
    {
        base.OnExit(player);
        var stack = player.GetEffectStack(StatusEffect, _guid);
        stack?.Duration = Duration;
        return true;
    }

    public StatusEffectAerial(Vector3 sourcePos, int intensity, float duration, byte maxIntensity = 255) : base(sourcePos)
    {
        _guid = Guid.NewGuid().ToString();
        StatusEffect = typeof(T);
        _stack = new EffectStack(_guid) { Intensity = intensity, MaxIntensity = maxIntensity };
        Duration = duration;
    }
}