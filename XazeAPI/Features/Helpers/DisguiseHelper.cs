// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;
using Mirror;
using NorthwoodLib.Pools;
using RueI.API;
using RueI.API.Elements;
using RueI.Utils;
using XazeAPI.API.Structures;

namespace XazeAPI.API.Helpers
{
    public static class DisguiseHelper
    {
        public static readonly Dictionary<ReferenceHub, DisguisedPlayer> DisguisedPlayers = new();
        public static readonly Tag DisguiseReference = new("Xaze-DisguiseReference");

        public static RoleTypeId OnRoleSyncEvent(ReferenceHub user, ReferenceHub receiver, RoleTypeId role, NetworkWriter writer)
        {
            Player User = Player.Get(user);
            Player Receiver = Player.Get(receiver);

            if (User is null || Receiver is null || !role.IsAlive() || !DisguisedPlayers.TryGetValue(user, out DisguisedPlayer disguise) || disguise.NeedsDisguise != null && !disguise.NeedsDisguise(Receiver.ReferenceHub))
            {
                return role;
            }

            var roleBase = disguise.Disguise.GetRoleBase();
            if (roleBase is HumanRole { UsesUnitNames: true })
            {
                writer.WriteByte(disguise.UnitId);
            }
            
            return disguise.Disguise;
        }


        /// <summary>
        /// Sets a Players Appearance back to their Role
        /// </summary>
        /// <param name="player">Player which has their Appearance reset</param>
        public static void ResetAppearance(this Player player)
        {
            ResetAppearance(player?.ReferenceHub);
        }

        /// <summary>
        /// Sets a Players Appearance back to their Role
        /// </summary>
        /// <param name="player">Player which has their Appearance reset</param>
        public static void ResetAppearance(this ReferenceHub player)
        {
            if (player is null)
            {
                return;
            }
            
            if (!DisguisedPlayers.ContainsKey(player))
            {
                return;
            }

            var display = RueDisplay.Get(player);
            display.Remove(DisguiseReference);

            DisguisedPlayers.Remove(player);
        }
        
        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s Disguise is reset.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="disguise">Role type.</param>
        /// <param name="NeedsDisguise">Predicate for who sees the disguise</param>
        public static void ChangeAppearance(this Player player, RoleTypeId disguise, byte unitId = 0, Func<ReferenceHub, bool> NeedsDisguise = null) => ChangeAppearance(player?.ReferenceHub, new DisguisedPlayer(player?.ReferenceHub, disguise, unitId, NeedsDisguise));
        
        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s Disguise is reset.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="disguise">Disguise Variables</param>
        public static void ChangeAppearance(this Player player, DisguisedPlayer disguise) => ChangeAppearance(player?.ReferenceHub, disguise);
        
        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s Disguise is reset.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="disguise">Role type.</param>
        /// <param name="NeedsDisguise">Predicate for who sees the disguise</param>
        public static void ChangeAppearance(this ReferenceHub player, RoleTypeId disguise, byte unitId = 0, Func<ReferenceHub, bool> NeedsDisguise = null) => ChangeAppearance(player, new DisguisedPlayer(player, disguise, unitId, NeedsDisguise));

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s Disguise is reset.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="disguise">Disguise Variables</param>
        public static void ChangeAppearance(this ReferenceHub player, DisguisedPlayer disguise)
        {
            if (player is null)
            {
                return;
            }
            
            if (player.Mode != CentralAuth.ClientInstanceMode.ReadyClient)
                return;
            
            if (disguise.Disguise == RoleTypeId.None || !disguise.Disguise.IsAlive())
            {
                throw new InvalidEnumArgumentException("Disguise can't be dead/None");
            }
            
            var roleBase = disguise.Disguise.GetRoleBase();

            var display = RueDisplay.Get(player);
            StringBuilder sb = StringBuilderPool.Shared.Rent();
            sb.SetSize(65, RueI.Utils.Enums.MeasurementUnit.Percentage)
                .SetAlignment(RueI.Utils.Enums.AlignStyle.Left)
                .Append("Current Disguise: " + roleBase.RoleName);
            
            display.Show(DisguiseReference, new BasicElement(150, StringBuilderPool.Shared.ToStringReturn(sb)));
            
            DisguisedPlayers[player] = disguise;
        }

        public static bool TryGetDisguise(Player target, out DisguisedPlayer disguise)
        {
            disguise = default;
            if (target is null)
            {
                return false;
            }
            
            foreach (var plr in DisguisedPlayers.Values.Where(plr => plr.Player.UserId == target.UserId))
            {
                disguise = plr;
                return true;
            }
            
            return false;
        }
    }
}
