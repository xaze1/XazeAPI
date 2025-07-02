using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CustomPlayerEffects;
using Footprinting;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.MarshmallowMan;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using MapGeneration;
using MEC;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using Respawning;
using RueI.Displays;
using RueI.Extensions.HintBuilding;
using UnityEngine;
using XazeAPI.API.AudioCore.FakePlayers;
using XazeAPI.API.Enums;
using XazeAPI.API.Extensions;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Scp018Projectile = InventorySystem.Items.ThrowableProjectiles.Scp018Projectile;
using ThrowableItem = InventorySystem.Items.ThrowableProjectiles.ThrowableItem;

namespace XazeAPI.API.Helpers
{
    public static class MainHelper
    {
        public const int RecontainmentDamageTypeID = 27;
        public const int WarheadDamageTypeID = 2;
        public const int MicroHidTypeID = 18;
        public const int GrenadeTypeID = 19;
        public const int Scp018TypeID = 38;
        public const int DisruptorTypeID = 46;

        public static readonly Dictionary<ItemType, int> FirearmDamageTypeIDs = new()
        {
            {
                ItemType.GunCOM15, 12
            },
            {
                ItemType.GunE11SR, 14
            },
            {
                ItemType.GunLogicer, 16
            },
            {
                ItemType.GunCOM18, 32
            },
            {
                ItemType.GunAK, 33
            },
            {
                ItemType.GunShotgun, 34
            },
            {
                ItemType.GunCrossvec, 35
            },
            {
                ItemType.GunFSP9, 36
            },
            {
                ItemType.MicroHID, 18
            },
            {
                ItemType.GunRevolver, 31
            },
            {
                ItemType.ParticleDisruptor, 45
            },
            {
                ItemType.GunCom45, 50
            },
        };

        public static readonly List<ItemType> FirearmTypes = new()
        {
            ItemType.GunCom45,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunE11SR,
            ItemType.GunFSP9,
            ItemType.GunCrossvec,
            ItemType.GunRevolver,
            ItemType.GunShotgun,
            ItemType.GunA7,
            ItemType.GunAK,
            ItemType.GunLogicer,
            ItemType.GunFRMG0,
            ItemType.ParticleDisruptor
        };

        public static readonly List<ItemType> ArmorTypes = new()
        {
            ItemType.ArmorCombat,
            ItemType.ArmorHeavy,
            ItemType.ArmorLight,
        };
        
        public static readonly List<ItemType> SpecialItemTypes = new()
        {
            ItemType.ParticleDisruptor,
            ItemType.MicroHID,
            ItemType.Jailbird,
            ItemType.GunCom45,
            ItemType.GunSCP127,
        };
        
        public static readonly List<ItemType> HeavyItemTypes = new()
        {
            ItemType.GunFRMG0,
            ItemType.GunLogicer,
            ItemType.GunShotgun,
            ItemType.GunA7,
        };

        public static readonly Dictionary<DeathTranslation, int> UniversalDamageTypeIDs = [];

        public static readonly Dictionary<RoleTypeId, int> RoleDamageTypeIDs = new()
        {
            {
                RoleTypeId.Scp173, 24
            },
            {
                RoleTypeId.Scp106, 23
            },
            {
                RoleTypeId.Scp049, 20
            },
            {
                RoleTypeId.Scp096, 22
            },
            {
                RoleTypeId.Scp0492, 21
            },
            {
                RoleTypeId.Scp939, 25
            }
        };

        public static readonly Dictionary<Scp096DamageHandler.AttackType, int> Scp096DamageTypeIDs = new()
        {
            {
                Scp096DamageHandler.AttackType.GateKill, 40
            },
            {
                Scp096DamageHandler.AttackType.SlapLeft, 22
            },
            {
                Scp096DamageHandler.AttackType.SlapRight, 22
            },
            {
                Scp096DamageHandler.AttackType.Charge, 39
            }
        };

        public static readonly Dictionary<Scp939DamageType, int> Scp939DamageTypeIDs = new()
        {
            {
                Scp939DamageType.None, 46
            },
            {
                Scp939DamageType.Claw, 47
            },
            {
                Scp939DamageType.LungeTarget, 48
            },
            {
                Scp939DamageType.LungeSecondary, 49
            }
        };

        public static readonly List<ItemType> AmmoTypes = new()
        {
            ItemType.Ammo44cal,
            ItemType.Ammo9x19,
            ItemType.Ammo556x45,
            ItemType.Ammo762x39,
            ItemType.Ammo12gauge,
        };
        
        public static readonly List<ItemType> ConsumableTypes = new()
        {
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Adrenaline,
            ItemType.SCP500,
            ItemType.SCP207,
            ItemType.AntiSCP207,
        };
        
        public static readonly List<ItemType> HealItemTypes = new()
        {
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.SCP500,
        };

        public static readonly List<ItemType> ScpItemTypes = new()
        {
            ItemType.SCP018,
            ItemType.SCP1344,
            ItemType.SCP1853,
            ItemType.SCP207,
            ItemType.AntiSCP207,
            ItemType.SCP2176,
            ItemType.SCP244a,
            ItemType.SCP244b,
            ItemType.SCP244b,
            ItemType.SCP268,
            ItemType.SCP500,
            ItemType.SCP330
        };
        
        public static ThrownProjectile SpawnActive(
            this ThrowableItem item,
            Vector3 position,
            float fuseTime = -1f,
            Player owner = null
        )
        {
            TimeGrenade grenade = (TimeGrenade)Object.Instantiate(item.Projectile, position, Quaternion.identity);
            if (fuseTime >= 0)
                grenade._fuseTime = fuseTime;
            grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
            grenade.PreviousOwner = new Footprint(owner?.ReferenceHub?? ReferenceHub._hostHub);
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
            if (grenade.TryGetComponent<Rigidbody>(out var rb) && grenade is Scp018Projectile)
            {
                item.PropelBody(rb, new Vector3(2, 0, 0), new Vector3(3, 0, 9));
            }
            return grenade;

        }
        
        public static ThrownProjectile SpawnActive(
            this ThrowableItem item,
            Vector3 position,
            Footprint print,
            float fuseTime = -1f
        )
        {
            TimeGrenade grenade = (TimeGrenade)Object.Instantiate(item.Projectile, position, Quaternion.identity);
            if (fuseTime >= 0)
                grenade._fuseTime = fuseTime;
            grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
            grenade.PreviousOwner = print;
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
            if (grenade.TryGetComponent<Rigidbody>(out var rb) && grenade is Scp018Projectile)
            {
                item.PropelBody(rb, new Vector3(2, 0, 0), new Vector3(3, 0, 9));
            }
            return grenade;

        }
        
        public static ThrownProjectile SpawnActive(
            this ThrowableItem item,
            Vector3 position,
            ReferenceHub owner = null,
            float fuseTime = -1f
        )
        {
            TimeGrenade grenade = (TimeGrenade)Object.Instantiate(item.Projectile, position, Quaternion.identity);
            if (fuseTime >= 0)
                grenade._fuseTime = fuseTime;
            grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
            grenade.PreviousOwner = new Footprint(owner?? ReferenceHub._hostHub);
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
            if (grenade.TryGetComponent<Rigidbody>(out var rig) && grenade is Scp018Projectile)
            {
                rig.AddForce(new Vector3(2, 4, 0));
            }
            return grenade;
        }

        public static ThrowableItem CreateThrowable(ItemType type, Player player = null) =>
            (player?.ReferenceHub ?? ReferenceHub._hostHub).inventory.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;

        public static Firearm CreateFirearm(ItemType type, Player player = null) =>
            (player?.ReferenceHub ?? ReferenceHub._hostHub).inventory.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()), false) as Firearm;

        public static ItemBase CreateItem(ItemType type, Player player = null)
        {
            ReferenceHub target = player?.ReferenceHub;

            if (target == null && ReferenceHub.TryGetHostHub(out var host))
            {
                target = host;
            }

            if (target == null || target.inventory == null)
            {
                throw new TargetInvocationException(new Exception("Target or Target inventory was null"));
            }

            return target.inventory.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()), false);
        }

        public static ReadOnlyCollection<Pickup> GetPickups() => Pickup.List.ToList().AsReadOnly();

        [Obsolete("Use CreatePickup instead. This doesn't even work anymore")]
        public static string createItem(Vector3 pos, string itemID, int amount = 1)
        {
            Enum.TryParse(itemID, out ItemType type);
            InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase item);

            for (int i = 0; i < amount; i++)
            {
                ItemPickupBase spawnedItem = CreatePickup(pos, item);
                NetworkServer.Spawn(spawnedItem.gameObject);
            }

            if (amount != 1)
                return $"{type} spawned on {pos}, {amount} times";
            return $"{type} spawned on {pos}";
        }

        public static ItemPickupBase CreatePickup(Vector3 position, ItemBase prefab)
        {
            if (prefab == null || prefab.PickupDropModel == null)
            {
                return null;
            }

            ItemPickupBase pickup = Object.Instantiate(prefab.PickupDropModel, position, Quaternion.identity);
            pickup.gameObject.transform.position = position;
            pickup.NetworkInfo = new PickupSyncInfo(prefab.ItemTypeId, prefab.Weight, prefab.ItemSerial);

            if (prefab.OwnerInventory != null)
            {
                prefab.OwnerInventory.ServerRemoveItem(prefab.ItemSerial, pickup);
            }

            return pickup;
        }

        public static ItemPickupBase CreatePickup(ItemType type, Vector3 position, Action<ItemPickupBase> setupMethod = null)
        {
            if (type == ItemType.None || !InventoryItemLoader.AvailableItems.TryGetValue(type, out var item))
            {
                return null;
            }

            PickupSyncInfo pickupSyncInfo = default(PickupSyncInfo);
            pickupSyncInfo.ItemId = type;
            pickupSyncInfo.Serial = ItemSerialGenerator.GenerateNext();
            pickupSyncInfo.WeightKg = item.Weight;
            var pickup = InventoryExtensions.ServerCreatePickup(item, pickupSyncInfo, position, Quaternion.identity, false, setupMethod);
            NetworkServer.Spawn(pickup.gameObject);
            return pickup;
        }

        public static ItemPickupBase CreatePickup(ItemType type, Vector3 position, bool spawn, Action<ItemPickupBase> setupMethod = null)
        {
            if (type == ItemType.None || !InventoryItemLoader.AvailableItems.TryGetValue(type, out var item))
            {
                return null;
            }

            PickupSyncInfo pickupSyncInfo = default(PickupSyncInfo);
            pickupSyncInfo.ItemId = type;
            pickupSyncInfo.Serial = ItemSerialGenerator.GenerateNext();
            pickupSyncInfo.WeightKg = item.Weight;
            var pickup = InventoryExtensions.ServerCreatePickup(item, pickupSyncInfo, position, Quaternion.identity, spawn, setupMethod);
            NetworkServer.Spawn(pickup.gameObject);
            return pickup;
        }

        public static T CreatePickup<T>(Vector3 position, Action<T> setupMethod = null) where T : ItemPickupBase
        {
            if (!InventoryItemLoader.AvailableItems.TryGetFirst(x => x.Value.PickupDropModel is T, out var result))
            {
                return null;
            }

            PickupSyncInfo pickupSyncInfo = default(PickupSyncInfo);
            pickupSyncInfo.ItemId = result.Key;
            pickupSyncInfo.Serial = ItemSerialGenerator.GenerateNext();
            pickupSyncInfo.WeightKg = result.Value.Weight;
            var pickup = InventoryExtensions.ServerCreatePickup(result.Value, pickupSyncInfo, position, Quaternion.identity, false, (pickup) => setupMethod?.Invoke(pickup as T));
            NetworkServer.Spawn(pickup.gameObject);
            return pickup as T;
        }

        public static void logPlayerEffects(Player player)
        {
            Logging.Info($"Player: {player.Nickname}");
            Logging.Info($"Status Effects:");
            foreach (StatusEffectBase effect in player.ReferenceHub.playerEffectsController.AllEffects)
            {
                byte intensity = effect.Intensity;
                float duration = effect.Duration;
                Logging.Info($"Effect: {effect.name} \t Duration: {duration} \t Intensity: {intensity}");
            }
        }

        public static void addIntensity(this Player player, string effect, int intensity, int maxIntensity, float duration = 0f)
        {
            addIntensity(player.ReferenceHub, effect, intensity, maxIntensity, duration);
        }
        
        public static void addIntensity(this ReferenceHub player, string effect, int intensity, int maxIntensity, float duration = 0f)
        {
            player.playerEffectsController.TryGetEffect(effect, out StatusEffectBase effectBase);

            if (maxIntensity == 0 || maxIntensity > effectBase.MaxIntensity)
                maxIntensity = effectBase.MaxIntensity;

            effectBase.Intensity = (byte)Mathf.Clamp(effectBase.Intensity + intensity, 0, maxIntensity);
        }

        public static void addIntensity<T>(this Player player, int intensity, int maxIntensity = 0, float duration = 0f) where T : StatusEffectBase
        {
            addIntensity<T>(player.ReferenceHub, intensity, maxIntensity, duration);
        }
        
        public static void addIntensity<T>(this ReferenceHub player, int intensity, int maxIntensity = 0, float duration = 0f) where T : StatusEffectBase
        {
            player.playerEffectsController.TryGetEffect(out T effectBase);

            if (maxIntensity is 0 or > 255)
                maxIntensity = effectBase.MaxIntensity;

            effectBase.Intensity = (byte)Mathf.Clamp(effectBase.Intensity + intensity, 0, maxIntensity);

            if (duration == 0) return;
            effectBase.Duration = duration;
            effectBase.TimeLeft = duration;
        }
        
        public static void removeIntensity<T>(this ReferenceHub player, int intensity, int minIntensity = 0, float duration = 0f) where T : StatusEffectBase
        {
            player.playerEffectsController.TryGetEffect(out T effectBase);

            if (minIntensity < 0)
                minIntensity = 0;

            effectBase.Intensity = (byte)Mathf.Clamp(effectBase.Intensity - intensity, minIntensity, 255);

            if (duration == 0) return;
            effectBase.Duration = duration;
        }
        
        public static void removeIntensity<T>(this Player player, int intensity, int minIntensity = 0, float duration = 0f) where T : StatusEffectBase
        {
            player.ReferenceHub.removeIntensity<T>(intensity, minIntensity, duration);
        }



        public static bool getArrayText(ArraySegment<string> array, int indexToStartFrom, out string text, int lengthLimit = 0)
        {
            text = string.Empty;
            if (array.At(indexToStartFrom) == null)
                return false;

            for (int i = indexToStartFrom; i < array.Count; i++)
            {
                text += array.At(i);
                text += ' ';
            }

            text = text.TrimEnd();

            if (lengthLimit != 0 && text.Length > lengthLimit)
            {
                text = null;
                return false;
            }

            return true;
        }

        public static string GetEffectsHub(
            this ReferenceHub hub
            )

        {
            StatusEffectBase[] plrEffects = hub.playerEffectsController.AllEffects;

            StringBuilder sb = new StringBuilder();

            if (hub.roleManager.CurrentRole is SpectatorRole || !hub.IsAlive())
                return "";

            foreach (var effect in plrEffects)
            {
                if (effect.Intensity <= 0)
                {
                    continue;
                }

                string effectName;
                if (effect is Vitality)
                {
                    effectName = "Negative Effect Immunity";
                }
                else if (effect is ISpectatorDataPlayerEffect specData && specData.GetSpectatorText(out string data))
                {
                    effectName = data;
                }
                else
                {
                    effectName = effect switch
                    {
                        Invigorated => "Invigorated",
                        Invisible => "SCP-268",
                        RainbowTaste => "Rainbow Taste",
                        BodyshotReduction reduction => $"Damage Reduction (Body, -{Mathf.Round((1f - reduction.CurrentMultiplier) * 1000f) / 10f}%)",
                        DamageReduction => $"Damage Reduction(All, {effect.Intensity * 0.5}%)",
                        MovementBoost => $"{effect.Intensity}% Movement Boost",
                        Slowness => $"{effect.Intensity}% Slowness",
                        Scp1853 => $"SCP-1853 ({get1853Danger(hub)} Danger)",
                        SpawnProtected => "Spawn Protection",
                        Scp207 => $"{effectIntensityDisplay(effect.Intensity)}SCP-207 ({effect.getSpeedMult()}, {effect.getHealthMod()})",
                        AntiScp207 => $"{effectIntensityDisplay(effect.Intensity)}SCP-207? ({effect.getSpeedMult()}, {effect.getHealthMod()})",
                        AmnesiaVision => "Amnesia Vision",
                        AmnesiaItems => "Amnesia Items",
                        Asphyxiated => "Asphyxiated",
                        Bleeding => $"Bleeding ({effect.getHealthMod()})",
                        Blindness => $"Blindness ({effect.Intensity})",
                        Burned => "Burned (+25% Damage)",
                        Concussed => "Concussed",
                        Corroding => $"Corroding ({effect.getHealthMod()})",
                        PocketCorroding => $"Corroding (Pocket Dimension, {effect.getHealthMod()})",
                        Deafened => "Deafened",
                        Decontaminating => "Decontaminating",
                        Disabled => "Disabled (-20% Movement Boost)",
                        Ensnared => "Ensnared",
                        Exhausted => "Exhausted (-50% MAX Stamina, -100% Stamina Regen)",
                        Flashed => "Flashed",
                        Hemorrhage => $"Hemorrhage ({effect.getHealthMod()})",
                        Hypothermia => "Hypothermia",
                        Poisoned => $"Poisoned ({effect.getHealthMod()})",
                        Sinkhole => "Sinkhole",
                        Stained => "Stained (-20% Movement Boost)",
                        Traumatized => "Traumatized",
                        CardiacArrest => "Cardiac Arrest",
                        Scanned => "Scanned",
                        SilentWalk => "Silent Walk",

                        Scp1344 => "SCP-1344",
                        Blurred => "Blurred",

                        // Halloween Effects

                        // Spicy => "Spicy",
                        Ghostly => "Ghostly",
                        // OrangeCandy => "Orange Candy (God Light)",
                        MarshmallowEffect => "MarshmallowEffect",
                        // SugarRush => $"Sugar Rush ({effect.getSpeedMult()})",
                        // SugarHigh => "Sugar High",
                        // SugarCrave => "Sugar Crave",
                        // OrangeWitness => "Orange Witness",
                        // TraumatizedByEvil => "Traumatized (Evil)",
                        // SlowMetabolism => "Slow Metabolism",
                        // TemporaryBypass => "Bypass (Temp)",
                        // Metal => "Metal",
                        // Prismatic => String.Format("Prismatic ({0})", effect.getHealthMod()),
                        _ => ""
                    };
                }

                if (effectName == "")
                    continue;

                System.Drawing.Color effectColor = effect.Classification switch
                {
                    StatusEffectBase.EffectClassification.Technical => System.Drawing.Color.FromArgb(255, 242, 219, 92),
                    StatusEffectBase.EffectClassification.Negative => System.Drawing.Color.FromArgb(255, 255, 0, 0),
                    StatusEffectBase.EffectClassification.Positive => System.Drawing.Color.FromArgb(255, 144, 238, 144),
                    StatusEffectBase.EffectClassification.Mixed => System.Drawing.Color.FromArgb(255, 255, 150, 222),
                    _ => System.Drawing.Color.FromArgb(0, 0, 0, 0),
                };

                sb.SetColor(effectColor)
                    .Append(effectName);

                if (effect.Duration != 0 && effectName != "")
                    sb.Append($" - " + (int)effect.TimeLeft + "s");

                sb.CloseColor()
                    .AddLinebreak();
            }

            /*
            if (AudioManager.ActiveFakes.Contains(hub))
            {
                return sb.ToString();
            }
            try
            {
                if (CustomEffectsController.TryGet(hub, out CustomEffectsController controller))
                {
                    foreach (CustomEffectBase effect in controller.AllEffects)
                    {
                        if (!effect.IsEnabled)
                        {
                            continue;
                        }

                        string effectName = effect switch
                        {
                            _ => effect.GetSpectatorText(),
                        };

                        System.Drawing.Color effectColor = effect.Classification switch
                        {
                            CustomEffectBase.EffectClassification.Technical=> System.Drawing.Color.FromArgb(255, 242, 219, 92),
                            CustomEffectBase.EffectClassification.Negative=> System.Drawing.Color.FromArgb(255, 255, 0, 0),
                            CustomEffectBase.EffectClassification.Positive=> System.Drawing.Color.FromArgb(255, 144, 238, 144),
                            CustomEffectBase.EffectClassification.Mixed=> System.Drawing.Color.FromArgb(255, 255, 150, 222),
                            _ => System.Drawing.Color.FromArgb(0, 0, 0, 0),
                        };

                        sb.SetColor(effectColor)
                            .Append(effectName);

                        if (effect.Duration != 0 && effectName != "")
                            sb.Append($" - " + (int)effect.TimeLeft + "s");

                        sb.CloseColor()
                            .AddLinebreak();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error($"[EffectList - CustomEffects] Failed to list custom effects\n" + ex);
            }
            */
            return sb.ToString();
        }

        public static float get1853Danger(ReferenceHub hub)
        {
            float currentDanger = hub.playerEffectsController.GetEffect<Scp1853>().CurrentDanger;

            return currentDanger;
        }

        public static string effectIntensityDisplay(float Intensity)
        {
            if (Mathf.Approximately(Intensity, 1))
                return "";
            else
                return $"{Intensity}x ";
        }

        // This is unneeded.... why did I make this?
        public static DisplayCore createCoreForPlayer(Player player)
        {
            ReferenceHub hub = player.ReferenceHub;
            DisplayCore core = DisplayCore.Get(hub);
            return core;
        }

        public static string getSpeedMult(this StatusEffectBase effect)
        {
            switch (effect)
            {
                case Scp207 cola:
                    {
                        return $"{cola.CurrentStack.SpeedMultiplier}x";
                    }

                case AntiScp207 antiCola:
                    {
                        return $"{antiCola.CurrentStack.SpeedMultiplier}x";
                    }

                

                // Halloween Effects

                /*
                case Prismatic prismatic:
                    {
                        return $"{prismatic.MovementSpeedMultiplier}x";
                    }

                case SugarRush sugarRush:
                    {
                        return $"{sugarRush.MovementSpeedMultiplier}x";
                    }
                */
                default:
                    return "";
            }
        }

        public static string getHealthMod(this StatusEffectBase effect)
        {
            switch (effect)
            {
                case Scp207 cola:
                    {
                        return $"-{cola.CurrentStack.DamageAmount * cola.GetMovementStateMultiplier()} HP/{(Mathf.Approximately(cola.TimeBetweenTicks, 1) ? "" : cola.TimeBetweenTicks)}s";
                    }

                case AntiScp207 antiCola:
                    {
                        if (antiCola.IsImmunityActive)
                            return "Immunity Active";
                        return $"+ {antiCola.CurrentStack.HealAmount} HP/{(Mathf.Approximately(antiCola.TimeBetweenTicks, 1) ? "" : antiCola.TimeBetweenTicks)}s";
                    }

                case Poisoned poison:
                    {
                        return $"- {poison.damagePerTick} HP/{(Mathf.Approximately(poison.TimeBetweenTicks, 1) ? "" : poison.TimeBetweenTicks)}s";
                    }

                case Hemorrhage hemorrhage:
                    {
                        float damage = hemorrhage.damagePerTick * RainbowTaste.CurrentMultiplier(hemorrhage.Hub);
                        return $"- {damage:F2} HP/{(Mathf.Approximately(hemorrhage.TimeBetweenTicks, 1) ? "" : hemorrhage.TimeBetweenTicks)}s";
                    }

                case PocketCorroding pocket:
                    {
                        return $"- {pocket._damagePerTick:F1} HP/{(Mathf.Approximately(pocket.TimeBetweenTicks, 1) ? "" : pocket.TimeBetweenTicks)}s";
                    }

                case Corroding corroding:
                    {
                        float damage = Corroding.DamagePerTick;
                        return $"- {damage:F1} HP/{(Mathf.Approximately(corroding.TimeBetweenTicks, 1) ? "" : corroding.TimeBetweenTicks)}s";
                    }

                case Bleeding bleeding:
                    {
                        return $"- {bleeding.damagePerTick:F1} HP/{(Mathf.Approximately(bleeding.TimeBetweenTicks, 1)? "" : bleeding.TimeBetweenTicks)}s";
                    }

                // Halloween Effects

                    /*
                case Prismatic prismatic:
                    {
                        if (prismatic.IsImmunityActive)
                            return "Immunity Active";
                        return $"+ {Prismatic.HealingPerTick} HP/s";
                    }
                    */
                default:
                    return "";
            }
        }

        public static void spawnItemsAllRoom()
        {
            foreach (var room in Map.Rooms)
            {
                switch (room.Zone)
                {
                    case FacilityZone.LightContainment:
                        if (DecontaminationController.Singleton.IsDecontaminating)
                        {
                            break;
                        }
                        Timing.RunCoroutine(spawnItems(room.Base));
                        break;
                    
                    case FacilityZone.Surface:
                        Timing.RunCoroutine(spawnItems(room.Base));
                        break;
                    
                    default:
                        if (AlphaWarheadController.Detonated)
                        {
                            break;
                        }
                        
                        Timing.RunCoroutine(spawnItems(room.Base));
                        break;
                }
            }
        }

        private static IEnumerator<float> spawnItems(RoomIdentifier room)
        {
            Vector3 spawnArea = room.transform.position;
            for (int i = 0; i < 5; i++)
            {
                int randRadius = Random.Range(-5, 5);

                Vector3 itemArea = new(spawnArea.x + randRadius, spawnArea.y + randRadius, spawnArea.z);

                ItemType randItem = ItemPoolGen();
                int randAmount = Random.Range(1, 3);
                
                // The ONLY time this function should ever be called
                
                for (int j = 0; j < randAmount; j++)
                {
                    CreatePickup(randItem, itemArea + UnityCirclePos(2f), true);
                }
            }

            yield return Timing.WaitForSeconds(3f);
        }

        public static readonly int[] randomItemPool = {
            600, // Scp207
            500, // Janitor Keycard
            450, // Scientist Keycard
                 // Uncommon Custom Item
            400, // Com-15
            300, // Jailbird
            300, // Micro HID
            250, // Particle Disruptor
                 // Rare Custom Item
            200, // O5 Keycard
                 // Legendary Custom Item
            200, // Anti207
            150 // Com-45
        };

        public static readonly ItemType[] itemPoolID =
        {
            // "34" Painkillers
            // "14" Medkit
            // "33" Adrenaline
            // "17"´Scp500
            ItemType.SCP207, // Scp207
            ItemType.KeycardJanitor,  // Janitor Keycard
            ItemType.KeycardScientist,  // Scientist Keycard
                  // Uncommon Custom Item
            ItemType.GunCOM15, // Com-15
            ItemType.Jailbird, // Jailbird
            ItemType.MicroHID, // Micro HID
            ItemType.ParticleDisruptor, // Particle Disruptor
                  // Rare Custom Item
            ItemType.KeycardO5, // O5 Keycard
                  // Legendary Custom Item
            ItemType.AntiSCP207, // Anti207
            ItemType.GunCom45 // Com-45
        };

        public static ItemType ItemPoolGen()
        {
            int total = randomItemPool.Sum();

            int randomNumber = Random.Range(0, total);
            int firstRandomItem = Random.Range(1, 8);

            return firstRandomItem switch
            {
                1 => ItemType.Painkillers,
                2 => ItemType.Medkit,
                3 => ItemType.Adrenaline,
                4 => ItemType.SCP500,
                _ => testFunction(randomNumber, randomItemPool)
            };
        }

        public static ItemType testFunction(int randomNumber, int[] ItemPool)
        {
            for (int i = 0; i < ItemPool.Length; i++)
            {
                if (randomNumber >= ItemPool[i])
                {
                    return itemPoolID[i];
                }
                else
                {
                    randomNumber -= ItemPool[i];
                }
            }

            return ItemType.None;
        }

        public static T RandomEnumValue<T>()
        {
            System.Random _R = new();
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

        public static IList<T> CollectAllowedElements<T>(IList<T> allElements, IList<T> excludedElements)
        {
            List<T> allowedElements = new List<T>();
            foreach (T element in allElements)
                if (!excludedElements.Contains(element))
                    allowedElements.Add(element);
            return allowedElements;
        }

        public static T SelectRandomElement<T>(IList<T> allowedElements)
        {
            System.Random random = new();
            int randomIndex = random.Next(allowedElements.Count);
            return allowedElements[randomIndex];
        }

        public static T SelectRandomElement<T>(IList<T> allElements, IList<T> excludedElements)
        {
            IList<T> allowedElements = CollectAllowedElements(allElements, excludedElements);
            return SelectRandomElement(allowedElements);
        }

        public static void grenadeOverload(Vector3 position, Player plr = null)
        {
            Timing.CallDelayed(0.01f, () =>
            {
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, 0.01f, plr);
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, 0.01f, plr);
            });

            Timing.CallDelayed(0.5f, () =>
            {
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, 0.01f, plr);
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, 0.01f, plr);
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, 0.01f, plr);
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, 0.01f, plr);
            });


            Timing.CallDelayed(1f, () =>
            {
                Vector3 tripleBang = position;
                tripleBang.x += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(tripleBang, 0.01f, plr);
                tripleBang.x -= 2f;
                tripleBang.y += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(tripleBang, 0.01f, plr);
                tripleBang.y -= 2f;
                tripleBang.z += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(tripleBang, 0.01f, plr);
            });

        }
        
        public static void grenadeOverload(Vector3 position, ReferenceHub plr = null)
        {
            Timing.CallDelayed(0.01f, () =>
            {
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, plr, 0.01f);
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, plr, 0.01f);
            });

            Timing.CallDelayed(0.5f, () =>
            {
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, plr, 0.01f);
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, plr, 0.01f);
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, plr, 0.01f);
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, plr, 0.01f);
            });


            Timing.CallDelayed(1f, () =>
            {
                Vector3 tripleBang = position;
                tripleBang.x += 2f;
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, plr, 0.01f);
                tripleBang.x -= 2f;
                tripleBang.y += 2f;
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, plr, 0.01f);
                tripleBang.y -= 2f;
                tripleBang.z += 2f;
                CreateThrowable(ItemType.GrenadeFlash).SpawnActive(position, plr, 0.01f);
            });

        }
        
        public static void grenadeOverload(Vector3 position, Footprint print)
        {
            Timing.CallDelayed(0.01f, () =>
            {
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
            });

            Timing.CallDelayed(0.5f, () =>
            {
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
            });


            Timing.CallDelayed(1f, () =>
            {
                Vector3 tripleBang = position;
                tripleBang.x += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
                tripleBang.x -= 2f;
                tripleBang.y += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
                tripleBang.y -= 2f;
                tripleBang.z += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, print, 0.01f);
            });

        }

        public static void superOverload(Vector3 position, Player plr = null)
        {
            Timing.CallContinuously(1f, () => CreateThrowable(ItemType.GrenadeHE).SpawnActive(position, 0.01f, plr));

            Timing.CallDelayed(1f, () =>
            {
                Vector3 tripleBang = position;
                tripleBang.x += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(tripleBang, 0.01f, plr);
                tripleBang.x -= 2f;
                tripleBang.y += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(tripleBang, 0.01f, plr);
                tripleBang.y -= 2f;
                tripleBang.z += 2f;
                CreateThrowable(ItemType.GrenadeHE).SpawnActive(tripleBang, 0.01f, plr);
            });

        }
        
        public static void nukeGrenade(Vector3 position, Player plr = null, int range = 3)
        {
            AlphaWarheadController.Singleton.RpcShake(true);

            Vector3 offsetPos = position;
            offsetPos.x -= (2f * range);
            offsetPos.z -= (2f * range);

            range = range * 2;

            for (int z = range - 1; z >= 0; z--)
            {
                for (int x = 0; x < range; x++)
                {
                    CreateThrowable(ItemType.GrenadeHE).SpawnActive(offsetPos, 0.01f, plr);
                    CreateThrowable(ItemType.GrenadeHE).SpawnActive(offsetPos, 0.01f, plr);
                    offsetPos.x += 2f;
                }

                offsetPos.z += 2f;
                offsetPos.x -= (2f * (range));
            }

            insideRadius<Deafened>(100f, position, 1, 20f);
            insideRadius<Concussed>(100f, position, 1, 20f);
            insideRadius<Burned>(100f, position, 1, 20f);
            //InsideRadius<RadiationEffect>(10f, position, 20, 5);
            insideRadius(7f, position, (target) =>
            {
                DisruptorDamageHandler handler = new(null, target.PlayerCameraReference.forward, -1f);
                target.playerStats.KillPlayer(handler);
            });

            AlphaWarheadController.Singleton.RpcShake(false);
        }

        /*
        public static void nukeGrenade2(Vector3 position, Player plr = null, int range = 3)
        {
            AlphaWarheadController.Singleton.RpcShake(true);

            Vector3 offsetPos = position;
            offsetPos.x -= (2f * range);
            offsetPos.z -= (2f * range);

            range = range * 2;

            for (int z = range - 1; z >= 0; z--)
            {
                for (int x = 0; x < range; x++)
                {
                    CreateThrowable(ItemType.GrenadeHE).SpawnActive(offsetPos, 0.01f, plr);
                    CreateThrowable(ItemType.GrenadeHE).SpawnActive(offsetPos, 0.01f, plr);
                    offsetPos.x += 2f;
                }

                offsetPos.z += 2f;
                offsetPos.x -= (2f * (range));
            }

            PrimitiveObjectToy primObject = PrimitiveHandler.spawnPrimitive(position, PrimitiveType.Cylinder, new Color(255f / 255f, 128f / 255f, 13f / 255f), new Vector3(25, 50, 25));

            insideRadius<Deafened>(100f, position, 1, 20f);
            insideRadius<Concussed>(100f, position, 1, 20f);
            insideRadius<Burned>(100f, position, 1, 20f);
            InsideRadius<RadiationEffect>(10f, position, 20, 5);
            insideRadius(7f, position, (target) =>
            {
                DisruptorDamageHandler handler = new(null, -target.PlayerCameraReference.forward, -1f);
                target.playerStats.KillPlayer(handler);
            });

            Timing.CallDelayed(10f, () => NetworkServer.Destroy(primObject.GameObject));

            AlphaWarheadController.Singleton.RpcShake(false);
        }*/

        public static void insideRadius(float radius, Vector3 origin, Action<ReferenceHub> action)
        {
            foreach (ReferenceHub player in ReferenceHub.AllHubs)
            {
                if (player.authManager.InstanceMode != CentralAuth.ClientInstanceMode.ReadyClient) continue;

                if (Vector3.Distance(player.transform.position, origin) <= radius)
                {
                    action(player);
                }
            }
        }
        
        public static void insideRadius<T>(float radius, Vector3 origin, byte intensity, float duration, bool addDuration = false) where T : StatusEffectBase
        {
            foreach (ReferenceHub player in ReferenceHub.AllHubs)
            {
                if (player.authManager.InstanceMode != CentralAuth.ClientInstanceMode.ReadyClient) continue;

                if (Vector3.Distance(origin, Player.Get(player).Position) <= radius)
                {
                    player.playerEffectsController.ChangeState<T>(intensity, duration, addDuration);
                }
            }
        }
        
        /*
        public static void InsideRadius<T>(float radius, Vector3 origin, byte intensity, float duration, bool addDuration = false) where T : CustomEffectBase
        {
            foreach (CustomPlayer player in CustomPlayer.AllPlayers)
            {
                if (player.ReferenceHub.authManager.InstanceMode != CentralAuth.ClientInstanceMode.ReadyClient) continue;

                if (Vector3.Distance(origin, player.Player.Position) <= radius)
                {
                    player.CustomEffects.ChangeState<T>(intensity, duration, addDuration);
                }
            }
        }
        */

        public static T getPrefab<T>() where T : Object
        {
            foreach (GameObject gameObject in NetworkClient.prefabs.Values)
            {
                if (gameObject.TryGetComponent(out T prefab))
                {
                    return prefab;
                }
            }

            return null;
        }

        public static LightSourceToy spawnLight(Transform transform, Color color, float intensity = -1f, float range = 1.35f)
        {
            LightSourceToy light = LightSourceToy.Create(transform, false);
            light.Color = color;
            light.Range = range;
            if (!Mathf.Approximately(intensity, -1f))
                light.Intensity = intensity;

            light.IsStatic = false;

            NetworkServer.Spawn(light.GameObject);
            return light;
        }

        public static LightSourceToy spawnLight(Vector3 position, Color color, float intensity = 0.1f, float range = 1.35f, bool increaseOverTime = false, float increaseTime = 10f)
        {
            LightSourceToy light = LightSourceToy.Create(position, Quaternion.identity, null, false);

            float addedRange = range * 0.05f;
            float addedIntensity = intensity * 0.1f;

            float setRange = addedRange;
            float setIntensity = addedIntensity;

            light.Color = color;
            light.Range = increaseOverTime ? 1 : range;
            light.Intensity = increaseOverTime ? 1 : intensity;
            light.ShadowType = LightShadows.Soft;
            light.Position = position;

            NetworkServer.Spawn(light.GameObject);

            if (!increaseOverTime)
            {
                return light;
            }

            Timing.CallPeriodically(increaseTime, 0.2f, () =>
            {
                try
                {
                    light.Color = color;
                    if (light.Range < range)
                    {
                        light.Range = setRange;

                        setRange += addedRange;
                    }

                    if (light.Intensity < intensity)
                    {
                        light.Intensity = setIntensity;
                        setIntensity += addedIntensity;
                    }

                    if (light.Range >= range && light.Intensity >= intensity)
                    {
                        Timing.KillCoroutines(Timing.CurrentCoroutine);
                    }

                    if (!MainStaticVars.Debug)
                    {
                        return;
                    }
                    
                    Logging.Warn($"NetId: {light.Base.netId}");
                    Logging.Info($"Light Source Position is {light.Position} | Intensity: {light.Intensity} | Range: {light.Range}");
                }
                catch (Exception ex)
                {
                    Logging.Warn($"NetId: {light.Base.netId}");
                    Logging.Error($"{ex.Message}: {ex.Source}\n{ex.InnerException}");
                }

            }, () =>
            {
                NetworkServer.Destroy(light.GameObject);
            });
            return light;

        }

        public static void destroyLight(LightSourceToy lightToy)
        {
            NetworkServer.Destroy(lightToy.GameObject);
        }

        public static void destroyLight(uint lightNetId)
        {
            AdminToys.LightSourceToy lightToy = Resources.FindObjectsOfTypeAll(typeof(AdminToys.LightSourceToy)).First(light => light is AdminToys.LightSourceToy lightToy && lightToy.netId == lightNetId) as AdminToys.LightSourceToy;
            NetworkServer.Destroy(lightToy?.gameObject);
        }


        public static Color ColorToRGB(System.Drawing.Color color)
        {
            return new Color(
                color.R / 255f,
                color.G / 255f,
                color.B / 255f,
                color.A / 255f
                );
        }

        public static System.Drawing.Color ColorFromRGB(Color color)
        {
            return System.Drawing.Color.FromArgb(
                Mathf.RoundToInt(color.a * 255f),
                Mathf.RoundToInt(color.r * 255f),
                Mathf.RoundToInt(color.b * 255f),
                Mathf.RoundToInt(color.g * 255f)
                );
        }

        public static void ColorToHSV(System.Drawing.Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static System.Drawing.Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return System.Drawing.Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return System.Drawing.Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return System.Drawing.Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return System.Drawing.Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return System.Drawing.Color.FromArgb(255, t, p, v);
            else
                return System.Drawing.Color.FromArgb(255, v, p, q);
        }

        public static float randomChance(float min, float max)
        {
            return Random.Range(min, max);
        }

        /*
        public static bool replacePlayer(Player oldPlayer, Player newPlayer)
        {
            try
            {
                if (oldPlayer == null || newPlayer == null) return false;
                if (oldPlayer.getCustomHealthStat() == null || oldPlayer.getCustomHealthStat() == null) return false;
                if (AudioManager.ActiveFakes.Contains(oldPlayer.ReferenceHub) || AudioManager.ActiveFakes.Contains(newPlayer.ReferenceHub)) return false;

                PlayerRoleBase oldRole = oldPlayer.ReferenceHub.roleManager.CurrentRole;
                Dictionary<ushort, ItemBase> oldItems = oldPlayer.ReferenceHub.inventory.UserInventory.Items;
                Dictionary<ItemType, ushort> oldAmmo = oldPlayer.ReferenceHub.inventory.UserInventory.ReserveAmmo;
                Vector3 oldPosition = oldPlayer.Position;
                Quaternion oldRotation = oldPlayer.Rotation;
                float oldHealth = oldPlayer.Health;
                float oldMaxHealth = oldPlayer.MaxHealth;
                float oldArtificialHealth = oldPlayer.ArtificialHealth;
                bool wasCuffed = oldPlayer.IsDisarmed;
                Player cuffedOwner = oldPlayer.DisarmedBy;
                string oldCustomInfo = oldPlayer.CustomInfo;

                Scp207Stack colaStack = default;
                bool colaEnabled = false;
                AntiScp207Stack antiColaStack = default;
                bool antiColaEnabled = false;

                bool needsDisguise = false;
                RoleTypeId disguise = RoleTypeId.None;

                bool isCustomZombie = false;

                bool usesUnitNames = false;
                byte unitName = 0;

                if (oldPlayer.TryGetEffect(out AntiScp207 antiCola) && antiCola.IsEnabled)
                {
                    antiColaStack = antiCola.CurrentStack;
                    antiColaEnabled = true;
                }
                
                if (oldPlayer.TryGetEffect(out Scp207 cola) && cola.IsEnabled)
                {
                    colaStack = cola.CurrentStack;
                    colaEnabled = true;
                }

                if (CriticalMultiplier.TryGetCriticals(oldPlayer.ReferenceHub, out var crits))
                {
                    CriticalsHandler.AddMultiplier(newPlayer.ReferenceHub, crits.Intensity);
                    CriticalsHandler.ResetMultiplier(oldPlayer.ReferenceHub);
                }

                if (Scp173S.Scp173Ss.Contains(oldPlayer.ReferenceHub))
                {
                    Scp173S.Scp173Ss.Add(newPlayer.ReferenceHub);
                    Scp173S.Scp173Ss.Remove(oldPlayer.ReferenceHub);
                    Scp173S.Spawn173S(newPlayer.ReferenceHub);
                }

                if (ZombieRolesController.ActiveSpecialZombies.TryGetValue(oldPlayer.ReferenceHub, out ZombieRoleTypes specialZombie))
                {
                    isCustomZombie = true;
                    ZombieRolesController.DeinitializeZombie(oldPlayer.ReferenceHub);
                }

                if (EventHandler.activeLarryOnehits.TryGetValue(oldPlayer.ReferenceHub, out var instakill))
                {
                    Timing.KillCoroutines(instakill);
                    EventHandler.activeLarryOnehits.Remove(oldPlayer.ReferenceHub);

                    CoroutineHandle larryInstakillAura = newPlayer.ReferenceHub.createAura("Traumatized");
                    EventHandler.activeLarryOnehits.Add(newPlayer.ReferenceHub, larryInstakillAura);
                }

                if (CustomPlayer.TryGet(oldPlayer.ReferenceHub, out CustomPlayer plr))
                {
                    if (ChaosSpyHandler.CISpies.TryGetValue(oldPlayer.ReferenceHub, out bool damagedMTF) && !damagedMTF)
                    {
                        ChaosSpyHandler.CISpies.Add(newPlayer.ReferenceHub, false);
                        newPlayer.SendConsoleMessage($"[DCReplace] You replaced a CISpy. You take no damage from MTF", "red");
                        Logging.Debug($"[DCReplace] Replacing CISpy ({oldPlayer.Nickname} -> {newPlayer.Nickname})", Plugin.Debug);

                        disguise = plr.Disguise;
                        needsDisguise = true;
                        ChaosSpyHandler.CISpies.Remove(oldPlayer.ReferenceHub);
                    }

                    if (SerpentHandler.Spies.Contains(oldPlayer.ReferenceHub))
                    {
                        disguise = plr.Disguise;
                        needsDisguise = true;
                        SerpentHandler.Spies.Add(newPlayer.ReferenceHub);
                        SerpentHandler.Spies.Remove(oldPlayer.ReferenceHub);
                    }
                }

                if (oldPlayer.RoleBase is HumanRole { UsesUnitNames: true } human)
                {
                    unitName = human.UnitNameId;
                    usesUnitNames = true;
                }

                newPlayer.ReferenceHub.roleManager.ServerSetRole(oldRole.RoleTypeId, isCustomZombie? RoleChangeReason.Revived : RoleChangeReason.Respawn, RoleSpawnFlags.None);
                newPlayer.DisableEffect<SpawnProtected>();

                if (usesUnitNames && newPlayer.RoleBase is HumanRole { UsesUnitNames: true } human2)
                {
                    human2.UnitNameId = unitName;
                }

                if (wasCuffed)
                {
                    newPlayer.IsDisarmed = true;
                    newPlayer.DisarmedBy = cuffedOwner;
                }

                if (isCustomZombie)
                {
                    ZombieRolesController.InitializeZombie(specialZombie, newPlayer.ReferenceHub);
                }

                if (oldRole is Scp079Role oldPcRole && newPlayer.ReferenceHub.roleManager.CurrentRole is Scp079Role pcRole)
                {
                    if (oldPcRole.SubroutineModule.TryGetSubroutine(out Scp079TierManager oldTierManager) && pcRole.SubroutineModule.TryGetSubroutine(out Scp079TierManager tierManager))
                    {
                        tierManager.TotalExp = oldTierManager.TotalExp;
                    }

                    if (oldPcRole.SubroutineModule.TryGetSubroutine(out Scp079AuxManager oldAuxManager) && pcRole.SubroutineModule.TryGetSubroutine(out Scp079AuxManager auxManager))
                    {
                        auxManager.CurrentAux = oldAuxManager.CurrentAux;
                    }

                    pcRole._curCamSync.CurrentCamera = oldPcRole.CurrentCamera;
                }
                else
                {
                    newPlayer.CustomInfo = oldCustomInfo;
                    newPlayer.ReferenceHub.TryOverridePosition(oldPosition);
                    newPlayer.Rotation = oldRotation;
                    newPlayer.Health = oldHealth;
                    newPlayer.MaxHealth = oldMaxHealth;
                    newPlayer.ArtificialHealth = oldArtificialHealth;

                    foreach (StatusEffectBase effect in oldPlayer.ReferenceHub.playerEffectsController.AllEffects)
                    {
                        newPlayer.ReferenceHub.playerEffectsController.TryGetEffect(effect.ToString(), out StatusEffectBase playerEffect);
                        playerEffect.ServerSetState(effect.Intensity, effect.Duration == 0? 0 : effect.TimeLeft);

                        if (playerEffect is Scp207 cola2 && colaEnabled)
                        {
                            cola2.CurrentStack = colaStack;
                        }
                        
                        if (playerEffect is AntiScp207 antiCola2 && antiColaEnabled)
                        {
                            antiCola2.CurrentStack = antiColaStack;
                        }
                    }

                    foreach (var item in oldItems.Values)
                    {
                        ItemBase newitem = newPlayer.ReferenceHub.inventory.ServerAddItem(item.ItemTypeId, 0);

                        if (newitem is Firearm firearm)
                        {
                            if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(newPlayer.ReferenceHub, out var value) && value.TryGetValue(newitem.ItemTypeId, out var value2))
                            {
                                firearm.ApplyAttachmentsCode(value2, reValidate: true);
                            }

                            if (firearm.TryGetModule(out MagazineModule ammo))
                            {
                                ammo.ServerSetInstanceAmmo(firearm.ItemSerial, ammo.AmmoMax);
                            }
                        }

                    }

                    foreach (var ammo in oldAmmo)
                    {
                        newPlayer.AddAmmo(ammo.Key, ammo.Value);
                    }
                }

                if (needsDisguise && disguise.GetTeam() != Team.Dead)
                {
                    Timing.CallDelayed(0.05f, () => newPlayer.ChangeAppearance(disguise));
                }

                newPlayer.SendBroadcast("You replaced a disconnected Player!", 5, Broadcast.BroadcastFlags.Normal, true);
                newPlayer.SendConsoleMessage("[DCReplace] You replaced " + oldPlayer.DisplayName + " as " + oldRole);

                foreach(Player spectator in Player.List.Where(player => !player.IsAlive))
                {
                    if (!oldPlayer.ReferenceHub.IsSpectatedBy(spectator.ReferenceHub))
                    {
                        continue;
                    }
                    
                    if (spectator.RoleBase is not SpectatorRole role)
                    {
                        continue;
                    }
                    
                    role.SyncedSpectatedNetId = newPlayer.NetworkId;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging.Error("[StandardHandler.replacePlayer] Failed \n" + ex);
                return false;
            }
        }
        
        
        public static bool replaceSCP(Player firstScp, Player secondScp)
        {
            if (firstScp.GameObject == secondScp.GameObject)
            {
                return false;
            }

            PlayerRoleBase firstRole = firstScp.ReferenceHub.roleManager.CurrentRole;
            PlayerRoleBase secRole = secondScp.ReferenceHub.roleManager.CurrentRole;

            Vector3 firstPosition = firstScp.Position;
            Vector3 secPosition = secondScp.Position;

            float firstHealth = firstScp.Health;
            float firstMaxHealth = firstScp.MaxHealth;
            float firstArtificialHealth = firstScp.ArtificialHealth;
            
            float secHealth = secondScp.Health;
            float secMaxHealth = secondScp.MaxHealth;
            float secArtificialHealth = secondScp.ArtificialHealth;

            // Only set if someone is switching from PC

            Scp079Role pcRole = firstRole as Scp079Role ?? secRole as Scp079Role;
            int totalExp = 0;
            float aux = 0;
            Scp079Camera cam = null;

            if (pcRole != null && pcRole.SubroutineModule.TryGetSubroutine(out Scp079TierManager tierManager) && pcRole.SubroutineModule.TryGetSubroutine(out Scp079AuxManager auxManager))
            {
                cam = pcRole.CurrentCamera;
                totalExp = tierManager.TotalExp;
                aux = auxManager.CurrentAux;
            }

            switch (firstRole)
            {
                case Scp079Role:
                    {
                        secondScp.SetRole(RoleTypeId.Scp079);

                        if (secondScp.RoleBase is not Scp079Role secPc)
                        {
                            break;
                        }
                        
                        secPc.SubroutineModule.TryGetSubroutine(out Scp079TierManager tierManager1);
                        secPc.SubroutineModule.TryGetSubroutine(out Scp079AuxManager auxManager1);
                        tierManager1.TotalExp = totalExp;
                        auxManager1.CurrentAux = aux;
                        secPc._curCamSync.CurrentCamera = cam;

                        break;
                    }

                case Scp173Role:
                    {
                        if (Scp173S.Scp173Ss.Contains(firstScp.ReferenceHub))
                        {
                            firstScp.CustomInfo = null!;

                            secondScp.SetRole(RoleTypeId.Scp173);
                            Scp173S.Spawn173S(secondScp.ReferenceHub);
                        }
                        else
                        {
                            secondScp.SetRole(RoleTypeId.Scp173);

                            secondScp.Health = firstHealth;
                            secondScp.ArtificialHealth = firstArtificialHealth;
                        }

                        secondScp.Position = firstPosition;

                        break;
                    }

                default:
                    {
                        secondScp.SetRole(firstRole.RoleTypeId);

                        if (!Mathf.Approximately(firstMaxHealth, 0))
                        {
                            secondScp.MaxHealth = firstMaxHealth;
                        }

                        secondScp.Health = firstHealth;
                        secondScp.ArtificialHealth = firstArtificialHealth;

                        secondScp.Position = firstPosition;
                        break;
                    }
            }
            
            switch (secRole)
            {
                case Scp079Role:
                    {
                        firstScp.SetRole(RoleTypeId.Scp079);

                        if (firstScp.RoleBase is not Scp079Role firstPc)
                        {
                            break;
                        }
                        
                        firstPc.SubroutineModule.TryGetSubroutine(out Scp079TierManager tierManager1);
                        firstPc.SubroutineModule.TryGetSubroutine(out Scp079AuxManager auxManager1);
                        tierManager1.TotalExp = totalExp;
                        auxManager1.CurrentAux = aux;
                        firstPc._curCamSync.CurrentCamera = cam;

                        break;
                    }

                case Scp173Role:
                    {
                        if (Scp173S.Scp173Ss.Contains(secondScp.ReferenceHub))
                        {
                            firstScp.CustomInfo = null!;

                            firstScp.SetRole(RoleTypeId.Scp173);
                            Scp173S.Spawn173S(firstScp.ReferenceHub);
                        }
                        else
                        {
                            firstScp.SetRole(RoleTypeId.Scp173);

                            firstScp.Health = secHealth;
                            firstScp.ArtificialHealth = secArtificialHealth;
                        }

                        firstScp.Position = secPosition;

                        break;
                    }

                default:
                    {
                        firstScp.SetRole(secRole.RoleTypeId);


                        if (!Mathf.Approximately(secMaxHealth, 0))
                        {
                            firstScp.MaxHealth = secMaxHealth;
                        }

                        firstScp.Health = secHealth;
                        firstScp.ArtificialHealth = secArtificialHealth;

                        firstScp.Position = secPosition;
                        break;
                    }
            }


            firstScp.SendBroadcast($"You swapped SCP with {secondScp.DisplayName}", 5, Broadcast.BroadcastFlags.Normal, true);
            firstScp.SendConsoleMessage("[SCPSwap] You swapped with " + secondScp.DisplayName + " as " + secRole.RoleName);
            
            secondScp.SendBroadcast($"You swapped SCP with {firstScp.DisplayName}", 5, Broadcast.BroadcastFlags.Normal, true);
            secondScp.SendConsoleMessage("[SCPSwap] You swapped with " + firstScp.DisplayName + " as " + firstRole.RoleName);

            Logging.Debug($"[SCPSwap] Swapped {firstScp.Nickname} ({firstRole.RoleName}) with {secondScp.Nickname} ({secRole.RoleName})", Plugin.Debug);
            return true;
        }*/

        public static void SetPlayerScale(Player target, Vector3 scale)
        {
            GameObject go = target.GameObject;
            if (go.transform.localScale == scale)
                return;
            try
            {
                NetworkIdentity identity = target.ReferenceHub.networkIdentity;
                go.transform.localScale = scale;
                foreach (Player player in Player.List)
                {
                    NetworkServer.SendSpawnMessage(identity, player.Connection);
                }
            }
            catch (Exception e)
            {
                Logging.Info($"Set Scale error: {e}");
            }
        }

        public static bool EventAttackerCheck(Player Attacker, DamageHandlerBase handler, bool checkFF = false)
        {
            if (handler is AttackerDamageHandler { IsSuicide: true } atHandler && (!checkFF || atHandler.IsFriendlyFire )|| handler is WarheadDamageHandler || Attacker == null)
            {
                return false;
            }

            return true;
        }

        public static DamageType getDamageType(this DamageHandlerBase Base)
        {
            switch (Base)
            {
                case CustomReasonDamageHandler:
                    return DamageType.Custom;

                case WarheadDamageHandler:
                    return DamageType.Warhead;

                case ExplosionDamageHandler:
                    return DamageType.Explosion;

                case Scp018DamageHandler:
                    return DamageType.Scp018;

                case RecontainmentDamageHandler:
                    return DamageType.Recontainment;

                case MicroHidDamageHandler:
                    return DamageType.MicroHid;

                case DisruptorDamageHandler:
                    return DamageType.ParticleDisruptor;

                case Scp939DamageHandler scp939Handler:
                    return scp939Handler.Scp939DamageType switch
                    {
                        Scp939DamageType.Claw => DamageType.Scp939Claw,
                        Scp939DamageType.LungeTarget => DamageType.Scp939Lunge,
                        Scp939DamageType.LungeSecondary => DamageType.Scp939Lunge,
                        _ => DamageType.Scp939                    
                    };

                case JailbirdDamageHandler:
                    return DamageType.Jailbird;

                case Scp3114DamageHandler scp3114DamageHandler:
                    return scp3114DamageHandler.Subtype switch
                    {
                        Scp3114DamageHandler.HandlerType.Strangulation => DamageType.Strangled,
                        Scp3114DamageHandler.HandlerType.SkinSteal => DamageType.Scp3114,
                        Scp3114DamageHandler.HandlerType.Slap => DamageType.Scp3114,
                        _ => DamageType.Unknown,
                    };

                case Scp049DamageHandler scp049DamageHandler:
                    return scp049DamageHandler.DamageSubType switch
                    {
                        Scp049DamageHandler.AttackType.CardiacArrest => DamageType.CardiacArrest,
                        Scp049DamageHandler.AttackType.Instakill => DamageType.Scp049,
                        Scp049DamageHandler.AttackType.Scp0492 => DamageType.Scp0492,
                        _ => DamageType.Unknown,
                    };

                case FirearmDamageHandler firearmHandler:
                    return firearmHandler.AmmoType switch
                    {
                        ItemType.Ammo9x19 => DamageType.Ammo9x19,
                        ItemType.Ammo556x45 => DamageType.Ammo556x45,
                        ItemType.Ammo762x39 => DamageType.Ammo762x39,
                        ItemType.Ammo44cal => DamageType.Ammo44cal,
                        ItemType.Ammo12gauge => DamageType.Ammo12gauge,

                        _ => DamageType.Firearm
                    };

                    /*
                case MetalPipeDamageHandler:
                    return DamageType.MetalPipe;

                case SilentDamageHandler:
                    return DamageType.SilentDeath;
                    */

                case ScpDamageHandler scpHandler:
                    {
                        return GetScpDamageType(scpHandler);
                    }

                case UniversalDamageHandler universal:
                    {
                        DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];
                        return GetTranslationDamageType(translation);
                    }

                default:
                    return DamageType.Unknown;

            }
        }

        public static DamageType GetScpDamageType(ScpDamageHandler handler)
        {
            switch (handler.Attacker.Role)
            {
                case RoleTypeId.Scp173:
                    return DamageType.Scp173;

                case RoleTypeId.Scp106:
                    return DamageType.Scp106;

                case RoleTypeId.Scp049:
                    return DamageType.Scp049;

                case RoleTypeId.Scp079:
                    return DamageType.TeslaGate;

                case RoleTypeId.Scp096:
                    return DamageType.Scp096;

                case RoleTypeId.Scp0492:
                    return DamageType.Scp0492;

                case RoleTypeId.Scp939:
                    return DamageType.Scp939;

                case RoleTypeId.Scp3114:
                    return DamageType.Scp3114;

                default:
                    return GetTranslationDamageType(DeathTranslations.TranslationsById[handler._translationId]);
            }
        }

        public static DamageType GetTranslationDamageType(DeathTranslation translation)
        {
            return translation.Id switch
            {
                0 => DamageType.Recontainment,
                1 => DamageType.Warhead,
                2 => DamageType.Scp049,
                3 => DamageType.Unknown,
                4 => DamageType.Asphyxiated,
                5 => DamageType.Bleeding,
                6 => DamageType.FallDamage,
                7 => DamageType.PocketDimension,
                8 => DamageType.Decontamination,
                9 => DamageType.Poison,
                10 => DamageType.Scp207,
                11 => DamageType.SeveredHands,
                12 => DamageType.MicroHid,
                13 => DamageType.TeslaGate,
                14 => DamageType.Explosion,
                15 => DamageType.Scp096,
                16 => DamageType.Scp173,
                17 => DamageType.Scp939Lunge,
                18 => DamageType.Scp0492,
                19 => DamageType.Firearm,
                20 => DamageType.Crushed,
                21 => DamageType.Recontainment,
                22 => DamageType.FriendlyFireDetector,
                23 => DamageType.Hypothermia,
                24 => DamageType.CardiacArrest,
                25 => DamageType.Scp939Claw,
                26 => DamageType.Scp3114,
                27 => DamageType.MarshmallowMan,
                _ => DamageType.Unknown
            };
        }

        public static void changeEventDamage(this DamageHandlerBase Base, float newDamage)
        {
            switch (Base)
            {
                case CustomReasonDamageHandler customHandler:
                    customHandler.Damage = newDamage;
                    return;

                case WarheadDamageHandler warheadHandler:
                    warheadHandler.Damage = newDamage;
                    return;

                case ExplosionDamageHandler explosionHandler:
                    explosionHandler.Damage = newDamage;
                    return;

                case Scp018DamageHandler scp018Handler:
                    scp018Handler.Damage = newDamage;
                    return;

                case RecontainmentDamageHandler recontainmentHandler:
                    recontainmentHandler.Damage = newDamage;
                    return;

                case MicroHidDamageHandler microHandler:
                    microHandler.Damage = newDamage;
                    return;

                case DisruptorDamageHandler disruptorHandler:
                    disruptorHandler.Damage = newDamage;
                    return;

                case Scp939DamageHandler scp939Handler:
                    scp939Handler.Damage = newDamage;
                    return;

                case JailbirdDamageHandler jailbirdHandler:
                    jailbirdHandler.Damage = newDamage;
                    return;

                case Scp3114DamageHandler scp3114DamageHandler:
                    scp3114DamageHandler.Damage = newDamage;
                    return;

                case Scp049DamageHandler scp049DamageHandler:
                    scp049DamageHandler.Damage = newDamage;
                    return;

                case ScpDamageHandler scpHandler:
                    scpHandler.Damage = newDamage;
                    return;
                
                case AttackerDamageHandler attHandler:
                    attHandler.Damage = newDamage;
                    return;

                case UniversalDamageHandler universal:
                    universal.Damage = newDamage;
                    return;

                default:
                    ((StandardDamageHandler)Base).Damage = newDamage;
                    return;
            }
        }

        public static void MessageTranslated(string message, string translation, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
        {
            StringBuilder announcement = StringBuilderPool.Shared.Rent();
            string[] cassies = message.Split('\n');
            string[] translations = translation.Split('\n');
            for (int i = 0; i < cassies.Length; i++)
                announcement.Append($"{translations[i].Replace(' ', ' ')}<size=0> {cassies[i]} </size><split>");

            RespawnEffectsController.PlayCassieAnnouncement(announcement.ToString(), isHeld, isNoisy, isSubtitles);
            StringBuilderPool.Shared.Return(announcement);
        }

        public static void SendFakeSyncVar<T>(this Player target, NetworkIdentity behaviorOwner, Type targetType, string propertyName, T value)
        {
            if (target.GameObject == null)
                return;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            NetworkWriterPooled writer2 = NetworkWriterPool.Get();
            MakeCustomSyncWriter(behaviorOwner, targetType, null, CustomSyncVarGenerator, writer, writer2);
            target.Connection.Send(new EntityStateMessage
            {
                netId = behaviorOwner.netId,
                payload = writer.ToArraySegment(),
            });

            NetworkWriterPool.Return(writer);
            NetworkWriterPool.Return(writer2);
            void CustomSyncVarGenerator(NetworkWriter targetWriter)
            {
                targetWriter.WriteULong(SyncVarDirtyBits[$"{targetType.Name}.{propertyName}"]);
                WriterExtensions[typeof(T)]?.Invoke(null, new object[] { targetWriter, value });
            }
        }

        private static void MakeCustomSyncWriter(NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncObject, Action<NetworkWriter> customSyncVar, NetworkWriter owner, NetworkWriter observer)
        {
            ulong value = 0;
            NetworkBehaviour behaviour = null;

            // Get NetworkBehaviors index (behaviorDirty use index)
            for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
            {
                if (behaviorOwner.NetworkBehaviours[i].GetType() != targetType) continue;
                behaviour = behaviorOwner.NetworkBehaviours[i];
                value = 1UL << (i & 31);
                break;
            }

            // Write target NetworkBehavior's dirty
            Compression.CompressVarUInt(owner, value);

            // Write init position
            int position = owner.Position;
            owner.WriteByte(0);
            int position2 = owner.Position;

            // Write custom sync data
            if (customSyncObject is not null)
                customSyncObject(owner);
            else
                behaviour?.SerializeObjectsDelta(owner);

            // Write custom syncvar
            customSyncVar?.Invoke(owner);

            // Write syncdata position data
            int position3 = owner.Position;
            owner.Position = position;
            owner.WriteByte((byte)(position3 - position2 & 255));
            owner.Position = position3;

            // Copy owner to observer
            if (behaviour != null && behaviour.syncMode != SyncMode.Observers)
                observer.WriteBytes(owner.ToArraySegment().Array, position, owner.Position - position);
        }

        private static readonly Dictionary<string, ulong> SyncVarDirtyBitsValue = new();
        private static readonly ReadOnlyDictionary<string, ulong> ReadOnlySyncVarDirtyBitsValue = new(SyncVarDirtyBitsValue);

        private static readonly Dictionary<Type, MethodInfo> WriterExtensionsValue = new();
        private static readonly ReadOnlyDictionary<Type, MethodInfo> ReadOnlyWriterExtensionsValue = new(WriterExtensionsValue);
        public static ReadOnlyDictionary<string, ulong> SyncVarDirtyBits
        {
            get
            {
                if (SyncVarDirtyBitsValue.Count == 0)
                {
                    foreach (PropertyInfo property in typeof(ServerConsole).Assembly.GetTypes()
                        .SelectMany(x => x.GetProperties())
                        .Where(m => m.Name.StartsWith("Network")))
                    {
                        MethodInfo setMethod = property.GetSetMethod();

                        if (setMethod is null)
                            continue;

                        MethodBody methodBody = setMethod.GetMethodBody();

                        if (methodBody is null)
                            continue;

                        byte[] bytecodes = methodBody.GetILAsByteArray();

                        if (!SyncVarDirtyBitsValue.ContainsKey($"{property.ReflectedType?.Name}.{property.Name}"))
                            SyncVarDirtyBitsValue.Add($"{property.ReflectedType?.Name}.{property.Name}", bytecodes[bytecodes.LastIndexOf((byte)OpCodes.Ldc_I8.Value) + 1]);
                    }
                }

                return ReadOnlySyncVarDirtyBitsValue;
            }
        }

        public static ReadOnlyDictionary<Type, MethodInfo> WriterExtensions
        {
            get
            {
                if (WriterExtensionsValue.Count != 0)
                {
                    return ReadOnlyWriterExtensionsValue;
                }
                
                foreach (MethodInfo method in typeof(NetworkWriterExtensions).GetMethods().Where(x => !x.IsGenericMethod && x.GetCustomAttribute(typeof(ObsoleteAttribute)) == null && (x.GetParameters()?.Length == 2)))
                    WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                Type fuckNorthwood = Assembly.GetAssembly(typeof(RoleTypeId)).GetType("Mirror.GeneratedNetworkCode");
                if (fuckNorthwood is null)
                {
                    return ReadOnlyWriterExtensionsValue;
                }
                foreach (MethodInfo method in fuckNorthwood.GetMethods().Where(x => !x.IsGenericMethod && x.GetParameters().Length == 2 && x.ReturnType == typeof(void)))
                    WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                foreach (Type serializer in typeof(ServerConsole).Assembly.GetTypes().Where(x => x.Name.EndsWith("Serializer")))
                {
                    foreach (MethodInfo method in serializer.GetMethods().Where(x => (x.ReturnType == typeof(void)) && x.Name.StartsWith("Write")))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);
                }

                return ReadOnlyWriterExtensionsValue;
            }
        }

        public static void SendBroadcast(this ReferenceHub hub, string broadcast, ushort duration, Broadcast.BroadcastFlags flags = Broadcast.BroadcastFlags.Normal, bool clearPrevious = false)
        {
            if (clearPrevious) Broadcast.Singleton.TargetClearElements(hub.authManager.connectionToClient);

            Broadcast.Singleton.TargetAddElement(hub.authManager.connectionToClient, broadcast, duration, flags);
        }

        public static string ReconstructQuotedString(List<string> segment, ref int index)
        {
            List<string> reconstructed = new List<string>();
            bool inQuotes = segment[index].StartsWith("\"");
            while (index < segment.Count)
            {
                string part = segment[index++];
                reconstructed.Add(part);
                if (inQuotes && part.EndsWith("\""))
                    break;
            }
            // Remove quotes and join parts
            return string.Join(" ", reconstructed).Trim('"');
        }

        public static float getDamage(this DamageHandlerBase handler)
        {
            if (handler is not StandardDamageHandler standard)
            {
                return 0;
            }

            return standard.Damage;
        }
        
        public static float getDealtDamage(this DamageHandlerBase handler)
        {
            if (handler is not StandardDamageHandler standard)
            {
                return 0;
            }

            return standard.DealtHealthDamage;
        }

        public static Component AddGameObjectComponent<T>(this Player plr) where T : Component
        {
            return plr.GameObject.AddComponent<T>();
        }
        
        public static Component AddGameObjectComponent<T>(this ReferenceHub hub) where T : Component
        {
            return hub.gameObject.AddComponent<T>();
        }
        
        public static Component AddGameObjectComponent<T>(this CustomPlayer plr) where T : Component
        {
            return plr.GameObject.AddComponent<T>();
        }
        
        public static void RemoveGameObjectComponent<T>(this Player plr) where T : Component
        {
            Object.Destroy(plr.GameObject.GetComponent<T>());
        }
        
        public static void RemoveGameObjectComponent<T>(this ReferenceHub hub) where T : Component
        {
            Object.Destroy(hub.gameObject.GetComponent<T>());
        }
        
        public static void RemoveGameObjectComponent<T>(this CustomPlayer plr) where T : Component
        {
            Object.Destroy(plr.GameObject.GetComponent<T>());
        }

        public static Vector3 GetDoorPosInRoom(this Room room, DoorPermissionFlags permissions)
        {
            var doors = room.Doors.Where(d => d.Permissions == permissions && !d.IsLocked && d is not ElevatorDoor).ToList();
            return doors.ElementAt(new System.Random().Next(doors.ToList().Count)).Position;
        }
        
        public static Vector3 GetDoorPosInRoom(this Room room)
        {
            var doors = room.Doors.Where(d => !d.IsLocked && d is not ElevatorDoor).ToList();
            return doors.ElementAt(new System.Random().Next(doors.ToList().Count)).Position;
        }

        public static Door GetDoorInRoom(this Room room, DoorPermissionFlags permissions)
        {
            var doors = room.Doors.Where(d =>
                d.Permissions == permissions && !d.IsLocked && d is not ElevatorDoor).ToList();
            return doors.ElementAt(new System.Random().Next(doors.ToList().Count));
        }

        public static Door GetDoorInRoom(this Room room)
        {
            var doors = room.Doors.Where(d => !d.IsLocked && d is not ElevatorDoor).ToList();
            return doors.ElementAt(new System.Random().Next(doors.ToList().Count));
        }

        public static DamageHandlerBase.HandlerOutput GetHandlerOutput(this StandardDamageHandler handler, ReferenceHub target)
        {
            float damage = handler.Damage;
            PlayerStats playerStats = target.playerStats;
            AhpStat ahpStat = playerStats.GetModule<AhpStat>();
            HealthStat healthStat = playerStats.GetModule<HealthStat>();
            HumeShieldStat humeStat = playerStats.GetModule<HumeShieldStat>();

            if (Mathf.Approximately(damage, -1f))
            {
                return DamageHandlerBase.HandlerOutput.Death;
            }

            foreach (var t in target.playerEffectsController.AllEffects)
            {
                if (t is IDamageModifierEffect { DamageModifierActive: true } damageModifierEffect)
                {
                    damage *= damageModifierEffect.GetDamageModifier(damage, handler, handler.Hitbox);
                }
            }

            if (damage <= 0f)
            {
                return DamageHandlerBase.HandlerOutput.Nothing;
            }

            float health = healthStat.CurValue;
            float newDamage = ahpStat.ServerProcessAhpDamage(damage);
            float newDamage2 = humeStat.CurValue - newDamage;
            health -= newDamage2;

            if (health > 0f)
            {
                return DamageHandlerBase.HandlerOutput.Damaged;
            }

            return DamageHandlerBase.HandlerOutput.Death;

        }

        private static float ServerProcessAhpDamage(this AhpStat stat, float damage)
        {
            if (damage <= 0f)
            {
                return damage;
            }

            foreach (AhpStat.AhpProcess activeProcess in stat._activeProcesses)
            {
                float num = damage * activeProcess.Efficacy;
                if (num >= activeProcess.CurrentAmount)
                {
                    damage -= activeProcess.CurrentAmount;
                    continue;
                }

                return damage - num;
            }

            return damage;
        }

        public static string getMinutes(float timeInSeconds)
        {
            int minutes = (int)(timeInSeconds / 60);
            int seconds = (int)(timeInSeconds - (minutes * 60));
            return $"{minutes}:{seconds:D2}";
        }

        public static string getMinutes(double timeInSeconds)
        {
            int minutes = (int)(timeInSeconds / 60);
            int seconds = (int)(timeInSeconds - (minutes * 60));
            return $"{minutes}:{seconds:D2}";
        }

        public static System.Drawing.Color getColorFromTeam(Team team)
        {
            return team switch
            {
                Team.SCPs => System.Drawing.Color.Red,
                Team.FoundationForces => System.Drawing.Color.CadetBlue,
                Team.ChaosInsurgency => System.Drawing.Color.DarkGreen,
                Team.Scientists => System.Drawing.Color.Yellow,
                Team.ClassD => System.Drawing.Color.Orange,
                _ => System.Drawing.Color.DeepPink,
            };
        }

        public static string GetSCPName(RoleTypeId roleType)
        {
            return roleType switch
                {
                    RoleTypeId.Scp096 => "SCP 0 9 6",
                    RoleTypeId.Scp079 => "SCP 0 7 9",
                    RoleTypeId.Scp049 => "SCP 0 4 9",
                    RoleTypeId.Scp173 => "SCP 1 7 3",
                    RoleTypeId.Scp106 => "SCP 1 0 6",
                    RoleTypeId.Scp939 => "SCP 9 3 9",
                    RoleTypeId.Scp3114 => "SCP 3 1 1 4",
                    _ => "SCP 0 0 0",
                };
        }

        public static bool CanBeSpawned(ReferenceHub hub)
        {
            if (AudioManager.ActiveFakes.Contains(hub))
            {
                return false;
            }

            if (hub.serverRoles.IsInOverwatch)
            {
                return false;
            }

            if (hub.IsAlive())
            {
                return false;
            }

            if (hub.Mode != CentralAuth.ClientInstanceMode.ReadyClient)
            {
                return false;
            }

            return true;
        }

        public static bool CanBeSpawned(Player User) => CanBeSpawned(User.ReferenceHub); 
        
        public static void SetHostHitboxes(ReferenceHub target, bool state)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            IFpcRole fpcRole = target.roleManager.CurrentRole as IFpcRole;
            if (fpcRole == null)
            {
                return;
            }
            HitboxIdentity[] hitboxes = fpcRole.FpcModule.CharacterModelInstance.Hitboxes;
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].SetColliders(state);
            }
        }

        public static bool RaycastTest(Ray ray, out RaycastHit hitInfo, bool toggleHitboxes = true, float maxDistance = 10f, int mask = -5)
        {
            if (toggleHitboxes)
            {
                ReferenceHub.AllHubs.ForEach(x => SetHostHitboxes(x, false));
            }

            bool outcome = Physics.Raycast(ray, out hitInfo, maxDistance, mask);

            if (toggleHitboxes)
            {
                ReferenceHub.AllHubs.ForEach(x => SetHostHitboxes(x, true));
            }

            return outcome;
        }

        public static bool RaycastTest(Ray ray, out RaycastHit hitInfo, Func<Player, bool> toggleHitboxCheck, float maxDistance = 10f, int mask = -5)
        {
            List<ReferenceHub> targets = new();

            Player.List.ForEach(x =>
            {
                if (!toggleHitboxCheck(x))
                {
                    return;
                }

                SetHostHitboxes(x.ReferenceHub, false);
                targets.Add(x.ReferenceHub);
            });

            bool outcome = Physics.Raycast(ray, out hitInfo, maxDistance, mask);
            targets.ForEach(x => SetHostHitboxes(x, true));

            return outcome;
        }

        public static bool RaycastTest(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, bool toggleHitboxes = true, float maxDistance = 10f, int mask = -5) => RaycastTest(new Ray(origin, direction), out hitInfo, toggleHitboxes, maxDistance, mask);
        public static bool RaycastTest(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, Func<Player, bool> toggleCheck, float maxDistance = 10f, int mask = -5) => RaycastTest(new Ray(origin, direction), out hitInfo, toggleCheck, maxDistance, mask);
        public static bool RaycastTest(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, ReferenceHub user, float maxDistance = 10f, int mask = -5) => RaycastTest(new Ray(origin, direction), out hitInfo, (plr) => plr.ReferenceHub == user, maxDistance, mask);
        public static bool RaycastTest(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, Player user, float maxDistance = 10f, int mask = -5) => RaycastTest(new Ray(origin, direction), out hitInfo, (plr) => plr.ReferenceHub == user.ReferenceHub, maxDistance, mask);
        
        private static readonly System.Random Randomizer = new();
        public static double PseudoRandom(double max = 1.0)
        {
            return Randomizer.NextDouble() * max; // Returns a value between 0 and max
        }

        public static int RandomInt(int min, int max)
        {
            return Randomizer.Next(min, max + 1); // max is exclusive, so we add 1
        }

        public static bool RandomChance(double probability = 1.0, float chance = 1)
        {
            return PseudoRandom(probability) < chance;
        }

        public static string GetCameraRoomName(RoomIdentifier roomIdentifier)
        {
            if (roomIdentifier is null)
            {
                return "UNKNOWN";
            }
            Vector3 position = roomIdentifier.transform.position;

            bool flag = false;
            float num = 0f;
            Scp079Camera scp079Camera = null;
            Scp079Camera scp079Camera2 = null;
            foreach (Scp079InteractableBase allInstance in Scp079InteractableBase.AllInstances)
            {
                if (allInstance is not Scp079Camera scp079Camera3 || scp079Camera3.Room != roomIdentifier)
                {
                    continue;
                }
                
                float sqrMagnitude = (scp079Camera3.Position - position).sqrMagnitude;
                if (scp079Camera3.IsMain)
                {
                    scp079Camera2 = scp079Camera3;
                }

                if ((flag && sqrMagnitude > num) || Physics.Linecast(scp079Camera3.Position, position, out _, 1))
                {
                    continue;
                }
                scp079Camera = scp079Camera3;
                flag = true;
                num = sqrMagnitude;
            }

            if (flag)
            {
                return scp079Camera.Label;
            }
            
            if (scp079Camera2 is null)
            {
                return "UNKNOWN";
            }

            return scp079Camera2.Label;

        }

        public static float GetHpPercent(this ReferenceHub hub, float percentage)
        {
            return (hub.GetHealthStat().MaxValue * percentage) / 100;
        }

        public static float GetHpPercent(this Player plr, float percentage)
        {
            return (plr.MaxHealth * percentage) / 100;
        }

        public static Vector3 UnityCirclePos(float range, float z = 0)
        {
            var unitCircle = Random.insideUnitCircle * range;
            return new Vector3(unitCircle.x, z, unitCircle.y);
        }
    }
}
