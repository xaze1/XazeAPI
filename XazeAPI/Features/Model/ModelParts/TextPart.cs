// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.Collections.Generic;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace XazeAPI.Features.Model.ModelParts;

public class TextPart : ModelPart
{
    /// <summary>
    /// Isn't set on creation or during builder action
    /// </summary>
    public new TextToy Part => base.Part as TextToy;

    public string TextFormat
    {
        get;
        set
        {
            field = value;
            Part?.TextFormat = value;
        }
    }

    public Vector2 DisplaySize
    {
        get;
        set
        {
            field = value;
            Part?.DisplaySize = value;
        }
    }

    public IReadOnlyList<string> Arguments => _arguments.AsReadOnly();
    private List<string> _arguments { get; } = new();

    public TextPart WithArguments(params string[] args)
    {
        _arguments.AddRange(args);
        Part?.Arguments.AddRange(args);
        return this;
    }

    public override void Spawn()
    {
        base.Part = TextToy.Create(Position, Rotation, Scale, Parent, false);
        Part.TextFormat = TextFormat;
        Part.Arguments.AddRange(Arguments);
        Part.DisplaySize = DisplaySize;
        Part.Spawn();
    }
}