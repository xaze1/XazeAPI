using InventorySystem;
using InventorySystem.Items.Keycards;
using UnityEngine;

namespace XazeAPI.API.Helpers
{
    public static class CustomKeycardHandler
    {
        public static KeycardItem? GiveCustomKeycard(this ReferenceHub hub, ItemType keycardType, string itemName = null, int containment = 0, int armory = 0, int admin = 0, string permColor = "default", string tint = "default", string label = null, string labelColor = "default")
        {
            if (!keycardType.TryGetTemplate(out KeycardItem keycard))
            {
                return null;
            }

            if (!keycard.Customizable)
            {
                return null;
            }

            CustomItemNameDetail._customText = itemName;
            CustomLabelDetail._customText = label;
            CustomPermsDetail._customLevels = new(containment, armory, admin);
            CustomPermsDetail._customColor = (Misc.TryParseColor(permColor, out var color) ? new Color32?(color) : null);
            Misc.TryParseColor(tint, out CustomTintDetail._customColor);
            Misc.TryParseColor(labelColor, out CustomLabelDetail._customColor);

            return hub.inventory.ServerAddItem(keycardType, InventorySystem.Items.ItemAddReason.AdminCommand) as KeycardItem;
        }

        public static KeycardItem? GiveCustomKeycard(this LabApi.Features.Wrappers.Player plr, ItemType keycardType, string itemName = null, int containment = 0, int armory = 0, int admin = 0, string permColor = "default", string tint = "default", string label = null, string labelColor = "default") => GiveCustomKeycard(plr.ReferenceHub, keycardType, itemName, containment, armory, admin, permColor, tint, label, labelColor);
    }
}
