// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using JetBrains.Annotations;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Pools;
using UnityEngine;

namespace XazeAPI.API.AudioCore.Speakers;

public static class SpeakerManager
{
    public static void PlayShortClip(byte id, SpeakerSettings settings, [CanBeNull] Transform parent = null)
    {
        var audioPlayer = AudioPlayerPool.Rent(id, settings, parent);
        
    }
}