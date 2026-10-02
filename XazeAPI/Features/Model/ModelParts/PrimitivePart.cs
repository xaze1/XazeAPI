// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using AdminToys;
using UnityEngine;
using PrimitiveObjectToy = LabApi.Features.Wrappers.PrimitiveObjectToy;

namespace XazeAPI.Features.Model.ModelParts;

public class PrimitivePart : ModelPart
{
    /// <summary>
    /// Isn't set on creation or during builder action
    /// </summary>
    public new PrimitiveObjectToy Part => base.Part as PrimitiveObjectToy;

    public Color Color
    {
        get;
        set
        {
            field = value;
            Part?.Color = value;
        }
    } = Color.clear;

    public PrimitiveType Type
    {
        get;
        set
        {
            field = value;
            Part?.Type = value;
        }
    } = PrimitiveType.Sphere;

    public PrimitiveFlags Flags
    {
        get;
        set
        {
            field = value;
            Part?.Flags = value;
        }
    } = PrimitiveFlags.Collidable;
    
    
    public PrimitivePart WithColor(byte r, byte g, byte b, float a = 1f) => WithColor(new Color(r / 255f, g / 255f, b / 255f, a));
    public PrimitivePart WithColor(float r, float g, float b, float a = 1f) => WithColor(new Color(r, g, b, a));
    public PrimitivePart WithColor(Color color)
    {
        Part.Color = color;
        return this;
    }
        
    public PrimitivePart WithFlags(PrimitiveFlags flags)
    {
        Part.Flags = flags;
        return this;
    }
        
    public PrimitivePart WithType(PrimitiveType type)
    {
        Part.Type = type;
        return this;
    }

    public override void Spawn()
    {
        base.Part = PrimitiveObjectToy.Create(Position, Rotation, Scale, Parent, false);
        Part.Color = Color;
        Part.Type = Type;
        Part.Flags = Flags;
        Part.Spawn();
    }
}