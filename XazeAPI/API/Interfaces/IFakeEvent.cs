// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using XazeAPI.API.AudioCore.FakePlayers;

namespace XazeAPI.API.Interfaces;

public interface IFakeEvent
{
    CustomAudioPlayer? AudioPlayer { get; }
    ReferenceHub? FakePlayer { get; }
}