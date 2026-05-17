using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlowCore.UI.Menus
{
    public interface IDisplayService
    {
        IReadOnlyList<Resolution> AvailableResolutions { get; }
        Resolution NativeResolution { get; }
        int RecommendedResolutionIndex { get; }
        float CameraSensitivityX { get; }
        float CameraSensitivityY { get; }
        void SetFullscreen(bool on);
        void SetResolution(int index);
        void SetCameraSensitivityX(float v);
        void SetCameraSensitivityY(float v);
        event Action<float> OnCameraSensitivityXChanged;
        event Action<float> OnCameraSensitivityYChanged;
    }
}
