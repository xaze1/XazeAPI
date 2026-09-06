// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using XazeAPI.API.Extensions;

namespace XazeAPI.API.Stats
{
    public abstract class PlayerBaseStat
    {
        internal static readonly List<PlayerBaseStat> List = new();
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
            List.Add(this);
        }

        internal void Reset()
        {
            Value = 0;
        }

        public static void Clear()
        {
            List.Do(s => s.Reset());
        }
    }
}
