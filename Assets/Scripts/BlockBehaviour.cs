using GlowCore.World;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockBehaviour : MonoBehaviour, IHandItem, IPlayerInventoryAware
{
    private Vector2Int? m_selectedTile;
    private GameObject m_activeBuildGhost;
    private PlayerInventory m_inventory;

    [SerializeField] private Block m_block;
    [SerializeField] private GameObject m_buildGhostAllowed;
    [SerializeField] private GameObject m_buildGhostOccupied;

    public void SetInventory(PlayerInventory inventory) => m_inventory = inventory;

    public void Use(InputValue inputValue)
    {
        if (inputValue.Get<float>() >= 0.5f)
            TryToBuild();
    }

    private void Start()
    {
        if (!m_block.NodeToBuild.TryGetComponent(out Node node))
            Debug.LogError($"{m_block.Name}'s NodeToBuild Prefab has no Node Component!");
    }

    private void OnDestroy() => ClearBuildGhost();

    private void OnEnable()
    {
        if (NodeActionSystem.Instance == null)
            return;
        NodeActionSystem.Instance.OnChangeSelectedTile += HandleTileChanged;
        HandleTileChanged(NodeActionSystem.Instance.CurrentTile);
    }

    private void OnDisable()
    {
        if (NodeActionSystem.Instance != null)
            NodeActionSystem.Instance.OnChangeSelectedTile -= HandleTileChanged;
    }

    private void HandleTileChanged(Vector2Int? newTile)
    {
        if (newTile != m_selectedTile)
        {
            m_selectedTile = newTile;
            ChangeBuildGhost();
        }
    }

    private void ChangeBuildGhost()
    {
        ClearBuildGhost();
        if (m_selectedTile is null)
            return;

        Vector3 spawnPosition = WorldGrid.Instance.GetSpawnPosition(m_selectedTile.Value);
        if (!IsWithinBuildRange(spawnPosition))
            return;

        if (WorldGrid.Instance.IsOccupied(m_selectedTile.Value) || WorldGrid.Instance.IsPlayerObstructing(spawnPosition))
            m_activeBuildGhost = Instantiate(m_buildGhostOccupied, spawnPosition, Quaternion.identity);
        else
            m_activeBuildGhost = Instantiate(m_buildGhostAllowed, spawnPosition, Quaternion.identity);
    }

    private void TryToBuild()
    {
        if (m_selectedTile is null)
            return;

        Vector3 spawnPosition = WorldGrid.Instance.GetSpawnPosition(m_selectedTile.Value);
        if (!IsWithinBuildRange(spawnPosition) || WorldGrid.Instance.IsOccupied(m_selectedTile.Value) || WorldGrid.Instance.IsPlayerObstructing(spawnPosition))
            return;

        var node = WorldGrid.Instance.CreateNodeAt(m_block.NodeToBuild, m_selectedTile.Value);
        if (node == null)
            Debug.LogWarning($"Failed to create Node at {m_selectedTile}");
        else
        {
            node.SourceBlock = m_block;
            m_inventory.ConsumeHandItem(1);
        }

        ChangeBuildGhost();
    }

    private bool IsWithinBuildRange(Vector3 spawnPosition) => Vector3.Distance(spawnPosition, transform.position) <= m_block.BuildRadius;

    private void ClearBuildGhost()
    {
        Destroy(m_activeBuildGhost);
        m_activeBuildGhost = null;
    }
}
