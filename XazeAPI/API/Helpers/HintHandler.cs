using LabApi.Features.Wrappers;
using MEC;
using RueI.Displays;
using RueI.Elements;
using System.Collections.Generic;
using System.Text;
using Random = UnityEngine.Random;

namespace XazeAPI.API.Helpers
{
    public static class HintHelper
    {
        // References
        public static IElemReference<SetElement> BroadcastRef = DisplayCore.GetReference<SetElement>();
        public static IElemReference<Element> DefaultRef = DisplayCore.GetReference<Element>();
        
        // Dictionaries
        public static Dictionary<ReferenceHub, Display> MainDisplays = new();
        public static Dictionary<ReferenceHub, Display> RandomTextDisplays = new();
        
        private static Dictionary<ReferenceHub, IElemReference<SetElement>> elementReferences = new();

        // Methods
        public static SetElement displayTextRand(Player plr, string text, int duration = 10)
        {
            ReferenceHub hub = plr.ReferenceHub;
            float position = RueI.Ruetility.ScaledPositionToFunctional(Random.Range(0.0f, 1000f));
            DisplayCore core = DisplayCore.Get(hub);
            SetElement element = new SetElement(position, text);

            if (RandomTextDisplays.TryGetValue(hub, out Display randDisplay))
            {
                randDisplay.AddAsReference(DefaultRef, element);
                randDisplay.Update();
            }
            else
            {
                Display newDisplay = new Display(hub);
                core.AddDisplay(newDisplay);
                newDisplay.AddAsReference(DefaultRef, element);
                newDisplay.Update();
                RandomTextDisplays[hub] = newDisplay;
            }

            return element;
        }

        public static void updateCore(ReferenceHub hub)
        {
            DisplayCore core = DisplayCore.Get(hub);
            core.Update();
        }

        public static IElemReference<SetElement> DisplayHint(this ReferenceHub hub, string Text, float duration = 5f, float position = 275f, IElemReference<SetElement> givenReference = null)
        {
            DisplayCore core = DisplayCore.Get(hub);

            IElemReference<SetElement> elementReference = givenReference;
            if (elementReference == null && !elementReferences.TryGetValue(hub, out elementReference))
            {
                elementReference = DisplayCore.GetReference<SetElement>();
            }

            elementReferences[hub] = elementReference;

            SetElement displayElement = core.GetElementOrNew(elementReference, () => new SetElement(position, Text));
            displayElement.Content = Text;
            displayElement.Position = position;

            core.AddAsReference(elementReference, displayElement);
            core.Update();

            if (duration > 0)
            {
                Timing.CallDelayed(duration, () =>
                {
                    core.RemoveReference(elementReference);
                    elementReferences.Remove(hub);
                    displayElement.Position = 1100;
                    displayElement.Enabled = false;
                    core.Update();
                });
            }

            return elementReference;
        }

        public static IElemReference<SetElement> DisplayHint(this ReferenceHub hub, string Text, float duration = 5f, IElemReference<SetElement> givenReference = null) =>
            DisplayHint(hub, Text, duration, 275f, givenReference);

        public static IElemReference<SetElement> DisplayHint(this ReferenceHub hub, string Text, IElemReference<SetElement> givenReference = null, float position = 275f) =>
            DisplayHint(hub, Text, 5f, position, givenReference);

        public static IElemReference<SetElement> DisplayHint(this ReferenceHub hub, string Text, float duration = 5f) =>
            DisplayHint(hub, Text, duration, 275f);

        public static IElemReference<SetElement> DisplayHint(this ReferenceHub hub, string Text, IElemReference<SetElement> givenReference = null) =>
            DisplayHint(hub, Text, 5f, 275f, givenReference);

        public static IElemReference<SetElement> DisplayHint(this ReferenceHub hub, string Text) =>
            DisplayHint(hub, Text, 5f, 275f);

        public static IElemReference<SetElement> DisplayHint(this Player player, string Text, float duration = 5f, float position = 275f, IElemReference<SetElement> givenReference = null)
        {
            return player.ReferenceHub.DisplayHint(Text, duration, position, givenReference);
        }

        public static IElemReference<SetElement> DisplayHint(this Player player, string Text, float duration = 5f, IElemReference<SetElement> givenReference = null) =>
            DisplayHint(player.ReferenceHub, Text, duration, 275f, givenReference);

        public static IElemReference<SetElement> DisplayHint(this Player player, string Text, float duration = 5f) =>
            DisplayHint(player.ReferenceHub, Text, duration, 275f);

        public static IElemReference<SetElement> DisplayHint(this Player player, string Text, IElemReference<SetElement> givenReference = null, float position = 275f) =>
            DisplayHint(player.ReferenceHub, Text, 5f, position, givenReference);

        public static IElemReference<SetElement> DisplayHint(this Player player, string Text, IElemReference<SetElement> givenReference = null) =>
            DisplayHint(player.ReferenceHub, Text, 5f, 275f, givenReference);

        public static IElemReference<SetElement> DisplayHint(this Player player, string Text) =>
            DisplayHint(player.ReferenceHub, Text, 5f, 275f);

        public static IElemReference<SetElement> DisplayBroadcast(this ReferenceHub hub, string Text, float duration = 5)
        {
            hub.SendBroadcast(" ", (ushort)duration, Broadcast.BroadcastFlags.Normal, true);
            return hub.DisplayHint(Text, duration, 950, BroadcastRef);
        }

        public static IElemReference<SetElement> DisplayBroadcast(this ReferenceHub hub, StringBuilder sb, float duration = 5) =>
            hub.DisplayBroadcast(sb.ToString(), duration);

        public static IElemReference<SetElement> DisplayBroadcast(this Player plr, string Text, float duration = 5) =>
            plr.ReferenceHub.DisplayBroadcast(Text, duration);
            

        public static IElemReference<SetElement> DisplayBroadcast(this Player plr, StringBuilder sb, float duration = 5) =>
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
    }
}