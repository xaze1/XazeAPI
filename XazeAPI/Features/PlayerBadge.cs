// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using UnityEngine;
using XazeAPI.Features.Helpers;

namespace XazeAPI.Features;

public class PlayerBadge(Player owner)
{
    public Player Owner { get; } = owner;
    public UserGroup Group { get; } = owner.UserGroup;

    public Color Color
    {
        get
        {
            if (!Owner.ReferenceHub.serverRoles.NamedColorsDic.TryGetValue(Owner.ReferenceHub.serverRoles.MyColor, out var color))
                return Color.clear;
            return color.SpeakingColor;
        }
    }

    public System.Drawing.Color DisplayColor => MainHelper.ColorFromRGB(Color);
    
    public string Name => Group.Name;
    public string Text => Group.BadgeText;
    public PlayerPermissions Permissions => (PlayerPermissions)Group.Permissions;
    public string[] PluginPermissions => Owner.GetPermissions();
}