// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using LabApi.Events.CustomHandlers;
using XazeAPI.API.Events;

namespace XazeAPI.API;

public abstract class XazeEventHandler : CustomEventsHandler
{
    public virtual void OnPlayerHearingFakePlayer(PlayerHearingFakePlayer hearingFakePlayer)
    {
    }
}