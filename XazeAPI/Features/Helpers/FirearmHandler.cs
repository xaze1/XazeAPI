// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Features.Wrappers;
using XazeAPI.API.Extensions;

namespace XazeAPI.API.Helpers
{
    public static class FirearmHandler
    {
        public static Item? GiveFirearmWithAttachments(this Player player, ItemType firearm, bool randomAttachments = false, bool fullAmmo = true)
        {
            if (MainHelper.AmmoTypes.Contains(firearm))
            {
                player.AddAmmo(firearm, 15);
                return null;
            }

            Item? item = player.AddItem(firearm);
            if (item is not FirearmItem firearm2)
            {
                return item;
            }
            
            if (!randomAttachments)
            {
                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(player.ReferenceHub, out var value) && value.TryGetValue(firearm, out var value2))
                {
                    firearm2.Base.ApplyAttachmentsCode(value2, reValidate: true);
                }
            }
            else
            {
                firearm2.Base.ApplyAttachmentsCode(AttachmentsUtils.GetRandomAttachmentsCode(firearm), true);
            }


            if (fullAmmo && firearm2.Base.TryGetModule(out MagazineModule ammo))
            {
                ammo.ServerSetInstanceAmmo(firearm2.Serial, ammo.AmmoMax);
            }

            return firearm2;

        }
        
        public static Item? GiveFirearmWithAttachments(this ReferenceHub player, ItemType firearm, bool randomAttachments = false, bool fullAmmo = true)
        {
            if (MainHelper.AmmoTypes.Contains(firearm))
            {
                player.inventory.ServerAddAmmo(firearm, 15);
                return null;
            }

            ItemBase item = player.inventory.ServerAddItem(firearm, ItemAddReason.Undefined);
            if (item is not Firearm firearm2)
            {
                return Item.Get(item);
            }
            
            if (!randomAttachments)
            {
                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(player, out var value) && value.TryGetValue(firearm, out var value2))
                {
                    firearm2.ApplyAttachmentsCode(value2, reValidate: true);
                }
            }
            else
            {
                firearm2.ApplyAttachmentsCode(AttachmentsUtils.GetRandomAttachmentsCode(firearm), true);
            }


            if (!fullAmmo)
            {
                return Item.Get(firearm2);
            }
            
            if (firearm2.TryGetModule(out IPrimaryAmmoContainerModule ammo))
            {
                ammo.ServerSetInstanceAmmo(firearm2.ItemSerial, ammo.AmmoMax);
            }

            return Item.Get(firearm2);
        }
    }
}
