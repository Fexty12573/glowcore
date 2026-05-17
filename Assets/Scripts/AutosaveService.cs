using System;
using System.Collections;
using UnityEngine;

public class AutosaveService : MonoBehaviour
{
    private ISaveService m_saveService;
    private TimeSpan m_saveInterval;
    private Coroutine m_autosaveCoroutine;

    public void Initialize(ISaveService saveService, TimeSpan saveInterval)
    {
        m_saveService = saveService;
        m_saveInterval = saveInterval;
        if (enabled && m_autosaveCoroutine == null)
            m_autosaveCoroutine = StartCoroutine(AutosaveLoop());
    }

    private void OnEnable()
    {
        if (m_saveService != null)
            m_autosaveCoroutine = StartCoroutine(AutosaveLoop());
    }

    private void OnDisable()
    {
        if (m_autosaveCoroutine != null)
        {
            StopCoroutine(m_autosaveCoroutine);
            m_autosaveCoroutine = null;
        }
    }

    private IEnumerator AutosaveLoop()
    {
        var wait = new WaitForSeconds((float)m_saveInterval.TotalSeconds);

        while (true)
        {
            yield return wait;
            m_saveService.Save();
        }
    }
}
