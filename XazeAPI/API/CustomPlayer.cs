// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.RoleAssign;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using EclipsePlugin.API.CustomModules;
using JetBrains.Annotations;
using XazeAPI.API.Enums;
using UnityEngine;
using Utils.NonAllocLINQ;
using XazeAPI.API.Extensions;
using XazeAPI.API.Helpers;
using XazeAPI.API.Stats.Player;
using XazeAPI.API.Structures;

namespace XazeAPI.API
{
    public class CustomPlayer : IEquatable<CustomPlayer>
    {
        public static readonly Dictionary<ReferenceHub, CustomPlayer> Dictionary = new();
        
        public string Username
        {
            get => Player.DisplayName;
            set => Player.DisplayName = value;
        }

        public string UniqueUserId
        {
            get
            {
                if (!Player.UserId.Contains("@")) return Player.UserId.Substring(0, Player.UserId.Length - 6);
                int index = Player.UserId.IndexOf("@", StringComparison.Ordinal);
                return Player.UserId.Substring(0, index);
            }
        }

        public string CustomInfo
        {
            get => Player.CustomInfo;
            set => Player.CustomInfo = value;
        }

        public Team Team
        {
            get
            {
                if (IsSCP)
                    return Team.SCPs;

                return Player.Team;
            }
        }

        public bool IsSCP
        {
            get
            {
                if (Player.IsSCP)
                    return true;

                return field;
            }
            set;
        }


        [NotNull]
        public ReferenceHub ReferenceHub { get; }

        [NotNull]
        public Player Player { get; }

        // Player Stats
        public PlayerKillStat KillStat => PlayerKillStat.GetStatOrDefault(ReferenceHub);
        public PlayerDeathStat DeathStat => PlayerDeathStat.GetValueOrDefault(ReferenceHub);

        // Custom Modules
        public CustomHealthStat? HealthStat => Player.getCustomHealthStat();
        // public CustomEffectsController CustomEffects => GameObject.GetComponent<CustomEffectsController>();
        // public CustomRoleManager CustomRoleManager => CustomRoleManager.Get(Player);

        // Base Values
        public GameObject GameObject => Player.GameObject;
        public PlayerRoleBase CurrentRole => ReferenceHub.roleManager.CurrentRole;
        // public CustomRoleBase CustomRole => CustomRoleManager.CurrentRole;

        // Bools
        // public bool IsCustomRole => CustomRoleManager.IsCustomRole(Player) || SuperController.IsSuperScp(Player);
        public bool IsDisguised => Disguise != RoleTypeId.None;
        // public bool IsCustomZombie => ZombieRolesController.ActiveSpecialZombies.ContainsKey(ReferenceHub);
        // public bool IsCISpy => ChaosSpyHandler.CISpies.ContainsKey(Player.ReferenceHub);
        public bool IsInvisible => (CurrentRole as IFpcRole)?.FpcModule.Motor.IsInvisible?? false;
        public bool IsInventoryFull => Player.IsInventoryFull;

        // Variables
        public int CoinUses { get; set; } = 0;
        public int SnakeHighScore { get; set; } = 0;
        public float TotalDamageDone { get; set; } = 0;
        public float TotalSCPDamageDone { get; set; } = 0;
        public RoleTypeId LastRole { get; set; } = RoleTypeId.None;

        // Var Getters
        public RoleTypeId Disguise => DisguiseHelper.DisguisedPlayers.TryGetValue(ReferenceHub, out DisguisedPlayer plr) ? plr.Disguise : RoleTypeId.None;
        //public int PersonnelSpawnChance => PlayerLevel.TryGet(Player, out var lvl) ? lvl.PersonnelChance : 10;
        
        public int ScpChance
        {
            get
            {
                using ScpTicketsLoader scpTicketsLoader = new ScpTicketsLoader();
                return scpTicketsLoader.GetTickets(ReferenceHub, 10);
            }
            set
            {
                using ScpTicketsLoader scpTickets = new ScpTicketsLoader();
                scpTickets.ModifyTickets(ReferenceHub, value);
            }
        }

        /// <summary>
        /// Gets the active CustomPlayer
        /// </summary>
        /// <param name="hub"></param>
        /// <returns>Return the active <see cref="CustomPlayer"/></returns>
        public static CustomPlayer Get(ReferenceHub hub)
        {
            if (Dictionary.TryGetValue(hub, out CustomPlayer cplr))
            {
                return cplr;
            }
            
            return new CustomPlayer(hub);
        }

        /// <summary>
        /// Gets the active CustomPlayer
        /// </summary>
        /// <param name="plr"></param>
        /// <returns>Return the active <see cref="CustomPlayer"/></returns>
        public static CustomPlayer Get(Player plr)
        {
            if (Dictionary.TryGetValue(plr.ReferenceHub, out CustomPlayer cplr))
            {
                return cplr;
            }
            
            return new CustomPlayer(plr);
        }

        /// <summary>
        /// Tries to get the <see cref="CustomPlayer"/> object of a ReferenceHub
        /// </summary>
        /// <param name="hub">Player object</param>
        /// <param name="cplr">CustomPlayer object of the ReferenceHub</param>
        /// <returns>Returns weither or not a <see cref="CustomPlayer"/> was found</returns>
        public static bool TryGet(ReferenceHub hub, out CustomPlayer cplr)
        {
            cplr = Get(hub);
            return cplr is not null;
        }

        /// <summary>
        /// Tries to get the <see cref="CustomPlayer"/> object of a Player
        /// </summary>
        /// <param name="ply">Player object</param>
        /// <param name="cplr">CustomPlayer object of the Player</param>
        /// <returns>Returns weither or not a <see cref="CustomPlayer"/> was found</returns>
        public static bool TryGet(Player ply, out CustomPlayer cplr)
        {
            cplr = Get(ply);
            return cplr is not null;
        }

        /// <summary>
        /// Goes through all the active and cached players to get highest damage done to Human classes
        /// </summary>
        /// <returns>Returns <see cref="CustomPlayer"/> object from player with highest damage</returns>
        public static CustomPlayer GetHighestDamageDone()
        {
            var cplr = Dictionary.Values.Where(x => x.LastRole != RoleTypeId.None || x.CurrentRole.Team != Team.Dead)?.FirstOrDefault();
            foreach (CustomPlayer player in Dictionary.Values.Where(plr => plr.LastRole != RoleTypeId.None && plr.CurrentRole.Team != Team.Dead))
            {
                if (player.TotalDamageDone <= cplr?.TotalDamageDone)
                {
                    continue;
                }

                cplr = player;
            }

            return cplr;
        }

        /// <summary>
        /// Goes through all the active and cached players to get highest SCP damage done
        /// </summary>
        /// <returns>Returns <see cref="CustomPlayer"/> object from player with highest SCP damage</returns>
        public static CustomPlayer GetHighestSCPDamageDone()
        {
            var cplr = Dictionary.Values.Where(x => x.LastRole != RoleTypeId.None || x.CurrentRole.Team != Team.Dead)?.FirstOrDefault();
            foreach (CustomPlayer player in Dictionary.Values.Where(plr => plr.LastRole != RoleTypeId.None && plr.CurrentRole.Team != Team.Dead))
            {
                if (player.TotalSCPDamageDone <= cplr?.TotalSCPDamageDone)
                {
                    continue;
                }

                cplr = player;
            }

            return cplr;
        }

        public static CustomPlayer GetMostCoinFlips()
        {
            var cplr = Dictionary.Values.Where(x => x.LastRole != RoleTypeId.None || x.CurrentRole.Team != Team.Dead)?.FirstOrDefault();
            foreach (CustomPlayer player in Dictionary.Values.Where(plr => plr.LastRole != RoleTypeId.None && plr.CurrentRole.Team != Team.Dead))
            {
                if (player.CoinUses <= cplr?.CoinUses)
                {
                    continue;
                }

                cplr = player;
            }

            return cplr;
        }

        public static CustomPlayer GetHighestSnakeScore()
        {
            var cplr = Dictionary.Values.Where(x => x.LastRole != RoleTypeId.None || x.CurrentRole.Team != Team.Dead)?.FirstOrDefault();
            foreach (CustomPlayer player in Dictionary.Values.Where(plr => plr.LastRole != RoleTypeId.None && plr.CurrentRole.Team != Team.Dead))
            {
                if (player.SnakeHighScore <= cplr?.SnakeHighScore)
                {
                    continue;
                }

                cplr = player;
            }

            return cplr;
        }

        /// <summary>
        /// Checks weather or not if it's friendly fire
        /// </summary>
        /// <param name="player"></param>
        /// <returns>Returns a bool value for yes or no</returns>
        public bool IsFriendlyFire(CustomPlayer player)
        {
            if (HitboxIdentity.IsDamageable(ReferenceHub, player.ReferenceHub)) 
                return false;

            return true;
        }

        /// <summary>
        /// Gets all the preferences of the Player
        /// </summary>
        /// <returns>Returns <see cref="Dictionary{RoleTypeId, int}"/> with every SCP role and preference of the player</returns>
        public Dictionary<RoleTypeId, int> GetSCPPreferences()
        {
            var scpPreference = new Dictionary<RoleTypeId, int>();

            scpPreference.Clear();

            scpPreference.Add(RoleTypeId.Scp079, ScpSpawner.GetCombinedPreferencePoints(ReferenceHub, RoleTypeId.Scp079, [])); // PC
            scpPreference.Add(RoleTypeId.Scp173, ScpSpawner.GetCombinedPreferencePoints(ReferenceHub, RoleTypeId.Scp173, [])); // Peanut
            scpPreference.Add(RoleTypeId.Scp939, ScpSpawner.GetCombinedPreferencePoints(ReferenceHub, RoleTypeId.Scp939, [])); // Dog
            scpPreference.Add(RoleTypeId.Scp106, ScpSpawner.GetCombinedPreferencePoints(ReferenceHub, RoleTypeId.Scp106, [])); // Larry
            scpPreference.Add(RoleTypeId.Scp096, ScpSpawner.GetCombinedPreferencePoints(ReferenceHub, RoleTypeId.Scp096, [])); // Shy Guy
            scpPreference.Add(RoleTypeId.Scp049, ScpSpawner.GetCombinedPreferencePoints(ReferenceHub, RoleTypeId.Scp049, [])); // Doctor
            scpPreference.Add(RoleTypeId.Scp3114, ScpSpawner.GetCombinedPreferencePoints(ReferenceHub, RoleTypeId.Scp3114, [])); // Skelly

            return scpPreference;
        }

        public void SendBroadcast(string message, ushort duration, Broadcast.BroadcastFlags type = Broadcast.BroadcastFlags.Normal, bool shouldClearPrevious = false)
        {
            Player.SendBroadcast(message, duration, type, shouldClearPrevious);
        }

        public bool Equals(CustomPlayer other)
        {
            return GameObject == other?.GameObject;
        }

        public CustomPlayer(Player plr)
        {
            Player = plr;
            ReferenceHub = plr.ReferenceHub;
            
            Dictionary.Add(plr.ReferenceHub, this);
        }

        public CustomPlayer(ReferenceHub hub)
        {
            Player = Player.Get(hub);
            ReferenceHub = hub;
            
            Dictionary.Add(hub, this);
        }
    }
}
