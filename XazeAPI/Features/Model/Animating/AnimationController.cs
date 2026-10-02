// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;
using XazeAPI.API.Extensions;
using XazeAPI.Features.Model.Animating.States;

namespace XazeAPI.Features.Model.Animating;

public class AnimationController
{
    public event Action<string, object> ParameterChanged;

    public string CurrentAnimationName { get; private set; } = string.Empty;
    
    public Dictionary<string, Animation> Animations { get; } = new();
    public Dictionary<string, object> Parameters { get; } = new();
    public Dictionary<string, AnimationTransition> Transitions { get; } = new();
    
    private AnimationTransition _activeTransition;
    private double _transitionStartTime;
    private double _sourceAnimStartTime;
    private double _targetAnimStartTime;
    
    public T Get<T>(string parameterName)
    {
        if (!Parameters.TryGetValue(parameterName, out var value) || value is not T typed)
            return default;
        return typed;
    }
    
    public void Set(string parameterName, object value)
    {
        if (Parameters.TryGetValue(parameterName, out var parameter) && Equals(parameter, value))
            return;
        
        Parameters[parameterName] = value;
        ParameterChanged?.InvokeSafely(parameterName, value);
    }

    [CanBeNull]
    public Animation GetAnimation(string animationName)
    {
        if (animationName.IsNullOrWhiteSpace())
            return null;
        
        return Animations.GetValueOrDefault(animationName);
    }
    
    public bool IsPlaying(string animationName)
    {
        var animation = GetAnimation(animationName);
        if (animation == null)
            return false;
        return animation.IsPlaying;
    }
    
    public void AddAnimation(string animationName, Animation animation)
    {
        Animations.Add(animationName, animation);
    }

    public void PlayAnimation(string animationName)
    {
        if (!Animations.TryGetValue(animationName, out var animation))
            return;

        if (CurrentAnimationName != null)
        {
            var oldAnim = GetAnimation(CurrentAnimationName);
            oldAnim?.Stop();
            if (oldAnim != null)
                oldAnim.OnComplete -= HandleAnimationComplete;
        }
        
        CurrentAnimationName = animationName;
        animation.Play();
        animation.OnComplete += HandleAnimationComplete;
    }

    public void PlayIdle(string animationName)
    {
        if (!Animations.TryGetValue(animationName, out var idle))
            return;
        
        if (IsPlaying(CurrentAnimationName) || idle.IsPlaying)
            return;
        idle.Play();
    }

    public void StopAnimation(string animationName)
    {
        if (!Animations.TryGetValue(animationName, out var animation))
            return;
        animation.Stop();
        animation.OnComplete -= HandleAnimationComplete;
    }

    [CanBeNull]
    public AnimationTransition GetTransition(string transitionName)
    {
        return Transitions.GetValueOrDefault(transitionName);
    }

    public void AddTransition(string transitionName, AnimationTransition transition)
    {
        transition.Setup(this);
        Transitions.Add(transitionName, transition);
    }

    public void AddTransition(string transitionName, Func<AnimationTransition> transitionBuilder)
    {
        var transition = transitionBuilder.InvokeSafely();
        transition.Setup(this);
        Transitions.Add(transitionName, transition);
    }
    
    public bool RemoveTransition(string transitionName)
    {
        var outcome = Transitions.Remove(transitionName, out var transition);
        transition.Remove(this);
        return outcome;
    }

    private void HandleAnimationComplete()
    {
        foreach (var transition in Transitions.Values)
        {
            if (transition.SourceAnimation != CurrentAnimationName || !transition.HasExitTime)
                continue;
            transition.Evaluate();
        }
    }

    internal void ExecuteTransition(AnimationTransition transition)
    {
        if (_activeTransition != null)
            return;
        
        var sourceAnim = GetAnimation(transition.SourceAnimation);
        var targetAnim = GetAnimation(transition.TargetAnimation);
        
        if (sourceAnim == null || targetAnim == null)
            return;
        
        _activeTransition = transition;
        _transitionStartTime = NetworkTime.time;
        
        sourceAnim.Stop();
        _sourceAnimStartTime = NetworkTime.time - (sourceAnim.GetNormalizedTime() * sourceAnim.Duration);
        _targetAnimStartTime = NetworkTime.time;

        StaticUnityMethods.OnUpdate += UpdateTransitionBlend;
    }

    private void UpdateTransitionBlend()
    {
        double timePassed = NetworkTime.time - _transitionStartTime;
        double blendTime = Math.Clamp(timePassed / _activeTransition.TransitionDuration, 0.0, 1.0);
        
        var sourceAnim = GetAnimation(_activeTransition.SourceAnimation);
        var targetAnim = GetAnimation(_activeTransition.TargetAnimation);

        if (sourceAnim == null || targetAnim == null)
        {
            StaticUnityMethods.OnUpdate -= UpdateTransitionBlend;
            _activeTransition = null;
            return;
        }

        var sourceTime = NetworkTime.time - _sourceAnimStartTime;
        var targetTime = NetworkTime.time - _targetAnimStartTime;
        
        var targetStateLookup = new Dictionary<GameObject, ObjectState>();
        var processedTargetStates = new HashSet<ObjectState>();

        // 2. Loop through Source states and match against Target states
        foreach (var sourceState in sourceAnim.States)
        {
            if (sourceState.Target != null && targetStateLookup.TryGetValue(sourceState.Target, out var matchingTargetState))
            {
                sourceState.EvaluateBlend(matchingTargetState, sourceTime, targetTime, blendTime);
                processedTargetStates.Add(matchingTargetState);
            }
            else
                sourceState.Evaluate(sourceTime);
        }

        foreach (var targetState in targetAnim.States)
        {
            if (!processedTargetStates.Contains(targetState))
                targetState.Evaluate(targetTime);
        }

        if (blendTime < 1.0) 
            return;
        
        StaticUnityMethods.OnUpdate -= UpdateTransitionBlend;
        var targetName = _activeTransition.TargetAnimation;
        _activeTransition = null;
        PlayAnimation(targetName);
    }
}