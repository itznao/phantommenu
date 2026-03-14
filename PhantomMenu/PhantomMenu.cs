using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;

namespace PhantomMenu
{
    [BepInPlugin("com.PhantomMenu.PhantomMenu", "PhantomMenu", "1.0.0")]
    [BepInProcess("Among Us.exe")]
    public class PhantomMenu : BasePlugin
    {
        public new static ManualLogSource Log = null!;
        public static bool ZoomEnabled  = true;
        public static float ZoomLevel   = 3f;

        public override void Load()
        {
            Log = base.Log;

            // Must register BEFORE any scene loads
            MenuOverlay.Register();

            Log.LogInfo("PhantomMenu loaded");
            new Harmony("com.PhantomMenu.PhantomMenu").PatchAll();
        }
    }

    [HarmonyPatch]
    public static class MenuPatches
    {
        private static bool _menuInit = false;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        [HarmonyPostfix]
        public static void InitMenu_Postfix()
        {
            if (_menuInit) return;
            _menuInit = true;

            MenuOverlay.Init("Phantom Menu");
            MenuOverlay.AddToggle("Show Player Roles",    defaultValue: true, v => PlayerRoleNameDisplay.Enabled = v);
            MenuOverlay.AddToggle("Mouse Zoom",           defaultValue: true, v => { PhantomMenu.ZoomEnabled = v; MouseZoom.Enabled = v; });
            MenuOverlay.AddToggle("Teleport Cursor",     defaultValue: true, v => TeleportCursor.Enabled = v);
            MenuOverlay.AddToggle("See Ghosts",          defaultValue: true, v => SeeGhosts.Enabled = v);
            MenuOverlay.AddLabel(" ");
            MenuOverlay.AddToggle("Unlock All Cosmetics", defaultValue: true, v => { UnlockAllCosmetics.Enabled = v; UnlockAllCosmetics.ForceRefresh(); });
            MenuOverlay.AddToggle("No Ban Penalty",       defaultValue: true, v => DisconnectPenalty.Enabled = v);
        }
    }
}