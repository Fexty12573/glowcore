using UnityEngine;

namespace GlowCore.World
{
    public class WorldGrid : MonoBehaviour
    {
        // Constants
        private const int kInitialSize = 5;
        private const float kBorderHeight = 5f;
        private const float kBorderThickness = 0.001f;

        // Instance Fields
        [SerializeField] private int m_gridSize = kInitialSize;
        [SerializeField] private Transform m_borderNorth;
        [SerializeField] private Transform m_borderSouth;
        [SerializeField] private Transform m_borderEast;
        [SerializeField] private Transform m_borderWest;
        private Node[,] m_tiles;
        private Vector2Int m_origin;

        // Properties
        public int GridSize => m_gridSize;

        // Public Methods
        public void Initialize()
        {
            m_tiles = new Node[m_gridSize, m_gridSize];
            m_origin = new Vector2Int(m_gridSize / 2, m_gridSize / 2);
        }

        public Node GetNodeAt(int x, int z)
        {
            Vector2Int index = WorldToGrid(x, z);
            if (!IsInBounds(index))
                return null;

            return m_tiles[index.x, index.y];
        }

        public bool SetNodeAt(int x, int z, Node node)
        {
            Vector2Int index = WorldToGrid(x, z);
            if (!IsInBounds(index))
                return false;

            m_tiles[index.x, index.y] = node;
            return true;
        }

        public bool IsInBounds(Vector2Int index)
        {
            return index.x >= 0 && index.x < m_gridSize
                && index.y >= 0 && index.y < m_gridSize;
        }

        public Vector2Int WorldToGrid(int worldX, int worldZ)
        {
            return new Vector2Int(worldX + m_origin.x, worldZ + m_origin.y);
        }

        public Vector2Int GridToWorld(int gridX, int gridZ)
        {
            return new Vector2Int(gridX - m_origin.x, gridZ - m_origin.y);
        }

        public void Expand(int amount)
        {
            int newSize = m_gridSize + amount * 2;
            Node[,] newTiles = new Node[newSize, newSize];
            Vector2Int newOrigin = new Vector2Int(newSize / 2, newSize / 2);

            for (int x = 0; x < m_gridSize; x++)
            {
                for (int z = 0; z < m_gridSize; z++)
                {
                    if (m_tiles[x, z] == null)
                        continue;

                    int newX = x + (newOrigin.x - m_origin.x);
                    int newZ = z + (newOrigin.y - m_origin.y);
                    newTiles[newX, newZ] = m_tiles[x, z];
                }
            }

            m_tiles = newTiles;
            m_gridSize = newSize;
            m_origin = newOrigin;
            UpdateBorders();
        }

        // Private Methods
        private void Awake()
        {
            Initialize();
            RegisterExistingNodes();
            UpdateBorders();
        }

        private void UpdateBorders()
        {
            float halfSize = m_gridSize / 2f;
            Vector3 borderScale = new Vector3(m_gridSize, kBorderHeight, kBorderThickness);
            Vector3 borderScaleSide = new Vector3(kBorderThickness, kBorderHeight, m_gridSize);

            m_borderNorth.position = new Vector3(0f, kBorderHeight / 2f, halfSize + kBorderThickness / 2f);
            m_borderNorth.localScale = borderScale;

            m_borderSouth.position = new Vector3(0f, kBorderHeight / 2f, -(halfSize + kBorderThickness / 2f));
            m_borderSouth.localScale = borderScale;

            m_borderEast.position = new Vector3(halfSize + kBorderThickness / 2f, kBorderHeight / 2f, 0f);
            m_borderEast.localScale = borderScaleSide;

            m_borderWest.position = new Vector3(-(halfSize + kBorderThickness / 2f), kBorderHeight / 2f, 0f);
            m_borderWest.localScale = borderScaleSide;
        }

        [ContextMenu("Expand Grid")]
        private void DebugExpand()
        {
            Expand(1);
        }

        private void RegisterExistingNodes()
        {
            Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            foreach (Node node in nodes)
            {
                int worldX = Mathf.RoundToInt(node.transform.position.x);
                int worldZ = Mathf.RoundToInt(node.transform.position.z);
                node.SetGridPosition(new Vector2Int(worldX, worldZ));
                SetNodeAt(worldX, worldZ, node);
            }
        }

    }
}
