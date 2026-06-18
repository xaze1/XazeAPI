// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using LabApi.Events.CustomHandlers;

namespace XazeAPI.API.Events.Handler;

public abstract class XazeEventHandler : CustomEventsHandler
{
    public virtual void OnPlayerHearingFakePlayer(PlayerHearingFakePlayer args)
    {
    }
    
    public virtual void OnPlayerScaleChanging(PlayerScaleChanging args)
    {
    }
    
    public virtual void OnPlayerHurting(PlayerHurting args)
    {
    }
}