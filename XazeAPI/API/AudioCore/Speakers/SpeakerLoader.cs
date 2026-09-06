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
    
    public AudioPlayer Base { get; private set; }
    public CustomSpeakerEngine SendEngine { get; private set; }
    public TimeSpan CurrentTimePosition => Base.CurrentTime;
    public bool IsPlaying => !Base.HasEnded;
    public bool IsFinished => Base.HasEnded;

    public FakePlayerCustomHearSoundCheck HearOverride
    {
        get => SendEngine.HearOverride;
        set => SendEngine.HearOverride = value;
    }

    public SpeakerSettings Settings
    {
        get => SpeakerSettings.From(Base);
        set => Base.ApplySettings(value);
    }
    
    public double VolumePercentage
    {
        get => Base.MasterAmplification * 200;
        set => SetVolume(value);
    }
    
    private string _trackName;

    public void Play(string filePath)
    {
        try
        {
            OnTrackSelecting?.Invoke(this);
            _trackName = filePath;
            Base.UseFile(filePath);
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
        Base.WithoutProvider();
    }

    public SpeakerLoader SetVolume(double volume)
    {
        Base.WithMasterAmplification((float)volume/200);
        return this;
    }

    public SpeakerLoader SetPersonalization(Func<Player, SpeakerSettings?, SpeakerSettings> personalization)
    {
        Base.WithLivePersonalizedSendEngine((player, current) => personalization(player, current),SendEngine);
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
        Base = Player.TryGet(gameObject, out _) ? AudioPlayerPool.Rent(SpeakerSettings.Default, gameObject.transform) : AudioPlayerPool.Rent(SpeakerSettings.GloballyAudible with
        {
            IsSpatial = false
        });
        Base.AlwaysRead = false;
        SendEngine = new();
        Base.WithSendEngine(SendEngine);
        Base.Ended += OnEnded;
        
        AudioSpeakers[Base.Speaker] = this;
    }

    private void OnDestroy()
    {
        Stop();
        AudioSpeakers.Remove(Base.Speaker);
        AudioPlayerPool.Return(Base);
    }
}