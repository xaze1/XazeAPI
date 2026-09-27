// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using LabApi.Features.Wrappers;
using UnityEngine;

namespace XazeAPI.Features.Model.Animating.States;

public class LightState(LightSourceToy light) : ObjectState(light.GameObject)
{
    private LightSourceToy Light { get; } = light;
    
    public override void Evaluate(double time)
    {
        if (Curves.Count == 0)
            return;

        base.Evaluate(time);
        if (EvaluateCurve(nameof(Light.Color), time, out Color color))
            Light.Color = color;
        
        if (EvaluateCurve(nameof(Light.Intensity), time, out float intensity))
            Light.Intensity = intensity;
        
        if (EvaluateCurve(nameof(Light.Range), time, out float range))
            Light.Range = range;
        
        if (EvaluateCurve(nameof(Light.SpotAngle), time, out float spotAngle))
            Light.SpotAngle = spotAngle;
        
        if (EvaluateCurve(nameof(Light.InnerSpotAngle), time, out float innerSpotAngle))
            Light.InnerSpotAngle = innerSpotAngle;
    }
    
    public override void EvaluateBlend(ObjectState targetState, double sourceTime, double targetTime, double blendTime)
    {
        if (Curves.Count == 0 || Target == null)
            return;
        
        base.EvaluateBlend(targetState, sourceTime, targetTime, blendTime);
        if (EvaluateCurve(nameof(Light.Color), sourceTime, out Color colorA) &&
            targetState.EvaluateCurve(nameof(Light.Color), targetTime, out Color colorB))
            Light.Color = (Color)Curves[nameof(Light.Color)].Interpolate(colorA, colorB, blendTime);
        
        if (EvaluateCurve(nameof(Light.Intensity), sourceTime, out float intensityA) &&
            targetState.EvaluateCurve(nameof(Light.Intensity), targetTime, out float intensityB))
            Light.Intensity = (float)Curves[nameof(Light.Intensity)].Interpolate(intensityA, intensityB, blendTime);

        if (EvaluateCurve(nameof(Light.Range), sourceTime, out float rangeA) &&
            targetState.EvaluateCurve(nameof(Light.Range), targetTime, out float rangeB))
            Light.Range = (float)Curves[nameof(Light.Range)].Interpolate(rangeA, rangeB, blendTime);
        
        if (EvaluateCurve(nameof(Light.SpotAngle), sourceTime, out float spotAngleA) &&
            targetState.EvaluateCurve(nameof(Light.SpotAngle), targetTime, out float spotAngleB))
            Light.SpotAngle = (float)Curves[nameof(Light.SpotAngle)].Interpolate(spotAngleA, spotAngleB, blendTime);
        
        if (EvaluateCurve(nameof(Light.InnerSpotAngle), sourceTime, out float innerSpotAngleA) &&
            targetState.EvaluateCurve(nameof(Light.InnerSpotAngle), targetTime, out float innerSpotAngleB))
            Light.InnerSpotAngle = (float)Curves[nameof(Light.InnerSpotAngle)].Interpolate(innerSpotAngleA, innerSpotAngleB, blendTime);
    }
}