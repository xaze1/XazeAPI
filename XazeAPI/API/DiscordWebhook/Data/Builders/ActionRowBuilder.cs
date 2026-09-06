// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;

namespace XazeAPI.API.DiscordWebhook.Data.Builders;

public class ActionRowBuilder
{
    private readonly List<LinkButton> _buttons = new List<LinkButton>();

    public ActionRowBuilder WithButton(string label, string url)
    {
        if (_buttons.Count >= 5)
            throw new InvalidOperationException("An ActionRow cannot exceed 5 buttons.");

        _buttons.Add(new LinkButton(label, url));
        return this;
    }

    internal ActionRow Build() => new ActionRow { Components = _buttons };
}