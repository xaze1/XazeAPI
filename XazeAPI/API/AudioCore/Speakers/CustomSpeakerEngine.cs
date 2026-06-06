// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.SendEngines;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;
using XazeAPI.API.AudioCore.FakePlayers;
using XazeAPI.API.Events;
using XazeAPI.API.Structures;

namespace XazeAPI.API.AudioCore.Speakers;

public class CustomSpeakerEngine : SendEngine
{
    public static event Action<Player, AudioMessage> BroadcastingSound; 
    public HashSet<int> BroadcastTo { get; } = [];
    public FakePlayerCustomHearSoundCheck HearOverride { get; set; } = new();
    public Predicate<Player> Filter { get; set; } = _ => true;
    
    protected override bool Broadcast(Player player, AudioMessage message)
    {
        if (!Filter.Invoke(player))
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
}