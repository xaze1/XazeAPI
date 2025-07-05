// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

namespace XazeAPI.API.Interfaces
{
    public interface ICustomGlow
    {
        public abstract LightSystem.LightConfig.LightState State { get; }
        public abstract UnityEngine.Color[] Colors { get; }
        public abstract float Intensity { get; }
        public abstract float Range { get; }
    }
}
