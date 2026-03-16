using UnityEngine;

namespace GlowCore.World
{
    [RequireComponent(typeof(Node))]
    [RequireComponent(typeof(Collider))]
    public class Fire : MonoBehaviour
    {
        // Instance Fields
        [SerializeField] private int m_expandPerWood = 1;
        private WorldGrid m_worldGrid;
        private int m_totalWoodReceived;

        // Properties
        public int TotalWoodReceived => m_totalWoodReceived;

        // Private Methods
        private void Awake()
        {
            m_worldGrid = FindFirstObjectByType<WorldGrid>();
        }

        // Public Methods
        public void FeedWood(int amount)
        {
            if (amount <= 0)
                return;

            m_totalWoodReceived += amount;
            m_worldGrid.Expand(amount * m_expandPerWood);
        }
    }
}
