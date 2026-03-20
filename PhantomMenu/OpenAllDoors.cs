using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class OpenAllDoors
    {
        public static bool Enabled = false;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class OpenAllDoorsPatch
    {
        private static int _updateCounter = 0;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!OpenAllDoors.Enabled) return;

            // Only run every 30 frames to avoid spam
            if (++_updateCounter < 30) return;
            _updateCounter = 0;

            foreach (var door in Object.FindObjectsOfType<PlainDoor>())
            {
                if (door != null && !door.Open)
                    door.SetDoorway(true);
            }
        }
    }
}
