using PlayerRoles;
using System;

namespace XazeAPI.API.Enums
{
    [Flags]
    public enum CustomFaction
    {
        /// <summary>
        /// SCP Faction
        /// </summary>
        SCP = 1 << Faction.SCP,

        /// <summary>
        /// SCP Foundation Staff
        /// Scientist, Facility Guard, NtfPrivate, NtfSpecialist NtfSergeant, NtfCaptain,
        /// </summary>
        FoundationStaff = 1 << Faction.FoundationStaff,

        /// <summary>
        /// SCP Foundation Enemy
        /// Class D, Chaos Conscript, Chaos Rifleman, Chaos Marauder, Chaos Respressor
        /// </summary>
        FoundationEnemy = 1 << Faction.FoundationEnemy,

        /// <summary>
        /// Unknown
        /// Spectator, Overwatch, None, Tutorial
        /// </summary>
        Unclassified = 1 << Faction.Unclassified,

        /// <summary>
        /// Flamingos
        /// AlphaFlamingo, Flamingo
        /// </summary>
        Flamingos = 1 << Faction.Flamingos,

        /// <summary>
        /// Such as Super SCP or Personnel
        /// </summary>
        Personnel = 1 << 5,

        /// <summary>
        /// Such as Null Entity and Time Breakers
        /// </summary>
        NullEvent = 1 << 6,

        /// <summary>
        /// Daybreak Zombies
        /// </summary>
        Daybreak = 1 << 7,
    }
}
