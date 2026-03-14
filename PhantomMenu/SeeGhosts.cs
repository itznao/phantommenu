using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class SeeGhosts
    {
        public static bool Enabled = true;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class SeeGhostsPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!SeeGhosts.Enabled) return;

            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == null) continue;
                if (!player.Data.IsDead) continue;

                var rend = player.cosmetics?.currentBodySprite?.BodySprite;
                if (rend == null) continue;

                var col = rend.color;
                col.a = 0.5f;
                rend.color = col;
                player.Visible = true;
            }
        }
    }
}