// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using PlayerRoles;
using XazeAPI.API.Enums;

namespace XazeAPI.CustomElements;

/// <summary>
/// Provides extensions for working with RueI <see cref="Enum"/>s.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Quickly determines if a <see cref="Roles"/> has a <see cref="RoleTypeId"/>.
    /// </summary>
    /// <param name="first">The first <see cref="Roles"/>.</param>
    /// <param name="second">The other <see cref="Roles"/>.</param>
    /// <returns>A value indicating whether or not the first has the <see cref="RoleTypeId"/> of the second.</returns>
    public static bool HasFlagFast(this Roles first, RoleTypeId second)
    {
        int toInt = (int)second;
        if (toInt == -1)
        {
            return false;
        }

        Roles secondCasted = (Roles)(1 << (int)second);
        return (first & secondCasted) == secondCasted;
    }
}