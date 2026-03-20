using HarmonyLib;

namespace PhantomMenu
{
    public static class AntiKick
    {
        public static bool Enabled = false;
    }

    [HarmonyPatch]
    public static class AntiKickPatch
    {
        static System.Reflection.MethodBase? TargetMethod() =>
            AccessTools.Method(typeof(AmongUsClient), "KickPlayer");

        [HarmonyPrefix]
        public static bool Prefix(int clientId, bool ban)
        {
            if (!AntiKick.Enabled) return true;
            return clientId != AmongUsClient.Instance.ClientId;
        }
    }
}
