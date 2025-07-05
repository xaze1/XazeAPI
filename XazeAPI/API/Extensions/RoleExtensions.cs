// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using PlayerRoles;
using System;
using UnityEngine;

namespace XazeAPI.API.Extensions
{
    public static class RoleExtensions
    {
        public static PlayerRoleBase GetRoleBase(this RoleTypeId targetId)
        {
            if (PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(targetId, out var result))
            {
                return result;
            }
            
            Debug.LogError($"Role #{targetId} could not be found.");
            if (!PlayerRoleLoader.TryGetRoleTemplate(RoleTypeId.None, out result))
            {
                throw new NotImplementedException("Role change failed. Default role is not correctly implemented.");
            }

            return result;
        }
    }
}
