using HarmonyLib;
using UnityEngine;

namespace PhantomMenu
{
    public static class MouseZoom
    {
        public static bool Enabled = true;

        internal const float  DEFAULT_ZOOM          = 3f;
        internal static float OriginalViewDistance  = -1f;
        internal static bool  ShadowWasActive       = true;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class MouseZoomPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!MouseZoom.Enabled) return;
            if (!PhantomMenu.ZoomEnabled) return;

            var cam = Camera.main;
            if (cam == null) return;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                PhantomMenu.ZoomLevel = Mathf.Clamp(
                    PhantomMenu.ZoomLevel - scroll * 5f, MouseZoom.DEFAULT_ZOOM, 20f);
            }

            cam.orthographicSize = PhantomMenu.ZoomLevel;

            var light  = PlayerControl.LocalPlayer?.lightSource;
            var shadow = HudManager.Instance?.ShadowQuad;

            bool isZoomedOut = PhantomMenu.ZoomLevel > MouseZoom.DEFAULT_ZOOM;

            if (isZoomedOut)
            {
                if (light != null && MouseZoom.OriginalViewDistance < 0f)
                    MouseZoom.OriginalViewDistance = light.viewDistance;
                if (shadow != null && shadow.gameObject.activeSelf)
                    MouseZoom.ShadowWasActive = true;

                if (light != null)
                    light.viewDistance = PhantomMenu.ZoomLevel * 2f;
                if (shadow != null)
                    shadow.gameObject.SetActive(false);
            }
            else
            {
                if (light != null && MouseZoom.OriginalViewDistance >= 0f)
                {
                    light.viewDistance = MouseZoom.OriginalViewDistance;
                    MouseZoom.OriginalViewDistance = -1f;
                }
                if (shadow != null)
                    shadow.gameObject.SetActive(MouseZoom.ShadowWasActive);
            }
        }
    }
}