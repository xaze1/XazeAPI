using PlayerStatsSystem;
using Respawning;
using Subtitles;
using System.Collections.Generic;
using System.Text;
using Utils.Networking;
using XazeAPI.API.Helpers;

namespace XazeAPI.API.Structures
{
    public struct CassieAnnouncement
    {
        public string Announcement;
        public string Translation;

        public SubtitlePart[] Subtitles;

        bool IsHeld;
        bool IsNoisy;
        bool IsSubtitles;
        public bool IsSet;

        public CassieAnnouncement()
        {
            IsSet = false;
        }

        public CassieAnnouncement(string announcemnt, string translation, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
        {
            IsSet = true;
            Announcement = announcemnt;
            Translation = translation ?? announcemnt;

            IsHeld = isHeld;
            IsNoisy = isNoisy;
            IsSubtitles = isSubtitles;
        }

        public CassieAnnouncement(StringBuilder announcement, StringBuilder translation, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
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

            IsHeld = isHeld;
            IsNoisy = isNoisy;
            IsSubtitles = isSubtitles;
        }

        public CassieAnnouncement(StringBuilder announcement, SubtitlePart[] subtitles = null, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
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

            IsHeld = isHeld;
            IsNoisy = isNoisy;
            IsSubtitles = isSubtitles;
        }

        public void PlayAnnouncement()
        {
            if (Subtitles != null)
            {
                RespawnEffectsController.PlayCassieAnnouncement(Announcement, IsHeld, IsNoisy, false);
                new SubtitleMessage(Subtitles).SendToAuthenticated();
                return;
            }

            MainHelper.MessageTranslated(Announcement, Translation, IsHeld, IsNoisy, IsSubtitles);
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

            tts = "";
            foreach (string newWord in newWords)
            {
                tts = tts + newWord + " ";
            }

            if (Subtitles != null)
            {
                RespawnEffectsController.PlayCassieAnnouncement(tts, IsHeld, IsNoisy, false);
                new SubtitleMessage(Subtitles).SendToAuthenticated();
                return;
            }

            MainHelper.MessageTranslated(tts, Translation, IsHeld, IsNoisy, IsSubtitles);
        }

        public void PlayGlitchyAnnouncement()
        {
            float num = (AlphaWarheadController.Detonated ? 3.5f : 1f);
            PlayGlitchyAnnouncement(UnityEngine.Random.Range(0.1f, 0.14f) * num, UnityEngine.Random.Range(0.07f, 0.08f) * num);
        }
    }
}
