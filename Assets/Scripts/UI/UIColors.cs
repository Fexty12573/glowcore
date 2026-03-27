using UnityEngine;

namespace GlowCore.UI
{
    /// <summary>Color tokens matching the approved HTML prototype (v4).</summary>
    public static class UIColors
    {
        // Panels
        public static readonly Color32 WoodDark        = new Color32(28,  20,  12,  235);
        public static readonly Color32 WoodBorder      = new Color32(90,  65,  35,  153);
        public static readonly Color32 WoodBorderLight = new Color32(130, 96,  50,  102);

        // Accent
        public static readonly Color32 Accent          = new Color32(232, 199, 119, 255);
        public static readonly Color32 AccentDim       = new Color32(196, 160, 90,  255);

        // Text
        public static readonly Color32 White           = new Color32(240, 230, 211, 255);
        public static readonly Color32 WhiteDim        = new Color32(240, 230, 211, 128);
        public static readonly Color32 WhiteFaint      = new Color32(240, 230, 211, 51);

        // Slots
        public static readonly Color32 SlotBg          = new Color32(12,  8,   4,   153);
        public static readonly Color32 SlotBorder      = new Color32(90,  65,  35,  102);
        public static readonly Color32 SlotHoverBg     = new Color32(232, 199, 119, 20);
        public static readonly Color32 SlotHoverBorder = new Color32(232, 199, 119, 102);
        public static readonly Color32 SlotFilledBg    = new Color32(20,  14,  8,   179);

        // Selected (hotbar active slot)
        public static readonly Color32 SelectedBorder  = new Color32(232, 199, 119, 255);

        // State
        public static readonly Color32 Green           = new Color32(122, 184, 94,  255);
        public static readonly Color32 MissingMat      = new Color32(196, 122, 90,  255);
    }
}
