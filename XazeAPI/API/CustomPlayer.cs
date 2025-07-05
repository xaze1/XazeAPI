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
using XazeAPI.API.Enums;
using UnityEngine;
using Utils.NonAllocLINQ;
using XazeAPI.API.Extensions;
using XazeAPI.API.Helpers;
using XazeAPI.API.Stats.Player;
using XazeAPI.API.Structures;

namespace XazeAPI.API
{
    public class CustomPlayer : MonoBehaviour, IEquatable<CustomPlayer>
    {
        public static List<CustomPlayer> AllPlayers = new();

        public static Action<CustomPlayer> OnPlayerAdded;
        public static Action<CustomPlayer> OnPlayerRemoved;

        public string Username
        {
            get => Player.DisplayName;
            set => Player.DisplayName = value;
        }

        public string UniqueUserId
        {
            get
            {
                if (Player.UserId.Contains("@"))
                {
                    int index = Player.UserId.IndexOf("@");
                    return Player.UserId.Substring(0, index);
                }

                return Player.UserId.Substring(0, Player.UserId.Length - 6);
            }
        }

        public string CustomInfo
        {
            get => Player.CustomInfo;
            set => Player.CustomInfo = value;
        }

        public CustomTeam Team
        {
            get
            {
                if (IsSCP)
                    return CustomTeam.SCPs;

                return Player.Team.ToCustomTeam();
            }
        }

        public bool IsSCP
        {
            get
            {
                if (Player.IsSCP)
                    return true;

                return _isScp;
            }
            set => _isScp = value;
        }
        
        private bool _isScp { get; set; } = false;


        public ReferenceHub ReferenceHub => ReferenceHub.GetHub(GameObject);
        public Player Player => Player.Get(ReferenceHub);

        // Player Stats
        public PlayerKillStat KillStat => PlayerKillStat.GetStatOrDefault(ReferenceHub);
        public PlayerDeathStat DeathStat => PlayerDeathStat.GetValueOrDefault(ReferenceHub);

        // Custom Modules
        public CustomHealthStat? HealthStat => Player.getCustomHealthStat();
        // public CustomEffectsController CustomEffects => GameObject.GetComponent<CustomEffectsController>();
        // public CustomRoleManager CustomRoleManager => CustomRoleManager.Get(Player);

        // Base Values
        public GameObject GameObject;
        public PlayerRoleBase CurrentRole => ReferenceHub.roleManager.CurrentRole;
        // public CustomRoleBase CustomRole => CustomRoleManager.CurrentRole;

        // Bools
        // public bool IsCustomRole => CustomRoleManager.IsCustomRole(Player) || SuperController.IsSuperScp(Player);
        public bool IsDisguised => Disguise != RoleTypeId.None;
        // public bool IsCustomZombie => ZombieRolesController.ActiveSpecialZombies.ContainsKey(ReferenceHub);
        // public bool IsCISpy => ChaosSpyHandler.CISpies.ContainsKey(Player.ReferenceHub);
        public bool IsInvisible => (CurrentRole as IFpcRole).FpcModule.Motor.IsInvisible;
        public bool IsInventoryFull => ReferenceHub.inventory.UserInventory.Items.Count == 8;

        // Variables
        public int CoinUses { get; set; }
        public int SnakeHighScore { get; set; }
        public float TotalDamageDone { get; set; }
        public float TotalSCPDamageDone { get; set; }
        public RoleTypeId LastRole { get; set; }

        // Var Getters
        public RoleTypeId Disguise => DisguiseHelper.DisguisedPlayers.TryGetValue(ReferenceHub, out DisguisedPlayer plr) ? plr.Disguise : RoleTypeId.None;
        //public int PersonnelSpawnChance => PlayerLevel.TryGet(Player, out var lvl) ? lvl.PersonnelChance : 10;
        
        public int ScpChance
        {
            get
            {
                using (ScpTicketsLoader scpTicketsLoader = new ScpTicketsLoader())
                {
                    return scpTicketsLoader.GetTickets(ReferenceHub, 10);
                }
            }
            set
            {
                using (ScpTicketsLoader scpTickets = new ScpTicketsLoader())
                {
                    scpTickets.ModifyTickets(ReferenceHub, value);
                }
            }
        }

        public void Awake()
        {
            AllPlayers.Add(this);

            GameObject = base.gameObject;

            LastRole = RoleTypeId.None;
            CoinUses = 0;
            TotalDamageDone = 0;
            TotalSCPDamageDone = 0;

            OnPlayerAdded?.Invoke(this);
        }

        /// <summary>
        /// Gets the active CustomPlayer
        /// </summary>
        /// <param name="hub"></param>
        /// <returns>Return the active <see cref="CustomPlayer"/></returns>
        public static CustomPlayer Get(ReferenceHub hub)
        {
            return AllPlayers.FirstOrDefault(plr => plr.ReferenceHub == hub);
        }

        /// <summary>
        /// Gets the active CustomPlayer
        /// </summary>
        /// <param name="plr"></param>
        /// <returns>Return the active <see cref="CustomPlayer"/></returns>
        public static CustomPlayer Get(Player plr)
        {
            return AllPlayers.FirstOrDefault(plr => plr.ReferenceHub == plr.ReferenceHub);
        }

        /// <summary>
        /// Tries to get the <see cref="CustomPlayer"/> object of a ReferenceHub
        /// </summary>
        /// <param name="hub">Player object</param>
        /// <param name="cplr">CustomPlayer object of the ReferenceHub</param>
        /// <returns>Returns weither or not a <see cref="CustomPlayer"/> was found</returns>
        public static bool TryGet(ReferenceHub hub, out CustomPlayer cplr)
        {
            cplr = null;
            if (AllPlayers.TryGetFirst(plr => plr.ReferenceHub == hub, out cplr))
            {
                return cplr is not null;
            }

            return false;
        }

        /// <summary>
        /// Tries to get the <see cref="CustomPlayer"/> object of a Player
        /// </summary>
        /// <param name="ply">Player object</param>
        /// <param name="cplr">CustomPlayer object of the Player</param>
        /// <returns>Returns weither or not a <see cref="CustomPlayer"/> was found</returns>
        public static bool TryGet(Player ply, out CustomPlayer cplr)
        {
            cplr = null;
            
            if (AllPlayers.TryGetFirst(plr => plr.ReferenceHub == ply.ReferenceHub, out cplr))
            {
                return cplr is not null;
            }

            return false;
        }

        /// <summary>
        /// Goes through all the active and cached players to get highest damage done to Human classes
        /// </summary>
        /// <returns>Returns <see cref="CustomPlayer"/> object from player with highest damage</returns>
        public static CustomPlayer GetHighestDamageDone()
        {
            CustomPlayer cplr = AllPlayers.Where(x => x.LastRole != RoleTypeId.None || x.CurrentRole.Team != PlayerRoles.Team.Dead)?.FirstOrDefault();
            foreach (CustomPlayer player in AllPlayers)
            {
                if (player.LastRole == RoleTypeId.None && player.CurrentRole.Team == PlayerRoles.Team.Dead)
                {
                    continue;
                }

                if (player.TotalDamageDone <= cplr.TotalDamageDone)
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
            CustomPlayer cplr = AllPlayers.Where(x => x.LastRole != RoleTypeId.None || x.CurrentRole.Team != PlayerRoles.Team.Dead)?.FirstOrDefault();
            foreach (CustomPlayer player in AllPlayers)
            {
                if (player.LastRole == RoleTypeId.None && player.CurrentRole.Team == PlayerRoles.Team.Dead)
                {
                    continue;
                }

                if (player.TotalSCPDamageDone <= cplr.TotalSCPDamageDone)
                {
                    continue;
                }

                cplr = player;
            }

            return cplr;
        }

        public static CustomPlayer GetMostCoinFlips()
        {
            CustomPlayer cplr = AllPlayers.Where(x => x.LastRole != RoleTypeId.None || x.CurrentRole.Team != PlayerRoles.Team.Dead)?.FirstOrDefault();
            foreach (CustomPlayer player in AllPlayers)
            {
                if (player.LastRole == RoleTypeId.None && player.CurrentRole.Team == PlayerRoles.Team.Dead)
                {
                    continue;
                }

                if (player.CoinUses <= cplr.CoinUses)
                {
                    continue;
                }

                cplr = player;
            }

            return cplr;
        }

        public static CustomPlayer GetHighestSnakeScore()
        {
            CustomPlayer cplr = AllPlayers.Where(x => x.LastRole != RoleTypeId.None || x.CurrentRole.Team != PlayerRoles.Team.Dead)?.FirstOrDefault();
            foreach (CustomPlayer player in AllPlayers)
            {
                if (player.LastRole == RoleTypeId.None && player.CurrentRole.Team == PlayerRoles.Team.Dead)
                {
                    continue;
                }

                if (player.SnakeHighScore <= cplr.SnakeHighScore)
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
            if (Team == player.Team) return true;

            return false;
        }

        /// <summary>
        /// Gets all the preferences of the Player
        /// </summary>
        /// <returns>Returns <see cref="Dictionary{RoleTypeId, int}"/> with every SCP role and preference of the player</returns>
        public Dictionary<RoleTypeId, int> GetSCPPreferences()
        {
            Dictionary<RoleTypeId, int> scpPreference = new Dictionary<RoleTypeId, int>();

            scpPreference.Clear();

            scpPreference.Add(RoleTypeId.Scp079, ScpSpawner.GetPreferenceOfPlayer(ReferenceHub, RoleTypeId.Scp079)); // PC
            scpPreference.Add(RoleTypeId.Scp173, ScpSpawner.GetPreferenceOfPlayer(ReferenceHub, RoleTypeId.Scp173)); // Peanut
            scpPreference.Add(RoleTypeId.Scp939, ScpSpawner.GetPreferenceOfPlayer(ReferenceHub, RoleTypeId.Scp939)); // Dog
            scpPreference.Add(RoleTypeId.Scp106, ScpSpawner.GetPreferenceOfPlayer(ReferenceHub, RoleTypeId.Scp106)); // Larry
            scpPreference.Add(RoleTypeId.Scp096, ScpSpawner.GetPreferenceOfPlayer(ReferenceHub, RoleTypeId.Scp096)); // Shy Guy
            scpPreference.Add(RoleTypeId.Scp049, ScpSpawner.GetPreferenceOfPlayer(ReferenceHub, RoleTypeId.Scp049)); // Doctor
            scpPreference.Add(RoleTypeId.Scp3114, ScpSpawner.GetPreferenceOfPlayer(ReferenceHub, RoleTypeId.Scp3114)); // Skelly

            return scpPreference;
        }

        public void OnDestroy()
        {
            OnPlayerRemoved?.Invoke(this);
            AllPlayers.Remove(this);
        }

        public void SendBroadcast(string message, ushort duration, Broadcast.BroadcastFlags type = Broadcast.BroadcastFlags.Normal, bool shouldClearPrevious = false)
        {
            Player.SendBroadcast(message, duration, type, shouldClearPrevious);
        }

        public static void ResetDictionary()
        {
            foreach(var plr in AllPlayers)
            {
                plr.OnDestroy();
            }
        }

        public bool Equals(CustomPlayer other)
        {
            return this.GameObject == other.GameObject;
        }
    }
}
