using PlayerRoles;
using System;

namespace XazeAPI.API.Enums
{
    [Flags]
    public enum CustomTeam
    {
        /// <summary>
        /// SCPs
        /// </summary>
        SCPs = 1 << Team.SCPs,

        /// <summary>
        /// Foundation Forces
        /// </summary>
        FoundationForces = 1 << Team.FoundationForces,

        /// <summary>
        /// Chaos Insurgency 
        /// </summary>
        ChaosInsurgency = 1 << Team.ChaosInsurgency,

        /// <summary>
        /// Scientists
        /// </summary>
        Scientists = 1 << Team.Scientists,

        /// <summary>
        /// Class D Personnel
        /// </summary>
        ClassD = 1 << Team.ClassD,

        /// <summary>
        /// Dead players like None, Spectator or Overwatch
        /// </summary>
        Dead = 1 << Team.Dead,

        /// <summary>
        /// Mostly Tutorials
        /// </summary>
        OtherAlive = 1 << Team.OtherAlive,

        /// <summary>
        /// Flamingos
        /// </summary>
        Flamingos = 1 << Team.Flamingos,

        // Custom Teams

        /// <summary>
        /// Personnel/Serpent
        /// </summary>
        Personnel = 1 << 8,

        /// <summary>
        /// Friends with Personnel and Scps
        /// </summary>
        SuperScp = 1 << 9,

        /// <summary>
        /// Friends with Time Breakers
        /// </summary>
        NullEntity = 1 << 10,

        /// <summary>
        /// Friends with Null Entity
        /// </summary>
        TimeBreakers = 1 << 11,

        /// <summary>
        /// Hostile to everything, but Daybreak Entities
        /// </summary>
        Daybreak = 1 << 12,
    }
}
