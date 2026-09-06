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

public class ContainerBuilder
{
    private int? _accentColor;
    private readonly List<ComponentV2> _components = new();
    
    public ContainerBuilder WithAccentColor(int color)
    {
        _accentColor = color;
        return this;
    }
    
    public ContainerBuilder WithAccentColor(System.Drawing.Color color)
    {
        _accentColor = color.ToArgb() & 0xFFFFFF;
        return this;
    }

    public ContainerBuilder WithTextDisplay(string content)
    {
        _components.Add(new TextDisplay(content));
        return this;
    }

    public ContainerBuilder WithTextDisplay(Action<StringBuilder> contentAction)
    {
        var sb = StringBuilderPool.Shared.Rent();
        contentAction.InvokeSafely(sb, (ex) =>
        {
            sb.Clear()
                .AppendLine(" ⚠️ | **Text Generation Exception** | ⚠️")
                .AppendLine(ex.Message)
                .AppendLine("**Stack Trace**:")
                .AppendLine(ex.StackTrace);
        });
        _components.Add(new TextDisplay(StringBuilderPool.Shared.ToStringReturn(sb)));
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