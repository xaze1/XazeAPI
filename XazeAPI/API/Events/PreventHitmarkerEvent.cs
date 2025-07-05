// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;

namespace XazeAPI.API.Events;

public class PreventHitmarkerEvent(AttackerDamageHandler damageHandler, ReferenceHub victim)
    : EventArgs, ITargetEvent, ICancellableEvent
{
    public Player Target { get; } = Player.Get(victim);
    public bool IsAllowed { get; set; } = true;
    public AttackerDamageHandler DamageHandler { get; } = damageHandler;
}