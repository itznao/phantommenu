using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class UnlockAllCosmetics
    {
        public static bool Enabled = true;
        public static HatManager? _hatManager = null;

        public static void ForceRefresh()
        {
            if (_hatManager == null) return;
            
            if (Enabled)
            {
                // Unlock all cosmetics
                foreach (var h in _hatManager.allHats) h.Free = true;
                foreach (var s in _hatManager.allSkins) s.Free = true;
                foreach (var p in _hatManager.allPets) p.Free = true;
                foreach (var n in _hatManager.allNamePlates) n.Free = true;
                foreach (var v in _hatManager.allVisors) v.Free = true;
                foreach (var b in _hatManager.allBundles) b.Free = true;
                foreach (var b in _hatManager.allFeaturedBundles) b.Free = true;
                foreach (var c in _hatManager.allFeaturedCubes) c.Free = true;
                foreach (var i in _hatManager.allFeaturedItems) i.Free = true;
                foreach (var b in _hatManager.allStarBundles) b.price = 0;
            }
            else
            {
                // Lock all cosmetics
                foreach (var h in _hatManager.allHats) h.Free = false;
                foreach (var s in _hatManager.allSkins) s.Free = false;
                foreach (var p in _hatManager.allPets) p.Free = false;
                foreach (var n in _hatManager.allNamePlates) n.Free = false;
                foreach (var v in _hatManager.allVisors) v.Free = false;
                foreach (var b in _hatManager.allBundles) b.Free = false;
                foreach (var b in _hatManager.allFeaturedBundles) b.Free = false;
                foreach (var c in _hatManager.allFeaturedCubes) c.Free = false;
                foreach (var i in _hatManager.allFeaturedItems) i.Free = false;
            }

            // Refresh cosmetics screen if it's open
            RefreshCosmeticsScreen();
        }

        private static void RefreshCosmeticsScreen()
        {
            // Cosmetics changes are applied directly to the HatManager
            // Screen refresh will happen naturally on next interaction
        }
    }

    [HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
    public static class UnlockAllCosmeticsPatch
    {
        [HarmonyPostfix]
        public static void Postfix(HatManager __instance)
        {
            UnlockAllCosmetics._hatManager = __instance;
            UnlockAllCosmetics.ForceRefresh();
        }
    }

    // Patch GetPurchase to dynamically return owned state
    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
    public static class ForcePurchaseOwnedPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(string productId, ref bool __result)
        {
            if (UnlockAllCosmetics.Enabled)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
