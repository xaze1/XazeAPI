// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using XazeAPI.API.DiscordWebhook.Data.Components;

namespace XazeAPI.API.DiscordWebhook.Data
{
    public class ComponentBuilderV2
    {
        private readonly List<ComponentV2> _components = new();
        
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        
        public ComponentBuilderV2 WithUsername(string username)
        {
            Username = username;
            return this;
        }

        public ComponentBuilderV2 WithAvatarUrl(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
            return this;
        }

        public ComponentBuilderV2 WithContainer(Action<ContainerBuilder> containerAction)
        {
            var builder = new ContainerBuilder();
            containerAction(builder);
            _components.Add(builder.Build());
            return this;
        }

        public ComponentBuilderV2 WithTextDisplay(string content)
        {
            _components.Add(new TextDisplay(content));
            return this;
        }

        public ComponentBuilderV2 WithSection(Action<SectionBuilder> sectionAction)
        {
            var builder = new SectionBuilder();
            sectionAction(builder);
            _components.Add(builder.Build());
            return this;
        }

        public ComponentBuilderV2 WithSeparator(bool divider = true, int? spacing = null)
        {
            _components.Add(new Separator { Divider = divider, Spacing = spacing });
            return this;
        }

        public ComponentBuilderV2 WithActionRow(Action<ActionRowBuilder> rowAction)
        {
            var builder = new ActionRowBuilder();
            rowAction(builder);
            _components.Add(builder.Build());
            return this;
        }

        public string BuildJson()
        {
            var payload = new Dictionary<string, object>
            {
                { "flags", 32768 }, // Mandatory IS_COMPONENTS_V2 flag
                { "components", _components }
            };

            if (!string.IsNullOrEmpty(Username)) payload["username"] = Username;
            if (!string.IsNullOrEmpty(AvatarUrl)) payload["avatar_url"] = AvatarUrl;

            return JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
        
        public StringContent BuildHttpContent()
        {
            return new StringContent(BuildJson(), Encoding.UTF8, "application/json");
        }
    }
}