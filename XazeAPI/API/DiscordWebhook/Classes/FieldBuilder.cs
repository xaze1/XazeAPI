// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

namespace XazeAPI.API.DiscordWebhook.Classes
{
    public class FieldBuilder
    {
        public FieldBuilder()
        {
            Name = "";
            Value = "";
            Inline = false;
        }
        
        public FieldBuilder(string name, string value = "", bool inline = false)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public bool Inline { get; set; }

        public object Build()
        {
            return new
            {
                name= Name,
                value= Value,
                inline= Inline,
            };
        }
    }
}
