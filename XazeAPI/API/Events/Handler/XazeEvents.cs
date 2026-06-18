// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using LabApi.Events;

namespace XazeAPI.API.Events.Handler;

public static class XazeEvents 
{
    public static event LabEventHandler<PlayerHearingFakePlayer> HearingFake;
    public static void OnPlayerHearingFake(PlayerHearingFakePlayer args)
    {
        HearingFake.InvokeEvent(args);
    }
    
    public static event LabEventHandler<PreventHitmarkerEvent> PreventHitmarker;
    public static void OnServerPreventHitmarker(PreventHitmarkerEvent args)
    {
        PreventHitmarker.InvokeEvent(args);
    }
    
    public static event LabEventHandler<PlayerScaleChanging> ScaleChanging;
    public static void OnPlayerScaleChanging(PlayerScaleChanging args)
    {
        ScaleChanging.InvokeEvent(args);
    }
    
    public static event LabEventHandler<PlayerHurting> Hurting;
    public static void OnPlayerHurting(PlayerHurting args)
    {
        Hurting.InvokeEvent(args);
    }
}