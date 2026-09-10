// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace XazeAPI.Features.AoEs;

public class DelegateAerial : AerialEffect
{
    private readonly Action<Player> _delegate;

    public override bool OnStay(Player player)
    {
        base.OnStay(player);
        _delegate?.Invoke(player);
        return true;
    }

    public DelegateAerial(Action<Player> @delegate, Vector3 sourcePos) : base(sourcePos)
    {
        _delegate = @delegate;
    }
}