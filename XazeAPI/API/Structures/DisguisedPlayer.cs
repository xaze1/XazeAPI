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
        public Player Player;
        public RoleTypeId Disguise = RoleTypeId.None;
        public byte UnitId = 0;
        public Func<ReferenceHub, bool>? NeedsDisguise;

        public DisguisedPlayer(ReferenceHub hub, RoleTypeId disguise, byte unit, Func<ReferenceHub, bool> predicate = null)
        {
            Player = Player.Get(hub);
            Disguise = disguise;
            UnitId = unit;
            NeedsDisguise = predicate;
        }
    }
}
