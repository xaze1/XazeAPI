// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

namespace XazeAPI.API.Extensions
{
    using System.Drawing;

    public static class ColorExtensions
    {
        public static string ToHex(this Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}
