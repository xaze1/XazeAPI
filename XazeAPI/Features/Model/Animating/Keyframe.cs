// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

namespace XazeAPI.Features.Model.Animating;

public readonly struct Keyframe<T> (double time, T value)
{
    public readonly double Time = time;
    public readonly T Value = value;
}