using System.Collections.Generic;
using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class DisplaySettingsCategory : ISettingsCategory
    {
        public const string kKeyFullscreen = "menu.display.fullscreen";
        public const string kKeyResolution = "menu.display.resolution";
        public const string kKeyCameraSensitivityX = "menu.display.cameraSensitivity.x";
        public const string kKeyCameraSensitivityY = "menu.display.cameraSensitivity.y";
        public const string kRecommendedSuffix = " (Recommended)";

        private readonly List<SettingDescriptor> m_descriptors;

        public DisplaySettingsCategory(ISettingsRepository repository, IDisplayService displayService)
        {
            var resolutionLabels = BuildResolutionLabels(displayService.AvailableResolutions, displayService.NativeResolution);

            m_descriptors = new List<SettingDescriptor>
            {
                SettingDescriptor.Toggle(kKeyFullscreen, "FULLSCREEN",
                    () => repository.GetBool(kKeyFullscreen, Screen.fullScreen),
                    v => { repository.SetBool(kKeyFullscreen, v); displayService.SetFullscreen(v); }),

                SettingDescriptor.Dropdown(kKeyResolution, "RESOLUTION", resolutionLabels,
                    () => repository.GetInt(kKeyResolution, FindNativeResolutionIndex(displayService.AvailableResolutions, displayService.NativeResolution)),
                    v => { repository.SetInt(kKeyResolution, v); displayService.SetResolution(v); }),

                SettingDescriptor.Slider(kKeyCameraSensitivityX, "CAMERA SENSITIVITY X", 0f, 1f,
                    () => repository.GetFloat(kKeyCameraSensitivityX, DisplayService.kSensitivityDefaultSlider),
                    v => { repository.SetFloat(kKeyCameraSensitivityX, v); displayService.SetCameraSensitivityX(v); }),

                SettingDescriptor.Slider(kKeyCameraSensitivityY, "CAMERA SENSITIVITY Y", 0f, 1f,
                    () => repository.GetFloat(kKeyCameraSensitivityY, DisplayService.kSensitivityDefaultSlider),
                    v => { repository.SetFloat(kKeyCameraSensitivityY, v); displayService.SetCameraSensitivityY(v); }),
            };
        }

        public string DisplayName => "DISPLAY";
        public IReadOnlyList<SettingDescriptor> Descriptors => m_descriptors;

        private static IReadOnlyList<string> BuildResolutionLabels(IReadOnlyList<Resolution> resolutions, Resolution native)
        {
            var labels = new List<string>(resolutions.Count);
            foreach (Resolution r in resolutions)
            {
                var label = $"{r.width} x {r.height}";
                if (r.width == native.width && r.height == native.height)
                    label += kRecommendedSuffix;
                labels.Add(label);
            }
            return labels;
        }

        private static int FindNativeResolutionIndex(IReadOnlyList<Resolution> resolutions, Resolution native)
        {
            for (var i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].width == native.width && resolutions[i].height == native.height)
                    return i;
            }
            return Mathf.Max(0, resolutions.Count - 1);
        }
    }
}
