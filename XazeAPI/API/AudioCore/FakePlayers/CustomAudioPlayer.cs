// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CentralAuth;
using MEC;
using NVorbis;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using SCPSLAudioApi.AudioCore;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;
using Random = UnityEngine.Random;
using LabApi.Features.Wrappers;
using UnityEngine.Networking;
using XazeAPI.API.Events;
using XazeAPI.API.Extensions;
using XazeAPI.API.Structures;

namespace XazeAPI.API.AudioCore.FakePlayers
{

    public class CustomAudioPlayer: AudioPlayerBase
    {
        public static CustomAudioPlayer Instance { get; set; }
        public TimeSpan CurrentTimePosition { get; private set; }
        public VorbisReader CurrentAudioReader { get; private set; }
        public FakePlayerCustomHearSoundCheck HearOverride { get; set; } = new();
        public Player Target => Player.Get(TargetHub);
        public ReferenceHub TargetHub;

#pragma warning disable CS0108
        public delegate void TrackFinished(CustomAudioPlayer playerBase, string track, bool directPlay);

        public static event TrackSelecting OnTrackSelecting;
        public static event TrackSelected OnTrackSelected;
        public static event TrackLoaded OnTrackLoaded;
        public static event TrackFinished OnFinishedTrack;

        public static CustomAudioPlayer Get(ReferenceHub hub)
        {
            if (AudioPlayers.TryGetValue(hub, out AudioPlayerBase player))
            {
                if (player is CustomAudioPlayer cplayer1)
                    return cplayer1;
            }

            var cplayer = hub.gameObject.AddComponent<CustomAudioPlayer>();
            cplayer.Owner = hub;
            cplayer.BroadcastChannel = VoiceChatChannel.Proximity;

            AudioPlayers.Add(hub, cplayer);
            return cplayer;
        }
#pragma warning restore CS0108

        public override void Update()
        {
            if (Owner == null || !ready || StreamBuffer.Count == 0 || !ShouldPlay)
            {
                return;
            }

            allowedSamples += Time.deltaTime * (float)samplesPerSecond;
            int num = Mathf.Min(Mathf.FloorToInt(allowedSamples), StreamBuffer.Count);

            Logging.Debug($"1 {num} {allowedSamples} {samplesPerSecond} {StreamBuffer.Count} {PlaybackBuffer.Length} {PlaybackBuffer.WriteHead}", LogDebug);

            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    PlaybackBuffer.Write(StreamBuffer.Dequeue() * (Volume / 100f));
                }
            }

            Logging.Debug($"2 {num} {allowedSamples} {samplesPerSecond} {StreamBuffer.Count} {PlaybackBuffer.Length} {PlaybackBuffer.WriteHead}", LogDebug);

            allowedSamples -= num;
            while (PlaybackBuffer.Length >= 480)
            {
                PlaybackBuffer.ReadTo(SendBuffer, 480L, 0L);
                int dataLen = Encoder.Encode(SendBuffer, EncodedBuffer);
                VoiceChatChannel broadcastVc = BroadcastChannel;
                if (HearOverride.IsSet && HearOverride.VoicechatOverride != VoiceChatChannel.None)
                {
                    broadcastVc = HearOverride.VoicechatOverride;
                }

                if (broadcastVc == VoiceChatChannel.Proximity && !Owner.IsAlive())
                {
                    continue;
                }

                foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                {
                    var hearingEvent = new PlayerHearingFakePlayer(allHub, this.Owner, this);
                    XazeEvents.OnPlayerHearingFake(hearingEvent);
                    if (!hearingEvent.IsAllowed)
                    {
                        continue;
                    }
                    
                    if (allHub.connectionToClient == null || !PlayerIsConnected(allHub))
                    {
                        continue;
                    }

                    if (!HearOverride.IsSet && CanHearSound(allHub) && (BroadcastTo.Count < 1 || BroadcastTo.Contains(allHub.PlayerId)) || HearOverride.PlayerCanHear(allHub))
                    {
                        allHub.connectionToClient.Send(new VoiceMessage(Owner, broadcastVc, EncodedBuffer, dataLen, isNull: false));
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (TargetHub == null || !Owner.IsAlive() || !Target.IsAlive)
            {
                return;
            }

            Owner.TryOverridePosition(Target.Position);
        }

        private bool PlayerIsConnected(ReferenceHub hub)
        {
            if (hub.authManager.InstanceMode == ClientInstanceMode.ReadyClient && hub.nicknameSync.NickSet && !hub.isLocalPlayer && !string.IsNullOrEmpty(hub.authManager.UserId))
            {
                return !hub.authManager.UserId.Contains("Dummy");
            }

            return false;
        }

        private bool CanHearSound(ReferenceHub hub)
        {
            if (BroadcastChannel == VoiceChatChannel.Proximity && !Owner.IsAlive())
            {
                return false;
            }

            if (hub.roleManager.CurrentRole is not SpectatorRole || BroadcastChannel != VoiceChatChannel.Proximity)
            {
                return true;
            }

            bool isSpectatingNearby = false;

            Player.List.ForEach(x =>
            {
                if (isSpectatingNearby)
                {
                    return;
                }

                if (!x.IsAlive)
                {
                    return;
                }

                if (Vector3.Distance(Owner.GetPosition(), x.Position) > 30)
                {
                    return;
                }

                if (!x.ReferenceHub.IsSpectatedBy(hub))
                {
                    return;
                }

                isSpectatingNearby = true;
            });

            return isSpectatingNearby;
        }

        public override IEnumerator<float> Playback(int index)
        {
            IsFinished = false;
            if (Shuffle)
                AudioToPlay = AudioToPlay.OrderBy(i => Random.value).ToList();

            OnTrackSelecting?.Invoke(this, index == -1, ref index);

            CurrentPlay = AudioToPlay[index];
            AudioToPlay.RemoveAt(index);
            if (Loop)
            {
                AudioToPlay.Add(CurrentPlay);
            }

            OnTrackSelected?.Invoke(this, index == -1, index, ref CurrentPlay);

            // Logging.Debug($"Loading Audio");

            if (AllowUrl && Uri.TryCreate(CurrentPlay, UriKind.Absolute, out var _))
            {
                UnityWebRequest www = new UnityWebRequest(CurrentPlay, "GET");
                DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
                www.downloadHandler = (DownloadHandler)(object)downloadHandler;
                yield return Timing.WaitUntilDone((AsyncOperation)(object)www.SendWebRequest());
                if (www.responseCode != 200)
                {
                    Logging.Error($"Failed to retrieve audio {www.responseCode} {www.downloadHandler.text}");
                    if (Continue && AudioToPlay.Count >= 1)
                    {
                        yield return Timing.WaitForSeconds(1f);
                        if (AudioToPlay.Count >= 1)
                        {
                            Timing.RunCoroutine(Playback(0));
                        }
                    }

                    yield break;
                }

                CurrentPlayStream = new MemoryStream(www.downloadHandler.data);
            }
            else
            {
                try
                {
                    string path = AudioManager.AudioPath;
                    CurrentPlayStream = new MemoryStream();
                    using var file = File.OpenRead(Path.Combine(path, CurrentPlay));
                    file.CopyTo(CurrentPlayStream);
                    CurrentPlayStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception e)
                {
                    Logging.Error($"{e}");
                }
            }
            

            OnTrackLoaded?.Invoke(this, index == -1, index, CurrentPlay);

            CurrentAudioReader = VorbisReader = new NVorbis.VorbisReader(CurrentPlayStream);
            // Logging.Debug($"Playing with samplerate of {VorbisReader.SampleRate}");
            Logging.Debug($"Loaded audio " + CurrentPlay, LogDebug);
            samplesPerSecond = VoiceChatSettings.SampleRate * VoiceChatSettings.Channels;
            //_samplesPerSecond = VorbisReader.Channels * VorbisReader.SampleRate / 5;
            SendBuffer = new float[samplesPerSecond / 5 + HeadSamples];
            ReadBuffer = new float[samplesPerSecond / 5 + HeadSamples];

            int cnt;
            while ((cnt = VorbisReader.ReadSamples(ReadBuffer, 0, ReadBuffer.Length)) > 0)
            {
                CurrentTimePosition = VorbisReader.TimePosition;
                if (stopTrack)
                {
                    VorbisReader.SeekTo(VorbisReader.TotalSamples - 1);
                    stopTrack = false;
                }
                while (!ShouldPlay)
                {
                    yield return Timing.WaitForOneFrame;
                }
                while (StreamBuffer.Count >= ReadBuffer.Length)
                {
                    ready = true;
                    yield return Timing.WaitForOneFrame;
                }
                for (int i = 0; i < ReadBuffer.Length; i++)
                {
                    StreamBuffer.Enqueue(ReadBuffer[i]);
                }
            }
            // Logging.Debug($"Track Complete.");
            OnFinishedTrack?.Invoke(this, CurrentPlay, index == -1);

            if (Continue && AudioToPlay.Count >= 1)
            {
                Timing.RunCoroutine(Playback(0));
            }
            IsFinished = true;
            yield break;
        }

        public override void OnDestroy()
        {
            if (PlaybackCoroutine.IsRunning)
            {
                Timing.KillCoroutines(PlaybackCoroutine);
            }

            VorbisReader.Dispose();
            Encoder.Dispose();
            base.OnDestroy();
        }
    }
}
