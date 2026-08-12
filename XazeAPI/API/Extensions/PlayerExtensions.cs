// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using EclipsePlugin.API.CustomModules;
using ProjectMER.Commands.Modifying.Scale.SubCommands;
using XazeAPI.API.EffectStacks;
using XazeAPI.API.Events;
using XazeAPI.API.Events.Handler;
using XazeAPI.API.Helpers;

namespace XazeAPI.API.Extensions
{
    using CustomPlayerEffects;
    using Footprinting;
    using InventorySystem;
    using InventorySystem.Disarming;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.ShotEvents;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.Ragdolls;
    using PlayerRoles.Spectating;
    using PlayerStatsSystem;
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using LabApi.Features.Wrappers;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using PlayerRoles.FirstPersonControl;

    public static class PlayerExtensions
    {
        extension(ReferenceHub target)
        {
            public void SendConsoleMessage(string message, string color) => target.gameConsoleTransmission.SendToClient(message, color);

            public CoroutineHandle createAura(string effectName = null, Action customFunction = null)
            {
                customFunction?.Invoke();

                return Timing.CallPeriodically(10000f, 0.5f, () =>
                {

                    if (!target.IsAlive())
                    {
                        return;
                    }

                    foreach (Player player in Player.List)
                    {
                        if (player.ReferenceHub == target || target.IsSCP() && player.IsSCP)
                        {
                            continue;
                        }

                        if (!(Vector3.Distance(target.transform.position, player.ReferenceHub.transform.position) <=
                              5)) continue;
                        if (effectName == null) continue;
                        PlayerEffectsController effectController = player.ReferenceHub.playerEffectsController;

                        effectController.ChangeState(effectName, 1);
                    }
                }, () => target.createAura(effectName, customFunction));
            }

            public CoroutineHandle createAura(DamageHandlerBase handler = null, string effectName = null, Func<bool> customFunction = null)
            {
                Footprint footprint = new Footprint(target);
                CoroutineHandle handle = Timing.CallPeriodically(10f, 0.5f, () =>
                {
                    if (customFunction != null)
                    {
                        if (!customFunction())
                        {
                            return;
                        }
                    }

                    if (!footprint.Role.IsAlive())
                    {
                        return;
                    }

                    foreach (Player player in Player.List)
                    {
                        if (player.ReferenceHub == footprint.Hub || footprint.Role.GetTeam() == player.Team)
                        {
                            continue;
                        }

                        if (!(Vector3.Distance(target.transform.position, player.Position) <= 5)) continue;
                        if (effectName != null)
                        {
                            PlayerEffectsController effectController = player.ReferenceHub.playerEffectsController;

                            effectController.ChangeState(effectName, 1);
                        }

                        if (handler == null)
                        {
                            try
                            {
                                DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), footprint, DisruptorActionModule.FiringState.FiringSingle), player.Camera.forward, -1f);
                                player.ReferenceHub.playerStats.DealDamage(vaporizeHandler);
                            }
                            catch (Exception ex)
                            {
                                ErrorHelper.ErrorLogStyling(ex, $"{MethodBase.GetCurrentMethod().Name} failed");
                            }
                        }
                        else
                        {
                            player.ReferenceHub.playerStats.DealDamage(handler);
                        }
                    }
                }, () => target.createAura(handler, effectName, customFunction));
                return handle;
            }

            public CoroutineHandle createAura(float distance, Action<Player> customFunction)
            {
                Footprint footprint = new Footprint(target);
                CoroutineHandle handle = Timing.CallPeriodically(10f, 0.5f, () =>
                {
                    if (!footprint.Role.IsAlive())
                    {
                        return;
                    }

                    foreach (Player player in Player.List)
                    {
                        if (player.ReferenceHub == footprint.Hub || footprint.Role.GetTeam() == player.Team)
                        {
                            continue;
                        }

                        if (Vector3.Distance(target.transform.position, player.Position) <= distance)
                        {
                            customFunction(player);
                        }
                    }
                }, () => target.createAura(distance, customFunction));
                return handle;
            }
        }

        /// <param name="attacker">Target which gets vaporized</param>
        extension(Player attacker)
        {
            public CoroutineHandle createAura(float damageMultiplier, float damagePerMultiplier, DeathTranslation deathTranslation, Action customFunction = null)
            {
                return Timing.CallPeriodically(10000f, 0.5f, () =>
                {
                    customFunction?.Invoke();

                    if (!attacker.IsAlive)
                    {
                        return;
                    }

                    foreach (Player player in Player.List)
                    {
                        if (player.ReferenceHub == attacker.ReferenceHub || attacker.IsSCP && player.IsSCP)
                        {
                            continue;
                        }

                        if (!(Vector3.Distance(attacker.ReferenceHub.transform.position,
                                player.ReferenceHub.transform.position) <= 5)) continue;
                    
                        float damage = damageMultiplier * damagePerMultiplier;
                        UniversalDamageHandler handler = new(damage, deathTranslation);
                        player.ReferenceHub.playerStats.DealDamage(handler);
                    }
                }, () => attacker.createAura(damageMultiplier, damagePerMultiplier, deathTranslation, customFunction));

            }

            public HealthStat GetHealthStat() => attacker.ReferenceHub.GetHealthStat();
            public void changeMaxHealth(float newMaxHealth) => attacker.ReferenceHub.changeMaxHealth(newMaxHealth);

            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            public void VaporizePlayer()
            {
                DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(attacker.ReferenceHub), DisruptorActionModule.FiringState.FiringSingle), attacker.Camera.forward, -1f);
                attacker.ReferenceHub.playerStats.KillPlayerWithEvents(vaporizeHandler);
            }
        }

        extension(ReferenceHub hub)
        {
            public HealthStat GetHealthStat()
            {
                return hub.playerStats.GetModule<HealthStat>();
            }

            public void changeMaxHealth(float newMaxHealth)
            {
                hub.playerStats.GetModule<HealthStat>().MaxValue = newMaxHealth;
            }
        }

#nullable enable
        public static CustomHealthStat? getCustomHealthStat(this ReferenceHub hub)
        {
            if (!hub.playerStats.TryGetModule(out CustomHealthStat? stat))
            {
                stat = hub.playerStats.GetModule<HealthStat>() as CustomHealthStat;
            }

            return stat;
        }

        public static CustomHealthStat? getCustomHealthStat(this Player plr) => plr.ReferenceHub.getCustomHealthStat();
#nullable disable

        /// <param name="target">Target which gets vaporized</param>
        extension(ReferenceHub target)
        {
            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer(ReferenceHub attacker = null)
            {
                DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(target), DisruptorActionModule.FiringState.FiringSingle), target.PlayerCameraReference.forward, -1f);

                target.playerStats.KillPlayerWithEvents(vaporizeHandler);
            }

            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer()
            {
                DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(target),DisruptorActionModule.FiringState.FiringSingle), target.PlayerCameraReference.forward, -1f);
                target.playerStats.KillPlayer(vaporizeHandler);
            }
            
            public void AddEffect<T>(byte intensity, float duration = 0) where T : StatusEffectBase => target.AddEffect(typeof(T), intensity, duration);

            public void AddEffect(Type effectType, byte intensity, float duration = 0)
            {
                if (!EffectStackManager.TryGet(target, out var manager))
                    return;
                
                manager.AddStack(effectType, new EffectStack
                {
                    Intensity = intensity,
                    Duration = duration
                });
            }
        }

        /// <param name="target">Target which gets vaporized</param>
        extension(Player target)
        {
            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer(ReferenceHub attacker = null)
            {
                target.ReferenceHub.VaporizePlayer(attacker);
            }

            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer(Player attacker = null)
            {
                target.ReferenceHub.VaporizePlayer(attacker?.ReferenceHub);
            }
            
            public void AddEffect<T>(Func<byte> intensityCalc, float duration = 0) where T : StatusEffectBase => target.AddEffect(typeof(T), intensityCalc, duration);
            public void AddEffect<T>(byte intensity, float duration = 0) where T : StatusEffectBase => target.AddEffect(typeof(T), intensity, duration);
            public void AddEffect<T>(EffectStack stack) where T : StatusEffectBase => target.AddEffect(typeof(T), stack);

            public void AddEffect(Type effectType, byte intensity, float duration = 0) => target.AddEffect(
                effectType,
                new EffectStack
                {
                    Intensity = intensity,
                    Duration = duration
                });

            public void AddEffect(Type effectType, Func<byte> intensityCalc, float duration = 0) => target.AddEffect(
                effectType, 
                new EffectStack(intensityCalc)
                {
                    Duration = duration
                });

            public void AddEffect(Type effectType, EffectStack stack)
            {
                if (!EffectStackManager.TryGet(target, out var manager))
                    return;
                
                manager.AddStack(effectType, stack);
            }

            public bool RemoveEffect<T>(EffectStack stack) where T : StatusEffectBase => target.RemoveEffect(typeof(T), stack);
            public bool RemoveEffect(Type effectType, EffectStack stack)
            {
                if (!EffectStackManager.TryGet(target, out var manager))
                    return false;
                
                return manager.RemoveStack(effectType, stack);
            }

            public bool RemoveEffect<T>() where T : StatusEffectBase => target.RemoveEffect(typeof(T));
            public bool RemoveEffect(Type effectType)
            {
                if (!EffectStackManager.TryGet(target, out var manager))
                    return false;
                
                return manager.RemoveStacks(effectType);
            }

            internal void EnableEffect(Type effectType, byte intensity, float duration = 0, bool addDuration = false)
            {
                target.ReferenceHub.playerEffectsController.GetEffect(effectType)?.ServerSetState(intensity, duration, addDuration);
            }

            internal void DisableEffect(Type effectType)
            {
                target.ReferenceHub.playerEffectsController.GetEffect(effectType)?.ServerDisable();
            }
            
            internal bool TryGetEffect(Type effectType, out StatusEffectBase effect)
            {
                effect = target.ReferenceHub.playerEffectsController.GetEffect(effectType);
                return effect != null;
            }
        }

        /// <summary>
        /// Vaporizes a Player instantly
        /// </summary>
        /// <param name="target">Target which gets vaporized</param>
        /// <param name="attacker">Attacker which vaporizes the Target</param>
        public static void VaporizePlayer(this ReferenceHub target, Player attacker = null)
        {
            target.VaporizePlayer(attacker.ReferenceHub);
        }

        public static void Explode(this Player plr)
        {
            if (!plr.IsAlive) return;

            MainHelper.CreateThrowable(ItemType.GrenadeHE).SpawnActive(plr.Position, 0.01f, plr);
        }

        public static void SetScale(this ReferenceHub plr, Vector3 newScale)
        {
            if (plr.roleManager.CurrentRole is not IFpcRole fpc)
            {
                return;
            }

            var scaleEvent = new PlayerScaleChanging(plr, newScale);
            XazeEvents.OnPlayerScaleChanging(scaleEvent);

            if (!scaleEvent.IsAllowed)
            {
                return;
            }

            fpc.FpcModule.Motor.ScaleController.Scale = scaleEvent.NewScale;
        }

        public static void SetScale(this Player plr, Vector3 Scale) => plr.ReferenceHub.SetScale(Scale);

        extension(PlayerEffectsController controller)
        {
            public StatusEffectBase GetEffect(Type effectType)
            {
                if (controller._effectsByType.TryGetValue(effectType, out StatusEffectBase effect))
                {
                    return effect;
                }

                return null;
            }

            public T GetEffect<T>(Type effectType) where T : StatusEffectBase
            {
                if (controller._effectsByType.TryGetValue(effectType, out StatusEffectBase effect))
                {
                    return effect as T;
                }

                return null;
            }
        }

        extension(PlayerStats stats)
        {
            public bool DealDamageWithoutRagdoll(DamageHandlerBase handler)
            {
                if (stats._hub.characterClassManager.GodMode)
                {
                    return false;
                }

                if (stats._hub.roleManager.CurrentRole is IDamageHandlerProcessingRole damageHandlerProcessingRole)
                {
                    handler = damageHandlerProcessingRole.ProcessDamageHandler(handler);
                }

                ReferenceHub attacker = null;
                AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
                if (attackerDamageHandler != null)
                {
                    attacker = attackerDamageHandler.Attacker.Hub;
                }
                PlayerHurtingEventArgs playerHurtingEventArgs = new PlayerHurtingEventArgs(attacker, stats._hub, handler);
                PlayerEvents.OnHurting(playerHurtingEventArgs);
                if (!playerHurtingEventArgs.IsAllowed)
                {
                    return false;
                }
                DamageHandlerBase.HandlerOutput handlerOutput = handler.ApplyDamage(stats._hub);
                PlayerEvents.OnHurt(new PlayerHurtEventArgs(attacker, stats._hub, handler));
                if (handlerOutput == DamageHandlerBase.HandlerOutput.Nothing)
                {
                    return false;
                }

                if (handlerOutput == DamageHandlerBase.HandlerOutput.Death)
                {
                    PlayerDyingEventArgs playerDyingEventArgs = new PlayerDyingEventArgs(stats._hub, attacker, handler);
                    PlayerEvents.OnDying(playerDyingEventArgs);
                    if (!playerDyingEventArgs.IsAllowed)
                    {
                        return false;
                    }

                    var ragdoll = stats.KillPlayerRagdoll(handler);

                    RoleTypeId role = stats._hub.roleManager.CurrentRole.RoleTypeId;
                    Vector3 vel = stats._hub.GetVelocity();
                    Vector3 pos = stats._hub.GetPosition();
                    Quaternion rot = stats._hub.PlayerCameraReference.rotation;

                    PlayerEvents.OnDeath(new PlayerDeathEventArgs(stats._hub, attacker, handler, role, pos, vel, rot));

                    Timing.CallDelayed(0.1f, () => NetworkServer.Destroy(ragdoll.gameObject));
                }

                return true;
            }

            public BasicRagdoll KillPlayerRagdoll(DamageHandlerBase handler)
            {
                var ragdoll = RagdollManager.ServerSpawnRagdoll(stats._hub, handler);
                stats._hub.inventory.ServerDropEverything();
                stats._hub.roleManager.ServerSetRole(RoleTypeId.Spectator, RoleChangeReason.Died);
                stats._hub.gameConsoleTransmission.SendToClient("You died. Reason: " + handler.ServerLogsText, "yellow");
                if (stats._hub.roleManager.CurrentRole is SpectatorRole spectatorRole)
                {
                    spectatorRole.ServerSetData(handler);
                }

                return ragdoll;
            }

            public BasicRagdoll KillPlayerWithEvents(DamageHandlerBase handler)
            {
                ReferenceHub attacker = null;
                if (handler is AttackerDamageHandler atHandler)
                {
                    attacker = atHandler.Attacker.Hub;
                }

                PlayerEvents.OnDying(new PlayerDyingEventArgs(stats._hub, attacker, handler));
                var ragdoll = RagdollManager.ServerSpawnRagdoll(stats._hub, handler);
                stats._hub.inventory.ServerDropEverything();

                RoleTypeId role = stats._hub.roleManager.CurrentRole.RoleTypeId;
                Vector3 vel = stats._hub.GetVelocity();
                Vector3 pos = stats._hub.GetPosition();
                Quaternion rot = stats._hub.PlayerCameraReference.rotation;

                stats._hub.roleManager.ServerSetRole(RoleTypeId.Spectator, RoleChangeReason.Died);
                stats._hub.gameConsoleTransmission.SendToClient("You died. Reason: " + handler.ServerLogsText, "yellow");
                if (stats._hub.roleManager.CurrentRole is SpectatorRole spectatorRole)
                {
                    spectatorRole.ServerSetData(handler);
                }

                PlayerEvents.OnDeath(new PlayerDeathEventArgs(stats._hub, attacker, handler, role, pos, vel, rot));

                return ragdoll;
            }
        }

        public static Player DisarmedBy(this ReferenceHub hub)
        {
            var entry = DisarmedPlayers.Entries.Find(x => x.DisarmedPlayer == hub.netId);

            if (!Player.TryGet(entry.Disarmer, out Player disarmer))
            {
                return null;
            }

            return disarmer;
        }

        public static bool TryGetInventoryItem(this ReferenceHub hub, ushort serial, out ItemBase item)
        {
            return hub.inventory.UserInventory.Items.TryGetValue(serial, out item);
        }
        public static bool TryGetInventoryItem(this ReferenceHub hub, ItemIdentifier identifier, out ItemBase item) => hub.TryGetInventoryItem(identifier.SerialNumber, out item);

        public static void FlingPlayer(this ReferenceHub hub, float strength = 1f)
        {
            var handler = new DisruptorDamageHandler(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(hub), DisruptorActionModule.FiringState.FiringRapid), hub.PlayerCameraReference.forward, -1f);
            handler.StartVelocity = hub.PlayerCameraReference.forward.NormalizeIgnoreY() * 15f * strength;
            handler.StartVelocity.y = 2f;
            hub.playerStats.KillPlayer(handler);
        }
        
        public static void RemoveItems(this Player plr, ItemType type)
        {
            var Items = plr.Items.ToList();
            foreach(var item in Items)
            {
                if (item.Type != type)
                {
                    continue;
                }

                plr.RemoveItem(item);
            }
        }

        public static void RemoveAmmo(this Player plr, ItemType ammo, ushort amount)
        {
            plr.SetAmmo(ammo, (ushort)(plr.GetAmmo(ammo) - ammo));
        }

        public static void RemoveAmmo(this ReferenceHub hub, ItemType ammo, ushort amount)
        {
            Inventory inv = hub.inventory;
            inv.ServerSetAmmo(ammo, (ushort)(inv.GetCurAmmo(ammo) - ammo));
        }

        public static void RemoveAmmo(this Player plr, ItemType ammo, int amount) =>
            plr.RemoveAmmo(ammo, (ushort)ammo);

        public static void RemoveAmmo(this ReferenceHub hub, ItemType ammo, int amount) =>
            hub.RemoveAmmo(ammo, (ushort)ammo);

        public static void GiveLoadout(this ReferenceHub hub, RoleTypeId role, bool resetInventory = false)
        {
            if (!role.TryGetRoleTemplate<PlayerRoleBase>(out var prb))
            {
                return;
            }

            InventoryItemProvider.ServerGrantLoadout(hub, prb, resetInventory);
        }

        public static void GiveLoadout(this Player plr, RoleTypeId role, bool resetInventory = false) => GiveLoadout(plr.ReferenceHub, role, resetInventory);
    }
}
