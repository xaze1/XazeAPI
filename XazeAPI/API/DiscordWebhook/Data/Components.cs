// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.Collections.Generic;
using Newtonsoft.Json;
using XazeAPI.API.DiscordWebhook.Data.Components;

namespace XazeAPI.API.DiscordWebhook.Data;

public class Container : ComponentV2
{
    public override int Type => 17;

    [JsonProperty("accent_color", NullValueHandling = NullValueHandling.Ignore)]
    public int? AccentColor { get; set; }

    [JsonProperty("components")]
    public List<ComponentV2> Components { get; set; }
}

public class TextDisplay : ComponentV2
{
    public override int Type => 10;

    [JsonProperty("content")]
    public string Content { get; set; }

    public TextDisplay(string content) => Content = content;
}

public class Section : ComponentV2
{
    public override int Type => 9;

    [JsonProperty("components")]
    public IReadOnlyList<TextDisplay> Components { get; set; }

    [JsonProperty("accessory", NullValueHandling = NullValueHandling.Ignore)]
    public ISectionAccessory Accessory { get; set; }
}

public class Separator : ComponentV2
{
    public override int Type => 14;

    [JsonProperty("divider")]
    public bool Divider { get; set; } = true;

    [JsonProperty("spacing", NullValueHandling = NullValueHandling.Ignore)]
    public int? Spacing { get; set; }
}

public class Thumbnail : ComponentV2, ISectionAccessory
{
    public override int Type => 11;

    [JsonProperty("url")]
    public string Url { get; set; }

    public Thumbnail(string url) => Url = url;
}

public class LinkButton : ComponentV2, ISectionAccessory
{
    public override int Type => 2;

    [JsonProperty("style")]
    public int Style => 5; // Link Style

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    public LinkButton(string label, string url)
    {
        Label = label;
        Url = url;
    }
}

public class ActionRow : ComponentV2
{
    public override int Type => 1;

    [JsonProperty("components")]
    public List<LinkButton> Components { get; set; }
}