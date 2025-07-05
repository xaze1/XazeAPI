// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using PlayerRoles;

namespace XazeAPI.API.Stats.Player
{
    public class PlayerKillStat : PlayerBaseStat
    {
        public static Action<LabApi.Features.Wrappers.Player> OnGainKill;
        public PlayerKillStat(ReferenceHub hub)
        {
            Hub = hub;
            Kills = 0;
            LastTeam = Team.Dead;
        }

        public PlayerKillStat(ReferenceHub hub, int kills, Team team)
        {
            Hub = hub;
            Kills = kills;
            LastTeam = team;
        }

        public PlayerKillStat()
        {
            Hub = null;
            Kills = 0;
            LastTeam = Team.Dead;
        }

        /// <summary>
        /// Entire <see cref="HashSet{T}</Player>"/> of all the tracked Player kills
        /// </summary>
        public static Dictionary<string, PlayerKillStat> RegisteredStats { get; set; } = new();

        /// <summary>
        /// Kills of the tracked Player
        /// </summary>
        public int Kills
        {
            get => _kills;
            set
            {
                if (value > _kills)
                {
                    try
                    {
                        OnGainKill?.Invoke(LabApi.Features.Wrappers.Player.Get(Hub));
                    }
                    catch
                    {
                    }
                }

                _kills = value;
            }
        }
        private int _kills { get; set; } = 0;

        /// <summary>
        /// Last recorded <see cref="Team"/> of the tracked Player
        /// </summary>
        public Team LastTeam { get; set; }

        public static PlayerKillStat Max()
        {
            PlayerKillStat highestStat = new();
            try
            {
                foreach(PlayerKillStat stat in RegisteredStats.Values)
                {
                    if (stat.Kills > highestStat.Kills)
                        highestStat = stat;
                }
            }
            catch (Exception ex)
            {
                Logging.Error("PlayerKillStat.Max() ran into a error\n" + ex.Message);
            }
            

            return highestStat;
        }

        public static bool IsTracked(ReferenceHub hub)
        {
            return RegisteredStats.ContainsKey(hub.authManager.UserId);
        }

        public static bool IsTracked(ReferenceHub hub, out PlayerKillStat trackedPlayer)
        {
            return RegisteredStats.TryGetValue(hub.authManager.UserId, out trackedPlayer);
        }

        public static bool TryGetMax(out PlayerKillStat HighestPlayerKillCount)
        {
            HighestPlayerKillCount = Max();

            return HighestPlayerKillCount.Hub is not null;
        }

        public static PlayerKillStat getHighestKills()
        {
            return Max();
        }

        public static PlayerKillStat GetStatOrDefault(ReferenceHub player)
        {
            if (RegisteredStats.TryGetValue(player.authManager.UserId, out PlayerKillStat trackedPlayer))
            {
                return trackedPlayer;
            }

            return null;
        }

        public static PlayerKillStat GetStatOrDefault(LabApi.Features.Wrappers.Player plr)
        {
            return GetStatOrDefault(plr.ReferenceHub);
        }
        
        public static int GetKillsOrDefault(ReferenceHub player)
        {
            if (RegisteredStats.TryGetValue(player.authManager.UserId, out PlayerKillStat trackedPlayer))
            {
                return trackedPlayer.Kills;
            }

            return 0;
        }
        
        public static int GetKillsOrDefault(LabApi.Features.Wrappers.Player plr)
        {
            if (RegisteredStats.TryGetValue(plr.UserId, out PlayerKillStat trackedPlayer))
            {
                return trackedPlayer.Kills;
            }

            return 0;
        }

        public static bool TryRegisterStat(ReferenceHub newPlayer, int kills = 0)
        {
            try
            {
                if (IsTracked(newPlayer))
                    return false;

                RegisteredStats.Add(newPlayer.authManager.UserId, new PlayerKillStat(newPlayer, kills, Team.Dead));
                return true;
            }
            catch (Exception ex)
            {
                Logging.Error(ex.ToString());
                return false;
            }
        }

        public static bool TryRemoveStat(ReferenceHub trackedPlayer)
        {
            if (IsTracked(trackedPlayer))
            {
                RegisteredStats.Remove(trackedPlayer.authManager.UserId);
                return true;
            }

            return false;
        }
    }
}
