using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class SpeedBoost
    {
        public static bool  Enabled    = false;
        public static float Multiplier = 2f;
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class SpeedBoostPatch
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!SpeedBoost.Enabled) return;
            if (__instance.myPlayer != PlayerControl.LocalPlayer) return;

            var body = __instance.body;
            if (body == null) return;

            var vel = body.velocity;
            if (vel.sqrMagnitude > 0.01f)
                body.velocity = vel * SpeedBoost.Multiplier;
        }
    }
}
