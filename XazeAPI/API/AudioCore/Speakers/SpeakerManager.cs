// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.IO;
using System.Linq;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;
using SecretLabNAudio.Core;
using UnityEngine;

namespace XazeAPI.API.AudioCore.Speakers;

public static class SpeakerManager
{
    public static SpeakerLoader PlayLocal(Player Target, string songName, params string[] subpath)
    {
        if (Target.GameObject == null)
        {
            return null;
        }
        
        var speaker = Target.GameObject.AddComponent<SpeakerLoader>();
        Play(speaker, songName, subpath);

        return speaker;
    }
    
    public static SpeakerLoader PlayLocal(Vector3 pos, string songName, params string[] subpath)
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
        Play(speaker, songName, subpath);
        
        return speaker;
    }
    
    public static SpeakerLoader PlayGlobal(string songName, params string[] subpath)
    {
        var speaker = new GameObject().AddComponent<SpeakerLoader>();
        Play(speaker, songName, subpath);

        return speaker;
    }
    
    public static void Play(SpeakerLoader speaker, string songName, params string[] subpath)
    {
        speaker.Play(Path.Combine(
            new[] { AudioManager.AudioPath }
                .Concat(subpath)
                .Append(songName)
                .ToArray()
            )
        );
    }
}