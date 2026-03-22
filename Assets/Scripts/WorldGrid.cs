using System.Collections.Generic;
using UnityEngine;

namespace GlowCore.World
{
    public enum WorldMode
    {
        ProceduralGeneration,
        DesignedWorld
    }

    public class WorldGrid : MonoBehaviour
    {
        // Constants
        private const int kInitialSize = 5;
        private const float kBorderHeight = 100f;
        private const float kBorderFogDepth = 100f;

        // Static Fields
        private static WorldGrid s_instance;

        // Instance Fields
        [Header("World Mode")]
        [SerializeField] private WorldMode m_worldMode = WorldMode.ProceduralGeneration;

        [Header("Grid")]
        [SerializeField] private int m_gridSize = kInitialSize;

        [Header("Borders")]
        [SerializeField] private Transform m_borderNorth;
        [SerializeField] private Transform m_borderSouth;
        [SerializeField] private Transform m_borderEast;
        [SerializeField] private Transform m_borderWest;

        [Header("Trees")]
        [SerializeField] private GameObject m_treePrefab;
        [SerializeField] private Transform m_nodesParent;

        [Header("Tree Expansion (ProceduralGeneration only)")]
        [SerializeField] [Min(0)] private int m_initialTreeCount = 10;
        [SerializeField] [Min(0)] private int m_treesPerRegularExpansion = 5;
        [SerializeField] [Min(0)] private int m_treesPerLevelUpExpansion = 20;

        private Node[,] m_tiles;
        private Vector2Int m_origin;
        private int m_totalTreeCount;
        private readonly List<Node> m_pendingNodes = new();

        // Properties
        public static WorldGrid Instance => s_instance;
        public WorldMode Mode => m_worldMode;
        public int GridSize => m_gridSize;
        public int TotalTileCount => m_tiles.Length;
        public int TotalTreeCount => m_totalTreeCount;

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

        public Node PlaceTree(int x, int z)
        {
            if (m_treePrefab == null)
            {
                Debug.LogError("WorldGrid: m_treePrefab is not assigned.");
                return null;
            }

            if (GetNodeAt(x, z) != null)
                return null;

            Transform parent = m_nodesParent != null ? m_nodesParent : transform;
            GameObject treeObject = Instantiate(m_treePrefab, new Vector3(x, 0f, z), Quaternion.identity, parent);

            if (!treeObject.TryGetComponent(out Node node))
            {
                Debug.LogError("WorldGrid: Tree prefab does not have a TreeNode component.");
                Destroy(treeObject);
                return null;
            }

            SetNodeAt(x, z, node);
            m_totalTreeCount++;
            return node;
        }

        public void SpawnRandomTrees(float density)
        {
            density = Mathf.Clamp01(density);

            for (int x = 0; x < m_gridSize; x++)
            {
                for (int z = 0; z < m_gridSize; z++)
                {
                    if (x == m_origin.x && z == m_origin.y)
                        continue;

                    if (Random.value <= density)
                    {
                        Vector2Int worldPos = GridToWorld(x, z);
                        PlaceTree(worldPos.x, worldPos.y);
                    }
                }
            }
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

        public void Expand(int amount, bool isLevelUp = false)
        {
            int newSize = m_gridSize + amount * 2;
            Node[,] newTiles = new Node[newSize, newSize];
            Vector2Int newOrigin = new(newSize / 2, newSize / 2);

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

            int oldSize = m_gridSize;
            m_tiles = newTiles;
            m_gridSize = newSize;
            m_origin = newOrigin;

            UpdateBorders();
            RegisterPendingNodes();

            if (m_worldMode == WorldMode.ProceduralGeneration)
            {
                int count = isLevelUp ? m_treesPerLevelUpExpansion : m_treesPerRegularExpansion;
                SpawnTreesOnNewRing(oldSize, count);
            }
        }


        // public void ClearAllTrees()
        // {
        //     ClearTreesInGrid();
        //
        //     // Also destroy any TreeNode children of the nodes parent that slipped through
        //     // (e.g. pending nodes outside the grid bounds, or designer-placed trees)
        //     Transform root = m_nodesParent != null ? m_nodesParent : transform;
        //     foreach (TreeNode tree in root.GetComponentsInChildren<TreeNode>())
        //         Destroy(tree.gameObject);
        //
        //     // Remove destroyed entries from the pending list
        //     m_pendingNodes.RemoveAll(n => n == null || n.TryGetComponent(out TreeNode _));
        // }

        // Private Methods
        private void Awake()
        {
            if (s_instance != null)
            {
                Debug.LogWarning("WorldGrid: Duplicate instance detected. Destroying this one.");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            Initialize();

            switch (m_worldMode)
            {
                case WorldMode.ProceduralGeneration:
                    InitializeProceduralWorld();
                    break;
                case WorldMode.DesignedWorld:
                    InitializeDesignedWorld();
                    break;
            }

            LogGrid();
            UpdateBorders();
        }

        private void InitializeProceduralWorld()
        {
            RegisterExistingNodes();
            // ClearTreesInGrid();
            SpawnTrees(m_initialTreeCount);
        }

        private void InitializeDesignedWorld()
        {
            RegisterExistingNodes();
        }

        private void UpdateBorders()
        {
            float halfSize = m_gridSize / 2f;
            float center = halfSize + kBorderFogDepth / 2f;
            float fullWidth = m_gridSize + kBorderFogDepth * 2f;

            // North/South: inner face sits exactly at the grid edge, extends outward by kBorderFogDepth.
            // Width is padded by kBorderFogDepth on each side to cover the corners.
            Vector3 borderScaleNS = new(fullWidth, kBorderHeight, kBorderFogDepth);
            m_borderNorth.position = new Vector3(0f, kBorderHeight / 2f, center);
            m_borderNorth.localScale = borderScaleNS;

            m_borderSouth.position = new Vector3(0f, kBorderHeight / 2f, -center);
            m_borderSouth.localScale = borderScaleNS;

            // East/West: same principle on the X axis.
            Vector3 borderScaleEW = new(kBorderFogDepth, kBorderHeight, fullWidth);
            m_borderEast.position = new Vector3(center, kBorderHeight / 2f, 0f);
            m_borderEast.localScale = borderScaleEW;

            m_borderWest.position = new Vector3(-center, kBorderHeight / 2f, 0f);
            m_borderWest.localScale = borderScaleEW;

        }

        private void LogGrid()
        {
            System.Text.StringBuilder sb = new();
            sb.AppendLine($"WorldGrid ({m_gridSize}x{m_gridSize}) [{m_worldMode}]:");

            for (int z = m_gridSize - 1; z >= 0; z--)
            {
                for (int x = 0; x < m_gridSize; x++)
                {
                    Node node = m_tiles[x, z];
                    sb.Append(node != null ? $"[{node.name}]" : "[ . ]");
                }

                sb.Append($"  worldZ={z - m_origin.y}");
                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }

        // private void ClearTreesInGrid()
        // {
        //     for (int x = 0; x < m_gridSize; x++)
        //     {
        //         for (int z = 0; z < m_gridSize; z++)
        //         {
        //             if (m_tiles[x, z] == null || !m_tiles[x, z].TryGetComponent(out Node _))
        //                 continue;
        //
        //             Destroy(m_tiles[x, z].gameObject);
        //             m_tiles[x, z] = null;
        //         }
        //     }
        //
        //     m_totalTreeCount = 0;
        // }

        private void SpawnTrees(int count)
        {
            List<Vector2Int> freeCells = CollectFreeCells();
            Shuffle(freeCells);

            int toSpawn = Mathf.Min(count, freeCells.Count);
            for (int i = 0; i < toSpawn; i++)
                PlaceTree(freeCells[i].x, freeCells[i].y);
        }

        private void SpawnTreesOnNewRing(int oldSize, int count)
        {
            if (count <= 0)
                return;

            int offset = (m_gridSize - oldSize) / 2;
            List<Vector2Int> ringCells = new();

            for (int x = 0; x < m_gridSize; x++)
            {
                for (int z = 0; z < m_gridSize; z++)
                {
                    bool wasInOldGrid = x >= offset && x < offset + oldSize
                                    && z >= offset && z < offset + oldSize;
                    if (!wasInOldGrid && m_tiles[x, z] == null)
                        ringCells.Add(GridToWorld(x, z));
                }
            }

            Shuffle(ringCells);

            int toSpawn = Mathf.Min(count, ringCells.Count);
            for (int i = 0; i < toSpawn; i++)
                PlaceTree(ringCells[i].x, ringCells[i].y);
        }

        private List<Vector2Int> CollectFreeCells()
        {
            List<Vector2Int> freeCells = new();

            for (int x = 0; x < m_gridSize; x++)
            {
                for (int z = 0; z < m_gridSize; z++)
                {
                    if (m_tiles[x, z] == null)
                        freeCells.Add(GridToWorld(x, z));
                }
            }

            return freeCells;
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private void RegisterExistingNodes()
        {
            Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            foreach (Node node in nodes)
            {
                int worldX = Mathf.RoundToInt(node.transform.position.x);
                int worldZ = Mathf.RoundToInt(node.transform.position.z);
                Vector2Int gridIndex = WorldToGrid(worldX, worldZ);

                if (!IsInBounds(gridIndex))
                {
                    m_pendingNodes.Add(node);
                    continue;
                }

                if (GetNodeAt(worldX, worldZ) != null)
                {
                    Debug.LogError($"WorldGrid: Duplicate node at ({worldX}, {worldZ}). '{node.name}' will be destroyed.");
                    Destroy(node.gameObject);
                    continue;
                }

                SetNodeAt(worldX, worldZ, node);
            }
        }

        private void RegisterPendingNodes()
        {
            for (int i = m_pendingNodes.Count - 1; i >= 0; i--)
            {
                Node node = m_pendingNodes[i];

                if (node == null)
                {
                    m_pendingNodes.RemoveAt(i);
                    continue;
                }

                int worldX = Mathf.RoundToInt(node.transform.position.x);
                int worldZ = Mathf.RoundToInt(node.transform.position.z);
                Vector2Int gridIndex = WorldToGrid(worldX, worldZ);

                if (!IsInBounds(gridIndex))
                    continue;

                // In procedural mode, trees outside the initial grid are replaced by the
                // ring spawner as the grid expands over them.
                if (m_worldMode == WorldMode.ProceduralGeneration && node.TryGetComponent(out Node _))
                {
                    Destroy(node.gameObject);
                    m_pendingNodes.RemoveAt(i);
                    continue;
                }

                if (GetNodeAt(worldX, worldZ) != null)
                {
                    Debug.LogError($"WorldGrid: Duplicate node at ({worldX}, {worldZ}). '{node.name}' will be destroyed.");
                    Destroy(node.gameObject);
                    m_pendingNodes.RemoveAt(i);
                    continue;
                }

                SetNodeAt(worldX, worldZ, node);
                m_pendingNodes.RemoveAt(i);
            }
        }
    }
}
