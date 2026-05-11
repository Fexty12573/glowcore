using System;
using System.Collections.Generic;
using GlowCore.UI.Menus;
using UnityEngine;

public class FakeDisplayService : IDisplayService
{
    private float m_mouseSensitivity;

    public List<Resolution> Resolutions { get; } = new List<Resolution>();
    public Resolution Native { get; set; }
    public int LastResolutionIndex { get; private set; } = -1;
    public bool? LastFullscreen { get; private set; }

    public IReadOnlyList<Resolution> AvailableResolutions => Resolutions;
    public Resolution NativeResolution => Native;
    public float MouseSensitivity => m_mouseSensitivity;

    public event Action<float> OnMouseSensitivityChanged;

    public void SetFullscreen(bool on) => LastFullscreen = on;

    public void SetResolution(int index) => LastResolutionIndex = index;

    public void SetMouseSensitivity(float v)
    {
        m_mouseSensitivity = v;
        OnMouseSensitivityChanged?.Invoke(v);
    }
}
