// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.IO;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;
using SecretLabNAudio.Core;
using UnityEngine;

namespace XazeAPI.API.AudioCore.Speakers;

public static class SpeakerManager
{
    public static SpeakerLoader PlayLocal(Player Target, string songName)
    {
        if (Target.GameObject == null)
        {
            return null;
        }
        
        var speaker = Target.GameObject.AddComponent<SpeakerLoader>();
        speaker.Play(Path.Combine(AudioManager.AudioPath, songName));

        return speaker;
    }
    
    public static SpeakerLoader PlayLocal(Vector3 pos, string songName)
    {
        var gameObject = new GameObject("XazeApi-Speaker-LocalSound")
        {
            transform =
            {
                position = pos
            }
        };

        var speaker = gameObject.AddComponent<SpeakerLoader>();
        speaker.Settings = SpeakerSettings.Default;
        speaker.Play(Path.Combine(AudioManager.AudioPath, songName));
        
        return speaker;
    }
    
    public static SpeakerLoader PlayGlobal(string songName)
    {
        var speaker = new GameObject().AddComponent<SpeakerLoader>();
        speaker.Play(Path.Combine(AudioManager.AudioPath, songName));

        return speaker;
    }
    
    public static void Play(SpeakerLoader speaker, string songName)
    {
        speaker.Play(Path.Combine(AudioManager.AudioPath, songName));
    }
}