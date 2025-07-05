// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using JetBrains.Annotations;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using XazeAPI.API.AudioCore.FakePlayers;
using XazeAPI.API.Interfaces;

namespace XazeAPI.API.Events;

public class PlayerHearingFakePlayer(ReferenceHub target, ReferenceHub fakePlayer, CustomAudioPlayer audioPlayer)
    : EventArgs, IPlayerEvent, IFakeEvent, ICancellableEvent
{
    public Player? Player { get; } = Player.Get(target);
    public CustomAudioPlayer? AudioPlayer { get; } = audioPlayer;
    public ReferenceHub? FakePlayer { get; } = fakePlayer;
    public bool IsAllowed { get; set; } = true;
}