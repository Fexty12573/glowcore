using System.Collections;
using GlowCore.World;
using UnityEngine;

public class MiningPotion : Consumable
{
    private static Coroutine s_activeRoutine;

    [SerializeField] private float m_breakMultiplier = 0.5f; // smaller => Nodes break faster
    [SerializeField] private float m_duration = 300f;

    protected override void ApplyConsumeEffect()
    {
        if (NodeActionSystem.Instance is null)
            return;

        if (s_activeRoutine != null)
            NodeActionSystem.Instance.StopCoroutine(s_activeRoutine);

        NodeActionSystem.Instance.PotionBreakMultiplier = m_breakMultiplier;
        s_activeRoutine = NodeActionSystem.Instance?.StartCoroutine(RemoveEffect());
    }

    private IEnumerator RemoveEffect()
    {
        yield return new WaitForSeconds(m_duration);
        NodeActionSystem.Instance.PotionBreakMultiplier = 1f;
        s_activeRoutine = null;
    }
}
