// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using LabApi.Features.Wrappers;
using UnityEngine;

namespace XazeAPI.Features.Model.ModelParts;

public class LightPart : ModelPart
{
    /// <summary>
    /// Isn't set on creation or during builder action
    /// </summary>
    public new LightSourceToy Part => base.Part as LightSourceToy;

    public Color Color
    {
        get;
        set
        {
            field = value;
            Part?.Color = value;
        }
    } = Color.clear;

    public LightType Type
    {
        get;
        set
        {
            field = value;
            Part?.Type = value;
        }
    } = LightType.Spot;

    public float Intensity
    {
        get;
        set
        {
            field = value;
            Part?.Intensity = value;
        }
    }

    public float Range
    {
        get;
        set
        {
            field = value;
            Part?.Range = value;
        }
    }

    public float ShadowStrength
    {
        get;
        set
        {
            field = value;
            Part?.ShadowStrength = value;
        }
    }

    public LightShadows ShadowType
    {
        get;
        set
        {
            field = value;
            Part?.ShadowType = value;
        }
    }

    public float SpotAngle
    {
        get;
        set
        {
            field = value;
            Part?.SpotAngle = value;
        }
    }

    public float InnerSpotAngle
    {
        get;
        set
        {
            field = value;
            Part?.InnerSpotAngle = value;
        }
    }
    
    public LightPart WithColor(byte r, byte g, byte b, float a = 1f) => WithColor(new Color(r / 255f, g / 255f, b / 255f, a));
    public LightPart WithColor(float r, float g, float b, float a = 1f) => WithColor(new Color(r, g, b, a));
    public LightPart WithColor(Color color)
    {
        Part.Color = color;
        return this;
    }
        
    public LightPart WithIntensity(float intensity)
    {
        Part.Intensity = intensity;
        return this;
    }
        
    public LightPart WithRange(float range)
    {
        Part.Range = range;
        return this;
    }
        
    public LightPart WithType(LightType type)
    {
        Part.Type = type;
        return this;
    }
    
    public override void Spawn()
    {
        base.Part = LightSourceToy.Create(Position, Rotation, Scale, Parent, false);
        
        Part.Color = Color;
        Part.Type = Type;
        
        Part.Intensity = Intensity;
        Part.Range = Range;
        
        Part.ShadowStrength = ShadowStrength;
        Part.ShadowType = ShadowType;
        
        Part.SpotAngle = SpotAngle;
        Part.InnerSpotAngle = InnerSpotAngle;
        
        Part.Spawn();
    }
}