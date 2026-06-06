// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using LabApi.Features.Wrappers;
using MEC;
using System.Collections.Generic;
using System.Text;
using LabApi.Events.Arguments.PlayerEvents;
using RueI.API;
using RueI.API.Elements;

namespace XazeAPI.API.Helpers
{
    public static class HintHelper
    {
        // References
        public static readonly Tag BroadcastRef = new("Xaze-BroadcastRef");
        public static readonly Tag DefaultRef = new("Xaze-DefaultRef");
        
        // Dictionaries
        private static readonly Dictionary<ReferenceHub, Tag> elementReferences = new();
        private static readonly Dictionary<ReferenceHub, Dictionary<Tag, CoroutineHandle>> delays = new();

        // Methods
        public static void updateDisplay(this ReferenceHub hub)
        {
            var display = RueDisplay.Get(hub);
            display.Update();
        }
        
        public static void updateDisplay(this Player plr)
        {
            var display = RueDisplay.Get(plr.ReferenceHub);
            display.Update();
        }

        public static Tag DisplayHint(this ReferenceHub hub, string Text, float duration, float position, Tag givenReference = null)
        {
            var display = RueDisplay.Get(hub);

            Tag elementReference = givenReference;
            
            if ((givenReference?.Equals(null)?? true) && !elementReferences.TryGetValue(hub, out elementReference))
            {
                elementReference = new();
            }

            elementReferences[hub] = elementReference;

            BasicElement displayElement = new BasicElement(position, Text);
            display.Show(elementReference, displayElement);
            display.Update();

            if (duration <= 0) return elementReference;

            if (delays.TryGetValue(hub, out var handles))
            {
                if (handles.TryGetValue(elementReference, out var handle))
                    Timing.KillCoroutines(handle);
            }
            else
                delays[hub] = [];
            
            delays[hub][elementReference] = Timing.CallDelayed(duration, () =>
            {
                display.Remove(elementReference);
                elementReferences.Remove(hub);
                delays[hub]?.Remove(elementReference);
                display.Update();
            });

            return elementReference;
        }

        public static Tag DisplayHint(this ReferenceHub hub, string Text, float duration, Tag givenReference = null) =>
            DisplayHint(hub, Text, duration, 275f, givenReference);

        public static Tag DisplayHint(this ReferenceHub hub, string Text, Tag givenReference = null, float position = 275f) =>
            DisplayHint(hub, Text, 5f, position, givenReference);

        public static Tag DisplayHint(this ReferenceHub hub, string Text, float duration) =>
            DisplayHint(hub, Text, duration, 275f);

        public static Tag DisplayHint(this ReferenceHub hub, string Text, Tag givenReference = null) =>
            DisplayHint(hub, Text, 5f, 275f, givenReference);

        public static Tag DisplayHint(this ReferenceHub hub, string Text) =>
            DisplayHint(hub, Text, 5f, 275f);

        public static Tag DisplayHint(this Player player, string Text, float duration = 5f, float position = 275f, Tag givenReference = null)
        {
            return player.ReferenceHub.DisplayHint(Text, duration, position, givenReference);
        }

        public static Tag DisplayHint(this Player player, string Text, float duration = 5f, Tag givenReference = null) =>
            DisplayHint(player.ReferenceHub, Text, duration, 275f, givenReference);

        public static Tag DisplayHint(this Player player, string Text, float duration = 5f) =>
            DisplayHint(player.ReferenceHub, Text, duration, 275f);

        public static Tag DisplayHint(this Player player, string Text, Tag givenReference = null, float position = 275f) =>
            DisplayHint(player.ReferenceHub, Text, 5f, position, givenReference);

        public static Tag DisplayHint(this Player player, string Text, Tag givenReference = null) =>
            DisplayHint(player.ReferenceHub, Text, 5f, 275f, givenReference);

        public static Tag DisplayHint(this Player player, string Text) =>
            DisplayHint(player.ReferenceHub, Text, 5f, 275f);

        public static Tag DisplayBroadcast(this ReferenceHub hub, string Text, float duration = 5)
        {
            hub.SendBroadcast(" ", (ushort)duration, Broadcast.BroadcastFlags.Normal, true);
            return hub.DisplayHint(Text, duration, 950, BroadcastRef);
        }

        public static Tag DisplayBroadcast(this ReferenceHub hub, StringBuilder sb, float duration = 5) =>
            hub.DisplayBroadcast(sb.ToString(), duration);

        public static Tag DisplayBroadcast(this Player plr, string Text, float duration = 5) =>
            plr.ReferenceHub.DisplayBroadcast(Text, duration);
            

        public static Tag DisplayBroadcast(this Player plr, StringBuilder sb, float duration = 5) =>
            plr.ReferenceHub.DisplayBroadcast(sb.ToString(), duration);

        public static void DisplayBroadcast(string Text, float duration = 5)
        {
            foreach (var plr in Player.ReadyList)
            {
                plr.DisplayBroadcast("<align=center><line-height=25>" + Text + "</align></line-height>", duration);
            }
        }

        public static void DisplayBroadcast(StringBuilder sb, float duration = 5) => 
            DisplayBroadcast(sb.ToString(), duration);

        internal static void RemoveHub(PlayerLeftEventArgs args)
        {
            delays.Remove(args.Player.ReferenceHub);
            elementReferences.Remove(args.Player.ReferenceHub);
        }
    }
}