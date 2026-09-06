// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using XazeAPI.API.DiscordWebhook.Data.Components;

namespace XazeAPI.API.DiscordWebhook.Data;

public class SectionBuilder
{
    private readonly List<TextDisplay> _textDisplays = new List<TextDisplay>();
    private ISectionAccessory _accessory;

    public SectionBuilder WithText(string content)
    {
        if (_textDisplays.Count >= 3)
            throw new InvalidOperationException("A Section cannot exceed 3 TextDisplay items.");

        _textDisplays.Add(new TextDisplay(content));
        return this;
    }

    public SectionBuilder WithThumbnailAccessory(string url)
    {
        _accessory = new Thumbnail(url);
        return this;
    }

    public SectionBuilder WithButtonAccessory(string label, string url)
    {
        _accessory = new LinkButton(label, url);
        return this;
    }

    internal Section Build()
    {
        return new Section
        {
            Components = _textDisplays.AsReadOnly(),
            Accessory = _accessory
        };
    }
}