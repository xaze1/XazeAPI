// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

namespace XazeAPI.API.DiscordWebhook.Classes
{
    public class AuthorBuilder
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public object Build()
        {
            return new
            {
                name= Name,
                ulr= Url,
            };
        }
    }
}
