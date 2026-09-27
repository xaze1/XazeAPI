// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.Model.Animating.States;

public class ObjectState(GameObject gameObject)
{
    [CanBeNull] public GameObject Target { get; } = gameObject;
    public Dictionary<string, AnimationCurve> Curves { get; } = new();

    public double GetDuration()
    {
        return Curves.Count > 0? Curves.Values.Max(c => c.GetMaxTime()) : 0.0;
    }

    public bool EvaluateCurve<T>(string fieldName, double time, out T value)
    {
        value = default;
        if (!Curves.TryGetValue(fieldName, out var curve))
            return false;
        
        value = (T)curve.Evaluate(time);
        return true;
    }

    public virtual void Evaluate(double time)
    {
        if (Curves.Count == 0)
            return;

        if (EvaluateCurve("Position", time, out Vector3 pos))
            Target?.transform.localPosition = pos;

        if (EvaluateCurve("Rotation", time, out Quaternion rotation))
            Target?.transform.localRotation = rotation;

        if (EvaluateCurve("Scale", time, out Vector3 scale))
            Target?.transform.localScale = scale;
    }
    
    public virtual void EvaluateBlend(ObjectState targetState, double sourceTime, double targetTime, double blendTime)
    {
        if (Curves.Count == 0 || Target == null)
            return;

        if (EvaluateCurve("Position", sourceTime, out Vector3 posA) && 
            targetState.EvaluateCurve("Position", targetTime, out Vector3 posB))
        {
            Target.transform.localPosition = (Vector3)Curves["Position"].Interpolate(posA, posB, blendTime);
        }

        if (EvaluateCurve("Rotation", sourceTime, out Quaternion rotA) && 
            targetState.EvaluateCurve("Rotation", targetTime, out Quaternion rotB))
        {
            Target.transform.localRotation = (Quaternion)Curves["Rotation"].Interpolate(rotA, rotB, blendTime);
        }

        if (EvaluateCurve("Scale", sourceTime, out Vector3 scaleA) && 
            targetState.EvaluateCurve("Scale", targetTime, out Vector3 scaleB))
        {
            Target.transform.localScale = (Vector3)Curves["Scale"].Interpolate(scaleA, scaleB, blendTime);
        }
    }

    public ObjectState WithPosition(AnimationCurve<Vector3> positionCurve)
    {
        Curves["Position"] = positionCurve;
        return this;
    }
    
    public ObjectState WithRotation(AnimationCurve<Quaternion> rotationCurve)
    {
        Curves["Rotation"] = rotationCurve;
        return this;
    }
    
    public ObjectState WithScale(AnimationCurve<Vector3> scaleCurve)
    {
        Curves["Scale"] = scaleCurve;
        return this;
    }
    
    public ObjectState WithPosition(Action<AnimationCurve<Vector3>> curveBuilder)
    {
        var curve = AnimationCurves.Position();
        curveBuilder.InvokeSafely(curve);
        WithPosition(curve);
        return this;
    }
    
    public ObjectState WithRotation(Action<AnimationCurve<Quaternion>> curveBuilder)
    {
        var curve = AnimationCurves.Rotation();
        curveBuilder.InvokeSafely(curve);
        WithRotation(curve);
        return this;
    }
    
    public ObjectState WithScale(Action<AnimationCurve<Vector3>> curveBuilder)
    {
        var curve = AnimationCurves.Scale();
        curveBuilder.InvokeSafely(curve);
        WithScale(curve);
        return this;
    }
}