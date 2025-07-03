using PlayerRoles.FirstPersonControl;
using XazeAPI.API.Helpers;
using XazeAPI.API.Interfaces;

namespace XazeAPI.API
{
    using CustomPlayerEffects;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp106;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class LightSystem : MonoBehaviour
    {
        public static LightSystem Singleton;
        public static readonly HashSet<LightConfig> Lights = new();
        public static float TransitionSpeed = 10f;
        
        void Awake()
        {
            if (Singleton != null)
            {
                Destroy(Singleton);
            }

            Singleton = this;
            //ItemPickupBase.OnPickupAdded += HandlePickupCreation;
            AdminToys.AdminToyBase.OnRemoved += OnLightRemoved;

            Logging.Debug("[LightSystem] Spawned!", APILoader.Debug);
        }

        void OnDestroy()
        {
            if (Singleton == this)
            {
                Singleton = null;
            }

            //ItemPickupBase.OnPickupAdded -= HandlePickupCreation;
            AdminToys.AdminToyBase.OnRemoved -= OnLightRemoved;

            foreach (var config in Lights)
            {
                config.LightToy.Destroy();
            }
            Lights.Clear();
        }

        void Update()
        {
            foreach(var config in Lights)
            {
                if (config.LightToy.GameObject is null || config.TargetHub != null && config.TargetHub.gameObject is null)
                {
                    continue;
                }

                switch (config.Status)
                {
                    case LightConfig.LightState.SolidColor:
                        break;

                    case LightConfig.LightState.Rainbow:
                        config.hue += TransitionSpeed / 10000f;
                        if (config.hue >= 1)
                        {
                            config.hue = 0;
                        }
                        config.CurrentColor = Color.HSVToRGB(config.hue, 1, 1);
                        break;

                    case LightConfig.LightState.Gradient:
                        config.hue += Time.deltaTime * (TransitionSpeed / 30f);

                        if (config.hue >= 1f)
                        {
                            config.hue = 0f;
                            config.index = (config.index + 1) % config.ColorArray.Length;
                        }

                        int next = (config.index + 1) % config.ColorArray.Length;

                        config.CurrentColor = Color.Lerp(
                            config.ColorArray[config.index],
                            config.ColorArray[next],
                            config.hue
                        );
                        break;
                }

                float lightRange = config.Range;
                if (config.TargetHub != null && (config.TargetHub.playerEffectsController.GetEffect<Invisible>().IsEnabled || config.TargetHub.roleManager.CurrentRole is IFpcRole fpc && fpc.FpcModule.Motor.IsInvisible))
                {
                    lightRange = 0;
                }
                else if (config.TargetRole is Scp106Role larry && larry.SubroutineModule.TryGetSubroutine(out Scp106StalkAbility stalk) && stalk.StalkActive)
                {
                    lightRange = 0;
                }

                config.LightToy.Range = lightRange;
            }

            if (!Lights.Any(x => x.TargetHub != null && x.TargetHub.gameObject == null))
            {
                return;
            }

            Lights.RemoveWhere(x => x.TargetHub != null && x.TargetHub.gameObject == null);
        }

        public static void AddLight(Transform origin, float intensity = 5, float range = 10)
        {
            new LightConfig(origin, intensity, range);
        }

        public static void AddLight(ReferenceHub Target, Color color, float intensity = 5, float range = 10, LightConfig.LightState state = LightConfig.LightState.SolidColor)
        {
            new LightConfig(Target, color, intensity, range, state);
        }

        public static void AddLight(Player Target, Color color, float intensity = 5, float range = 10, LightConfig.LightState state = LightConfig.LightState.SolidColor) =>
            AddLight(Target.ReferenceHub, color, intensity, range, state);

        public static void RemoveLight(ReferenceHub Owner)
        {
            var lights = Lights.ToList();
            foreach(var config in lights)
            {
                if (config.TargetHub != Owner)
                {
                    continue;
                }

                config.LightToy.Destroy();
                Lights.Remove(config);
            }
        }

        public static void RemoveLight(Player Owner)
        {
            var lights = Lights.ToList();
            foreach(var config in lights)
            {
                if (config.TargetHub != Owner.ReferenceHub)
                {
                    continue;
                }

                config.LightToy.Destroy();
                Lights.Remove(config);
            }
        }

        private void OnLightRemoved(AdminToys.AdminToyBase obj)
        {
            if (obj is not AdminToys.LightSourceToy light)
            {
                return;
            }

            Lights.RemoveWhere(x => x.LightToy.GameObject == light.gameObject);
        }

        /*
        static void HandlePickupCreation(ItemPickupBase pickupBase)
        {
            if (!CustomItem.TrackedSerials.TryGetValue(pickupBase.Info.Serial, out var id) || !CustomItem.TryGet(id, out var customItem))
            {
                return;
            }
            
            CustomItemSpawnFlags flag = CustomItem.GetFlag(pickupBase.Info.Serial);

            LightConfig config = null;
            if (customItem is ICustomGlow customGlow)
            {
                config = new(pickupBase.transform, customGlow);
            }
            else if (flag == CustomItemSpawnFlags.RainbowGlow)
            {
                config = new LightConfig(pickupBase.transform, 0.7f, 5f);
            }
            else
            {
                config = new LightConfig(pickupBase.transform, customItem.ObjectColor, 0.7f, 5f);
            }

            config.LightToy.Position = Vector3.zero;
            config.LightToy.ShadowType = LightShadows.None;
            config.LightToy.ShadowStrength = 0;

            Logging.Debug("[LightSystem] Spawned Light for " + customItem.GetType().Name, Plugin.Debug);
        }*/

        public class LightConfig
        {
            public enum LightState
            {
                SolidColor,
                Gradient,
                Rainbow
            }

            private LightSourceToy _light;
            private Color[] _colors;
            private LightState _state;
            public float hue = 0;
            public float Range = 0;
            public int index = 0;

            public ReferenceHub TargetHub;
            public PlayerRoleBase TargetRole => TargetHub?.roleManager?.CurrentRole;

            public Color[] ColorArray => _colors;
            public LightState Status => _state;
            public LightSourceToy LightToy => _light;
            public Color CurrentColor
            {
                get
                {
                    if (LightToy == null)
                    {
                        return Color.white;
                    }

                    return LightToy.Color;
                }
                set
                {
                    if (LightToy == null)
                    {
                        return;
                    }

                    LightToy.Color = value;
                }
            }

            public LightConfig(Transform origin, Color color, float intensity = 5, float range = 10, LightState state = LightState.SolidColor)
            {
                _colors = [color];
                _state = state;
                _light = MainHelper.spawnLight(origin, color, intensity, range);
                Range = range;

                Lights.Add(this);
            }

            public LightConfig(ReferenceHub Target, Color color, float intensity = 5, float range = 10, LightState state = LightState.SolidColor)
            {
                _colors = [color];
                _state = state;
                _light = MainHelper.spawnLight(Target.gameObject.transform, color, intensity, range);
                TargetHub = Target;
                Range = range;

                Lights.Add(this);
            }

            public LightConfig(Vector3 origin, Color color, float intensity = 5, float range = 10)
            {
                _colors = [color];
                _state = LightState.SolidColor;
                _light = MainHelper.spawnLight(origin, color, intensity, range);
                Range = range;

                Lights.Add(this);
            }

            public LightConfig(Transform origin, Color[] colors, float intensity = 5, float range = 10)
            {
                if (colors.Length <= 0)
                {
                    throw new System.ArgumentNullException("colors cannot be Empty");
                }

                _colors = colors;
                _state = LightState.Gradient;
                _light = MainHelper.spawnLight(origin, _colors[0], intensity, range);
                Range = range;

                Lights.Add(this);
            }

            public LightConfig(Vector3 origin, Color[] colors, float intensity = 5, float range = 10)
            {
                if (colors.Length <= 0)
                {
                    throw new System.ArgumentNullException("colors cannot be Empty");
                }

                _colors = colors;
                _state = LightState.Gradient;
                _light = MainHelper.spawnLight(origin, _colors[0], intensity, range);
                Range = range;

                Lights.Add(this);
            }

            public LightConfig(Transform origin, float intensity = 5, float range = 10)
            {
                _colors = [];
                _state = LightState.Rainbow;
                _light = MainHelper.spawnLight(origin, Color.HSVToRGB(0, 1, 1), intensity, range);
                Range = range;

                Lights.Add(this);
            }

            public LightConfig(Vector3 origin, float intensity = 5, float range = 10)
            {
                _colors = [];
                _state = LightState.Rainbow;
                _light = MainHelper.spawnLight(origin, Color.HSVToRGB(0, 1, 1), intensity, range);
                Range = range;

                Lights.Add(this);
            }

            public LightConfig(LightSourceToy light)
            {
                _colors = [];
                _state = LightState.SolidColor;
                _light = light;
                Range = _light.Range;

                Lights.Add(this);
            }

            public LightConfig(Transform origin, ICustomGlow customGlow)
            {
                if (customGlow.Colors.Length <= 0 && customGlow.State != LightState.Rainbow)
                {
                    throw new System.ArgumentNullException("colors cannot be Empty");
                }

                _colors = customGlow.Colors;
                _state = customGlow.State;
                _light = MainHelper.spawnLight(origin, _state == LightState.Rainbow? Color.HSVToRGB(0, 1, 1) : _colors[0], customGlow.Intensity, customGlow.Range);
                Range = customGlow.Range;

                Lights.Add(this);
            }

            public LightConfig(Vector3 origin, ICustomGlow customGlow)
            {
                if (customGlow.Colors.Length <= 0)
                {
                    throw new System.ArgumentNullException("colors cannot be Empty");
                }

                _colors = customGlow.Colors;
                _state = customGlow.State;
                _light = MainHelper.spawnLight(origin, _state == LightState.Rainbow? Color.HSVToRGB(0, 1, 1) : _colors[0], customGlow.Intensity, customGlow.Range);
                Range = customGlow.Range;

                Lights.Add(this);
            }
        }
    }
}
