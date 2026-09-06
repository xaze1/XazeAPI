// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using XazeAPI.API.Extensions;

namespace XazeAPI.API.Stats
{
    public abstract class PlayerBaseStat
    {
        public static event Action<PlayerBaseStat> OnValueChanged; 
        [CanBeNull] public Player Owner { get; private set; }

        public virtual int Value
        {
            get;
            set
            {
                field = value;
                OnValueChanged.InvokeSafely(this);
            }
        }

        protected bool IsSet { get; private set; }

        protected void Create(Player owner)
        {
            Owner = owner;
            IsSet = true;
        }
    }
}
