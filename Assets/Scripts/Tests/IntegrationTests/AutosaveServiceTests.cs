using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AutosaveServiceTests
{
    private GameObject m_go;

    [SetUp]
    public void SetUp() => m_go = new GameObject();

    [TearDown]
    public void TearDown() => UnityEngine.Object.Destroy(m_go);

    [UnityTest]
    public IEnumerator AutosaveService_CallsSave_AfterInterval()
    {
        var autosave = m_go.AddComponent<AutosaveService>();
        var mock = new MockSaveService();

        autosave.Initialize(mock, TimeSpan.FromSeconds(0.1f));

        yield return new WaitForSeconds(0.15f);

        Assert.AreEqual(1, mock.SaveCallCount);
    }

    [UnityTest]
    public IEnumerator AutosaveService_CallsSave_MultipleTimesOverInterval()
    {
        var autosave = m_go.AddComponent<AutosaveService>();
        var mock = new MockSaveService();

        autosave.Initialize(mock, TimeSpan.FromSeconds(0.1f));

        yield return new WaitForSeconds(0.35f);

        Assert.AreEqual(3, mock.SaveCallCount);
    }

    [UnityTest]
    public IEnumerator AutosaveService_StopsSaving_WhenDisabled()
    {
        var autosave = m_go.AddComponent<AutosaveService>();
        var mock = new MockSaveService();

        autosave.Initialize(mock, TimeSpan.FromSeconds(0.1f));

        yield return new WaitForSeconds(0.15f);

        autosave.enabled = false;

        var countAfterDisable = mock.SaveCallCount;
        yield return new WaitForSeconds(0.2f);

        Assert.AreEqual(countAfterDisable, mock.SaveCallCount);
    }
}
