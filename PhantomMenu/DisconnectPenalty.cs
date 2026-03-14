using HarmonyLib;
using AmongUs.Data.Player;

namespace PhantomMenu
{
    public static class DisconnectPenalty
    {
        public static bool Enabled = true;
    }

    [HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
    public static class DisconnectPenaltyPatch
    {
        [HarmonyPostfix]
        public static bool Prefix(PlayerBanData __instance, ref bool __result)
        {
            if (!DisconnectPenalty.Enabled) return true;
            __instance.BanPoints = 0f;
            __result = false;
            return false;
        }
    }
}
