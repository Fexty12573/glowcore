using System.Collections.Generic;
using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class DisplaySettingsCategory : ISettingsCategory
    {
        public const string kKeyFullscreen = "menu.display.fullscreen";
        public const string kKeyResolution = "menu.display.resolution";
        public const string kKeyMouseSensitivity = "menu.display.mouseSensitivity";

        private readonly List<SettingDescriptor> m_descriptors;

        public DisplaySettingsCategory(ISettingsRepository repository, IDisplayService displayService)
        {
            var resolutionLabels = BuildResolutionLabels(displayService.AvailableResolutions);

            m_descriptors = new List<SettingDescriptor>
            {
                SettingDescriptor.Toggle(kKeyFullscreen, "FULLSCREEN",
                    () => repository.GetBool(kKeyFullscreen, Screen.fullScreen),
                    v => { repository.SetBool(kKeyFullscreen, v); displayService.SetFullscreen(v); }),

                SettingDescriptor.Dropdown(kKeyResolution, "RESOLUTION", resolutionLabels,
                    () => repository.GetInt(kKeyResolution, FindCurrentResolutionIndex(displayService.AvailableResolutions)),
                    v => { repository.SetInt(kKeyResolution, v); displayService.SetResolution(v); }),

                SettingDescriptor.Slider(kKeyMouseSensitivity, "MOUSE SENSITIVITY", 0f, 1f,
                    () => repository.GetFloat(kKeyMouseSensitivity, DisplayService.kSensitivityDefaultSlider),
                    v => { repository.SetFloat(kKeyMouseSensitivity, v); displayService.SetMouseSensitivity(v); }),
            };
        }

        public string DisplayName => "DISPLAY";
        public IReadOnlyList<SettingDescriptor> Descriptors => m_descriptors;

        private static IReadOnlyList<string> BuildResolutionLabels(IReadOnlyList<Resolution> resolutions)
        {
            var labels = new List<string>(resolutions.Count);
            foreach (Resolution r in resolutions)
                labels.Add($"{r.width} x {r.height}");
            return labels;
        }

        private static int FindCurrentResolutionIndex(IReadOnlyList<Resolution> resolutions)
        {
            for (var i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                    return i;
            }
            return Mathf.Max(0, resolutions.Count - 1);
        }
    }
}
