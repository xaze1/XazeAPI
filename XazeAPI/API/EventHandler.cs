// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using PlayerRoles;
using XazeAPI.API.Stats;
using XazeAPI.Features;

namespace XazeAPI.API;

public class EventHandler : CustomEventsHandler
{
    public override void OnPlayerDeath(PlayerDeathEventArgs ev)
    {
        base.OnPlayerDeath(ev);
        if (ev.Attacker != null && XazePlayer.TryGet(ev.Attacker, out var xAlr))
        {
            xAlr.Kills += 1;
        }
        
        if (XazePlayer.TryGet(ev.Player, out var xPlr))
        {
            xPlr.Deaths += 1;
        }
    }

    public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
    {
        base.OnPlayerChangedRole(ev);
        if (XazePlayer.TryGet(ev.Player, out var xPlr) && ev.OldRole.GetTeam() != Team.Dead)
        {
            xPlr.LastRole = ev.OldRole;
        }
    }

    public override void OnServerRoundRestarted()
    {
        base.OnServerRoundRestarted();
        PlayerBaseStat.Clear();
    }

    public override void OnPlayerLeft(PlayerLeftEventArgs ev)
    {
        base.OnPlayerLeft(ev);
        if (!XazePlayer.Dictionary.TryGetValue(ev.Player.UserId, out var xPlr))
            return;
        XazePlayer.List.Remove(xPlr);
        XazePlayer.Dictionary.Remove(ev.Player.UserId);
    }
}