using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class DisplayService : IDisplayService
    {
        public const float kSensitivityMin = 0.1f;
        public const float kSensitivityMax = 3.0f;
        public const float kSensitivityDefaultSlider = 0.31f;

        private readonly List<Resolution> m_resolutions;
        private readonly Resolution m_nativeResolution;
        private float m_mouseSensitivity = kSensitivityDefaultSlider;

        public event Action<float> OnMouseSensitivityChanged;

        public DisplayService()
        {
            m_resolutions = BuildResolutionList();
            m_nativeResolution = QueryNativeResolution();
        }

        public IReadOnlyList<Resolution> AvailableResolutions => m_resolutions;

        public Resolution NativeResolution => m_nativeResolution;

        public float MouseSensitivity => m_mouseSensitivity;

        public void SetFullscreen(bool on)
        {
            Screen.fullScreen = on;
        }

        public void SetResolution(int index)
        {
            if (index < 0 || index >= m_resolutions.Count)
                return;

            Resolution r = m_resolutions[index];
            Screen.SetResolution(r.width, r.height, Screen.fullScreenMode, r.refreshRateRatio);
        }

        public void SetMouseSensitivity(float v)
        {
            m_mouseSensitivity = Mathf.Clamp01(v);
            OnMouseSensitivityChanged?.Invoke(m_mouseSensitivity);
        }

        public static float SliderToMultiplier(float slider)
        {
            return Mathf.Lerp(kSensitivityMin, kSensitivityMax, Mathf.Clamp01(slider));
        }

        private static Resolution QueryNativeResolution()
        {
            // Display.main reports the OS desktop resolution, which is stable even when the player
            // has selected a non-native windowed size. Refresh rate isn't on Display, so fall back
            // to the current screen's reported rate.
            return new Resolution
            {
                width = Display.main.systemWidth,
                height = Display.main.systemHeight,
                refreshRateRatio = Screen.currentResolution.refreshRateRatio,
            };
        }

        private static List<Resolution> BuildResolutionList()
        {
            var byWh = new Dictionary<(int, int), Resolution>();
            foreach (Resolution r in Screen.resolutions)
            {
                var key = (r.width, r.height);
                if (!byWh.TryGetValue(key, out Resolution existing) || r.refreshRateRatio.value > existing.refreshRateRatio.value)
                    byWh[key] = r;
            }

            var list = new List<Resolution>(byWh.Values);
            list.Sort((a, b) =>
            {
                if (a.width != b.width)
                    return a.width.CompareTo(b.width);
                return a.height.CompareTo(b.height);
            });
            return list;
        }
    }
}
