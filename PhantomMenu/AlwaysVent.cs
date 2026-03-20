using HarmonyLib;

namespace PhantomMenu
{
    public static class AlwaysVent
    {
        public static bool Enabled = false;
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class AlwaysVentCanUsePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Vent __instance, ref float __result, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse)
        {
            if (!AlwaysVent.Enabled) return;
            if (pc.Object != PlayerControl.LocalPlayer) return;
            canUse = true;
            couldUse = true;
            __result = 0f;
        }
    }
}
