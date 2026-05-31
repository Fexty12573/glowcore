using System.Collections.Generic;
using UnityEngine;

namespace GlowCore.Rendering
{
    /// <summary>
    /// Holds the set of renderers that should be written into the hover mask for the current frame.
    /// Gameplay code (e.g. <c>NodeActionSystem</c>) registers the hovered object's renderers here, and
    /// <c>HoverMaskRendererFeature</c> reads them to build the screen-space <c>_HoverMask</c> texture that
    /// the EdgeHighlight full-screen pass uses to tint the hovered object's outline white.
    /// </summary>
    public static class HoverMaskRegistry
    {
        private static readonly List<Renderer> s_renderers = new();

        public static IReadOnlyList<Renderer> Renderers => s_renderers;

        public static bool HasRenderers => s_renderers.Count > 0;

        public static void Set(IReadOnlyList<Renderer> renderers)
        {
            s_renderers.Clear();
            if (renderers == null)
                return;

            for (int i = 0; i < renderers.Count; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer != null)
                    s_renderers.Add(renderer);
            }
        }

        public static void Clear() => s_renderers.Clear();
    }
}
