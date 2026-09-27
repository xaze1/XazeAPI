// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XazeAPI.Features.Model.Animating;

public abstract class AnimationCurve
{
    public abstract object Evaluate(double time);
    public abstract object Interpolate(object a, object b, double time);
    public abstract double GetMaxTime();
}

public class AnimationCurve<T>(Func<T, T, double, T> interpolate) : AnimationCurve
{
    private List<Keyframe<T>> _keyframes { get; } = new();

    public IReadOnlyList<Keyframe<T>> Keyframes => _keyframes.AsReadOnly();

    public AnimationCurve<T> AddKey(double time, T value)
    {
        if (double.IsNaN(time) || double.IsInfinity(time))
            throw new ArgumentOutOfRangeException(nameof(time));
        
        int low = 0;
        int high = _keyframes.Count - 1;

        while (low <= high)
        {
            int middle = low + ((high - low) >> 1);
            double middleTime = _keyframes[middle].Time;

            if (Math.Abs(middleTime - time) < 0.001f)
            {
                _keyframes[middle] = new Keyframe<T>(time, value);
                return this;
            }

            if (middleTime < time)
                low = middle + 1;
            else
                high = middle - 1;
        }

        _keyframes.Insert(low, new Keyframe<T>(time, value));
        return this;
    }
    
    public T EvaluateTyped(double time)
    {
        if (_keyframes.Count == 0)
            return default!;

        if (time <= _keyframes[0].Time)
            return _keyframes[0].Value;

        int last = _keyframes.Count - 1;

        if (time >= _keyframes[last].Time)
            return _keyframes[last].Value;

        int nextIndex = FindNextKeyframe(time);

        Keyframe<T> a = _keyframes[nextIndex - 1];
        Keyframe<T> b = _keyframes[nextIndex];

        double t = (time - a.Time) / (b.Time - a.Time);

        return interpolate(a.Value, b.Value, t);
    }

    public override object Evaluate(double time)
        => EvaluateTyped(time);

    public override double GetMaxTime() => _keyframes.Max(s => s.Time);

    public override object Interpolate(object a, object b, double time)
    {
        return interpolate((T)a, (T)b, time); 
    }
    
    private int FindNextKeyframe(double time)
    {
        int low = 0;
        int high = _keyframes.Count - 1;

        while (low <= high)
        {
            int middle = low + ((high - low) >> 1);

            if (_keyframes[middle].Time <= time)
                low = middle + 1;
            else
                high = middle - 1;
        }

        return low;
    }
}

public static class AnimationCurves
{
    public static AnimationCurve<Vector3> Position()
    {
        return new AnimationCurve<Vector3>(
            (a, b, t) => Vector3.LerpUnclamped(a, b, (float)t));
    }

    public static AnimationCurve<Quaternion> Rotation()
    {
        return new AnimationCurve<Quaternion>(
            (a, b, t) => Quaternion.SlerpUnclamped(a, b, (float)t));
    }

    public static AnimationCurve<Vector3> Scale()
    {
        return new AnimationCurve<Vector3>(
            (a, b, t) => Vector3.LerpUnclamped(a, b, (float)t));
    }

    public static AnimationCurve<float> Float()
    {
        return new AnimationCurve<float>(
            (a, b, t) => Mathf.LerpUnclamped(a, b, (float)t));
    }

    public static AnimationCurve<Color> Color()
    {
        return new AnimationCurve<Color>(
            (a, b, t) => UnityEngine.Color.LerpUnclamped(a, b, (float)t));
    }
}