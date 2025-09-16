// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using PlayerRoles;
using LabApi.Features.Wrappers;
using System;

namespace XazeAPI.API.Structures
{
    public struct DisguisedPlayer
    {
        public readonly Player Player;
        public readonly RoleTypeId Disguise = RoleTypeId.None;
        public readonly Func<ReferenceHub, bool>? NeedsDisguise;
        public readonly byte UnitId;

        public DisguisedPlayer(ReferenceHub hub, RoleTypeId disguise, byte unitId = 0, Func<ReferenceHub, bool> predicate = null)
        {
            Player = Player.Get(hub);
            Disguise = disguise;
            NeedsDisguise = predicate;
            UnitId = unitId;
        }
    }
}
