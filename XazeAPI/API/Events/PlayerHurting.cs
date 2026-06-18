// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using JetBrains.Annotations;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;

namespace XazeAPI.API.Events;

public class PlayerHurting(ReferenceHub attacker, ReferenceHub target, DamageHandlerBase handler) : EventArgs, IPlayerEvent
{
    public Player Player { get; } = Player.Get(target);
    [CanBeNull] public Player Attacker { get; } = Player.Get(attacker);
    public StandardDamageHandler DamageHandler { get; set; } = handler as StandardDamageHandler;
}