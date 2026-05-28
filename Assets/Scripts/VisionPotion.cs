using System.Collections;
using UnityEngine;

public class VisionPotion : Consumable
{
    private static Coroutine s_activeRoutine;
    private static float s_defaultZoom;

    [SerializeField] private float m_zoomWhenBoosted = 5f;
    [SerializeField] private float m_duration = 300f;

    protected override void ApplyConsumeEffect()
    {
        PlayerCamera playerCamera = FindFirstObjectByType<PlayerCamera>();
        if (playerCamera is null)
            return;

        if (s_activeRoutine != null)
            playerCamera.StopCoroutine(s_activeRoutine);
        else
            s_defaultZoom = playerCamera.MainCamera.orthographicSize;

        playerCamera.MainCamera.orthographicSize = m_zoomWhenBoosted;
        playerCamera.Raycastcamera.orthographicSize = m_zoomWhenBoosted;
        s_activeRoutine = playerCamera.StartCoroutine(RemoveEffect(playerCamera));
    }

    private IEnumerator RemoveEffect(PlayerCamera playerCamera)
    {
        yield return new WaitForSeconds(m_duration);
        playerCamera.MainCamera.orthographicSize = s_defaultZoom;
        playerCamera.Raycastcamera.orthographicSize = s_defaultZoom;
        s_activeRoutine = null;
    }
}
