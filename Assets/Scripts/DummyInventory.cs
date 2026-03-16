using UnityEngine;

namespace GlowCore.Player
{
    public class DummyInventory : MonoBehaviour, IInventory
    {
        // Instance Fields
        [SerializeField] private int m_woodCount = 10;

        // Properties
        public int WoodCount => m_woodCount;

        // Public Methods
        public void AddWood(int amount)
        {
            m_woodCount += amount;
        }

        public int RemoveAllWood()
        {
            int wood = m_woodCount;
            m_woodCount = 0;
            return wood;
        }
    }
}
