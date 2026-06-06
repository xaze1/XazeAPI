// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.ComponentModel;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items.Keycards;

namespace XazeAPI.API.Helpers
{
    public static class CustomKeycardHandler
    {
        public static KeycardItem? GiveCustomKeycard(this ReferenceHub hub, ItemType keycardType, params object[] args)
        {
            if (!keycardType.TryGetTemplate(out KeycardItem keycard))
            {
                throw new ArgumentException(keycardType + " is not a Custom Keycard");
            }

            if (!keycard.Customizable)
            {
                throw new InvalidEnumArgumentException(keycardType + " is not a valid custom keycard type!");
            }

            int offset = 0;
            var customDetails = keycard.Details.OfType<ICustomizableDetail>().ToList();

            if (args.Length < customDetails.Sum(d => d.CustomizablePropertiesAmount))
            {
                throw new ArgumentException("Not enough custom detail Arguments for CustomKeycard Type: " + keycardType);
            }
            
            foreach (var detail in customDetails)
            {
                int detailLength = detail.CustomizablePropertiesAmount;
                
                var values = args.Skip(offset).Take(detailLength).ToArray();
                detail.SetDetail(values);
                
                offset += detailLength;
            }

            return hub.inventory.ServerAddItem(keycardType, InventorySystem.Items.ItemAddReason.AdminCommand) as KeycardItem;
        }

        public static KeycardItem? GiveCustomKeycard(this LabApi.Features.Wrappers.Player plr, ItemType keycardType, params object[] args) => 
            plr.ReferenceHub.GiveCustomKeycard(keycardType, args);

        public static KeycardItem? GiveCustomKeycardTaskForce(this LabApi.Features.Wrappers.Player plr, string itemName = null, int containment = 0, int armory = 0, int admin = 0, string permColor = "default", string tintColor = "default", string nametag = null, string serial = null, int rankIndex = 0) => 
            plr.ReferenceHub.GiveCustomKeycard(ItemType.KeycardCustomTaskForce, itemName, new KeycardLevels(containment, armory, admin), permColor, tintColor, nametag, serial, rankIndex);

        public static KeycardItem? GiveCustomKeycardManagement(this LabApi.Features.Wrappers.Player plr, string itemName = null, int containment = 0, int armory = 0, int admin = 0, string permColor = "default", string tintColor = "default", string label = null, string labelColor = null) => 
            plr.ReferenceHub.GiveCustomKeycard(ItemType.KeycardCustomManagement, itemName, new KeycardLevels(containment, armory, admin), permColor, tintColor, label, labelColor);

        public static KeycardItem? GiveCustomKeycardSite02(this LabApi.Features.Wrappers.Player plr, string itemName = null, int containment = 0, int armory = 0, int admin = 0, string permColor = "default", string tintColor = "default", string label = null, string labelColor = null, string nametag = null, byte wear = 0) => 
            plr.ReferenceHub.GiveCustomKeycard(ItemType.KeycardCustomManagement, itemName, new KeycardLevels(containment, armory, admin), permColor, tintColor, label, labelColor);

        public static KeycardItem? GiveCustomKeycardMetalCase(this LabApi.Features.Wrappers.Player plr, string itemName = null, int containment = 0, int armory = 0, int admin = 0, string permColor = "default", string tintColor = "default", string label = null, string labelColor = null, string nametag = null, string serial = null) => 
            plr.ReferenceHub.GiveCustomKeycard(ItemType.KeycardCustomMetalCase, itemName, new KeycardLevels(containment, armory, admin), permColor, tintColor, label, labelColor);

        public static void SetDetail(this ICustomizableDetail detail, params object[] args)
        {
            
            switch (detail)
            {
                case CustomItemNameDetail:
                    CustomItemNameDetail._customText = args[0].ToString();
                    break;
                
                case CustomLabelDetail:
                    CustomLabelDetail._customText = args[0].ToString();
                    Misc.TryParseColor(args[1].ToString(), out CustomLabelDetail._customColor);
                    break;
                
                case CustomPermsDetail:
                    CustomPermsDetail._customLevels = (KeycardLevels)args[0];
                    if (Misc.TryParseColor(args[1].ToString(), out var permsColor))
                    {
                        CustomPermsDetail._customColor = permsColor;
                    }
                    break;
                
                case CustomTintDetail:
                    Misc.TryParseColor(args[0].ToString(), out CustomTintDetail._customColor);
                    break;
                    
                case NametagDetail:
                    NametagDetail._customNametag = args[0].ToString();
                    break;
                    
                case CustomSerialNumberDetail:
                    CustomSerialNumberDetail._customVal =  args[0].ToString();
                    break;
                    
                case CustomRankDetail:
                    CustomRankDetail._index =  (int)args[0];
                    break;
                    
                case CustomWearDetail:
                    CustomWearDetail._customWearLevel = (byte)args[0];
                    break;
            }
        }
    }
}
