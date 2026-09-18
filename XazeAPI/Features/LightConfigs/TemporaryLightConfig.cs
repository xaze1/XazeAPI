// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using UnityEngine;

namespace XazeAPI.Features.LightConfigs;

public class TemporaryLightConfig : SolidLightConfig
{
    public float Duration
    {
        get;
        set
        {
            field = value;
            TimeLeft = value;
        }
    }
    
    public float TimeLeft { get; private set; }
    public float FadeTime { get; set; } = 1f;
    
    public bool DimLight { get; set; } = true;
    public bool FadeIn { get; set; } = false;
    
    private float _aliveTime = 0;
    private float _initialIntensity;
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (FadeIn && _aliveTime < FadeTime)
        {
            _aliveTime += deltaTime;
            var normalizedFade = Mathf.Clamp01(_aliveTime / FadeTime);
            Intensity = _initialIntensity * normalizedFade;
            return;
        }
        
        if (Duration == 0)
            return;
        
        TimeLeft -= deltaTime;
        
        if (DimLight)
        {
            var normalizedTime = Mathf.Clamp01(TimeLeft / Duration);
            Intensity = _initialIntensity * normalizedTime;
        }
        
        if (TimeLeft > 0)
            return;
        
        Destroy();
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        _initialIntensity = Intensity;
        if (FadeIn)
            Intensity = 0;
    }
}