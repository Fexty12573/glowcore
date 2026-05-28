using System.Collections;
using UnityEngine;

public class WeirdPotion : Consumable
{
    private static Coroutine s_activeRoutine;
    private static float s_defaultPlayerSmoothing;
    private static float s_defaultCameraFollowDelay;

    [SerializeField] private float m_playerMovementSmoothingEffect = 1f;
    [SerializeField] private float m_playerCameraFollowDelay = 3f;
    [SerializeField] private float m_duration = 50f;

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
            s_defaultPlayerSmoothing = movement.Smoothing;
            s_defaultCameraFollowDelay = movement.CameraFollowDelay;
        }

        movement.Smoothing = m_playerMovementSmoothingEffect;
        movement.CameraFollowDelay = m_playerCameraFollowDelay;
        s_activeRoutine = movement.StartCoroutine(RemoveEffect(movement));
    }

    private IEnumerator RemoveEffect(PlayerMovement movement)
    {
        yield return new WaitForSeconds(m_duration);
        movement.Smoothing = s_defaultPlayerSmoothing;
        movement.CameraFollowDelay = s_defaultCameraFollowDelay;
        s_activeRoutine = null;
    }
}
