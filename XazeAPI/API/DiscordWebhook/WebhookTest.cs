// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System.Linq;
using System.Threading.Tasks;
using InventorySystem.Items.Firearms;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using XazeAPI.API.DiscordWebhook.Data;
using XazeAPI.API.Helpers;
using XazeAPI.Features;

namespace XazeAPI.API.DiscordWebhook
{
    using System.Net.Http;
    using System;
    
    public class WebhookTest
    {
        public static HttpClient Client
        {
            get
            {
                field ??= new HttpClient();
                return field;
            }
        }

        public static string DiscordLog { get; private set; }
        public static string DiscordPath { get; private set; }
        public static bool isInitialized { get; private set; }

        public const int colorBlue = 0x1F61E6;
        public const int colorGreen = 0x80E61F;
        public const int colorRed = 0xE7421F;
        public const int colorPurple = 0xC61FE6;
        public const int colorYellow = 0xE6C71F;

        public const string AvatarUrl = "https://i.imgur.com/u5WGSbz.jpeg";
        
        public static void Initialize()
        {
            isInitialized = true;
        }

        public static void SendMessage(StringContent content, string webhookUrl)
        {
            if (!isInitialized || string.IsNullOrWhiteSpace(webhookUrl))
                return;
            
            if (!webhookUrl.Contains("with_components=true"))
                webhookUrl += webhookUrl.Contains("?") ? "&with_components=true" : "?with_components=true";

            if (!Uri.TryCreate(webhookUrl, UriKind.Absolute, out var uri))
                return;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Client.PostAsync(uri, content);
                }
                catch (Exception ex)
                {
                    Logging.Error("[Discord-Webhook]", ex);
                }
            });
        }

        public static void JoinLog(string webhoolUrl, Player player, string PluginName = "Plugin")
        {
            if (player == null)
                return;

            var builder = new ComponentBuilderV2()
                .WithUsername(PluginName + "-JoinLog")
                .WithAvatarUrl(AvatarUrl)
                .WithContainer(container => container
                    .WithAccentColor(colorGreen)
                    .WithTextDisplay($"### 📥 {player.DisplayName} joined the Server")
                    .WithSeparator(true, 1)
                    .WithTextDisplay(
                        $"**Round In Progress:** `{RoundSummary.RoundInProgress()}`\n" +
                        $"**Role:** {player.Role}\n" +
                        $"**Group:** {player.GroupName}\n" +
                        $"**User ID:** `{player.UserId}`"
                        ));

            SendMessage(builder.BuildHttpContent(), webhoolUrl);
        }
        
        public static void LeaveLog(string webhookUrl, Player player, string PluginName = "Plugin")
        {
            if (player == null) return;

            var builder = new ComponentBuilderV2()
                .WithUsername($"{PluginName}-LeaveLog")
                .WithAvatarUrl(AvatarUrl)
                .WithContainer(container => container
                    .WithAccentColor(colorRed)
                    .WithTextDisplay($"### 📤 {player.DisplayName} left the Server")
                    .WithSeparator(divider: true, spacing: 1)
                    .WithTextDisplay(
                        $"**Round In Progress:** `{RoundSummary.RoundInProgress()}`\n" +
                        $"**Role:** {player.Role}\n" +
                        $"**Group:** {player.GroupName}\n" +
                        $"**User ID:** `{player.UserId}`"
                    ));

            SendMessage(builder.BuildHttpContent(), webhookUrl);
        }

        public static void DeathLog(string webhookUrl, Player Attacker, Player Target, bool isSuicide, DamageHandlerBase damageHandler, string PluginName = "Plugin")
        {
            if (Target == null) return;

            var builder = new ComponentBuilderV2()
                .WithUsername($"{PluginName}-DeathLog")
                .WithAvatarUrl(AvatarUrl);

            builder.WithContainer(container =>
            {
                container.WithAccentColor(colorRed);

                container.WithTextDisplay(isSuicide
                    ? $"### 💀 {Target.DisplayName} died"
                    : $"### ⚔️ {Target.DisplayName} died to {Attacker?.DisplayName ?? "Unknown"}");

                container.WithSeparator(divider: true, spacing: 1);
                container.WithTextDisplay(
                    $"**Death Reason:** {damageHandler.ServerLogsText}\n" +
                    $"**Damage Type:** {damageHandler.getDamageType()}\n" +
                    $"**Damage:** `{damageHandler.getDamage()}`"
                );

                // Target Info Block
                bool hasDisguise = false;
                RoleTypeId disguise = RoleTypeId.None;
                if (XazePlayer.TryGet(Target, out var targetPlr) && targetPlr.IsDisguised)
                {
                    hasDisguise = true;
                    disguise = targetPlr.Disguise;
                }

                string targetInfo =
                    $"__**Target Info**__\n" +
                    $"**Username:** {Target.Nickname}\n" +
                    $"**User ID:** `{Target.UserId}`\n" +
                    $"**Group:** {Target.GroupName}\n" +
                    $"**Role:** {Target.Role}\n" +
                    $"**Custom Info:** {Target.CustomInfo}";

                if (!isSuicide)
                {
                    targetInfo += $"\n**Was Cuffed:** {Target.IsDisarmed} {(Target.IsDisarmed ? $"(by {Target.DisarmedBy?.Nickname})" : "")}";
                    targetInfo += $"\n**Is Armed:** {Target.Items.Any(x => x.Base is Firearm)}";
                }

                if (hasDisguise)
                {
                    targetInfo += $"\n**Disguise:** {disguise}";
                }

                container.WithTextDisplay(targetInfo);

                // Attacker Info Block (If not suicide)
                if (isSuicide || Attacker == null) 
                    return;
                
                bool hasDisguise2 = false;
                RoleTypeId disguise2 = RoleTypeId.None;
                if (XazePlayer.TryGet(Attacker, out var attPlr) && attPlr.IsDisguised)
                {
                    hasDisguise2 = true;
                    disguise2 = attPlr.Disguise;
                }

                string attackerInfo =
                    $"__**Attacker Info**__\n" +
                    $"**Username:** {Attacker.Nickname}\n" +
                    $"**User ID:** `{Attacker.UserId}`\n" +
                    $"**Group:** {Attacker.GroupName}\n" +
                    $"**Role:** {Attacker.Role}\n" +
                    $"**Custom Info:** {Attacker.CustomInfo}" +
                    $"{(hasDisguise2 ? $"\n**Disguise:** {disguise2}" : "")}";

                container.WithTextDisplay(attackerInfo);
            });

            SendMessage(builder.BuildHttpContent(), webhookUrl);
        }

        // BanLog Overload 1 (Player Issuer, Player Target)
        public static void BanLog(string webhookUrl, Player Issuer, Player Target, string reason, long duration, bool updated = false, string PluginName = "Plugin")
        {
            string issuerStr = Issuer != null ? $"{Issuer.Nickname}({Issuer.UserId})" : "Server/Console";
            string targetIp = Target != null ? Target.IpAddress : "N/A";

            BuildAndSendBanLog(webhookUrl, PluginName, updated, Target?.Nickname, Target?.UserId, issuerStr, targetIp, reason, duration);
        }

        // BanLog Overload 2 (String Issuer, Player Target)
        public static void BanLog(string webhookUrl, string Issuer, Player Target, string reason, long duration, bool updated = false, string PluginName = "Plugin")
        {
            string targetIp = Target != null ? Target.IpAddress : "N/A";

            BuildAndSendBanLog(webhookUrl, PluginName, updated, Target?.Nickname, Target?.UserId, Issuer, targetIp, reason, duration);
        }

        // BanLog Overload 3 (ReferenceHub Issuer, ReferenceHub Target)
        public static void BanLog(string webhookUrl, ReferenceHub Issuer, ReferenceHub Target, string reason, long duration, bool updated = false, string PluginName = "Plugin")
        {
            string targetNick = Target?.nicknameSync?.MyNick;
            string targetUserId = Target?.authManager?.UserId;
            string targetIp = Target?.connectionToClient?.address;

            string issuerNick = Issuer?.nicknameSync?.MyNick;
            string issuerUserId = Issuer?.authManager?.UserId;
            string issuerStr = Issuer != null ? $"{issuerNick}({issuerUserId})" : "Server/Console";

            BuildAndSendBanLog(webhookUrl, PluginName, updated, targetNick, targetUserId, issuerStr, targetIp, reason, duration);
        }

        // Private helper for consolidated BanLog generation
        private static void BuildAndSendBanLog(string webhookUrl, string pluginName, bool updated, string targetNick, string targetUserId, string issuerStr, string targetIp, string reason, long duration)
        {
            TimeSpan timespan = TimeSpan.FromSeconds(duration);
            int years = timespan.Days / 365;
            int days = timespan.Days - (years * 365);
            string readableTimespan = $"{years} Years {days} Days, {timespan.Hours} Hours, {timespan.Minutes} Minutes, {timespan.Seconds} Seconds";

            var builder = new ComponentBuilderV2()
                .WithUsername($"{pluginName}-BanLog")
                .WithAvatarUrl(AvatarUrl)
                .WithContainer(container =>
                {
                    container.WithAccentColor(colorRed);

                    string title = updated ? $"🔨 Player {targetNick}'s ban was updated" : $"🔨 Player {targetNick}({targetUserId}) was banned";
                    container.WithTextDisplay($"### {title}");

                    container.WithSeparator(divider: true, spacing: 1);

                    string desc = updated
                        ? $"**Ban updated by:** {issuerStr}"
                        : $"**Ban issued by:** {issuerStr}\n**Target IP:** || {targetIp} || *(unhide to unban)*";

                    container.WithTextDisplay(desc);

                    string durationLabel = updated ? "New Duration" : "Duration";
                    container.WithTextDisplay(
                        $"**Reason:** {reason}\n" +
                        $"**{durationLabel}:** `{readableTimespan}`"
                    );
                });

            SendMessage(builder.BuildHttpContent(), webhookUrl);
        }
    }
}
