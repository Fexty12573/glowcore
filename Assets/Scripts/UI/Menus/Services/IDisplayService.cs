using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlowCore.UI.Menus
{
    public interface IDisplayService
    {
        IReadOnlyList<Resolution> AvailableResolutions { get; }
        float MouseSensitivity { get; }
        void SetFullscreen(bool on);
        void SetResolution(int index);
        void SetMouseSensitivity(float v);
        event Action<float> OnMouseSensitivityChanged;
    }
}
