using HarmonyLib;

namespace PhantomMenu
{
    public static class InfiniteVision
    {
        public static bool Enabled = false;
        internal const float VisionRange = 100f;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class InfiniteVisionPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!InfiniteVision.Enabled) return;

            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            var light = localPlayer.lightSource;
            if (light != null)
                light.viewDistance = InfiniteVision.VisionRange;

            var shadow = HudManager.Instance?.ShadowQuad;
            if (shadow != null)
                shadow.gameObject.SetActive(false);
        }
    }
}
