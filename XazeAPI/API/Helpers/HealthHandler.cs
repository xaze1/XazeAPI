using InventorySystem.Items.Usables;
using UnityEngine;

namespace XazeAPI.API.Helpers
{
    public static class HealthHelper
    {
        /// <summary>
        /// Uses Usable Item Regeneration
        /// Inconsistent and hard to understand
        /// </summary>
        /// <param name="Hub">Target Player</param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="value"></param>
        /// <param name="speedMult"></param>
        /// <param name="healthMult"></param>
        public static void AddRegeneration(this ReferenceHub Hub, float timeStart = 0f, float timeEnd = 0.5f, float value = 4f, float speedMult = 0.1f, float healthMult = 100f)
        {
            UsableItemsController.GetHandler(Hub).ActiveRegenerations.Add(new RegenerationProcess(AnimationCurve.Constant(timeStart, timeEnd, value), speedMult, healthMult));
        }
    }
}
