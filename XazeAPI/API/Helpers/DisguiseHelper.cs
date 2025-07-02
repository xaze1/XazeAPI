using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.Spectating;
using RelativePositioning;
using RueI.Displays;
using RueI.Elements;
using RueI.Extensions.HintBuilding;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LabApi.Features.Extensions;
using UnityEngine;
using LabApi.Features.Wrappers;
using MEC;
using XazeAPI.API.Structures;

namespace XazeAPI.API.Helpers
{
    public static class DisguiseHelper
    {
        public static Dictionary<ReferenceHub, DisguisedPlayer> DisguisedPlayers = new();
        public static IElemReference<SetElement> DisguiseReference = DisplayCore.GetReference<SetElement>();

        public static void SendDisguises(ReferenceHub user, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            if (newRole.Team != Team.Dead && oldRole.Team != Team.Dead)
            {
                return;
            }

            Timing.CallDelayed(0.05f, () =>
            {
                List<DisguisedPlayer> disguises = DisguisedPlayers.Values.ToList();
                foreach (DisguisedPlayer plr in disguises)
                {
                    ChangeAppearance(plr.Player, plr.Disguise, true, plr.UnitId);

                    if (plr.NeedsDisguise == null)
                    {
                        continue;
                    }

                    ResetAppearance(plr.Player, Player.List.Where(x => !plr.NeedsDisguise(x.ReferenceHub) && x.NetworkId != plr.Player.NetworkId), false);
                }
            });
        }

        public static void ResetAppearance(this ReferenceHub player, bool remove = true)
        {
            ResetAppearance(player, Player.List.Where(x => x.ReferenceHub.Mode == CentralAuth.ClientInstanceMode.ReadyClient), remove);
        }
        
        public static void ResetAppearance(this ReferenceHub player, IEnumerable<Player> playersToAffect, bool remove = true)
        {
            ResetAppearance(Player.Get(player), playersToAffect, remove);
        }
        
        public static void ResetAppearance(this Player player, bool remove = true)
        {
            ResetAppearance(player, Player.List.Where(x => x.ReferenceHub.Mode == CentralAuth.ClientInstanceMode.ReadyClient), remove);
        }

        /// <summary>
        /// Sets a Players Appearance back to their Role
        /// </summary>
        /// <param name="player">Player which has their Appearance reset</param>
        public static void ResetAppearance(this Player player, IEnumerable<Player> playersToAffect, bool remove = true)
        {
            if (!DisguisedPlayers.ContainsKey(player.ReferenceHub))
            {
                return;
            }

            if (player.IsAlive)
            {
                ChangeAppearance(player, player.Role, playersToAffect);
            }

            if (!remove)
            {
                return;
            }

            DisplayCore core = DisplayCore.Get(player.ReferenceHub);
            SetElement element = core.GetElement(DisguiseReference);

            element.Content = "";
            element.Position = 1100;
            core.RemoveReference(DisguiseReference);
            core.Update();

            DisguisedPlayers.Remove(player.ReferenceHub);
        }

        public static void OnRoleChange(ReferenceHub user, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (newRole.Team == Team.Dead || prevRole.Team == Team.Dead)
            {
                return;
            }

            if (!Player.TryGet(user.gameObject, out Player player) || !player.IsAlive)
            {
                return;
            }

            if (!DisguisedPlayers.TryGetValue(user, out DisguisedPlayer plr)) return;

            ChangeAppearance(player, plr.Disguise, false, plr.UnitId);
        }

        public static void ChangeAppearance(this Player player, RoleTypeId type, bool skipJump = false, byte unitId = 0) => ChangeAppearance(player, type, Player.List.Where(x => x.UserId != player.UserId && x.ReferenceHub.Mode == CentralAuth.ClientInstanceMode.ReadyClient), skipJump, unitId);
        public static void ChangeAppearance(this Player player, DisguisedPlayer disguise, bool skipJump = false) => ChangeAppearance(player, disguise, Player.List.Where(x => x.UserId != player.UserId && x.ReferenceHub.Mode == CentralAuth.ClientInstanceMode.ReadyClient), skipJump);

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        /// <param name="playersToAffect">The players who should see the changed appearance.</param>
        /// <param name="skipJump">Whether or not to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public static void ChangeAppearance(this Player player, RoleTypeId type, IEnumerable<Player> playersToAffect, bool skipJump = false, byte unitId = 0)
        {
            if (!(player.ReferenceHub.Mode == CentralAuth.ClientInstanceMode.ReadyClient))
                return;

            bool isRisky = type.GetTeam() is Team.Dead || !player.IsAlive;
            PlayerRoleBase roleBase = type.GetRoleBase();
            List<Player> spectators = playersToAffect.Where((Player plr) => player.ReferenceHub.IsSpectatedBy(plr.ReferenceHub)).ToList();

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteUShort(38952);
            writer.WriteUInt(player.NetworkId);
            writer.WriteRoleType(type);

            if (roleBase is HumanRole humanRole && humanRole.UsesUnitNames)
            {
                if (player.RoleBase is not HumanRole)
                    isRisky = true;
                writer.WriteByte(unitId);
            }

            if (roleBase is FpcStandardRoleBase fpc)
            {
                if (player.RoleBase is not FpcStandardRoleBase playerfpc)
                    isRisky = true;
                else
                    fpc = playerfpc;

                ushort value = 0;
                fpc?.FpcModule.MouseLook.GetSyncValues(0, out value, out ushort _);
                writer.WriteRelativePosition(new RelativePosition(player.ReferenceHub));
                writer.WriteUShort(value);
            }

            if (roleBase is ZombieRole)
            {
                if (player.RoleBase is not ZombieRole)
                    isRisky = true;

                writer.WriteUShort((ushort)Mathf.Clamp(Mathf.CeilToInt(player.MaxHealth), ushort.MinValue, ushort.MaxValue));
            }

            foreach (Player target in playersToAffect.Where(player => player.ReferenceHub.Mode == CentralAuth.ClientInstanceMode.ReadyClient))
            {
                if (!isRisky)
                    target.Connection.Send(writer.ToArraySegment());
            }

            if (playersToAffect.Contains(player))
            {
                StringBuilder sb = new StringBuilder()
                    .SetSize(65, RueI.Parsing.Enums.MeasurementUnit.Percentage)
                    .SetAlignment(HintBuilding.AlignStyle.Left)
                    .Append("Current Disguise: " + type switch
                    {
                        RoleTypeId.NtfSpecialist => "NTF Specialist",
                        RoleTypeId.NtfSergeant => "NTF Sergeant",
                        RoleTypeId.NtfCaptain => "NTF Captain",
                        RoleTypeId.ChaosConscript => "Chaos Conscript",
                        RoleTypeId.ChaosRifleman => "Chaos Rifleman",
                        RoleTypeId.ChaosMarauder => "Chaos Marauder",
                        RoleTypeId.ChaosRepressor => "Chaos Repressor",
                        RoleTypeId.ClassD => "Class-D",
                        RoleTypeId.FacilityGuard => "Facility Guard",
                        _ => type
                    });

                DisplayCore core = DisplayCore.Get(player.ReferenceHub);
                SetElement element = core.GetElementOrNew(DisguiseReference, () => new SetElement(150, sb.ToString()));

                core.AddAsReference(DisguiseReference, element);
                element.Content = sb.ToString();
                element.Position = 150;
                core.Update();
            }
            
            if (!DisguisedPlayers.TryGetValue(player.ReferenceHub, out DisguisedPlayer plr) || plr.Disguise != type)
            {
                DisguisedPlayers[player.ReferenceHub] = new DisguisedPlayer(player.ReferenceHub, type, unitId);
            }

            NetworkWriterPool.Return(writer);

            // To counter a bug that makes the player invisible until they move after changing their appearance, we will teleport them upwards slightly to force a new position update for all clients.
            if (!skipJump)
                player.Position += Vector3.up * 0.01f;

            foreach(var spectator in spectators)
            {
                if (spectator.ReferenceHub.roleManager.CurrentRole is not SpectatorRole spectatorRole) continue;
                spectatorRole.SyncedSpectatedNetId = 0u;

                if (player.IsAlive)
                    spectatorRole.SyncedSpectatedNetId = player.NetworkId;
            }
        }
        
        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        /// <param name="playersToAffect">The players who should see the changed appearance.</param>
        /// <param name="skipJump">Whether or not to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public static void ChangeAppearance(this Player player, DisguisedPlayer disguise, IEnumerable<Player> playersToAffect, bool skipJump = false)
        {
            if (disguise.Disguise == default || disguise.Disguise == RoleTypeId.None)
            {
                throw new InvalidEnumArgumentException("Disguise can't be None");
            }

            if (!(player.ReferenceHub.Mode == CentralAuth.ClientInstanceMode.ReadyClient))
                return;

            bool isRisky = disguise.Disguise.GetTeam() is Team.Dead || !player.IsAlive;
            PlayerRoleBase roleBase = disguise.Disguise.GetRoleBase();
            List<Player> spectators = playersToAffect.Where((Player plr) => player.ReferenceHub.IsSpectatedBy(plr.ReferenceHub)).ToList();

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteUShort(38952);
            writer.WriteUInt(player.NetworkId);
            writer.WriteRoleType(disguise.Disguise);

            if (roleBase is HumanRole humanRole && humanRole.UsesUnitNames)
            {
                if (player.RoleBase is not HumanRole)
                    isRisky = true;
                writer.WriteByte(disguise.UnitId);
            }

            if (roleBase is FpcStandardRoleBase fpc)
            {
                if (player.RoleBase is not FpcStandardRoleBase playerfpc)
                    isRisky = true;
                else
                    fpc = playerfpc;

                ushort value = 0;
                fpc?.FpcModule.MouseLook.GetSyncValues(0, out value, out ushort _);
                writer.WriteRelativePosition(new RelativePosition(player.ReferenceHub));
                writer.WriteUShort(value);
            }

            if (roleBase is ZombieRole)
            {
                if (player.RoleBase is not ZombieRole)
                    isRisky = true;

                writer.WriteUShort((ushort)Mathf.Clamp(Mathf.CeilToInt(player.MaxHealth), ushort.MinValue, ushort.MaxValue));
            }

            foreach (Player target in playersToAffect.Where(player => player.ReferenceHub.Mode == CentralAuth.ClientInstanceMode.ReadyClient))
            {
                if (target != player || !isRisky)
                    target.Connection.Send(writer.ToArraySegment());
            }

            if (playersToAffect.Contains(player))
            {
                StringBuilder sb = new StringBuilder()
                        .SetSize(65, RueI.Parsing.Enums.MeasurementUnit.Percentage)
                        .SetAlignment(HintBuilding.AlignStyle.Left)
                        .Append("Current Disguise: " + disguise.Disguise switch
                        {
                            RoleTypeId.NtfSpecialist => "NTF Specialist",
                            RoleTypeId.NtfSergeant => "NTF Sergeant",
                            RoleTypeId.NtfCaptain => "NTF Captain",
                            RoleTypeId.ChaosConscript => "Chaos Conscript",
                            RoleTypeId.ChaosRifleman => "Chaos Rifleman",
                            RoleTypeId.ChaosMarauder => "Chaos Marauder",
                            RoleTypeId.ChaosRepressor => "Chaos Repressor",
                            RoleTypeId.ClassD => "Class-D",
                            RoleTypeId.FacilityGuard => "Facility Guard",
                            _ => disguise.Disguise
                        });

                DisplayCore core = DisplayCore.Get(player.ReferenceHub);
                SetElement element = core.GetElementOrNew(DisguiseReference, () => new SetElement(150, sb.ToString()));

                core.AddAsReference(DisguiseReference, element);
                element.Content = sb.ToString();
                element.Position = 150;
                core.Update();
            }

            DisguisedPlayers[player.ReferenceHub] = disguise;

            NetworkWriterPool.Return(writer);

            // To counter a bug that makes the player invisible until they move after changing their appearance, we will teleport them upwards slightly to force a new position update for all clients.
            if (!skipJump)
                player.Position += Vector3.up * 0.01f;

            foreach(var spectator in spectators)
            {
                if (spectator.ReferenceHub.roleManager.CurrentRole is not SpectatorRole spectatorRole)
                {
                    continue;
                }

                spectatorRole.SyncedSpectatedNetId = 0u;

                if (player.IsAlive)
                {
                    spectatorRole.SyncedSpectatedNetId = player.NetworkId;
                }
            }
        }

        public static bool TryGetDisguise(Player target, out DisguisedPlayer disguise)
        {
            disguise = default;
            foreach(DisguisedPlayer plr in DisguisedPlayers.Values)
            {
                if (plr.Player.UserId == target.UserId)
                {
                    disguise = plr;
                    return true;
                }
            }
            
            return false;
        }
    }
}
