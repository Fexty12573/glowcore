using System.Linq;
using GlowCore.UI.Menus;
using NUnit.Framework;
using UnityEngine;

public class DisplaySettingsCategoryTests
{
    private FakePlayerPrefsBackend m_backend;
    private PlayerPrefsSettingsRepository m_repository;
    private FakeDisplayService m_display;
    private DisplaySettingsCategory m_category;

    [SetUp]
    public void SetUp()
    {
        m_backend = new FakePlayerPrefsBackend();
        m_repository = new PlayerPrefsSettingsRepository(m_backend);
        m_display = new FakeDisplayService();
        m_display.Resolutions.Add(new Resolution { width = 1280, height = 720 });
        m_display.Resolutions.Add(new Resolution { width = 1920, height = 1080 });
        m_display.Resolutions.Add(new Resolution { width = 2880, height = 1800 });
        m_display.Native = new Resolution { width = 2880, height = 1800 };
        m_category = new DisplaySettingsCategory(m_repository, m_display);
    }

    [Test]
    public void DisplayName_IsDisplay()
    {
        Assert.AreEqual("DISPLAY", m_category.DisplayName);
    }

    [Test]
    public void Descriptors_AreOrderedFullscreenResolutionSensitivity()
    {
        Assert.AreEqual(3, m_category.Descriptors.Count);
        Assert.AreEqual(DisplaySettingsCategory.kKeyFullscreen, m_category.Descriptors[0].Key);
        Assert.AreEqual(SettingControlKind.Toggle, m_category.Descriptors[0].Kind);
        Assert.AreEqual(DisplaySettingsCategory.kKeyResolution, m_category.Descriptors[1].Key);
        Assert.AreEqual(SettingControlKind.Dropdown, m_category.Descriptors[1].Kind);
        Assert.AreEqual(DisplaySettingsCategory.kKeyMouseSensitivity, m_category.Descriptors[2].Key);
        Assert.AreEqual(SettingControlKind.Slider, m_category.Descriptors[2].Kind);
    }

    [Test]
    public void ResolutionDropdown_MarksNativeWithRecommendedSuffix()
    {
        var dropdown = Dropdown();
        Assert.AreEqual("1280 x 720", dropdown.Options[0]);
        Assert.AreEqual("1920 x 1080", dropdown.Options[1]);
        Assert.AreEqual("2880 x 1800" + DisplaySettingsCategory.kRecommendedSuffix, dropdown.Options[2]);
    }

    [Test]
    public void ResolutionDropdown_AddsNoSuffix_WhenNativeIsNotInList()
    {
        m_display.Native = new Resolution { width = 9999, height = 9999 };
        var category = new DisplaySettingsCategory(m_repository, m_display);
        var dropdown = category.Descriptors.First(d => d.Key == DisplaySettingsCategory.kKeyResolution);

        foreach (var option in dropdown.Options)
            Assert.IsFalse(option.Contains(DisplaySettingsCategory.kRecommendedSuffix), $"unexpected suffix on '{option}'");
    }

    [Test]
    public void FullscreenWrite_PersistsAndUpdatesDisplay()
    {
        Fullscreen().Write(true);

        Assert.IsTrue(m_repository.GetBool(DisplaySettingsCategory.kKeyFullscreen, false));
        Assert.AreEqual(true, m_display.LastFullscreen);
    }

    [Test]
    public void FullscreenRead_ReturnsSavedValue()
    {
        m_repository.SetBool(DisplaySettingsCategory.kKeyFullscreen, true);
        Assert.AreEqual(true, (bool)Fullscreen().Read());
    }

    [Test]
    public void ResolutionWrite_PersistsAndUpdatesDisplay()
    {
        Dropdown().Write(1);

        Assert.AreEqual(1, m_repository.GetInt(DisplaySettingsCategory.kKeyResolution, -1));
        Assert.AreEqual(1, m_display.LastResolutionIndex);
    }

    [Test]
    public void ResolutionRead_ReturnsSavedIndex()
    {
        m_repository.SetInt(DisplaySettingsCategory.kKeyResolution, 2);
        Assert.AreEqual(2, (int)Dropdown().Read());
    }

    [Test]
    public void MouseSensitivityWrite_PersistsAndUpdatesDisplay()
    {
        Sensitivity().Write(0.42f);

        Assert.AreEqual(0.42f, m_repository.GetFloat(DisplaySettingsCategory.kKeyMouseSensitivity, -1f));
        Assert.AreEqual(0.42f, m_display.MouseSensitivity);
    }

    [Test]
    public void MouseSensitivityRead_FallsBackToDefault_WhenUnset()
    {
        Assert.AreEqual(DisplayService.kSensitivityDefaultSlider, (float)Sensitivity().Read());
    }

    [Test]
    public void MouseSensitivityRead_ReturnsSavedValue()
    {
        m_repository.SetFloat(DisplaySettingsCategory.kKeyMouseSensitivity, 0.7f);
        Assert.AreEqual(0.7f, (float)Sensitivity().Read());
    }

    private SettingDescriptor Fullscreen() =>
        m_category.Descriptors.First(d => d.Key == DisplaySettingsCategory.kKeyFullscreen);

    private SettingDescriptor Dropdown() =>
        m_category.Descriptors.First(d => d.Key == DisplaySettingsCategory.kKeyResolution);

    private SettingDescriptor Sensitivity() =>
        m_category.Descriptors.First(d => d.Key == DisplaySettingsCategory.kKeyMouseSensitivity);
}
