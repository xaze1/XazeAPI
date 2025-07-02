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