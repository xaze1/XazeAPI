// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using UnityEngine;
using LabApi.Features.Wrappers;
using PrimitiveFlags = AdminToys.PrimitiveFlags;

namespace XazeAPI.Features.Model;

public static class ModelPartExtensions
{
    extension(ModelPart<PrimitiveObjectToy> primitivePart)
    {
        public ModelPart<PrimitiveObjectToy> WithColor(byte r, byte g, byte b, float a = 1f) => primitivePart.WithColor(new Color(r / 255f, g / 255f, b / 255f, a));
        public ModelPart<PrimitiveObjectToy> WithColor(float r, float g, float b, float a = 1f) => primitivePart.WithColor(new Color(r, g, b, a));
        public ModelPart<PrimitiveObjectToy> WithColor(Color color)
        {
            primitivePart.Part.Color = color;
            return primitivePart;
        }
        
        public ModelPart<PrimitiveObjectToy> WithFlags(PrimitiveFlags flags)
        {
            primitivePart.Part.Flags = flags;
            return primitivePart;
        }
        
        public ModelPart<PrimitiveObjectToy> WithType(PrimitiveType type)
        {
            primitivePart.Part.Type = type;
            return primitivePart;
        }
    }
    
    extension(ModelPart<LightSourceToy> lightPart)
    {
        public ModelPart<LightSourceToy> WithColor(byte r, byte g, byte b, float a = 1f) => lightPart.WithColor(new Color(r / 255f, g / 255f, b / 255f, a));
        public ModelPart<LightSourceToy> WithColor(float r, float g, float b, float a = 1f) => lightPart.WithColor(new Color(r, g, b, a));
        public ModelPart<LightSourceToy> WithColor(Color color)
        {
            lightPart.Part.Color = color;
            return lightPart;
        }
        
        public ModelPart<LightSourceToy> WithIntensity(float intensity)
        {
            lightPart.Part.Intensity = intensity;
            return lightPart;
        }
        
        public ModelPart<LightSourceToy> WithRange(float range)
        {
            lightPart.Part.Range = range;
            return lightPart;
        }
        
        public ModelPart<LightSourceToy> WithType(LightType type)
        {
            lightPart.Part.Type = type;
            return lightPart;
        }
    }
}