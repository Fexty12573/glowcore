using GlowCore.UI.Menus;
using NUnit.Framework;
using UnityEngine;

public class SettingsEscapeConsumerTests
{
    private GameObject m_settingsGo;
    private SettingsScreen m_settings;
    private FakeMenuManager m_menuManager;
    private SettingsEscapeConsumer m_consumer;

    [SetUp]
    public void SetUp()
    {
        m_settingsGo = new GameObject("Settings");
        m_settings = m_settingsGo.AddComponent<SettingsScreen>();
        m_menuManager = new FakeMenuManager();
        m_consumer = new SettingsEscapeConsumer(m_menuManager, m_settings);

        // No categories injected → IsDirty() is false → TryLeave goes through the clean-Back path.
        m_settings.Initialize(m_menuManager, null, null, null);
    }

    [TearDown]
    public void TearDown()
    {
        if (m_settingsGo != null)
            Object.DestroyImmediate(m_settingsGo);
    }

    [Test]
    public void Priority_IsHigherThanPauseConsumer()
    {
        Assert.Greater(m_consumer.Priority, PauseEscapeConsumer.kPriority,
            "settings consumer must run before pause so Esc on settings triggers TryLeave");
    }

    [Test]
    public void TryConsumeEscape_ReturnsFalse_WhenCurrentIsNull()
    {
        m_menuManager.Current = null;
        Assert.IsFalse(m_consumer.TryConsumeEscape());
    }

    [Test]
    public void TryConsumeEscape_ReturnsFalse_WhenCurrentIsNotSettings()
    {
        m_menuManager.Current = new FakeMenuScreen(MenuScreenId.Pause);
        Assert.IsFalse(m_consumer.TryConsumeEscape());
        Assert.AreEqual(0, m_menuManager.BackCallCount, "non-settings escape must not be consumed");
    }

    [Test]
    public void TryConsumeEscape_ReturnsTrue_AndRoutesToTryLeave_WhenCurrentIsSettings()
    {
        m_menuManager.Current = new FakeMenuScreen(MenuScreenId.Settings);

        Assert.IsTrue(m_consumer.TryConsumeEscape());
        Assert.AreEqual(1, m_menuManager.BackCallCount,
            "clean settings (no dirty values) should immediately Back");
    }

    [Test]
    public void TryConsumeEscape_ReturnsFalse_WhenMenuManagerIsNull()
    {
        var consumer = new SettingsEscapeConsumer(null, m_settings);
        Assert.IsFalse(consumer.TryConsumeEscape());
    }

    [Test]
    public void TryConsumeEscape_ReturnsFalse_WhenSettingsScreenIsNull()
    {
        var consumer = new SettingsEscapeConsumer(m_menuManager, null);
        Assert.IsFalse(consumer.TryConsumeEscape());
    }
}
