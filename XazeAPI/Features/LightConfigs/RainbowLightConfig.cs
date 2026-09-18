// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using UnityEngine;

namespace XazeAPI.Features.LightConfigs;

public class RainbowLightConfig : LightConfigBase
{
    public float TransitionSpeed { get; set; } = 10;
    public float Hue { get; set; } = 0;
    
    public override void Update(float deltaTime)
    {
        Hue += TransitionSpeed / 10000f;
        if (Hue >= 1)
        {
            Hue = 0;
        }
        
        Light.Color = Color.HSVToRGB(Hue, 1, 1);
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        Light.Color = Color.HSVToRGB(Hue, 1, 1);
    }
}