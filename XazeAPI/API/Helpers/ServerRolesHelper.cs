using CentralAuth;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XazeAPI.API.Stats;
using static ServerRoles;

namespace XazeAPI.API.Helpers
{
    public static class ServerRolesHelper
    {
        public static void SetGroupWithNoMessages(this ServerRoles serverRoles, UserGroup group, bool disp = false)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void ServerRoles::SetGroup(UserGroup,System.Boolean,System.Boolean)' called when server was not active");
                return;
            }

            if (group == null)
            {
                serverRoles.RemoteAdmin = serverRoles.GlobalPerms != 0;
                serverRoles.Permissions = serverRoles.GlobalPerms;
                serverRoles.GlobalHidden = false;
                serverRoles.Group = null;
                serverRoles.SetColor(null);
                serverRoles.SetText(null);
                serverRoles.BadgeCover = false;
                serverRoles.RpcResetFixed();
                serverRoles.FinalizeSetGroupWithNoMessages();
                return;
            }

            serverRoles.Group = group;
            serverRoles.BadgeCover = group.Cover;
            if ((group.Permissions | serverRoles.GlobalPerms) != 0 && ServerStatic.PermissionsHandler.IsRaPermitted(group.Permissions | serverRoles.GlobalPerms))
            {
                serverRoles.Permissions = group.Permissions | serverRoles.GlobalPerms;
            }
            else
            {
                serverRoles.Permissions = group.Permissions | serverRoles.GlobalPerms;
            }

            ServerLogs.AddLog(ServerLogs.Modules.Permissions, serverRoles._hub.LoggedNameFromRefHub() + " has been assigned to group " + group.BadgeText + ".", ServerLogs.ServerLogType.ConnectionUpdate);
            if (group.BadgeColor == "none")
            {
                serverRoles.RpcResetFixed();
                serverRoles.FinalizeSetGroupWithNoMessages();
                return;
            }

            if ((serverRoles._localBadgeVisibilityPreferences == BadgeVisibilityPreferences.Hidden && !disp) || (group.HiddenByDefault && !disp && serverRoles._localBadgeVisibilityPreferences != BadgeVisibilityPreferences.Visible))
            {
                serverRoles.BadgeCover = serverRoles.UserBadgePreferences == BadgePreferences.PreferLocal;
                if (!string.IsNullOrEmpty(serverRoles.MyText))
                {
                    return;
                }

                serverRoles.SetText(null);
                serverRoles.SetColor("default");
                serverRoles.GlobalHidden = false;
                serverRoles.HiddenBadge = group.BadgeText;
                serverRoles.RefreshHiddenTag();
                serverRoles.TargetSetHiddenRole(serverRoles.connectionToClient, group.BadgeText);
            }
            else
            {
                serverRoles.HiddenBadge = null;
                serverRoles.GlobalHidden = false;
                serverRoles.RpcResetFixed();
                serverRoles.SetText(group.BadgeText);
                serverRoles.SetColor(group.BadgeColor);
            }

            serverRoles.FinalizeSetGroupWithNoMessages();
        }

        public static void FinalizeSetGroupWithNoMessages(this ServerRoles serverRole)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void ServerRoles::FinalizeSetGroup()' called when server was not active");
                return;
            }

            serverRole.Permissions |= serverRole.GlobalPerms;
            serverRole.RemoteAdmin = ServerStatic.PermissionsHandler.IsRaPermitted(serverRole.Permissions | serverRole.GlobalPerms);
            serverRole.AdminChatPerms = PermissionsHandler.IsPermitted(serverRole.Permissions, PlayerPermissions.AdminChat);
            serverRole._hub.queryProcessor.GameplayData = PermissionsHandler.IsPermitted(serverRole.Permissions, PlayerPermissions.GameplayData);
            if (serverRole.RemoteAdmin)
            {
                serverRole.OpenRemoteAdmin();
            }
            else
            {
                serverRole.TargetSetRemoteAdmin(open: false);
            }

            serverRole.SendRealIds();
            bool flag = PermissionsHandler.IsPermitted(serverRole.Permissions, PlayerPermissions.ViewHiddenBadges);
            bool flag2 = PermissionsHandler.IsPermitted(serverRole.Permissions, PlayerPermissions.ViewHiddenGlobalBadges);
            if (!(flag || flag2))
            {
                return;
            }

            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.Mode != ClientInstanceMode.DedicatedServer)
                {
                    ServerRoles serverRoles = allHub.serverRoles;
                    if (!string.IsNullOrEmpty(serverRoles.HiddenBadge) && (!serverRoles.GlobalHidden || flag2) && (serverRoles.GlobalHidden || flag))
                    {
                        serverRoles.TargetSetHiddenRole(serverRole.connectionToClient, serverRoles.HiddenBadge);
                    }
                }
            }
        }

        public static void SendAdminChatMessage(string message, string broadcast, IEnumerable<Player> targets)
        {
            string content = "0!" + message;
            foreach (var target in targets)
            {
                if (!target.ReferenceHub.serverRoles.AdminChatPerms || target.ReferenceHub.Mode != ClientInstanceMode.ReadyClient)
                {
                    continue;
                }

                if (broadcast != null)
                {
                    target.SendBroadcast(broadcast, 10, Broadcast.BroadcastFlags.AdminChat);
                }

                if (target.ReferenceHub.encryptedChannelManager.TrySendMessageToClient(content, EncryptedChannelManager.EncryptedChannel.AdminChat))
                {
                    continue;
                }

                PluginStatistics.ExceptionCaught(false);
                Logging.Error("Couldn't send " + target.Nickname + " a Admin Chat Message");
            }
        }

        public static void SendAdminChatMessage(string message) => SendAdminChatMessage(message, null, Player.ReadyList.Where(x => x.HasPermissions("xaze.customitem")));
        public static void SendAdminChatMessage(string message, string broadcast) => SendAdminChatMessage(message, broadcast, Player.ReadyList.Where(x => x.HasPermissions("xaze.customitem")));
    }
}
