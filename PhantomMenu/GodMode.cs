using HarmonyLib;

namespace PhantomMenu
{
    public static class GodMode
    {
        public static bool Enabled = false;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public static class GodModePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerControl __instance)
        {
            if (!GodMode.Enabled) return true;
            // Allow other players to die normally; block only the local player
            return __instance != PlayerControl.LocalPlayer;
        }
    }
}
