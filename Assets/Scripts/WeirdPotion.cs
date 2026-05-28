using System.Collections;
using UnityEngine;

public class WeirdPotion : Consumable
{
    private static Coroutine s_activeRoutine;

    [SerializeField] private float m_playerMovementSmoothingEffect = 2f;
    [SerializeField] private float m_playerCameraFollowDelay = 3f;
    [SerializeField] private float m_duration = 50f;

    private static float m_defaultPlayerSmoothing;
    private static float m_defaultCameraFollowDelay;
    protected override void ApplyConsumeEffect()
    {
        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        if (movement is null)
            return;

        if (s_activeRoutine != null)
        {
            movement.StopCoroutine(s_activeRoutine);
        }
        else
        {
            m_defaultPlayerSmoothing = movement.Smoothing;
            m_defaultCameraFollowDelay = movement.CameraFollowDelay;
        }

        movement.Smoothing = m_playerMovementSmoothingEffect;
        movement.CameraFollowDelay = m_playerCameraFollowDelay;
        s_activeRoutine = movement.StartCoroutine(RemoveEffect(movement));
    }

    private IEnumerator RemoveEffect(PlayerMovement movement)
    {
        yield return new WaitForSeconds(m_duration);
        movement.Smoothing = m_defaultPlayerSmoothing;
        movement.CameraFollowDelay = m_defaultCameraFollowDelay;
        s_activeRoutine = null;
    }
}
