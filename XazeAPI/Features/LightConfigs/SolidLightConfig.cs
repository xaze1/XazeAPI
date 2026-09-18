// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using UnityEngine;

namespace XazeAPI.Features.LightConfigs;

public class SolidLightConfig : LightConfigBase
{
    public Color LightColor { get; set; } = Color.clear;
    
    public override void Update(float deltaTime)
    {
        if (Light?.Color == LightColor)
            return;
        
        Light?.Color = LightColor;
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        Light.Color = LightColor;
    }
}