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

            MenuOverlay.Register();
            MenuOverlay.Init("Phantom Menu");

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
            // — Visuals —
            MenuOverlay.AddLabel("— Visuals —");
            MenuOverlay.AddToggle("Show Player Roles",    defaultValue: false,  v => PlayerRoleNameDisplay.Enabled = v);
            MenuOverlay.AddToggle("Mouse Zoom",           defaultValue: false,  v => { PhantomMenu.ZoomEnabled = v; MouseZoom.Enabled = v; });
            MenuOverlay.AddToggle("See Ghosts",           defaultValue: false,  v => SeeGhosts.Enabled = v);
            MenuOverlay.AddToggle("Infinite Vision",      defaultValue: false, v => InfiniteVision.Enabled = v);
            MenuOverlay.AddToggle("Rainbow Color",        defaultValue: false, v => RainbowColor.Enabled = v);

            // — Movement —
            MenuOverlay.AddLabel("— Movement —");
            MenuOverlay.AddToggle("Teleport Cursor",      defaultValue: false,  v => TeleportCursor.Enabled = v);
            MenuOverlay.AddToggle("Speed Boost",          defaultValue: false, v => SpeedBoost.Enabled = v);
            MenuOverlay.AddSlider("Speed Multiplier",     defaultValue: 2f,    min: 1f, max: 5f, v => SpeedBoost.Multiplier = v);
            MenuOverlay.AddToggle("No Clip",              defaultValue: false, v => NoClip.Enabled = v);
            MenuOverlay.AddToggle("Always Vent",          defaultValue: false, v => AlwaysVent.Enabled = v);
            MenuOverlay.AddToggle("Open All Doors",       defaultValue: false, v => OpenAllDoors.Enabled = v);

            // — Combat —
            MenuOverlay.AddLabel("— Combat —");
            MenuOverlay.AddToggle("No Kill Cooldown",     defaultValue: false, v => NoKillCooldown.Enabled = v);
            MenuOverlay.AddToggle("Extended Kill Range",  defaultValue: false, v => KillRange.Enabled = v);
            MenuOverlay.AddToggle("God Mode",             defaultValue: false, v => GodMode.Enabled = v);

            // — Meeting —
            MenuOverlay.AddLabel("— Meeting —");
            MenuOverlay.AddToggle("Anti Kick",            defaultValue: false, v => AntiKick.Enabled = v);

            // — Spoofing —
            MenuOverlay.AddLabel("— Spoofing —");
            MenuOverlay.AddToggle("Spoof Level",          defaultValue: false, v => Spoof.LevelEnabled = v);
            MenuOverlay.AddSlider("Level (1-150)",        defaultValue: 100f,  min: 1f, max: 150f, v => Spoof.SpoofedLevel = v);
            MenuOverlay.AddToggle("Spoof Device ID",      defaultValue: false, v => Spoof.DeviceIdEnabled = v);

            // — Utility —
            MenuOverlay.AddLabel("— Utility —");
            MenuOverlay.AddButton("Complete All Tasks",   InstantTasks.CompleteAll);
            MenuOverlay.AddToggle("Spam Chat",            defaultValue: false, v => SpamChat.Enabled = v);
            MenuOverlay.AddToggle("Fake Name",            defaultValue: false, v => FakeName.SetName(v));
            MenuOverlay.AddToggle("Unlock All Cosmetics", defaultValue: true,  v => { UnlockAllCosmetics.Enabled = v; UnlockAllCosmetics.ForceRefresh(); });
            MenuOverlay.AddToggle("No Ban Penalty",       defaultValue: true,  v => DisconnectPenalty.Enabled = v);
        }
    }
}