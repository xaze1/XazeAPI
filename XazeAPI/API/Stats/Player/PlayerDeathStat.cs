// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using PlayerRoles;
using Utils.NonAllocLINQ;

namespace XazeAPI.API.Stats.Player
{
    public class PlayerDeathStat
    {
        public static Action<LabApi.Features.Wrappers.Player> OnGainDeath;
        public PlayerDeathStat() 
        {
            Hub = null;
            Deaths = 0;
            TeamBeforeDeath = Team.Dead;
        }
        
        public PlayerDeathStat(ReferenceHub hub) 
        {
            Hub = hub;
            Deaths = 0;
            TeamBeforeDeath = Team.Dead;
        }

        public PlayerDeathStat(ReferenceHub hub, int deaths, Team lastTeam)
        { 
            Hub = hub;
            Deaths = deaths;
            TeamBeforeDeath = lastTeam;
        }

        /// <summary>
        /// Entire <see cref="HashSet{T}</Player>"/> of all the tracked Player Deaths
        /// </summary>
        public static List<PlayerDeathStat> RegisteredStats { get; set; } = new();

        /// <summary>
        /// <see cref="ReferenceHub"/> of the tracked Player
        /// </summary>
        public ReferenceHub Hub { get; set; }

        /// <summary>
        /// Deaths of the tracked Player
        /// </summary>
        public int Deaths
        {
            get => _deaths;
            set
            {
                if (value > _deaths)
                {
                    try
                    {
                        OnGainDeath?.Invoke(LabApi.Features.Wrappers.Player.Get(Hub));
                    }
                    catch
                    {
                    }
                }

                _deaths = value;
            }
        }
        private int _deaths { get; set; } = 0;

        /// <summary>
        /// Last recorded <see cref="Team"/> of the tracked Player
        /// </summary>
        public Team TeamBeforeDeath { get; set; }

        public static PlayerDeathStat Max()
        {
            PlayerDeathStat highestStat = new();
            try
            {
                foreach (PlayerDeathStat stat in RegisteredStats)
                {
                    if (stat.Deaths > highestStat.Deaths)
                        highestStat = stat;
                }
            }
            catch (Exception ex)
            {
                Logging.Error("PlayerDeathStat.Max() ran into a error\n" + ex.Message);
            }


            return highestStat;
        }

        public static bool IsTracked(ReferenceHub hub)
        {
            return RegisteredStats.TryGetFirst(x => x.Hub == hub, out PlayerDeathStat _);
        }
        
        public static bool IsTracked(ReferenceHub hub, out PlayerDeathStat trackedPlayer)
        {
            return RegisteredStats.TryGetFirst(x => x.Hub == hub, out trackedPlayer);
        }

        public static bool TryGetMax(out PlayerDeathStat HighestDeathCount)
        {
            HighestDeathCount = Max();

            return HighestDeathCount.Hub is not null;
        }

        public static PlayerDeathStat getHighestDeaths()
        {
            return Max();
        }

        public static PlayerDeathStat GetValueOrDefault(ReferenceHub player)
        {
            if (RegisteredStats.TryGetFirst(x => x.Hub == player, out PlayerDeathStat trackedPlayer))
            {
                return trackedPlayer;
            }

            return null;
        }

        public static int GetDeathsOrDefault(ReferenceHub player)
        {
            if (RegisteredStats.TryGetFirst(x => x.Hub == player, out PlayerDeathStat trackedPlayer))
            {
                return trackedPlayer.Deaths;
            }

            return 0;
        }

        public static bool TryRegisterStat(ReferenceHub newPlayer)
        {
            try
            {
                if (RegisteredStats.Any(stat => stat.Hub == newPlayer))
                {
                    return false;
                }

                RegisteredStats.Add(new PlayerDeathStat(newPlayer));
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
            if (!IsTracked(trackedPlayer))
            {
                return false;
            }
            
            RegisteredStats.Remove(GetValueOrDefault(trackedPlayer));
            return true;

        }
    }
}
