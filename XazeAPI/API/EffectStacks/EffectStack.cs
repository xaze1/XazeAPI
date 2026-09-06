// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using JetBrains.Annotations;
using UnityEngine;
using XazeAPI.API.Extensions;

namespace XazeAPI.API.EffectStacks;

public class EffectStack()
{
    public bool IsActive => Duration == 0f || TimeLeft > 0f;

    public byte MaxIntensity { get; set; } = byte.MaxValue;

    public int Intensity
    {
        get
        {
            if (_intensityCalc == null)
                return field;

            return _intensityCalc.InvokeSafely();
        }
        set;
    } = 1;

    public float Duration
    {
        get;
        set
        {
            field = value;
            TimeLeft = value;
        }
    } = 0;

    public float TimeLeft
    {
        get;
        set => field = Mathf.Max(0f, value);
    } = 0;

    public bool CanBeRemoved
    {
        get
        {
            if (_canBeRemovedCalc == null)
                return field;

            return _canBeRemovedCalc.InvokeSafely();
        }
        set;
    } = true;
    [CanBeNull] private readonly Func<int> _intensityCalc;
    [CanBeNull] private readonly Func<bool> _canBeRemovedCalc;

    public void RefreshTime(float deltaTime)
    {
        if (Duration == 0f)
            return;
        
        TimeLeft -= deltaTime;
    }

    public EffectStack([CanBeNull] Func<bool> canBeRemovedCalc = null) : this()
    {
        _canBeRemovedCalc = canBeRemovedCalc;
    }

    public EffectStack(Func<int> intensityCalc, [CanBeNull] Func<bool> canBeRemovedCalc = null) : this()
    {
        _intensityCalc = intensityCalc;
        _canBeRemovedCalc = canBeRemovedCalc;
    }
}