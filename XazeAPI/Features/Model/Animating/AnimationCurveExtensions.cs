// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using UnityEngine;

namespace XazeAPI.Features.Model.Animating;

public static class AnimationCurveExtensions
{
    extension(AnimationCurve<Vector3> vector3Curve)
    {
        public AnimationCurve<Vector3> AddKey(double time, float x, float y = 0f, float z = 0f) => vector3Curve.AddKey(time, new Vector3(x, y, z));
        public AnimationCurve<Vector3> AddKey(double time) => vector3Curve.AddKey(time, Vector3.zero);
    }
    
    extension(AnimationCurve<Quaternion> quaternionCurve)
    {
        public AnimationCurve<Quaternion> AddKey(double time, float x, float y = 0, float z = 0) => quaternionCurve.AddKey(time, Quaternion.Euler(x, y, z));
        public AnimationCurve<Quaternion> AddKey(double time) => quaternionCurve.AddKey(time, Quaternion.identity);
    }
    
    extension(AnimationCurve<Color> colorCurve)
    {
        public AnimationCurve<Color> AddKey(double time, byte r, byte g, byte b, byte a = 255) => colorCurve.AddKey(time, new Color(r / 255f, g / 255f, b / 255f, a / 255f));
        public AnimationCurve<Color> AddKey(double time, float r, float g, float b, float a = 1) => colorCurve.AddKey(time, new Color(r, g, b, a));
        public AnimationCurve<Color> AddKey(double time) => colorCurve.AddKey(time, Color.clear);
    }
}