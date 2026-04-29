using System.Collections;
using GlowCore.World;
using UnityEngine;

public class AutosaveService : MonoBehaviour
{
    [SerializeField] private float m_saveIntervalMinutes = 5f;

    private Coroutine m_autosaveCoroutine;

    private void OnEnable() => m_autosaveCoroutine = StartCoroutine(AutosaveLoop());

    private void OnDisable() => StopCoroutine(m_autosaveCoroutine);

    private IEnumerator AutosaveLoop()
    {
        var wait = new WaitForSeconds(m_saveIntervalMinutes * 60f);

        while (true)
        {
            yield return wait;

            Debug.Log("Saving Game...");
            WorldGrid.Instance.SaveGameData();
        }
    }
}
