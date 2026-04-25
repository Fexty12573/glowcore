using UnityEngine;

public class SpeedPotion : Consumable
{
    [SerializeField] private float m_speedBoostFactor = 1;
    protected override void ApplyConsumeEffect()
    {
        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        movement?.MultiplyMovementSpeed(m_speedBoostFactor);
    }
}
