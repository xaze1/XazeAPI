// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using UnityEngine;

namespace XazeAPI.Features.LightConfigs;

public class GradientLightConfig : LightConfigBase
{
    public Color[] LightColors { get; set; } = [];
    public float TransitionSpeed { get; set; } = 10;
    public float TransitionTimer { get; set; } = 0;

    public uint ColorIndex
    {
        get;
        set => field = value % (uint)LightColors.Length;
    }
    
    public override void Update(float deltaTime)
    {
        if (LightColors.Length == 0)
            return;
        
        TransitionTimer += deltaTime * (TransitionSpeed / 30f);

        if (TransitionTimer >= 1f)
        {
            TransitionTimer = 0f;
            ColorIndex += 1;
        }
        
        uint next = (ColorIndex + 1) % (uint)LightColors.Length;

        Light.Color = Color.Lerp(
            LightColors[ColorIndex],
            LightColors[next],
            TransitionTimer
        );
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        if (LightColors.Length == 0)
            return;
        Light.Color = LightColors[0];
    }
}