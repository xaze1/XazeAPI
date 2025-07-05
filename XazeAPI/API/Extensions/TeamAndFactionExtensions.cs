// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using XazeAPI.API.Enums;
using PlayerRoles;

namespace XazeAPI.API.Extensions
{
    public static class TeamAndFactionExtensions
    {
        public static CustomTeam ToCustomTeam(this Team team)
        {
            return team switch
            {
                Team.SCPs => CustomTeam.SCPs,
                Team.FoundationForces => CustomTeam.FoundationForces,
                Team.ChaosInsurgency => CustomTeam.ChaosInsurgency,
                Team.Scientists => CustomTeam.Scientists,
                Team.ClassD => CustomTeam.ClassD,
                Team.Dead => CustomTeam.Dead,
                Team.OtherAlive => CustomTeam.OtherAlive,
                Team.Flamingos => CustomTeam.Flamingos,
                _ => CustomTeam.OtherAlive
            };
        }

        public static Team ToTeam(this CustomTeam team)
        {
            return team switch
            {
                CustomTeam.SCPs => Team.SCPs,
                CustomTeam.SuperScp => Team.SCPs,
                CustomTeam.FoundationForces => Team.FoundationForces,
                CustomTeam.ChaosInsurgency => Team.ChaosInsurgency,
                CustomTeam.Scientists => Team.Scientists,
                CustomTeam.ClassD => Team.ClassD,
                CustomTeam.Dead => Team.Dead,
                CustomTeam.Flamingos => Team.Flamingos,
                CustomTeam.Daybreak => Team.Flamingos,
                _ => Team.OtherAlive
            };
        }

        public static CustomFaction ToCustomFaction(this Faction faction)
        {
            return faction switch
            {
                Faction.SCP => CustomFaction.SCP,
                Faction.FoundationStaff => CustomFaction.FoundationStaff,
                Faction.FoundationEnemy => CustomFaction.FoundationEnemy,
                Faction.Unclassified => CustomFaction.Unclassified,
                Faction.Flamingos => CustomFaction.Flamingos,
                _ => CustomFaction.Unclassified
            };
        }

        public static Faction ToFaction(this CustomFaction faction)
        {
            return faction switch
            {
                CustomFaction.SCP => Faction.SCP,
                CustomFaction.FoundationStaff => Faction.FoundationStaff,
                CustomFaction.FoundationEnemy => Faction.FoundationEnemy,
                CustomFaction.Unclassified => Faction.Unclassified,
                CustomFaction.Flamingos => Faction.Flamingos,
                _ => Faction.Unclassified
            };
        }

        public static CustomFaction GetFaction(this CustomTeam team)
        {
            if (team.HasFlag(CustomTeam.SuperScp))
            {
                return CustomFaction.SCP | CustomFaction.Personnel;
            }

            switch (team)
            {
                case CustomTeam.SCPs:
                    return CustomFaction.SCP;

                case CustomTeam.Flamingos:
                    return CustomFaction.Flamingos;

                case CustomTeam.FoundationForces:
                case CustomTeam.Scientists:
                    return CustomFaction.FoundationStaff;

                case CustomTeam.ChaosInsurgency:
                case CustomTeam.ClassD:
                    return CustomFaction.FoundationEnemy;

                case CustomTeam.Personnel:
                    return CustomFaction.Personnel;

                case CustomTeam.Daybreak:
                    return CustomFaction.Daybreak;

                case CustomTeam.TimeBreakers:
                case CustomTeam.NullEntity:
                    return CustomFaction.NullEvent;

                default:
                    return CustomFaction.Unclassified;
            }
        }
    }
}
