using HarmonyLib;
using AmongUs.Data;
using UnityEngine;
using System;
using System.Security.Cryptography;

namespace PhantomMenu
{
    public static class Spoof
    {
        // — Level —
        public static bool  LevelEnabled  = false;
        public static float SpoofedLevel  = 1000f;   // 1–150

        // — Platform —
        public static bool      PlatformEnabled = false;
        public static Platforms SpoofedPlatform = Platforms.Xbox;  // default spoof target

        // — Device ID —
        public static bool DeviceIdEnabled = false;
    }

    // ── Level ────────────────────────────────────────────────────────────────
    // Writes the desired level into DataManager every 5 s so the game and UI
    // always see the spoofed value (same approach as MalumMenu.SpoofLevel).

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class SpoofLevelPatch
    {
        private static float _timer = 0f;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!Spoof.LevelEnabled) return;

            _timer += Time.deltaTime;
            if (_timer < 5f) return;
            _timer = 0f;

            uint target = (uint)Mathf.Max(1f, Spoof.SpoofedLevel);
            if (DataManager.Player.Stats.Level == target - 1) return;

            DataManager.Player.stats.level = target - 1;
            DataManager.Player.Save();
        }
    }

    // ── Device ID ────────────────────────────────────────────────────────────
    // Returns a random hex string instead of the real hardware device ID.

    [HarmonyPatch(typeof(UnityEngine.SystemInfo), nameof(UnityEngine.SystemInfo.deviceUniqueIdentifier), MethodType.Getter)]
    public static class SpoofDeviceIdPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref string __result)
        {
            if (!Spoof.DeviceIdEnabled) return;
            var bytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            __result = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
