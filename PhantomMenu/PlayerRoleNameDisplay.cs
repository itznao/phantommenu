using HarmonyLib;
using AmongUs.GameOptions;
using UnityEngine;
using System.Collections.Generic;

namespace PhantomMenu
{
    public static class PlayerRoleNameDisplay
    {
        public static bool Enabled = true;
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    public static class CrewmateIntroRoleDisplay
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!PlayerRoleNameDisplay.Enabled) return;
            RoleDisplayHelper.ApplyRoleDisplay();
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    public static class ImpostorIntroRoleDisplay
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!PlayerRoleNameDisplay.Enabled) return;
            RoleDisplayHelper.ApplyRoleDisplay();
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingRoleDisplay
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!PlayerRoleNameDisplay.Enabled) return;
            RoleDisplayHelper.ApplyRoleDisplay();
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class GameplayRoleDisplay
    {
        private static int _updateCounter = 0;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!PlayerRoleNameDisplay.Enabled) return;

            _updateCounter++;
            if (_updateCounter >= 10)
            {
                _updateCounter = 0;
                RoleDisplayHelper.ApplyRoleDisplay();
            }
        }
    }

    // Clear cache and restore names when the game ends
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class GameEndRoleDisplayCleanup
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            RoleDisplayHelper.RestoreNames();
            RoleDisplayHelper.ClearCache();
        }
    }

    // Also clear on lobby return
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class ExitGameRoleDisplayCleanup
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            RoleDisplayHelper.RestoreNames();
            RoleDisplayHelper.ClearCache();
        }
    }

    public static class RoleDisplayHelper
    {
        private static Dictionary<byte, string> _originalNames = new Dictionary<byte, string>();

        public static void ClearCache() => _originalNames.Clear();

        public static void RestoreNames()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player?.Data == null) continue;

                byte playerId = player.Data.PlayerId;
                if (!_originalNames.TryGetValue(playerId, out string originalName)) continue;

                player.Data.PlayerName = originalName;

                if (player.cosmetics?.nameText != null)
                    player.cosmetics.nameText.text = originalName;
            }
        }

        public static void ApplyRoleDisplay()
        {
            if (!PlayerRoleNameDisplay.Enabled) return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player?.Data == null) continue;

                byte playerId = player.Data.PlayerId;
                string currentName = player.Data.PlayerName ?? "???";

                // Only store the original name if it hasn't been tagged yet
                if (!_originalNames.ContainsKey(playerId))
                {
                    // Strip any existing role tag just in case
                    _originalNames[playerId] = StripRoleTag(currentName);
                }

                string originalName = _originalNames[playerId];
                var (roleDisplayName, roleColor) = GetRoleDisplayInfo(player.Data.RoleType);
                string hexColor = "#" + ColorUtility.ToHtmlStringRGB(roleColor);
                string newDisplayName = $"<color={hexColor}><size=50%>{roleDisplayName}</size></color>\n{originalName}";

                if (player.Data.PlayerName != newDisplayName)
                    player.Data.PlayerName = newDisplayName;

                if (player.cosmetics?.nameText != null)
                    player.cosmetics.nameText.text = newDisplayName;
            }
        }

        // Strips the role tag line if the name was already modified
        private static string StripRoleTag(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var lines = name.Split('\n');
            // Role tag is always the first line, player name is always last
            return lines[lines.Length - 1];
        }

        private static (string displayName, Color color) GetRoleDisplayInfo(RoleTypes roleType)
        {
            return roleType switch
            {
                RoleTypes.Impostor      => ("Impostor",       Color.red),
                RoleTypes.Phantom       => ("Phantom",        Color.red),
                RoleTypes.Shapeshifter  => ("Shapeshifter",   Color.red),
                RoleTypes.Viper         => ("Viper",          Color.red),
                RoleTypes.Crewmate      => ("Crewmate",       Color.cyan),
                RoleTypes.Scientist     => ("Scientist",      Color.cyan),
                RoleTypes.Engineer      => ("Engineer",       Color.cyan),
                RoleTypes.Noisemaker    => ("Noisemaker",     Color.cyan),
                RoleTypes.Tracker       => ("Tracker",        Color.cyan),
                RoleTypes.Detective     => ("Detective",      Color.cyan),
                RoleTypes.GuardianAngel => ("Guardian Angel", Color.cyan),
                _ => (roleType.ToString() is string s && !string.IsNullOrEmpty(s) ? s : "Unknown", Color.gray),
            };
        }
    }
}