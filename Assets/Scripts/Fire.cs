using UnityEngine;

namespace GlowCore.World
{
    [RequireComponent(typeof(Node))]
    [RequireComponent(typeof(Collider))]
    public class Fire : MonoBehaviour
    {
        // Instance Fields
        [SerializeField] private GlowCoreObject m_glowCore;
        [SerializeField] [Min(1)] private int m_woodPerExpansion = 1;
        [SerializeField] [Min(1)] private int m_regularExpansionSize = 1;
        [SerializeField] [Min(1)] private int m_levelUpExpansionSize = 5;
        [Header("Fire VFX")]
        [SerializeField] private ParticleSystem m_fireParticles;
        [SerializeField] private float m_minFireScale = 0.5f;
        [SerializeField] private float m_maxFireScale = 3f;
        [SerializeField] private int m_woodForMaxFire = 50;
        private int m_totalWoodReceived;
        private int m_woodAccumulatedForExpansion;

        // Properties
        public int TotalWoodReceived => m_totalWoodReceived;

        // Public Methods
        public void FeedWood(int amount)
        {
            if (amount <= 0)
                return;

            m_totalWoodReceived += amount;
            m_woodAccumulatedForExpansion += amount;

            while (m_woodAccumulatedForExpansion >= m_woodPerExpansion)
            {
                m_woodAccumulatedForExpansion -= m_woodPerExpansion;
                WorldGrid.Instance.Expand(m_regularExpansionSize);

            }

            if (m_glowCore != null && m_glowCore.FeedWood(amount))
                WorldGrid.Instance.Expand(m_levelUpExpansionSize, isLevelUp: true);

             UpdateFireScale();
        }

        // Private Methods
        private void UpdateFireScale()
        {
            if (m_fireParticles == null)
                return;

            float t = Mathf.Clamp01((float)m_totalWoodReceived / m_woodForMaxFire);
            float scale = Mathf.Lerp(m_minFireScale, m_maxFireScale, t);
            m_fireParticles.transform.localScale = Vector3.one * scale;
        }

        [Header("Debug")]
        [SerializeField] [Min(1)] private int m_debugWoodAmount = 1;

        [ContextMenu("Debug: Feed Wood")]
        private void DebugFeedWood()
        {
            FeedWood(m_debugWoodAmount);
        }
    }
}
