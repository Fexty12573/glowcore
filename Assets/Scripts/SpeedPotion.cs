using System.Collections;
using UnityEngine;

public class SpeedPotion : Consumable
{
    private static Coroutine s_activeRoutine;

    [SerializeField] private float m_speedBoostFactor = 2f;
    [SerializeField] private float m_duration = 300f;

    protected override void ApplyConsumeEffect()
    {
        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        if (movement is null)
            return;

        if (s_activeRoutine != null)
            movement.StopCoroutine(s_activeRoutine);

        movement.PotionSpeedMultiplier = m_speedBoostFactor;
        s_activeRoutine = movement.StartCoroutine(RemoveEffect(movement));
    }

    private IEnumerator RemoveEffect(PlayerMovement movement)
    {
        yield return new WaitForSeconds(m_duration);
        movement.PotionSpeedMultiplier = 1f;
        s_activeRoutine = null;
    }
}
