// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace XazeAPI.API.DiscordWebhook.Classes
{
    public class MessageBuilder
    {
        public MessageBuilder(string username)
        {
            Username = username;
            Message = "";
            AvatarUrl = WebhookTest.AvatarUrl;
            Embeds = new();
        }
        
        public MessageBuilder()
        {
            Username = "";
            Message = "";
            AvatarUrl = WebhookTest.AvatarUrl;
            Embeds = new();
        }

        public MessageBuilder(string username, string message = "", string avatarUrl = WebhookTest.AvatarUrl, List<EmbedBuilder> embeds = null)
        {
            Username = username;
            Message = message;
            AvatarUrl = avatarUrl;
            Embeds = embeds?? new();
        }

        public string Username { get; set; }

        public string Message { get; set; }

        public string AvatarUrl { get; set; }

        public List<EmbedBuilder> Embeds { get; set; }

        public StringContent Build()
        {
            List<object> embed = new();

            Embeds.ForEach(x => embed.Add(x.Build()));

            return new StringContent(JsonConvert.SerializeObject(new
            {
                username = Username,
                content = Message,
                avatar_url = AvatarUrl,
                embeds = embed
            }), Encoding.UTF8, "application/json");
        }
    }
}
