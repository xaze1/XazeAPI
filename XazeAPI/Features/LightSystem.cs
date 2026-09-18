// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using UnityEngine;
using XazeAPI.API.Enums;
using XazeAPI.API.Interfaces;
using XazeAPI.Features.LightConfigs;

namespace XazeAPI.Features
{
    public static class LightSystem
    {
        internal static void Init()
        {
            AdminToys.AdminToyBase.OnRemoved += LightConfigBase.OnLightRemoved;
            StaticUnityMethods.OnUpdate += Update;
            ServerEvents.RoundRestarted += () =>
            {
                for (int i = LightConfigBase.Lights.Count - 1; i >= 0; i--)
                {
                    var config = LightConfigBase.Lights[i];
                    config.Destroy();
                }
            };
        }

        internal static void Update()
        {
            for (int i = LightConfigBase.Lights.Count - 1; i >= 0; i--)
            {
                var config = LightConfigBase.Lights[i];
                if (!config.IsSet)
                    continue;
                
                config.Update(Time.deltaTime);
                
                if (config._parent == null || config._parent.gameObject != null)
                    continue;
                config.Destroy();
            }
        }

        public static T AddLight<T>(Vector3 position, float intensity = 5, float range = 10) where T : LightConfigBase
        {
            var config = Activator.CreateInstance<T>();
            config.Intensity = intensity;
            config.Range = range;
            
            config.Create(position, Quaternion.identity);
            return config;
        }

        public static T AddLight<T>(Transform parent, float intensity = 5, float range = 10) where T : LightConfigBase
        {
            var config = Activator.CreateInstance<T>();
            config.Intensity = intensity;
            config.Range = range;
            
            config.Create(parent);
            return config;
        }

        public static LightConfigBase AddLight(Transform parent, ICustomGlow glow)
        {
            LightConfigBase config = glow.State switch
            {
                LightState.Gradient => new GradientLightConfig { LightColors = glow.Colors },
                LightState.Rainbow => new RainbowLightConfig(),
                _ => new SolidLightConfig { LightColor = glow.Colors[0] }
            };

            config.Intensity = glow.Intensity;
            config.Range = glow.Range;
            config.Create(parent);
            return config;
        }

        public static RainbowLightConfig AddLight(Transform origin, float intensity = 5, float range = 10)
        {
            var config = new RainbowLightConfig
            {
                Intensity = intensity,
                Range = range
            };
            config.Create(origin);
            return config;
        }

        public static SolidLightConfig AddLight(ReferenceHub Target, Color color, float intensity = 5, float range = 10)
        {
            var config = new SolidLightConfig
            {
                Intensity = intensity,
                Range = range,
                LightColor = color
            };
            config.Create(Target.transform);
            return config;
        }

        public static SolidLightConfig AddLight(Transform parent, Color color, float intensity = 5, float range = 10)
        {
            var config = new SolidLightConfig
            {
                Intensity = intensity,
                Range = range,
                LightColor = color
            };
            config.Create(parent);
            return config;
        }

        public static SolidLightConfig AddLight(Player Target, Color color, float intensity = 5, float range = 10) =>
            AddLight(Target.ReferenceHub, color, intensity, range);
    }
}
