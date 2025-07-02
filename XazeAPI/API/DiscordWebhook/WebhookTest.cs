using System.Linq;
using InventorySystem.Items.Firearms;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using XazeAPI.API.DiscordWebhook.Classes;
using XazeAPI.API.Helpers;

namespace XazeAPI.API.DiscordWebhook
{
    using System.Net.Http;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Newtonsoft.Json;
    
    public class WebhookTest
    {
        private static HttpClient client;
        public static HttpClient Client
        {
            get
            {
                if (client == null)
                    client = new HttpClient();

                return client;
            }
        }

        public static string DiscordLog { get; private set; }
        public static string DiscordPath { get; private set; }
        public static bool isInitialized { get; private set; }

        public const string colorBlue = "1F61E6";
        public const string colorGreen = "80E61F";
        public const string colorRed = "E7421F";
        public const string colorPurple = "C61FE6";
        public const string colorYellow = "E6C71F";

        public const string AvatarUrl = "https://i.imgur.com/u5WGSbz.jpeg";
        public static void Main(string webhoolUrl)
        {
            var test = new
            {
                username = "Test",
                content = "Gay Nerd",
                avatar_url = AvatarUrl,
            };
            var message = JsonConvert.SerializeObject(test);


            Thread messageThread = new Thread(() => Client.PostAsync(webhoolUrl, new StringContent(message, Encoding.UTF8, "application/json")).Wait());
            messageThread.Start();
        }

        public static void JoinLog(string webhoolUrl, Player player, string PluginName = "Plugin")
        {

            var SuccessWebHook = new
            {
                username = PluginName + "-JoinLog",
                content = "",
                avatar_url = AvatarUrl,
                embeds = new List<object>
                {
                    new
                    {
                        title = player.DisplayName + " joined the Server\n",
                        url = "",
                        description = "RoundInProgress: " + RoundSummary.RoundInProgress() + "\n" +
                        "Role: " + player.Role + "\n" +
                        "Group: " + player.GroupName + "\n" +
                        "UserId: " + player.UserId,
                        color = int.Parse(colorGreen, System.Globalization.NumberStyles.HexNumber),
                        timestamp = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")
                    }
                }
            };

            SendMessage(new StringContent(JsonConvert.SerializeObject(SuccessWebHook), Encoding.UTF8, "application/json"), webhoolUrl);
        }
        
        public static void LeaveLog(string webhoolUrl, Player player, string PluginName = "Plugin")
        {
            if (player == null)
            {
                return;
            }

            var SuccessWebHook = new
            {
                username = PluginName + "-LeaveLog",
                content = "",
                avatar_url = AvatarUrl,
                embeds = new List<object>
                {
                    new
                    {
                        title = player.DisplayName + " left the Server\n",
                        url="",
                        description="RoundInProgress: " + RoundSummary.RoundInProgress() + "\n" +
                        "Role: " + player.Role + "\n" +
                        "Group: " + player.GroupName + "\n" +
                        "UserId: " + player.UserId,
                        color= int.Parse(colorRed, System.Globalization.NumberStyles.HexNumber),
                        timestamp = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")
                    }
                }
            };

            SendMessage(new StringContent(JsonConvert.SerializeObject(SuccessWebHook), Encoding.UTF8, "application/json"), webhoolUrl);
        }
        
        public static void DeathLog(string webhoolUrl, Player Attacker, Player Target, bool isSuicide, DamageHandlerBase damageHandler, string PluginName = "Plugin")
        {
            MessageBuilder msgBuilder = new MessageBuilder(PluginName + "-DeathLog");

            if (isSuicide)
            {
                EmbedBuilder suicideEmbed = new EmbedBuilder()
                {
                    Title = Target.DisplayName + " died",
                    TitleUrl = "",
                    Description = "Death Reason: " + damageHandler.ServerLogsText + $"\n" +
                    $"Damage Type: {MainHelper.getDamageType(damageHandler)}\n" +
                    $"Damage: " + damageHandler.getDamage(),
                    Color = System.Drawing.Color.Red,
                    Timestamp = DateTimeOffset.Now,
                    Fields = new()
                };

                bool hasDisguise = false;
                RoleTypeId disguise = RoleTypeId.None;
                if (CustomPlayer.TryGet(Target, out CustomPlayer targetPlr) && targetPlr.Disguise != RoleTypeId.None)
                {
                    hasDisguise = true;
                    disguise = targetPlr.Disguise;
                }

                suicideEmbed.Fields.Add(new FieldBuilder()
                {
                    Name = "__Target__",
                    Value = $"Username: {Target.Nickname}\n" +
                    $"UserId: {Target.UserId}\n" +
                    $"Group: {Target.GroupName}\n" +
                    $"Role: {Target.Role}\n" +
                    $"CustomInfo: {Target.CustomInfo}\n" +
                    $"{(hasDisguise? "\nDisguise: " + disguise : "")}",
                    Inline = true,
                });

                msgBuilder.Embeds.Add(suicideEmbed);
            }
            else
            {
                EmbedBuilder killedEmbed = new EmbedBuilder()
                {
                    Title = Target.DisplayName + " died to " + Attacker.DisplayName,
                    TitleUrl = "",
                    Description = "Death Reason: " + damageHandler.ServerLogsText + $"\n" +
                    $"Damage Type: {MainHelper.getDamageType(damageHandler)}",
                    Color = System.Drawing.Color.Red,
                    Timestamp = DateTimeOffset.Now,
                    Fields = new()
                };

                bool hasDisguise = false;
                RoleTypeId disguise = RoleTypeId.None;
                if (CustomPlayer.TryGet(Target, out CustomPlayer targetPlr) && targetPlr.Disguise != RoleTypeId.None)
                {
                    hasDisguise = true;
                    disguise = targetPlr.Disguise;
                }

                killedEmbed.Fields.Add(new FieldBuilder()
                {
                    Name = "__Target__",
                    Value = $"Username: {Target.Nickname}\n" +
                    $"UserId: {Target.UserId}\n" +
                    $"Group: {Target.GroupName}\n" +
                    $"Role: {Target.Role}\n" +
                    $"CustomInfo: {Target.CustomInfo}\n" +
                    $"WasCuffed: {Target.IsDisarmed} {(Target.IsDisarmed? $"(by {Target.DisarmedBy.Nickname})" : "")}\n" +
                    $"IsArmed: {Target.Items.Any(x => x.Base is Firearm)}"+
                    $"{(hasDisguise ? "\nDisguise: " + disguise : "")}",
                    Inline = true,
                });

                bool hasDisguise2 = false;
                RoleTypeId disguise2 = RoleTypeId.None;
                if (CustomPlayer.TryGet(Target, out CustomPlayer attPlr) && attPlr.Disguise != RoleTypeId.None)
                {
                    hasDisguise2 = true;
                    disguise = attPlr.Disguise;
                }

                killedEmbed.Fields.Add(new FieldBuilder()
                {
                    Name = "__Attacker__",
                    Value = $"Username: {Attacker.Nickname}\n" +
                    $"UserId: {Attacker.UserId}\n" +
                    $"Group: {Attacker.GroupName}\n" +
                    $"Role: {Attacker.Role}\n" +
                    $"CustomInfo: {Attacker.CustomInfo}" +
                    $"{(hasDisguise2 ? "\nDisguise: " + disguise2 : "")}",
                    Inline = true,
                });

                msgBuilder.Embeds.Add(killedEmbed);
            }

            SendMessage(msgBuilder.Build(), webhoolUrl);
        }

        public static void SendMessage(StringContent content, string webhookUrl)
        {
            if (!isInitialized) return;

            Thread messageThread = new Thread(() => Client.PostAsync(webhookUrl, content).Wait());
            messageThread.Start();
        }

        public static void Initialize()
        {
            // Removed the Discord Log file as it's no longer needed for Debug
            isInitialized = true;
        }

        public static void BanLog(string webhookUrl, Player Issuer, Player Target, string reason, long duration, bool updated = false, string PluginName = "Plugin")
        {
            MessageBuilder msgBuilder = new MessageBuilder(PluginName + "-BanLog");

            EmbedBuilder BanEmbed = new()
            {
                Title = updated? $"Player {Target.Nickname}'s ban was updated" : $"Player {Target.Nickname}({Target.UserId}) was banned",
                TitleUrl = "",
                Color = System.Drawing.Color.Red,
                Description = updated? $"Ban updated by {Issuer.Nickname}({Issuer.UserId})" : $"Ban issued by {Issuer.Nickname}({Issuer.UserId})\nTarget Ip: || {Target.IpAddress} || (Only unhide to unban)",
                Timestamp = DateTimeOffset.Now,
                Fields = new()
            };

            BanEmbed.Fields.Add(new FieldBuilder()
            {
                Name = "Reason",
                Value = reason,
                Inline = true,
            });

            TimeSpan timespan = TimeSpan.FromSeconds(duration);
            int years = timespan.Days / 365;
            int days = timespan.Days - (years * 365);

            string readableTimespan = String.Format("{0} Years {1} Days, {2} Hours, {3} Mintues, {4} Seconds", years, days, timespan.Hours, timespan.Minutes, timespan.Seconds);

            BanEmbed.Fields.Add(new FieldBuilder()
            {
                Name = updated? "New Duration" : "Duration",
                Value = $"{readableTimespan}",
                Inline = true,
            });

            msgBuilder.Embeds.Add(BanEmbed);

            SendMessage(msgBuilder.Build(), webhookUrl);
        }
        
        public static void BanLog(string webhookUrl, string Issuer, Player Target, string reason, long duration, bool updated = false, string PluginName = "Plugin")
        {
            MessageBuilder msgBuilder = new MessageBuilder(PluginName + "-BanLog");

            EmbedBuilder BanEmbed = new()
            {
                Title = updated? $"Player {Target.Nickname}'s ban was updated" : $"Player {Target.Nickname}({Target.UserId}) was banned",
                TitleUrl = "",
                Color = System.Drawing.Color.Red,
                Description = updated? $"Ban updated by {Issuer}" : $"Ban issued by {Issuer}\nTarget Ip: || {Target.IpAddress} || (Only unhide to unban)",
                Timestamp = DateTimeOffset.Now,
                Fields = new()
            };

            BanEmbed.Fields.Add(new FieldBuilder()
            {
                Name = "Reason",
                Value = reason,
                Inline = true,
            });

            TimeSpan timespan = TimeSpan.FromSeconds(duration);
            int years = timespan.Days / 365;
            int days = timespan.Days - (years * 365);

            string readableTimespan = String.Format("{0} Years {1} Days, {2} Hours, {3} Mintues, {4} Seconds", years, days, timespan.Hours, timespan.Minutes, timespan.Seconds);

            BanEmbed.Fields.Add(new FieldBuilder()
            {
                Name = updated? "New Duration" : "Duration",
                Value = $"{readableTimespan}",
                Inline = true,
            });

            msgBuilder.Embeds.Add(BanEmbed);

            SendMessage(msgBuilder.Build(), webhookUrl);
        }
        
        public static void BanLog(string webhookUrl, ReferenceHub Issuer, ReferenceHub Target, string reason, long duration, bool updated = false, string PluginName = "Plugin")
        {
            MessageBuilder msgBuilder = new MessageBuilder(PluginName + "-BanLog");

            EmbedBuilder BanEmbed = new()
            {
                Title = updated? $"Player {Target.nicknameSync.MyNick}'s ban was updated" : $"Player {Target.nicknameSync.MyNick}({Target.authManager.UserId}) was banned",
                TitleUrl = "",
                Color = System.Drawing.Color.Red,
                Description = updated? $"Ban updated by {Issuer.nicknameSync.MyNick}({Issuer.authManager.UserId})" : $"Ban issued by {Issuer.nicknameSync.MyNick}({Issuer.authManager.UserId})\nTarget Ip: || {Target.connectionToClient.address} || (Only unhide to unban)",
                Timestamp = DateTimeOffset.Now,
                Fields = new()
            };

            BanEmbed.Fields.Add(new FieldBuilder()
            {
                Name = "Reason",
                Value = reason,
                Inline = true,
            });

            TimeSpan timespan = TimeSpan.FromSeconds(duration);
            int years = timespan.Days / 365;
            int days = timespan.Days - (years * 365);

            string readableTimespan = String.Format("{0} Years {1} Days, {2} Hours, {3} Mintues, {4} Seconds", years, days, timespan.Hours, timespan.Minutes, timespan.Seconds);

            BanEmbed.Fields.Add(new FieldBuilder()
            {
                Name = updated? "New Duration" : "Duration",
                Value = $"{readableTimespan}",
                Inline = true,
            });

            msgBuilder.Embeds.Add(BanEmbed);

            SendMessage(msgBuilder.Build(), webhookUrl);
        }
    }
}
