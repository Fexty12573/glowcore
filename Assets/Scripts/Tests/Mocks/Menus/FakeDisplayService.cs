using System;
using System.Collections.Generic;
using GlowCore.UI.Menus;
using UnityEngine;

public class FakeDisplayService : IDisplayService
{
    private float m_cameraSensitivityX;
    private float m_cameraSensitivityY;

    public List<Resolution> Resolutions { get; } = new List<Resolution>();
    public Resolution Native { get; set; }
    public int RecommendedResolutionIndex { get; set; }
    public int LastResolutionIndex { get; private set; } = -1;
    public bool? LastFullscreen { get; private set; }

    public IReadOnlyList<Resolution> AvailableResolutions => Resolutions;
    public Resolution NativeResolution => Native;
    public float CameraSensitivityX => m_cameraSensitivityX;
    public float CameraSensitivityY => m_cameraSensitivityY;

    public event Action<float> OnCameraSensitivityXChanged;
    public event Action<float> OnCameraSensitivityYChanged;

    public void SetFullscreen(bool on) => LastFullscreen = on;

    public void SetResolution(int index) => LastResolutionIndex = index;

    public void SetCameraSensitivityX(float v)
    {
        m_cameraSensitivityX = v;
        OnCameraSensitivityXChanged?.Invoke(v);
    }

    public void SetCameraSensitivityY(float v)
    {
        m_cameraSensitivityY = v;
        OnCameraSensitivityYChanged?.Invoke(v);
    }
}
