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

public class ContainerBuilder
{
    private int? _accentColor;
    private readonly List<ComponentV2> _components = new();
    
    public ContainerBuilder WithAccentColor(int color)
    {
        _accentColor = color;
        return this;
    }

    public ContainerBuilder WithTextDisplay(string content)
    {
        _components.Add(new TextDisplay(content));
        return this;
    }

    public ContainerBuilder WithSeparator(bool divider = true, int? spacing = null)
    {
        _components.Add(new Separator { Divider = divider, Spacing = spacing });
        return this;
    }

    public ContainerBuilder WithSection(Action<SectionBuilder> sectionAction)
    {
        var builder = new SectionBuilder();
        sectionAction(builder);
        _components.Add(builder.Build());
        return this;
    }

    public ContainerBuilder WithActionRow(Action<ActionRowBuilder> rowAction)
    {
        var builder = new ActionRowBuilder();
        rowAction(builder);
        _components.Add(builder.Build());
        return this;
    }

    internal Container Build()
    {
        return new Container
        {
            AccentColor = _accentColor,
            Components = _components
        };
    }
}