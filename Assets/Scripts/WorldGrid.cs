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

        [System.Serializable]
        private struct SpawnableNode
        {
            public GameObject Prefab;
            [Min(0)] public float Weight;
        }

        // Static Fields
        private static WorldGrid s_instance;

        // Instance Fields
        [Header("General")][SerializeField] private Transform m_player;

        [Header("World Mode")]
        [SerializeField] private WorldMode m_worldMode = WorldMode.ProceduralGeneration;

        [Header("Grid")]
        [SerializeField] private int m_gridSize = kInitialSize;

        [Header("Borders")]
        [SerializeField] private Transform m_borderNorth;
        [SerializeField] private Transform m_borderSouth;
        [SerializeField] private Transform m_borderEast;
        [SerializeField] private Transform m_borderWest;

        [Header("Nodes")]
        [SerializeField] private SpawnableNode[] m_spawnableNodes;
        [SerializeField] private Transform m_nodesParent;

        [Header("Node Expansion (ProceduralGeneration only)")]
        [SerializeField][Min(0)] private int m_initialNodeCount = 10;
        [SerializeField][Min(0)] private int m_nodesPerRegularExpansion = 5;
        [SerializeField][Min(0)] private int m_nodesPerLevelUpExpansion = 20;

        private Node[,] m_tiles;
        private Vector2Int m_origin;
        private int m_totalNodeCount;
        private readonly List<Node> m_pendingNodes = new();

        // Properties
        public static WorldGrid Instance => s_instance;
        public WorldMode Mode => m_worldMode;
        public int GridSize => m_gridSize;
        public int TotalTileCount => m_tiles.Length;
        public int TotalNodeCount => m_totalNodeCount;

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

        public bool PlaceNodeAtTile(Vector2Int tile, Node node)
        {
            if (!IsInBounds(tile) || IsOccupied(tile))
                return false;
            m_tiles[tile.x, tile.y] = node;
            node.TilesUsed.Add(tile);
            return true;
        }

        public bool PlaceNodeAt(Node node, int x, int z)
        {
            Vector2Int index = WorldToGrid(x, z);
            return PlaceNodeAtTile(index, node);
        }

        public void PlaceNodeAt(Node node, int x, int z, int tileCount)
        {
            var side = Mathf.RoundToInt(Mathf.Sqrt(tileCount));
            var start = -(side / 2);
            var end = start + side;
            for (var dx = start; dx < end; dx++)
            {
                for (var dz = start; dz < end; dz++)
                {
                    Vector2Int tile = WorldToGrid(x + dx, z + dz);
                    if (IsInBounds(tile) && !IsOccupied(tile))
                        PlaceNodeAtTile(tile, node);
                }
            }
        }

        public void ClearNodeAt(Vector2Int tile) => m_tiles[tile.x, tile.y] = null;

        public bool CreateNodeAt(GameObject prefab, Vector2Int tile)
        {
            Vector3 spawnPosition = GetSpawnPosition(tile);
            // GC-142: Use prefab's own rotation so placed nodes respect their saved orientation
            GameObject nodeObject = Instantiate(prefab, spawnPosition, prefab.transform.rotation, m_nodesParent);
            if (!nodeObject.TryGetComponent(out Node node) || IsPlayerObstructing(spawnPosition) || !PlaceNodeAtTile(tile, node))
            {
                Destroy(nodeObject);
                return false;
            }
            return true;
        }

        public Vector3 GetSpawnPosition(Vector2Int tile)
        {
            Vector2Int position = GridToWorld(tile);
            return new(position.x, 0, position.y);
        }

        public Node PlaceNode(int x, int z)
        {
            GameObject prefab = PickRandomNodePrefab();
            if (prefab == null)
            {
                Debug.LogError("WorldGrid: No spawnable nodes assigned.");
                return null;
            }

            if (GetNodeAt(x, z) != null)
                return null;

            Transform parent = m_nodesParent ?? transform;
            GameObject nodeObject = Instantiate(prefab, new Vector3(x, 0f, z), Quaternion.identity, parent);

            if (!nodeObject.TryGetComponent(out Node node))
            {
                Debug.LogError($"WorldGrid: Prefab '{prefab.name}' does not have a Node component.");
                Destroy(nodeObject);
                return null;
            }

            PlaceNodeAt(node, x, z);
            m_totalNodeCount++;
            return node;
        }


        public bool IsInBounds(Vector2Int tile)
        {
            return tile.x >= 0 && tile.x < m_gridSize
                && tile.y >= 0 && tile.y < m_gridSize;
        }

        public bool IsOccupied(Vector2Int tile)
        {
            return !IsInBounds(tile) || m_tiles[tile.x, tile.y] is not null;
        }

        public bool IsPlayerObstructing(Vector3 worldPosition)
        {
            return Vector3.Distance(worldPosition, m_player.position) <= 0.9f;
        }

        public Vector2Int WorldToGrid(int worldX, int worldZ)
        {
            return new Vector2Int(worldX + m_origin.x, worldZ + m_origin.y);
        }

        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            return WorldToGrid(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
        }

        public Vector2Int GridToWorld(int gridX, int gridZ)
        {
            return new Vector2Int(gridX - m_origin.x, gridZ - m_origin.y);
        }

        public Vector2Int GridToWorld(Vector2Int tile)
        {
            return GridToWorld(tile.x, tile.y);
        }

        public void Expand(int amount, bool isLevelUp = false)
        {
            var newSize = m_gridSize + amount * 2;
            Node[,] newTiles = new Node[newSize, newSize];
            Vector2Int newOrigin = new(newSize / 2, newSize / 2);

            var offset = new Vector2Int(newOrigin.x - m_origin.x, newOrigin.y - m_origin.y);
            for (var x = 0; x < m_gridSize; x++)
            {
                for (var z = 0; z < m_gridSize; z++)
                {
                    if (m_tiles[x, z] == null)
                        continue;

                    newTiles[x + offset.x, z + offset.y] = m_tiles[x, z];
                }
            }

            var oldSize = m_gridSize;
            m_tiles = newTiles;
            m_gridSize = newSize;
            m_origin = newOrigin;

            UpdateNodeTileIndices(offset);

            UpdateBorders();
            RegisterPendingNodes();

            if (m_worldMode == WorldMode.ProceduralGeneration)
            {
                var count = isLevelUp ? m_nodesPerLevelUpExpansion : m_nodesPerRegularExpansion;
                SpawnNodesOnNewRing(oldSize, count);
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
                Debug.LogError("WorldGrid: Duplicate instance detected. Destroying this one.");
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
            SpawnNodes(m_initialNodeCount);
        }

        private void InitializeDesignedWorld()
        {
            RegisterExistingNodes();
        }

        private void UpdateBorders()
        {
            var halfSize = m_gridSize / 2f;
            var center = halfSize + kBorderFogDepth / 2f;
            var fullWidth = m_gridSize + kBorderFogDepth * 2f;

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

            for (var z = m_gridSize - 1; z >= 0; z--)
            {
                for (var x = 0; x < m_gridSize; x++)
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

        private GameObject PickRandomNodePrefab()
        {
            if (m_spawnableNodes == null || m_spawnableNodes.Length == 0)
                return null;

            var totalWeight = 0f;
            foreach (var entry in m_spawnableNodes)
                totalWeight += entry.Weight;

            if (totalWeight <= 0f)
                return null;

            var roll = Random.Range(0f, totalWeight);
            var cumulative = 0f;
            foreach (var entry in m_spawnableNodes)
            {
                cumulative += entry.Weight;
                if (roll < cumulative)
                    return entry.Prefab;
            }

            return m_spawnableNodes[^1].Prefab;
        }

        private void SpawnNodes(int count)
        {
            List<Vector2Int> freeCells = CollectFreeCells();
            Shuffle(freeCells);

            var toSpawn = Mathf.Min(count, freeCells.Count);
            for (var i = 0; i < toSpawn; i++)
                PlaceNode(freeCells[i].x, freeCells[i].y);
        }

        private void SpawnNodesOnNewRing(int oldSize, int count)
        {
            if (count <= 0)
                return;

            var offset = (m_gridSize - oldSize) / 2;
            List<Vector2Int> ringCells = new();

            for (var x = 0; x < m_gridSize; x++)
            {
                for (var z = 0; z < m_gridSize; z++)
                {
                    var wasInOldGrid = x >= offset && x < offset + oldSize
                                    && z >= offset && z < offset + oldSize;
                    if (!wasInOldGrid && m_tiles[x, z] == null)
                        ringCells.Add(GridToWorld(x, z));
                }
            }

            Shuffle(ringCells);

            var toSpawn = Mathf.Min(count, ringCells.Count);
            for (var i = 0; i < toSpawn; i++)
                PlaceNode(ringCells[i].x, ringCells[i].y);
        }

        private List<Vector2Int> CollectFreeCells()
        {
            List<Vector2Int> freeCells = new();

            for (var x = 0; x < m_gridSize; x++)
            {
                for (var z = 0; z < m_gridSize; z++)
                {
                    if (m_tiles[x, z] == null)
                        freeCells.Add(GridToWorld(x, z));
                }
            }

            return freeCells;
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private void UpdateNodeTileIndices(Vector2Int offset)
        {
            HashSet<Node> updated = new();
            for (var x = 0; x < m_gridSize; x++)
            {
                for (var z = 0; z < m_gridSize; z++)
                {
                    Node node = m_tiles[x, z];
                    if (node == null || !updated.Add(node))
                        continue;
                    for (var i = 0; i < node.TilesUsed.Count; i++)
                        node.TilesUsed[i] += offset;
                }
            }
        }

        private void RegisterExistingNodes()
        {
            Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            foreach (Node node in nodes)
            {
                var worldX = Mathf.RoundToInt(node.transform.position.x);
                var worldZ = Mathf.RoundToInt(node.transform.position.z);
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

                PlaceNodeAt(node, worldX, worldZ);
            }
        }

        private void RegisterPendingNodes()
        {
            for (var i = m_pendingNodes.Count - 1; i >= 0; i--)
            {
                Node node = m_pendingNodes[i];

                if (node == null)
                {
                    m_pendingNodes.RemoveAt(i);
                    continue;
                }

                var worldX = Mathf.RoundToInt(node.transform.position.x);
                var worldZ = Mathf.RoundToInt(node.transform.position.z);
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

                PlaceNodeAt(node, worldX, worldZ);
                m_pendingNodes.RemoveAt(i);
            }
        }
    }
}
