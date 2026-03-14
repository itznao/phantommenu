using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class TeleportCursor
    {
        public static bool Enabled = true;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TeleportCursorPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!TeleportCursor.Enabled) return;

            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            if (Input.GetMouseButtonDown(1))
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                localPlayer.NetTransform.RpcSnapTo(worldPos);
            }
        }
    }
}