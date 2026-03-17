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
        }

        // Private Methods
        [ContextMenu("Debug: Feed 1 Wood")]
        private void DebugFeedWood()
        {
            FeedWood(1);
        }
    }
}
