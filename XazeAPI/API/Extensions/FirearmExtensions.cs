// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

namespace XazeAPI.API.Extensions
{

    using InventorySystem.Items.Firearms.Modules;
    using UnityEngine;

    public static class FirearmExtensions
    {
        public static void ServerSetInstanceAmmo(this IPrimaryAmmoContainerModule container, ushort serial, int amount)
        {
            switch (container)
            {
                case CylinderAmmoModule mag:
                {
                    CylinderAmmoModule.Chamber[] chambers = CylinderAmmoModule.GetChambersArrayForSerial(serial, mag.AmmoMax);

                    int remainingAmount = Mathf.Clamp(amount, 0, mag.AmmoMax);
                    foreach(CylinderAmmoModule.Chamber chamber in chambers)
                    {
                        chamber.ContextState = remainingAmount <= 0 ? CylinderAmmoModule.ChamberState.Empty : CylinderAmmoModule.ChamberState.Live;

                        remainingAmount--;
                    }

                    mag.ServerResync();
                    return;
                }
                
                case MagazineModule ammo:
                    ammo.ServerSetInstanceAmmo(serial, amount);
                    return;
            }
        }
    }
}
