// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace XazeAPI.API.Stats
{
    public class PlayerDeathStat : PlayerBaseStat
    {
        public static Dictionary<string, PlayerDeathStat> List { get; set; } = new();
        
        public Team LastTeam { get; set; } = Team.Dead;

        public static PlayerDeathStat Get(Player plr)
        {
            if (plr.IsDestroyed || plr.IsHost)
                return new PlayerDeathStat();
            
            if (List.TryGetValue(plr.UserId, out var stats))
                return stats;

            var stat = new PlayerDeathStat();
            stat.Create(plr);
            List.Add(plr.UserId, stat);
            return stat;
        }

        public static bool TryGet(Player plr, out PlayerDeathStat stat)
        {
            if (!List.TryGetValue(plr.UserId, out stat))
            {
                stat = Get(plr);
            }
            
            return stat.IsSet;
        }
        
        public static PlayerDeathStat Highest()
        {
            PlayerDeathStat highestStat = new();
            try
            {
                foreach(var stat in List.Values)
                {
                    if (stat.Value > highestStat.Value)
                        highestStat = stat;
                }
            }
            catch (Exception ex)
            {
                Logging.Error("PlayerKillStat.Max() ran into a error\n" + ex.Message);
            }

            return highestStat;
        }
    }
}
