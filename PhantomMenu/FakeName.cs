using HarmonyLib;

namespace PhantomMenu
{
    public static class FakeName
    {
        public static bool   Enabled    = false;
        public static string CustomName = "Phantom";

        private static string? _originalName = null;

        public static void SetName(bool enabled)
        {
            Enabled = enabled;

            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer?.Data == null) return;

            if (enabled)
            {
                _originalName ??= localPlayer.Data.PlayerName;
                localPlayer.RpcSetName(CustomName);
            }
            else if (_originalName != null)
            {
                localPlayer.RpcSetName(_originalName);
                _originalName = null;
            }
        }
    }

    // Reapply the fake name after meetings reset it
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static class FakeNameMeetingPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!FakeName.Enabled) return;
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;
            localPlayer.RpcSetName(FakeName.CustomName);
        }
    }
}
