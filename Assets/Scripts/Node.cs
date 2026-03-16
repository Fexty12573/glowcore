using UnityEngine;

namespace GlowCore.World
{
    public class Node : MonoBehaviour
    {
        // Instance Fields
        private Vector2Int m_gridPosition;

        // Properties
        public Vector2Int GridPosition => m_gridPosition;

        // Public Methods
        public void SetGridPosition(Vector2Int position)
        {
            m_gridPosition = position;
        }
    }
}
