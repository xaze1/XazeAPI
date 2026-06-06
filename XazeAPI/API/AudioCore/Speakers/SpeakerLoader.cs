// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.SendEngines;
using UnityEngine;
using XazeAPI.API.Helpers;
using XazeAPI.API.Stats;
using XazeAPI.API.Structures;

namespace XazeAPI.API.AudioCore.Speakers;

public class SpeakerLoader : MonoBehaviour
{
    public delegate void TrackFinished(SpeakerLoader fakeLoader, string track);
    
    public static Dictionary<SpeakerToy, SpeakerLoader> AudioSpeakers = new();
    public static event Action<SpeakerLoader> OnTrackSelecting;
    public static event Action<SpeakerLoader, string> OnTrackSelected;
    public static event TrackFinished OnFinishedTrack;
    
    public CustomSpeakerEngine SendEngine { private set; get; }
    public TimeSpan CurrentTimePosition => _audioPlayer.CurrentTime;
    public bool IsPlaying => !_audioPlayer.HasEnded;
    public bool IsFinished => _audioPlayer.HasEnded;
    
    [CanBeNull] public event Action Ended
    {
        add => _audioPlayer.Ended += value;
        remove => _audioPlayer.Ended -= value;
    }

    public FakePlayerCustomHearSoundCheck HearOverride
    {
        get => SendEngine.HearOverride;
        set => SendEngine.HearOverride = value;
    }

    public SpeakerSettings Settings
    {
        get => new()
        {
            IsSpatial = _audioPlayer.Speaker.IsSpatial,
            MaxDistance =  _audioPlayer.Speaker.MaxDistance,
            MinDistance =  _audioPlayer.Speaker.MinDistance,
            Volume =  _audioPlayer.Speaker.Volume,
        };
        set => _audioPlayer.ApplySettings(value);
    }
    
    public double VolumePercentage
    {
        get => _audioPlayer.MasterAmplification * 200;
        set => SetVolume(value);
    }
    
    private AudioPlayer _audioPlayer;
    private string _trackName;

    public void Play(string filePath)
    {
        try
        {
            OnTrackSelecting?.Invoke(this);
            _trackName = filePath;
            _audioPlayer.UseFile(filePath);
            OnTrackSelected?.Invoke(this, _trackName);
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

    public SpeakerLoader SetVolume(double volume)
    {
        _audioPlayer.WithMasterAmplification((float)volume/200);
        return this;
    }

    public SpeakerLoader SetPersonalization(Func<Player, SpeakerSettings?, SpeakerSettings> personalization)
    {
        _audioPlayer.WithLivePersonalizedSendEngine((player, current) => personalization(player, current),SendEngine);
        return this;
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
        _audioPlayer = Player.TryGet(gameObject, out _) ? AudioPlayerPool.Rent(SpeakerSettings.Default, gameObject.transform) : AudioPlayerPool.Rent(SpeakerSettings.GloballyAudible with
        {
            IsSpatial = false
        });
        _audioPlayer.AlwaysRead = false;
        SendEngine = new();
        _audioPlayer.WithSendEngine(SendEngine);
        _audioPlayer.Ended += OnEnded;
        
        AudioSpeakers[_audioPlayer.Speaker] = this;
    }

    private void OnDestroy()
    {
        Stop();
        AudioSpeakers.Remove(_audioPlayer.Speaker);
        AudioPlayerPool.Return(_audioPlayer);
    }
}