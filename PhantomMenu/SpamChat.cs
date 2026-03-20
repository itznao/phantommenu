using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class SpamChat
    {
        public static bool   Enabled  = false;
        public static string Message  = "PhantomMenu";
        public static float  Interval = 3f;
        internal static float _timer  = 0f;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class SpamChatPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!SpamChat.Enabled) return;

            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            SpamChat._timer += Time.deltaTime;
            if (SpamChat._timer < SpamChat.Interval) return;

            SpamChat._timer = 0f;
            localPlayer.RpcSendChat(SpamChat.Message);
        }
    }
}
