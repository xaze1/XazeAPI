// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using XazeAPI.API.Extensions;

namespace XazeAPI.API.DiscordWebhook.Data.Builders;

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
    
    public SectionBuilder WithText(Action<StringBuilder> contentAction)
    {
        if (_textDisplays.Count >= 3)
            throw new InvalidOperationException("A Section cannot exceed 3 TextDisplay items.");

        var sb = StringBuilderPool.Shared.Rent();
        contentAction.InvokeSafely(sb, (ex) =>
        {
            sb.Clear()
                .AppendLine(" ⚠️ | **Text Generation Exception** | ⚠️")
                .AppendLine(ex.Message)
                .AppendLine("**Stack Trace**:")
                .AppendLine(ex.StackTrace);
        });
        _textDisplays.Add(new TextDisplay(StringBuilderPool.Shared.ToStringReturn(sb)));
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