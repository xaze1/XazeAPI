// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using PlayerRoles;
using LabApi.Features.Wrappers;

namespace XazeAPI.API.Stats
{
    public class PlayerKillStat : PlayerBaseStat
    {
        public static Dictionary<string, PlayerKillStat> List { get; set; } = new();
        
        public Team LastTeam { get; set; } = Team.Dead;
        
        public static PlayerKillStat Get(Player plr)
        {
            if (plr.IsDestroyed || plr.IsHost)
                return new PlayerKillStat();
            
            if (List.TryGetValue(plr.UserId, out var stats))
                return stats;

            var stat = new PlayerKillStat();
            stat.Create(plr);
            List.Add(plr.UserId, stat);
            return stat;
        }

        public static bool TryGet(Player plr, out PlayerKillStat stat)
        {
            if (!List.TryGetValue(plr.UserId, out stat))
            {
                stat = Get(plr);
            }
            
            return stat.IsSet;
        }
        
        public static PlayerKillStat Highest()
        {
            PlayerKillStat highestStat = new();
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
