// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using LabApi.Events;

namespace XazeAPI.API.Events;

public static class XazeEvents 
{
    public static event LabEventHandler<PlayerHearingFakePlayer> HearingFake;
    public static void OnPlayerHearingFake(PlayerHearingFakePlayer hearingFakePlayer)
    {
        HearingFake.InvokeEvent(hearingFakePlayer);
    }
    
    public static event LabEventHandler<PreventHitmarkerEvent> PreventHitmarker;
    public static void OnServerPreventHitmarker(PreventHitmarkerEvent preventingHitmarker)
    {
        PreventHitmarker.InvokeEvent(preventingHitmarker);
    }
}