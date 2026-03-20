using HarmonyLib;

namespace PhantomMenu
{
    public static class NoKillCooldown
    {
        public static bool Enabled = false;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class NoKillCooldownPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!NoKillCooldown.Enabled) return;

            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;
            if (localPlayer.Data?.Role == null) return;
            if (!localPlayer.Data.Role.IsImpostor) return;

            localPlayer.killTimer = 0f;
        }
    }
}
