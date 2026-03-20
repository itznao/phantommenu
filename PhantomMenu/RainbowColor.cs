using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class RainbowColor
    {
        public static bool Enabled     = false;
        internal static float _timer   = 0f;
        internal static int _colorIndex = 0;
        internal const int ColorCount  = 18;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class RainbowColorPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!RainbowColor.Enabled) return;

            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            RainbowColor._timer += Time.deltaTime;
            if (RainbowColor._timer < 0.5f) return;

            RainbowColor._timer = 0f;
            RainbowColor._colorIndex = (RainbowColor._colorIndex + 1) % RainbowColor.ColorCount;
            localPlayer.RpcSetColor((byte)RainbowColor._colorIndex);
        }
    }
}
