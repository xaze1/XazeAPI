// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using UnityEngine;
using XazeAPI.Features.Helpers;

namespace XazeAPI.Features.Model.Animating.States;

public class PlayerSpecificState<T>(T toy)  : ObjectState(toy.GameObject) where T : AdminToy
{
    public T Toy { get; private init; } = toy;
    [CanBeNull] public Player Receiver { get; set; }
    
    public override void Evaluate(double time)
    {
        if (Curves.Count == 0)
            return;

        if (EvaluateCurve("Position", time, out Vector3 pos))
            Toy.Position = pos;

        if (EvaluateCurve("Rotation", time, out Quaternion rotation))
            Toy.Rotation = rotation;

        if (EvaluateCurve("Scale", time, out Vector3 scale))
            Toy.Scale = scale;
        
        if (EvaluateCurve("Color", time, out Color color))
            typeof(T).GetProperty("Color")?.SetValue(Toy, color);
        
        if (Receiver == null)
            return;
        Toy.Update(Receiver);
    }
    
    public override void EvaluateBlend(ObjectState targetState, double sourceTime, double targetTime, double blendTime)
    {
        if (Curves.Count == 0 || Target == null)
            return;

        if (EvaluateCurve("Position", sourceTime, out Vector3 posA) && 
            targetState.EvaluateCurve("Position", targetTime, out Vector3 posB))
        {
            Toy.Position = (Vector3)Curves["Position"].Interpolate(posA, posB, blendTime);
        }

        if (EvaluateCurve("Rotation", sourceTime, out Quaternion rotA) && 
            targetState.EvaluateCurve("Rotation", targetTime, out Quaternion rotB))
        {
            Toy.Rotation = (Quaternion)Curves["Rotation"].Interpolate(rotA, rotB, blendTime);
        }

        if (EvaluateCurve("Scale", sourceTime, out Vector3 scaleA) && 
            targetState.EvaluateCurve("Scale", targetTime, out Vector3 scaleB))
        {
            Toy.Scale = (Vector3)Curves["Scale"].Interpolate(scaleA, scaleB, blendTime);
        }
        
        if (EvaluateCurve("Color", sourceTime, out Color colorA) && 
            targetState.EvaluateCurve("Color", targetTime, out Color colorB))
        {
            typeof(T).GetProperty("Color")?.SetValue(Toy, (Color)Curves["Color"].Interpolate(colorA, colorB, blendTime));
        }
        
        if (Receiver == null)
            return;
        Toy.Update(Receiver);
    }
}