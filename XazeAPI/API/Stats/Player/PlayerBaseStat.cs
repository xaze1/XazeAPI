// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

namespace XazeAPI.API.Stats.Player
{
    public abstract class PlayerBaseStat
    {
        public ReferenceHub Hub { get; set; } 

        public PlayerBaseStat()
        {
            Hub = null;
        }
        
        public PlayerBaseStat(ReferenceHub hub)
        {
            Hub = hub;
        }
    }
}
