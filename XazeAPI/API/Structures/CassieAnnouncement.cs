// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using PlayerStatsSystem;
using Respawning;
using Subtitles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cassie;
using Utils.Networking;
using XazeAPI.API.Helpers;

namespace XazeAPI.API.Structures
{
    public struct CassieAnnouncement
    {
        public string Announcement;
        public string Translation;

        public SubtitlePart[] Subtitles;

        bool IsNoisy;
        public bool IsSet;

        public CassieAnnouncement()
        {
            IsSet = false;
        }

        public CassieAnnouncement(string announcemnt, string translation, bool isNoisy = true)
        {
            IsSet = true;
            Announcement = announcemnt;
            Translation = translation ?? announcemnt;

            IsNoisy = isNoisy;
        }

        public CassieAnnouncement(StringBuilder announcement, StringBuilder translation, bool isNoisy = true)
        {
            IsSet = true;
            Announcement = announcement.ToString();

            if (translation == null)
            {
                Translation = announcement.ToString();
            }
            else
            {
                Translation = translation.ToString();
            }

            IsNoisy = isNoisy;
        }

        public CassieAnnouncement(StringBuilder announcement, SubtitlePart[] subtitles = null, bool isNoisy = true)
        {
            IsSet = true;
            Announcement = announcement.ToString();

            if (subtitles == null)
            {
                Subtitles = DamageHandlerBase.CassieAnnouncement.Default.SubtitleParts;
            }
            else
            {
                Subtitles = subtitles;
            }

            IsNoisy = isNoisy;
        }

        public void PlayAnnouncement()
        {
            if (Subtitles != null)
            {
                new Cassie.CassieAnnouncement(new CassieTtsPayload(Announcement, IsNoisy, Subtitles)).AddToQueue();
                return;
            }

            MainHelper.MessageTranslated(Announcement, Translation, IsNoisy);
        }

        public void PlayGlitchyAnnouncement(float glitchChance, float jamChance)
        {
            string tts = Announcement;
            string[] array = tts.Split(' ');
            List<string> newWords = new();
            newWords.EnsureCapacity(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                newWords.Add(array[i]);
                if (i < array.Length - 1)
                {
                    if (UnityEngine.Random.value < glitchChance)
                    {
                        newWords.Add(".G" + UnityEngine.Random.Range(1, 7));
                    }

                    if (UnityEngine.Random.value < jamChance)
                    {
                        newWords.Add("JAM_" + UnityEngine.Random.Range(0, 70).ToString("000") + "_" + UnityEngine.Random.Range(2, 6));
                    }
                }
            }

            tts = newWords.Aggregate("", (current, newWord) => current + newWord + " ");

            if (Subtitles != null)
            {
                new Cassie.CassieAnnouncement(new CassieTtsPayload(tts, IsNoisy, Subtitles)).AddToQueue();
                return;
            }

            MainHelper.MessageTranslated(tts, Translation, IsNoisy);
        }

        public void PlayGlitchyAnnouncement()
        {
            float num = (AlphaWarheadController.Detonated ? 3.5f : 1f);
            PlayGlitchyAnnouncement(UnityEngine.Random.Range(0.1f, 0.14f) * num, UnityEngine.Random.Range(0.07f, 0.08f) * num);
        }
    }
}
