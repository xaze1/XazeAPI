// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using JetBrains.Annotations;
using Utils;
using XazeAPI.API.EffectStacks;
using XazeAPI.API.Events;
using XazeAPI.API.Events.Handler;
using XazeAPI.API.Stats;
using XazeAPI.Features.AoEs;

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
    using UnityEngine;
    using LabApi.Features.Wrappers;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using PlayerRoles.FirstPersonControl;

    public static class PlayerExtensions
    {
        extension(Player plr)
        {
            public FollowingAerial<T> CreateAura<T>(T aerialEffect) where T : AerialEffect
            {
                return FollowingAerial<T>.Create(plr, aerialEffect);
            }
            
            public FollowingAerial<DelegateAerial> CreateAura(Action<Player> action)
            {
                return FollowingAerial<DelegateAerial>.Create(plr, new DelegateAerial(action, plr.Position));
            }
            
            public FollowingAerial<StatusEffectAerial<T>> CreateAura<T>(int intensity, float duration) where T : StatusEffectBase
            {
                return FollowingAerial<StatusEffectAerial<T>>.Create(plr, new StatusEffectAerial<T>(plr.Position, intensity, duration));
            }
            
            public HealthStat GetHealthStat() => plr.ReferenceHub.GetHealthStat();
            public void changeMaxHealth(float newMaxHealth) => plr.ReferenceHub.changeMaxHealth(newMaxHealth);

            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            public void VaporizePlayer()
            {
                DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(plr.ReferenceHub), DisruptorActionModule.FiringState.FiringSingle), plr.Camera.forward, -1f);
                plr.ReferenceHub.playerStats.KillPlayerWithEvents(vaporizeHandler);
            }
            
            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer(ReferenceHub attacker = null)
            {
                plr.ReferenceHub.VaporizePlayer(attacker);
            }

            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer(Player attacker = null)
            {
                plr.ReferenceHub.VaporizePlayer(attacker?.ReferenceHub);
            }
            
            public void AddEffect<T>(Func<int> intensityCalc, float duration = 0, string id = null) where T : StatusEffectBase => plr.AddEffect(typeof(T), intensityCalc, duration, id);
            public void AddEffect<T>(byte intensity = 1, float duration = 0, string id = null) where T : StatusEffectBase => plr.AddEffect(typeof(T), intensity, duration, id);
            public void AddEffect<T>(EffectStack stack) where T : StatusEffectBase => plr.AddEffect(typeof(T), stack);

            public void AddEffect(Type effectType, byte intensity = 1, float duration = 0, string id = null) => plr.AddEffect(
                effectType,
                new EffectStack(id)
                {
                    Intensity = intensity,
                    Duration = duration
                });

            public void AddEffect(Type effectType, Func<int> intensityCalc, float duration = 0, string id = null) => plr.AddEffect(
                effectType, 
                new EffectStack(intensityCalc, null, id)
                {
                    Duration = duration
                });

            public void AddEffect(Type effectType, EffectStack stack)
            {
                if (!EffectStackManager.TryGet(plr, out var manager))
                    return;
                
                if (stack.IsPrefab)
                    stack = stack.Clone();
                
                manager.AddStack(effectType, stack);
            }

            [CanBeNull]
            public EffectStack GetEffectStack<T>(string id) where T : StatusEffectBase => plr.GetEffectStack(typeof(T), id);
            
            [CanBeNull]
            public EffectStack GetEffectStack(Type effectType, string id)
            {
                if (id.IsNullOrWhiteSpace())
                    throw new ArgumentNullException(nameof(id) + " cannot be null or empty.");
                
                if (!EffectStackManager.TryGet(plr, out var manager))
                    return null;
                
                return manager.GetStack(effectType, id);
            }

            public bool TryGetEffectStack<T>(string id, out EffectStack stack) where T : StatusEffectBase => plr.TryGetEffectStack(typeof(T), id, out stack);
            public bool TryGetEffectStack(Type effectType, string id, out EffectStack stack)
            {
                if (id.IsNullOrWhiteSpace())
                    throw new ArgumentNullException(nameof(id) + " cannot be null or empty.");

                stack = null;
                if (!EffectStackManager.TryGet(plr, out var manager))
                    return false;
                
                stack =  manager.GetStack(effectType, id);
                return stack != null;
            }
            
            public bool RemoveEffect<T>(EffectStack stack) where T : StatusEffectBase => plr.RemoveEffect(typeof(T), stack);
            public bool RemoveEffect(Type effectType, EffectStack stack)
            {
                if (!EffectStackManager.TryGet(plr, out var manager))
                    return false;
                
                return manager.RemoveStack(effectType, stack);
            }

            public bool RemoveEffect<T>(string id) where T : StatusEffectBase => plr.RemoveEffect(typeof(T), id);
            public bool RemoveEffect(Type effectType, string id)
            {
                if (!EffectStackManager.TryGet(plr, out var manager))
                    return false;
                
                return manager.RemoveStack(effectType, manager.GetStack(effectType, id));
            }

            public bool RemoveEffect<T>() where T : StatusEffectBase => plr.RemoveEffect(typeof(T));
            public bool RemoveEffect(Type effectType)
            {
                if (!EffectStackManager.TryGet(plr, out var manager))
                    return false;
                
                return manager.RemoveStacks(effectType);
            }
            
            public void RemoveEffects()
            {
                if (!EffectStackManager.TryGet(plr, out var manager))
                    return;
                
                manager.RemoveStacks();
            }

            internal void EnableEffect(Type effectType, byte intensity, float duration = 0, bool addDuration = false)
            {
                plr.ReferenceHub.playerEffectsController.GetEffect(effectType)?.ServerSetState(intensity, duration, addDuration);
            }

            internal void DisableEffect(Type effectType)
            {
                plr.ReferenceHub.playerEffectsController.GetEffect(effectType)?.ServerDisable();
            }
            
            internal bool TryGetEffect(Type effectType, out StatusEffectBase effect)
            {
                effect = plr.ReferenceHub.playerEffectsController.GetEffect(effectType);
                return effect != null;
            }
            
            public void Explode()
            {
                if (!plr.IsAlive) 
                    return;
                
                ExplosionUtils.ServerExplode(plr.ReferenceHub, ExplosionType.PinkCandy);
            }
            
            public void SetScale(Vector3 Scale) => plr.ReferenceHub.SetScale(Scale);
            
            public void RemoveItems(ItemType type)
            {
                var Items = plr.Items.ToList();
                foreach(var item in Items)
                {
                    if (item.Type != type)
                        continue;

                    plr.RemoveItem(item);
                }
            }

            public void RemoveAmmo(ItemType ammo, int amount)
            {
                plr.SetAmmo(ammo, (ushort)(plr.GetAmmo(ammo) - amount));
            }
            
            public void GiveLoadout(RoleTypeId role, bool resetInventory = false) => GiveLoadout(plr.ReferenceHub, role, resetInventory);
            
            public CustomHealthStat? getCustomHealthStat() => plr.ReferenceHub.getCustomHealthStat();
        }

        extension(ReferenceHub hub)
        {
            public void SendConsoleMessage(string message, string color) => hub.gameConsoleTransmission.SendToClient(message, color);
            
            public HealthStat GetHealthStat()
            {
                return hub.playerStats.GetModule<HealthStat>();
            }

            public void changeMaxHealth(float newMaxHealth)
            {
                hub.playerStats.GetModule<HealthStat>().MaxValue = newMaxHealth;
            }
            
            [CanBeNull]
            public CustomHealthStat getCustomHealthStat()
            {
                if (!hub.playerStats.TryGetModule(out CustomHealthStat stat))
                {
                    stat = hub.playerStats.GetModule<HealthStat>() as CustomHealthStat;
                }

                return stat;
            }
            
            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer(Player attacker)
            {
                hub.VaporizePlayer(attacker.ReferenceHub);
            }
            
            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer(ReferenceHub attacker)
            {
                DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(attacker), DisruptorActionModule.FiringState.FiringSingle), hub.PlayerCameraReference.forward, -1f);
                hub.playerStats.KillPlayerWithEvents(vaporizeHandler);
            }

            /// <summary>
            /// Vaporizes a Player instantly
            /// </summary>
            /// <param name="attacker">Attacker which vaporizes the Target</param>
            public void VaporizePlayer()
            {
                DisruptorDamageHandler vaporizeHandler = new(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(hub),DisruptorActionModule.FiringState.FiringSingle), hub.PlayerCameraReference.forward, -1f);
                hub.playerStats.KillPlayer(vaporizeHandler);
            }
            
            public void AddEffect<T>(Func<int> intensityCalc, float duration = 0, string id = null) where T : StatusEffectBase => hub.AddEffect(typeof(T), intensityCalc, duration, id);
            public void AddEffect<T>(byte intensity = 1, float duration = 0, string id = null) where T : StatusEffectBase => hub.AddEffect(typeof(T), intensity, duration, id);
            public void AddEffect<T>(EffectStack stack) where T : StatusEffectBase => hub.AddEffect(typeof(T), stack);

            public void AddEffect(Type effectType, byte intensity = 1, float duration = 0, string id = null) => hub.AddEffect(
                effectType,
                new EffectStack(id)
                {
                    Intensity = intensity,
                    Duration = duration
                });

            public void AddEffect(Type effectType, Func<int> intensityCalc, float duration = 0, string id = null) => hub.AddEffect(
                effectType, 
                new EffectStack(intensityCalc, null, id)
                {
                    Duration = duration
                });

            public void AddEffect(Type effectType, EffectStack stack)
            {
                if (!EffectStackManager.TryGet(hub, out var manager))
                    return;

                if (stack.IsPrefab)
                    stack = stack.Clone();
                
                manager.AddStack(effectType, stack);
            }
            
            public bool RemoveEffect<T>(EffectStack stack) where T : StatusEffectBase => hub.RemoveEffect(typeof(T), stack);
            public bool RemoveEffect(Type effectType, EffectStack stack)
            {
                if (!EffectStackManager.TryGet(hub, out var manager))
                    return false;
                
                return manager.RemoveStack(effectType, stack);
            }

            public bool RemoveEffect<T>() where T : StatusEffectBase => hub.RemoveEffect(typeof(T));
            public bool RemoveEffect(Type effectType)
            {
                if (!EffectStackManager.TryGet(hub, out var manager))
                    return false;
                
                return manager.RemoveStacks(effectType);
            }
            
            public void RemoveEffects()
            {
                if (!EffectStackManager.TryGet(hub, out var manager))
                    return;
                
                manager.RemoveStacks();
            }
            
            public void SetScale(Vector3 newScale)
            {
                if (hub.roleManager.CurrentRole is not IFpcRole fpc)
                {
                    return;
                }

                var scaleEvent = new PlayerScaleChanging(hub, newScale);
                XazeEvents.OnPlayerScaleChanging(scaleEvent);

                if (!scaleEvent.IsAllowed)
                {
                    return;
                }

                fpc.FpcModule.Motor.ScaleController.Scale = scaleEvent.NewScale;
            }
            
            public Player DisarmedBy()
            {
                var entry = DisarmedPlayers.Entries.Find(x => x.DisarmedPlayer == hub.netId);

                if (!Player.TryGet(entry.Disarmer, out Player disarmer))
                {
                    return null;
                }

                return disarmer;
            }
            
            public bool TryGetInventoryItem(ushort serial, out ItemBase item)
            {
                return hub.inventory.UserInventory.Items.TryGetValue(serial, out item);
            }
            
            public bool TryGetInventoryItem(ItemIdentifier identifier, out ItemBase item) => hub.TryGetInventoryItem(identifier.SerialNumber, out item);
            
            public void FlingPlayer(float strength = 1f)
            {
                var handler = new DisruptorDamageHandler(new DisruptorShotEvent(new ItemIdentifier(), new Footprint(hub), DisruptorActionModule.FiringState.FiringRapid), hub.PlayerCameraReference.forward, -1f)
                {
                    StartVelocity = hub.PlayerCameraReference.forward.NormalizeIgnoreY() * 15f * strength
                };
                handler.StartVelocity.y = 2f;
                hub.playerStats.KillPlayer(handler);
            }
            
            public void RemoveAmmo(ItemType ammo, int amount)
            {
                Inventory inv = hub.inventory;
                inv.ServerSetAmmo(ammo, (ushort)(inv.GetCurAmmo(ammo) - amount));
            }

            public void GiveLoadout(RoleTypeId role, bool resetInventory = false)
            {
                if (!role.TryGetRoleTemplate<PlayerRoleBase>(out var prb))
                {
                    return;
                }

                InventoryItemProvider.ServerGrantLoadout(hub, prb, resetInventory);
            }
        }
        
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
    }
}
