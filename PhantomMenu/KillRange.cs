using HarmonyLib;
using AmongUs.GameOptions;

namespace PhantomMenu
{
    public static class KillRange
    {
        public static bool Enabled = false;
    }

    // Override the kill distance reported by game options to maximum
    [HarmonyPatch(typeof(NormalGameOptionsV09), nameof(NormalGameOptionsV09.KillDistance), MethodType.Getter)]
    public static class KillRangePatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result)
        {
            if (!KillRange.Enabled) return;
            __result = 999;
        }
    }
}
