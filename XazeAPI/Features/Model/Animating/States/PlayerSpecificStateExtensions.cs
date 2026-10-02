// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace XazeAPI.Features.Model.Animating.States;

using API.Extensions;

public static class PlayerSpecificStateExtensions
{
    extension(PlayerSpecificState<PrimitiveObjectToy> primState)
    {
        public PlayerSpecificState<PrimitiveObjectToy> WithColor(AnimationCurve<Color> curve)
        {
            primState.Curves["Color"] = curve;
            return primState;
        }
    
        public PlayerSpecificState<PrimitiveObjectToy> WithColor(Action<AnimationCurve<Color>> curveBuilder)
        {
            var curve = AnimationCurves.Color();
            curveBuilder.InvokeSafely(curve);
            primState.WithColor(curve);
            return primState;
        }
    }
}