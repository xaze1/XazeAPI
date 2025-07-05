// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using EclipsePlugin.API.CustomModules;
using XazeAPI.API.Helpers;

namespace XazeAPI.API.Extensions
{
    using CommandSystem;
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
    using RemoteAdmin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using LabApi.Features.Wrappers;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using PlayerRoles.FirstPersonControl;

    public static class PlayerExtensions
    {
        public static void SendConsoleMessage(this ReferenceHub target, string message, string color) => target.gameConsoleTransmission.SendToClient(message, color);

        public static CoroutineHandle createAura(this ReferenceHub attacker, string effectName = null, Action customFunction = null)
        {
            if (customFunction != null)
            {
                customFunction();
            }

            return Timing.CallPeriodically(10000f, 0.5f, () =>
            {

                if (!attacker.IsAlive())
                {
                    return;
                }

                foreach (Player player in Player.List)
                {
                    if (player.ReferenceHub == attacker || attacker.IsSCP() && player.IsSCP)
                    {
                        continue;
                    }

                    if (Vector3.Distance(attacker.transform.position, player.ReferenceHub.transform.position) <= 5)
                    {
                        if (effectName != null)
                        {
                            PlayerEffectsController effectController = player.ReferenceHub.playerEffectsController;

                            effectController.ChangeState(effectName, 1);
                        }
                    }
                }
            }, () => createAura(attacker, effectName, customFunction));

        }
        
        public static CoroutineHandle createAura(this ReferenceHub attacker, DamageHandlerBase handler = null, string effectName = null, Func<bool> customFunction = null)
        {
            Footprint footprint = new Footprint(attacker);
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

                    if (Vector3.Distance(attacker.transform.position, player.Position) <= 5)
                    {
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
                }
            }, () => createAura(attacker, handler, effectName, customFunction));
            return handle;
        }

        public static CoroutineHandle createAura(this Player attacker, float damageMultiplier, float damagePerMultiplier, DeathTranslation deathTranslation, Action customFunction = null)
        {
            return Timing.CallPeriodically(10000f, 0.5f, () =>
            {
                if (customFunction != null)
                {
                    customFunction();
                }

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

                    if (Vector3.Distance(attacker.ReferenceHub.transform.position, player.ReferenceHub.transform.position) <= 5)
                    {
                        float damage = damageMultiplier * damagePerMultiplier;
                        UniversalDamageHandler handler = new(damage, deathTranslation);
                        player.ReferenceHub.playerStats.DealDamage(handler);
                    }
                }
            }, () => attacker.createAura(damageMultiplier, damagePerMultiplier, deathTranslation, customFunction));

        }

        public static CoroutineHandle createAura(this ReferenceHub attacker, float distance, Action<Player> customFunction)
        {
            Footprint footprint = new Footprint(attacker);
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

                    if (Vector3.Distance(attacker.transform.position, player.Position) <= distance)
                    {
                        customFunction(player);
                    }
                }
            }, () => createAura(attacker, distance, customFunction));
            return handle;
        }

        public static HealthStat GetHealthStat(this Player plr) => plr.ReferenceHub.GetHealthStat();
        public static HealthStat GetHealthStat(this ReferenceHub hub)
        {
            return hub.playerStats.GetModule<HealthStat>();
        }

        public static void changeMaxHealth(this ReferenceHub hub, float newMaxHealth)
        {
            hub.playerStats.GetModule<HealthStat>().MaxValue = newMaxHealth;
        }

        public static void changeMaxHealth(this Player plr, float newMaxHealth) => plr.ReferenceHub.changeMaxHealth(newMaxHealth);

#nullable enable
        public static CustomHealthStat? getCustomHealthStat(this ReferenceHub hub)
        {
            CustomHealthStat? stat;
            if (!hub.playerStats.TryGetModule(out stat))
            {
                stat = hub.playerStats.GetModule<HealthStat>() as CustomHealthStat;
            }

            return stat;
        }

        public static CustomHealthStat? getCustomHealthStat(this Player plr) => plr.ReferenceHub.getCustomHealthStat();
#nullable disable

        /// <summary>
        /// Vaporizes a Player instantly
        /// </summary>
        /// <param name="target">Target which gets vaporized</param>
        /// <param name="attacker">Attacker which vaporizes the Target</param>
        public static void VaporizePlayer(this ReferenceHub target, ReferenceHub attacker = null)
        {
            DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(target), DisruptorActionModule.FiringState.FiringSingle), target.PlayerCameraReference.forward, -1f);

            target.playerStats.KillPlayerWithEvents(vaporizeHandler);
        }

        /// <summary>
        /// Vaporizes a Player instantly
        /// </summary>
        /// <param name="target">Target which gets vaporized</param>
        /// <param name="attacker">Attacker which vaporizes the Target</param>
        public static void VaporizePlayer(this Player target)
        {
            DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(target.ReferenceHub), DisruptorActionModule.FiringState.FiringSingle), target.Camera.forward, -1f);
            target.ReferenceHub.playerStats.KillPlayerWithEvents(vaporizeHandler);
        }
        
        /// <summary>
        /// Vaporizes a Player instantly
        /// </summary>
        /// <param name="target">Target which gets vaporized</param>
        /// <param name="attacker">Attacker which vaporizes the Target</param>
        public static void VaporizePlayer(this ReferenceHub target)
        {
            DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(target),DisruptorActionModule.FiringState.FiringSingle), target.PlayerCameraReference.forward, -1f);
            target.playerStats.KillPlayer(vaporizeHandler);
        }

        /// <summary>
        /// Vaporizes a Player instantly
        /// </summary>
        /// <param name="target">Target which gets vaporized</param>
        /// <param name="attacker">Attacker which vaporizes the Target</param>
        public static void VaporizePlayer(this Player target, ReferenceHub attacker = null)
        {
            VaporizePlayer(target.ReferenceHub, attacker);
        }

        /// <summary>
        /// Vaporizes a Player instantly
        /// </summary>
        /// <param name="target">Target which gets vaporized</param>
        /// <param name="attacker">Attacker which vaporizes the Target</param>
        public static void VaporizePlayer(this Player target, Player attacker = null)
        {
            VaporizePlayer(target.ReferenceHub, attacker.ReferenceHub);
        }

        /// <summary>
        /// Vaporizes a Player instantly
        /// </summary>
        /// <param name="target">Target which gets vaporized</param>
        /// <param name="attacker">Attacker which vaporizes the Target</param>
        public static void VaporizePlayer(this ReferenceHub target, Player attacker = null)
        {
            VaporizePlayer(target, attacker.ReferenceHub);
        }

        /*
        */

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

            fpc.FpcModule.Motor.ScaleController.Scale = newScale;
        }

        public static void SetScale(this Player plr, Vector3 Scale) => SetScale(plr.ReferenceHub, Scale);

        public static StatusEffectBase GetEffect(this PlayerEffectsController controller, Type effectType)
        {
            if (controller._effectsByType.TryGetValue(effectType, out StatusEffectBase effect))
            {
                return effect;
            }

            return null;
        }
        
        public static T GetEffect<T>(this PlayerEffectsController controller, Type effectType) where T : StatusEffectBase
        {
            if (controller._effectsByType.TryGetValue(effectType, out StatusEffectBase effect))
            {
                return effect as T;
            }

            return null;
        }

        public static bool DealDamageWithoutRagdoll(this PlayerStats stats, DamageHandlerBase handler)
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

        public static BasicRagdoll KillPlayerRagdoll(this PlayerStats stats, DamageHandlerBase handler)
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

        public static BasicRagdoll KillPlayerWithEvents(this PlayerStats stats, DamageHandlerBase handler)
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
            if (!PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(role, out var prb))
            {
                return;
            }

            InventoryItemProvider.ServerGrantLoadout(hub, prb, resetInventory);
        }

        public static void GiveLoadout(this Player plr, RoleTypeId role, bool resetInventory = false) => GiveLoadout(plr.ReferenceHub, role, resetInventory);
    }
}
