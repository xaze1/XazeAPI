// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using XazeAPI.API.Extensions;
using XazeAPI.Features.Model.Animating.States;

namespace XazeAPI.Features.Model.Animating;

public class Animation
{
    public event Action OnComplete;
    
    public IReadOnlyList<ObjectState> States => _states.AsReadOnly();
    private List<ObjectState> _states { get; } = new();

    public double Duration { get; private set; } = 1.0;
    public bool IsLooping { get; set; } = false;
    
    public bool IsPaused { get; private set; }
    public bool IsPlaying { get; private set; }

    private double _startTime;
    private double _pauseTime;
    
    public void Play()
    {
        if (States.Count == 0 || IsPlaying)
            return;
        
        _startTime = NetworkTime.time;
        IsPlaying = true;
        IsPaused = false;
        
        StaticUnityMethods.OnUpdate += UpdateAnimation;
    }

    public void Stop()
    {
        if (!IsPlaying)
            return;
        
        IsPlaying = false;
        IsPaused = false;
        StaticUnityMethods.OnUpdate -= UpdateAnimation;
    }

    public double GetNormalizedTime()
    {
        if (!IsPlaying) return 0;
        double timePassed = NetworkTime.time - _startTime;
        return timePassed / Duration;
    }

    public void Pause()
    {
        if (!IsPlaying || IsPaused)
            return;

        _pauseTime = NetworkTime.time;
        IsPaused = true;
    }

    public void Resume()
    {
        if (!IsPlaying || !IsPaused)
            return;

        _startTime += NetworkTime.time - _pauseTime;
        IsPaused = false;
    }

    private void UpdateAnimation()
    {
        if (IsPaused)
            return;
        
        double time = NetworkTime.time - _startTime;
        if (time >= Duration)
        {
            if (IsLooping)
            {
                _startTime = NetworkTime.time - (time % Duration);
                time %= Duration;
                OnComplete?.InvokeSafely();
            }
            else
            {
                time = Duration;
                foreach (var state in States)
                    state.Evaluate(time);
                
                Stop();
                OnComplete?.InvokeSafely();
                return;
            }
        }
        
        foreach (var state in States)
            state.Evaluate(time);
    }

    public static Animation Create(params ObjectState[] states)
    {
        var animation = new Animation();
        animation._states.AddRange(states);
        animation.Duration = states.Max(s => s.GetDuration());
        return animation;
    }

    private Animation()
    {
    }
}