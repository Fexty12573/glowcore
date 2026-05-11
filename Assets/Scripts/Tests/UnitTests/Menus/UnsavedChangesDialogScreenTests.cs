using System.Reflection;
using GlowCore.UI.Menus;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnsavedChangesDialogScreenTests
{
    private GameObject m_go;
    private UnsavedChangesDialogScreen m_screen;
    private FakeMenuManager m_menuManager;
    private Button m_apply;
    private Button m_discard;
    private Button m_cancel;
    private TextMeshProUGUI m_message;
    private CanvasGroup m_canvasGroup;

    [SetUp]
    public void SetUp()
    {
        m_go = new GameObject("UnsavedDialog");
        m_canvasGroup = m_go.AddComponent<CanvasGroup>();
        m_screen = m_go.AddComponent<UnsavedChangesDialogScreen>();

        m_apply = MakeButton("Apply");
        m_discard = MakeButton("Discard");
        m_cancel = MakeButton("Cancel");

        var messageGo = new GameObject("Message");
        messageGo.transform.SetParent(m_go.transform);
        m_message = messageGo.AddComponent<TextMeshProUGUI>();

        SetField("m_canvasGroup", m_canvasGroup);
        SetField("m_messageText", m_message);
        SetField("m_applyButton", m_apply);
        SetField("m_discardButton", m_discard);
        SetField("m_cancelButton", m_cancel);

        m_menuManager = new FakeMenuManager();
        m_screen.Initialize(m_menuManager);
    }

    [TearDown]
    public void TearDown()
    {
        if (m_go != null)
            Object.DestroyImmediate(m_go);
    }

    [Test]
    public void SetArgs_AppliesMessage()
    {
        m_screen.SetArgs(new UnsavedChangesDialogArgs { Message = "Save?", OnApply = null, OnDiscard = null });
        Assert.AreEqual("Save?", m_message.text);
    }

    [Test]
    public void SetArgs_IgnoresEmptyMessage_LeavingDefault()
    {
        m_message.text = "default";
        m_screen.SetArgs(new UnsavedChangesDialogArgs { Message = null });
        Assert.AreEqual("default", m_message.text);
    }

    [Test]
    public void ApplyButton_InvokesApplyCallback_AndCallsBack()
    {
        var applyFired = false;
        var discardFired = false;
        m_screen.SetArgs(new UnsavedChangesDialogArgs
        {
            OnApply = () => applyFired = true,
            OnDiscard = () => discardFired = true,
        });

        m_apply.onClick.Invoke();

        Assert.IsTrue(applyFired);
        Assert.IsFalse(discardFired);
        Assert.AreEqual(1, m_menuManager.BackCallCount);
    }

    [Test]
    public void DiscardButton_InvokesDiscardCallback_AndCallsBack()
    {
        var applyFired = false;
        var discardFired = false;
        m_screen.SetArgs(new UnsavedChangesDialogArgs
        {
            OnApply = () => applyFired = true,
            OnDiscard = () => discardFired = true,
        });

        m_discard.onClick.Invoke();

        Assert.IsTrue(discardFired);
        Assert.IsFalse(applyFired);
        Assert.AreEqual(1, m_menuManager.BackCallCount);
    }

    [Test]
    public void CancelButton_InvokesNoCallback_AndCallsBack()
    {
        var applyFired = false;
        var discardFired = false;
        m_screen.SetArgs(new UnsavedChangesDialogArgs
        {
            OnApply = () => applyFired = true,
            OnDiscard = () => discardFired = true,
        });

        m_cancel.onClick.Invoke();

        Assert.IsFalse(applyFired);
        Assert.IsFalse(discardFired);
        Assert.AreEqual(1, m_menuManager.BackCallCount);
    }

    [Test]
    public void ApplyButton_DoesNotReinvokeAfterClick()
    {
        var applyCallCount = 0;
        m_screen.SetArgs(new UnsavedChangesDialogArgs { OnApply = () => applyCallCount++ });

        m_apply.onClick.Invoke();
        m_apply.onClick.Invoke();

        Assert.AreEqual(1, applyCallCount, "callbacks should clear after first invoke to prevent stale firing");
    }

    private Button MakeButton(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(m_go.transform);
        return go.AddComponent<Button>();
    }

    private void SetField(string fieldName, Object value)
    {
        FieldInfo field = typeof(UnsavedChangesDialogScreen).GetField(fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.SetValue(m_screen, value);
    }
}
