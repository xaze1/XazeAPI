// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using LabApi.Features.Wrappers;
using UnityEngine;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.Model.Animating.States;

public class PrimitiveState(PrimitiveObjectToy primitive) : ObjectState(primitive.GameObject)
{
    public PrimitiveObjectToy Primitive { get; } = primitive;
    
    public override void Evaluate(double time)
    {
        if (Curves.Count == 0)
            return;

        base.Evaluate(time);
        if (EvaluateCurve(nameof(Primitive.Color), time, out Color color))
             Primitive.Color = color;
    }
    
    public override void EvaluateBlend(ObjectState targetState, double sourceTime, double targetTime, double blendTime)
    {
        if (Curves.Count == 0 || Target == null)
            return;
        
        base.EvaluateBlend(targetState, sourceTime, targetTime, blendTime);
        if (EvaluateCurve(nameof(Primitive.Color), sourceTime, out Color colorA) &&
            targetState.EvaluateCurve(nameof(Primitive.Color), targetTime, out Color colorB))
            Primitive.Color = (Color)Curves[nameof(Primitive.Color)].Interpolate(colorA, colorB, blendTime);
    }
    
    public PrimitiveState WithColor(AnimationCurve<Color> curve)
    {
        Curves["Color"] = curve;
        return this;
    }
    
    public PrimitiveState WithColor(Action<AnimationCurve<Color>> curveBuilder)
    {
        var curve = AnimationCurves.Color();
        curveBuilder.InvokeSafely(curve);
        WithColor(curve);
        return this;
    }
}