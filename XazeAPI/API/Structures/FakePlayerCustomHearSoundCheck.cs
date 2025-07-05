// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using MapGeneration;
using PlayerRoles;
using PlayerRoles.Spectating;
using LabApi.Features.Wrappers;
using RueI.Displays;
using UnityEngine;
using VoiceChat;
using XazeAPI.API.AudioCore.FakePlayers;

namespace XazeAPI.API.Structures
{
    public struct FakePlayerCustomHearSoundCheck
    {
        public bool IsSet {  get; private set; }
        private CustomAudioPlayer _player;
        private Player _ApiPlayer => Player.Get(_player.Owner);
        public VoiceChatChannel VoicechatOverride { get; private set; }

        public FacilityZone Zones { get; private set; }
        public float MaxDistance { get; private set; }

        public Roles PermittedRoles { get; private set; }
        public Team PermittedTeams { get; private set; }
        public RoomName Rooms { get; private set; }

        public FakePlayerCustomHearSoundCheck()
        {
            IsSet = false;
        }

        public FakePlayerCustomHearSoundCheck(CustomAudioPlayer player, FacilityZone zones = FacilityZone.None, float maxDistance = 30, Roles roles = Roles.None, Team teams = Team.OtherAlive, VoiceChatChannel vc = VoiceChatChannel.None)
        {
            Zones = zones;
            MaxDistance = maxDistance;
            PermittedRoles = roles;
            PermittedTeams = teams;
            VoicechatOverride = vc;
            IsSet = true;
            _player = player;
        }

        public void OverrideVc(VoiceChatChannel channel)
        {
            VoicechatOverride = channel;
        }

        public void OverrideDistance(float distance)
        {
            MaxDistance = distance;
        }

        public void OverrideZones(FacilityZone zones)
        {
            Zones = zones;
        }

        public void OverrideRoles(Roles roles)
        {
            PermittedRoles = roles;
        }

        public void OverrideTeams(Team teams)
        {
            PermittedTeams = teams;
        }
        
        public void OverrideRooms(RoomName rooms)
        {
            Rooms = rooms;
        }

        public bool PlayerCanHear(ReferenceHub hub)
        {
            // Override isn't setup
            if (!IsSet)
            {
                return true;
            }

            Player User = Player.Get(hub);
            // If the user isn't a real user
            if (User == null)
            {
                return false;
            }

            // If player is dead/spectating
            if (User.RoleBase is SpectatorRole spectator)
            {
                // Player isn't spectating anyone
                if (spectator.SyncedSpectatedNetId == 0u)
                {
                    return false;
                }
                // Player is spectating someone
                else
                {
                    // Check for the user spectated instead of the User spectating
                    User = Player.Get(spectator.SyncedSpectatedNetId);
                }
            }
            
            // If player is inside a valid rooms Boundaries
            if (User.Room != null)
            {
                bool roomSet = Rooms != RoomName.Unnamed;
                bool zoneSet = Zones != FacilityZone.None;

                if (zoneSet)
                {
                    // If user isn't in that zone and isn't inside one of the rooms, if rooms are given
                    if (!Zones.HasFlag(User.Zone) && (!roomSet || !Rooms.HasFlag(User.Room.Name)))
                    {
                        return false;
                    }
                }
                // If only Rooms are given and player isn't inside of the rooms
                else if (roomSet && !Rooms.HasFlag(User.Room.Name))
                {
                    return false;
                }
            }

            // If player has a Role
            if (User.RoleBase is not null)
            {
                bool teamsSet = PermittedTeams != Team.OtherAlive;
                bool rolesSet = PermittedRoles != Roles.None;
                // If the teams are set
                if (teamsSet)
                {
                    // If user isn't in that Team and isn't one of the roles, if roles are given
                    if (!PermittedTeams.HasFlag(User.Team) && (!rolesSet || !PermittedRoles.HasFlag(User.Role)))
                    {
                        return false;
                    }
                }
                // If only the roles are set and player isn't one of the roles
                else if (rolesSet && !PermittedRoles.HasFlag(User.Role))
                {
                    return false;
                }
            }

            // If player is further away from the Audio Source than the max distance
            if (Vector3.Distance(_ApiPlayer.Position, User.Position) > MaxDistance)
            {
                return false;
            }

            return true;
        }
    }
}
