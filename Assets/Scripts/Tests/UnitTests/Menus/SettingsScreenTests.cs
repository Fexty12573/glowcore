using GlowCore.UI.Menus;
using NUnit.Framework;
using UnityEngine;

public class SettingsScreenTests
{
    private GameObject m_go;
    private SettingsScreen m_screen;
    private FakeMenuManager m_menuManager;
    private FakePlayerPrefsBackend m_backend;
    private PlayerPrefsSettingsRepository m_repository;
    private float m_sliderValue;

    [SetUp]
    public void SetUp()
    {
        m_sliderValue = 0.5f;
        m_go = new GameObject("SettingsScreen");
        m_screen = m_go.AddComponent<SettingsScreen>();

        var descriptor = SettingDescriptor.Slider("test.slider", "TEST", 0f, 1f,
            () => m_sliderValue,
            v => m_sliderValue = v);
        var category = new FakeSettingsCategory(new[] { descriptor });

        m_menuManager = new FakeMenuManager();
        m_backend = new FakePlayerPrefsBackend();
        m_repository = new PlayerPrefsSettingsRepository(m_backend);

        m_screen.Initialize(m_menuManager, m_repository, null, new[] { category });
        m_screen.Show(); // snapshots current values
    }

    [TearDown]
    public void TearDown()
    {
        if (m_go != null)
            Object.DestroyImmediate(m_go);
    }

    [Test]
    public void IsDirty_ReturnsFalse_WhenNothingChanged()
    {
        Assert.IsFalse(m_screen.IsDirty());
    }

    [Test]
    public void IsDirty_ReturnsTrue_WhenValueDiffersFromSnapshot()
    {
        m_sliderValue = 0.9f;
        Assert.IsTrue(m_screen.IsDirty());
    }

    [Test]
    public void IsDirty_ReturnsFalse_WhenValueChangedAndChangedBack()
    {
        m_sliderValue = 0.9f;
        Assert.IsTrue(m_screen.IsDirty());

        m_sliderValue = 0.5f;
        Assert.IsFalse(m_screen.IsDirty());
    }

    [Test]
    public void Show_PreservesSnapshot_AcrossDialogRoundTrip()
    {
        // Dialog round-trips call Show() again; the snapshot must NOT be re-captured,
        // otherwise the dirty value becomes the new "original" and Discard becomes a no-op.
        m_sliderValue = 0.9f;
        Assert.IsTrue(m_screen.IsDirty(), "dirty before reshow");

        m_screen.Show();

        Assert.IsTrue(m_screen.IsDirty(), "snapshot must be preserved across re-Show");
    }

    [Test]
    public void TryLeave_WhenClean_CallsBackImmediately_AndDoesNotOpenDialog()
    {
        m_screen.TryLeave();

        Assert.AreEqual(1, m_menuManager.BackCallCount);
        Assert.AreEqual(MenuScreenId.None, m_menuManager.LastOpenedId, "no dialog when clean");
    }

    [Test]
    public void TryLeave_WhenDirty_OpensUnsavedChangesDialog_WithBothCallbacks()
    {
        m_sliderValue = 0.9f;

        m_screen.TryLeave();

        Assert.AreEqual(MenuScreenId.UnsavedChanges, m_menuManager.LastOpenedId);
        Assert.AreEqual(0, m_menuManager.BackCallCount, "dirty path must not Back directly");

        var args = (UnsavedChangesDialogArgs)m_menuManager.LastOpenedArgs;
        Assert.IsNotNull(args.OnApply, "Apply callback must be wired");
        Assert.IsNotNull(args.OnDiscard, "Discard callback must be wired");
    }

    [Test]
    public void DiscardCallback_RevertsToSnapshot_AndCallsBack()
    {
        m_sliderValue = 0.9f;
        m_screen.TryLeave();
        var args = (UnsavedChangesDialogArgs)m_menuManager.LastOpenedArgs;

        args.OnDiscard();

        Assert.AreEqual(0.5f, m_sliderValue, "value must revert to snapshot");
        Assert.AreEqual(1, m_menuManager.BackCallCount, "Discard must Back to leave Settings");
    }

    [Test]
    public void ApplyCallback_PersistsAndCallsBack()
    {
        m_sliderValue = 0.9f;
        m_screen.TryLeave();
        var args = (UnsavedChangesDialogArgs)m_menuManager.LastOpenedArgs;

        args.OnApply();

        Assert.AreEqual(0.9f, m_sliderValue, "Apply must not revert");
        Assert.AreEqual(1, m_backend.SaveCallCount, "Apply must persist via repository.Save");
        Assert.AreEqual(1, m_menuManager.BackCallCount, "Apply must Back to leave Settings");
    }

    [Test]
    public void Apply_ClearsSnapshot_SoNextShowCapturesFreshValues()
    {
        m_sliderValue = 0.9f;
        m_screen.TryLeave();
        var args = (UnsavedChangesDialogArgs)m_menuManager.LastOpenedArgs;
        args.OnApply();

        // Re-enter settings; the new "original" should be the applied value, not the stale 0.5.
        m_screen.Show();
        m_sliderValue = 0.9f;
        Assert.IsFalse(m_screen.IsDirty(), "applied value should be the new baseline after re-show");
    }

    [Test]
    public void Discard_ClearsSnapshot_SoNextShowCapturesFreshValues()
    {
        m_sliderValue = 0.9f;
        m_screen.TryLeave();
        var args = (UnsavedChangesDialogArgs)m_menuManager.LastOpenedArgs;
        args.OnDiscard();
        // After Discard, m_sliderValue is reverted to 0.5.

        // Re-enter settings, change to a new value, ensure new baseline is captured.
        m_screen.Show();
        Assert.IsFalse(m_screen.IsDirty(), "no changes after re-show");
        m_sliderValue = 0.8f;
        Assert.IsTrue(m_screen.IsDirty(), "fresh snapshot at 0.5 should detect 0.8 as dirty");
    }

    [Test]
    public void CleanTryLeave_ClearsSnapshot_SoNextShowCapturesFreshValues()
    {
        m_screen.TryLeave(); // clean exit

        m_sliderValue = 0.7f;
        m_screen.Show(); // re-enter — should snapshot 0.7 since previous snapshot was cleared

        m_sliderValue = 0.9f;
        Assert.IsTrue(m_screen.IsDirty(), "snapshot should be 0.7, current 0.9 is dirty");

        m_sliderValue = 0.7f;
        Assert.IsFalse(m_screen.IsDirty(), "back to snapshot value");
    }
}
