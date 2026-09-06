// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using Newtonsoft.Json;

namespace XazeAPI.API.DiscordWebhook.Data;

public abstract class ComponentV2
{
    [JsonProperty("type")]
    public abstract int Type { get; }
}