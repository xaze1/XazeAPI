// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using XazeAPI.API.Structures;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using SecretLabNAudio.Core.SendEngines;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;
using XazeAPI.API.Events;
using XazeAPI.API.Events.Handler;

namespace XazeAPI.API.AudioCore.FakePlayers;


public class CustomSendEngine(Player source, VoiceChatChannel channel)
    : VoiceMessageSendEngine(source, channel)
{

    public static event Action<Player, AudioMessage> BroadcastingSound; 
    public Player Owner { get; } = source;
    public HashSet<int> BroadcastTo { get; } = [];
    public FakePlayerCustomHearSoundCheck HearOverride { get; set; } = new();

    public override void Broadcast(AudioMessage message)
    {
        if (Channel != VoiceChatChannel.Proximity || Owner.IsAlive)
            base.Broadcast(message);
    }

    protected override bool Broadcast(Player player, AudioMessage message)
    {
        var hearingEvent = new PlayerHearingFakePlayer(player.ReferenceHub, Owner.ReferenceHub, FakeLoader.AudioPlayers[Owner.ReferenceHub]);
        XazeEvents.OnPlayerHearingFake(hearingEvent);
        if (!hearingEvent.IsAllowed)
        {
            return false;
        }

        if (!CanHear(player))
        {
            return false;
        }

        if (!BroadcastTo.IsEmpty() && !BroadcastTo.Contains(player.PlayerId))
        {
            return false;
        }

        if (HearOverride.IsSet && !HearOverride.PlayerCanHear(player))
        {
            return false;
        }
        
        BroadcastingSound?.Invoke(player, message);
        return base.Broadcast(player, message);
    }

    public bool CanHear(Player player)
    {
        if (player.IsAlive)
            return true;

        var spectated = player.CurrentlySpectating;
        if (spectated == null)
        {
            return false;
        }
        
        if (Vector3.Distance(Owner.Position, spectated.Position) > 30)
            return false;

        return true;
    }
}