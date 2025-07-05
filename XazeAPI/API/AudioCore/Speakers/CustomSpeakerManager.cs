// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils.NonAllocLINQ;
using XazeAPI.API.AudioCore.Speakers.Models;

namespace XazeAPI.API.AudioCore.Speakers
{
    public class CustomSpeakerManager
    {
        public static HashSet<CustomSpeakerAudio> PooledSpeakers = new();

        public static CustomSpeakerAudio SpawnSpeaker(string speakerName, Transform target, bool isSpatial, string audioPath, string clipName = null, float volume = 1f)
        {
            string name = clipName;

            if (string.IsNullOrEmpty(clipName))
                name = Path.GetFileNameWithoutExtension(audioPath);

            if (!CheckIfClipExists(name))
            {
                AudioClipStorage.LoadClip(audioPath, name);
            }

            CustomSpeakerAudio speaker = null;
            if (PooledSpeakers.TryGetFirst(x => !x.IsInUse, out speaker))
            {
                speaker.IsInUse = true;
                speaker.IsSpatial = isSpatial;
                speaker.Volume = volume;
                speaker.Owner.transform.parent = target;
                speaker.transform.parent = target;
                speaker.transform.localPosition = Vector3.zero;

                return speaker;
            }

            AudioPlayer plr = AudioPlayer.CreateOrGet(speakerName, name, onIntialCreation: (p) =>
            {
                p.DestroyWhenAllClipsPlayed = false;
                speaker = p.GetOrAddSpeaker(speakerName, 5f, isSpatial, 0f, 30f);
                speaker.Owner = p;
                p.transform.parent = target;
                speaker.transform.parent = target;
                speaker.transform.localPosition = Vector3.zero;
                speaker.Volume = volume;
                PooledSpeakers.Add(speaker);
            });

            return speaker;
        }

        public static CustomSpeakerAudio SpawnSpeaker(string speakerName, Vector3 position, bool isSpatial, string audioPath, string clipName = null, float volume = 1f)
        {
            string name = clipName;

            if (string.IsNullOrEmpty(clipName))
                name = Path.GetFileNameWithoutExtension(audioPath);

            if (!CheckIfClipExists(name))
            {
                AudioClipStorage.LoadClip(audioPath, name);
            }

            CustomSpeakerAudio speaker = null;
            if (PooledSpeakers.TryGetFirst(x => !x.IsInUse, out speaker))
            {
                speaker.IsInUse = true;
                speaker.IsSpatial = isSpatial;
                speaker.Volume = volume;
                speaker.Position = position;
                return speaker;
            }

            AudioPlayer plr = AudioPlayer.CreateOrGet(speakerName, name, onIntialCreation: (p) =>
            {
                p.DestroyWhenAllClipsPlayed = false;
                speaker = p.AddSpeaker(speakerName, 5f, isSpatial, 0f, 30f);
                speaker.Volume = volume;
                PooledSpeakers.Add(speaker);
            });
            return speaker;
        }

        private static bool CheckIfClipExists(string clipName)
        {
            if (AudioClipStorage.AudioClips.ContainsKey(clipName))
            {
                return true;
            }

            return false;
        }
    }
}
