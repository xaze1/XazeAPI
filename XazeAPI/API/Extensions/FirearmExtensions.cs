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
