using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class NoClip
    {
        public static bool Enabled = false;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class NoClipPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            var col = localPlayer.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = !NoClip.Enabled;
        }
    }
}
