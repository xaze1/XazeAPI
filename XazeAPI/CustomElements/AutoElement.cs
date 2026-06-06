// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using RueI.API;
using RueI.API.Elements;
using XazeAPI.API.Enums;
using XazeAPI.API.Extensions;

namespace XazeAPI.CustomElements;

public class AutoElement
{
    public Roles TargetRoles { get; }
    public Element Element { get; }
    private readonly Tag _ref = new();
    
    public AutoElement(Roles targetRoles, Element element)
    {
        TargetRoles = targetRoles;
        Element = element;
        
        PlayerEvents.ChangedRole += OnRoleChanged;
    }

    public void Disable()
    {
        Player.ReadyList.ForEach(p => RueDisplay.Get(p).Remove(_ref));
        PlayerEvents.ChangedRole -= OnRoleChanged;
    }

    private void OnRoleChanged(PlayerChangedRoleEventArgs args)
    {
        var display = RueDisplay.Get(args.Player);
        if (TargetRoles.HasFlagFast(args.NewRole.RoleTypeId))
        {
            display.Show(_ref, Element);
        }
        else
        {
            display.Remove(_ref);
        }
    }
}