// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.IO;
using LabApi.Features.Wrappers;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using UnityEngine;
using VoiceChat;
using XazeAPI.API.Helpers;
using XazeAPI.API.Stats;
using XazeAPI.API.Structures;

namespace XazeAPI.API.AudioCore.FakePlayers;

// It's a public "FUCK YOU SEALED CLASS"
public class FakeLoader : MonoBehaviour
{
    public delegate void TrackFinished(FakeLoader fakeLoader, string track);
    
    public static Dictionary<ReferenceHub, FakeLoader> AudioPlayers = new();
    public static event Action<FakeLoader> OnTrackSelecting;
    public static event Action<FakeLoader> OnTrackSelected;
    public static event TrackFinished OnFinishedTrack;
    
    public Player Target;
    public Player Dummy => Player.Get(_hub);
    public CustomSendEngine SendEngine { private set; get; }
    public TimeSpan CurrentTimePosition => _audioPlayer.CurrentTime;
    public bool IsPlaying => !_audioPlayer.HasEnded;
    public bool IsFinished => _audioPlayer.HasEnded;
    public VoiceChatChannel Channel
    {
        get => SendEngine.Channel;
        set => SendEngine.Channel = value;
    }

    public FakePlayerCustomHearSoundCheck HearOverride
    {
        get => SendEngine.HearOverride;
        set => SendEngine.HearOverride = value;
    }
    
    public double VolumePercentage
    {
        get => _audioPlayer.MasterAmplification * 100;
        set => _audioPlayer.MasterAmplification = (float)value / 100;
    }
    
    private AudioPlayer _audioPlayer;
    private ReferenceHub _hub;
    private string _trackName;

    public void Play(string filePath)
    {
        try
        {
            OnTrackSelecting?.Invoke(this);
            _trackName = filePath;
            _audioPlayer.UseFile(filePath);
            OnTrackSelected?.Invoke(this);
        }
        catch (FileNotFoundException)
        {
            Logging.Warn("Missing Audio: " + filePath);
            PluginStatistics.ExceptionCaught(false);
        }
        catch (Exception ex)
        {
            ErrorHelper.ErrorLogStyling(ex);
            PluginStatistics.ExceptionCaught(false);
        }
    }

    public void Stop()
    {
        _audioPlayer.WithoutProvider();
    }

    public void SetTarget(ReferenceHub target)
    {
        Target = Player.Get(target);
    }

    public void SetChannel(VoiceChatChannel channel)
    {
        SendEngine.Channel = channel;
    }

    public void SetVolume(double volume)
    {
        _audioPlayer.WithMasterAmplification((float)volume/100);
    }

    public void Destroy()
    {
        Destroy(this);
    }

    private void OnEnded()
    {
        OnFinishedTrack?.Invoke(this, _trackName);
    }

    private void Awake()
    {
        _hub = ReferenceHub.GetHub(gameObject);
        if (_hub == null)
        {
            Destroy(this);
            return;
        }

        _audioPlayer = AudioPlayerPool.Rent(SpeakerSettings.Default, Dummy?.GameObject?.transform);
        _audioPlayer.AlwaysRead = false;
        SendEngine = new(Dummy, VoiceChatChannel.RoundSummary);
        _audioPlayer.WithSendEngine(SendEngine);
        _audioPlayer.Ended += OnEnded;
        AudioPlayers[_hub] = this;
    }

    private void FixedUpdate()
    {
        if (Target == null || !Dummy.IsAlive || !Target.IsAlive)
        {
            return;
        }
        
        Dummy.Position = Target.Position;
    }

    private void OnDestroy()
    {
        Stop();
        AudioPlayers.Remove(_hub);
        AudioPlayerPool.Return(_audioPlayer);
    }
}