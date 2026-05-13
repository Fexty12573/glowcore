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
        private readonly int m_recommendedResolutionIndex;
        private float m_cameraSensitivityX = kSensitivityDefaultSlider;
        private float m_cameraSensitivityY = kSensitivityDefaultSlider;
        private int m_resolutionIndex;
        private bool m_isFullscreen;

        public event Action<float> OnCameraSensitivityXChanged;
        public event Action<float> OnCameraSensitivityYChanged;

        public DisplayService()
        {
            m_resolutions = BuildResolutionList();
            m_nativeResolution = QueryNativeResolution();
            m_recommendedResolutionIndex = FindResolutionIndex(m_resolutions, m_nativeResolution);
            m_resolutionIndex = m_recommendedResolutionIndex;
            m_isFullscreen = Screen.fullScreen;
        }

        public IReadOnlyList<Resolution> AvailableResolutions => m_resolutions;

        public Resolution NativeResolution => m_nativeResolution;

        public int RecommendedResolutionIndex => m_recommendedResolutionIndex;

        public float CameraSensitivityX => m_cameraSensitivityX;

        public float CameraSensitivityY => m_cameraSensitivityY;

        public void SetFullscreen(bool on)
        {
            m_isFullscreen = on;
            ApplyDisplayMode();
        }

        public void SetResolution(int index)
        {
            if (index < 0 || index >= m_resolutions.Count)
                return;

            m_resolutionIndex = index;
            ApplyDisplayMode();
        }

        public void SetCameraSensitivityX(float v)
        {
            m_cameraSensitivityX = Mathf.Clamp01(v);
            OnCameraSensitivityXChanged?.Invoke(m_cameraSensitivityX);
        }

        public void SetCameraSensitivityY(float v)
        {
            m_cameraSensitivityY = Mathf.Clamp01(v);
            OnCameraSensitivityYChanged?.Invoke(m_cameraSensitivityY);
        }

        public static float SliderToMultiplier(float slider)
        {
            return Mathf.Lerp(kSensitivityMin, kSensitivityMax, Mathf.Clamp01(slider));
        }

        // Single Screen.SetResolution call carries both the desired resolution AND the desired
        // fullscreen mode explicitly — so a SetFullscreen→SetResolution sequence can't lose the
        // fullscreen change to a stale Screen.fullScreenMode read between the two calls.
        private void ApplyDisplayMode()
        {
            if (m_resolutionIndex < 0 || m_resolutionIndex >= m_resolutions.Count)
                return;

            Resolution r = m_resolutions[m_resolutionIndex];
            FullScreenMode mode = m_isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.SetResolution(r.width, r.height, mode, r.refreshRateRatio);
        }

        private static int FindResolutionIndex(IReadOnlyList<Resolution> resolutions, Resolution target)
        {
            for (var i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].width == target.width && resolutions[i].height == target.height)
                    return i;
            }
            return Mathf.Max(0, resolutions.Count - 1);
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
