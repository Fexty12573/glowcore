using System;
using GlowCore.World;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockBehaviour : MonoBehaviour, IHandItem
{
    private Vector2Int? m_selectedTile;
    public Node m_nodeToBuild;
    private GameObject m_activeBuildGhost;

    [SerializeField] private float BuildRadius = 4f;
    [SerializeField] private Block Block;
    [SerializeField] private GameObject BuildGhostAllowed;
    [SerializeField] private GameObject BuildGhostOccupied;
    
    public void Use(InputValue inputValue)
    {
        if (inputValue.Get<float>() >= 0.5f)
        {
            TryToBuild();
        }
    }

    private void Start()
    {
        if (!Block.NodeToBuild.TryGetComponent(out Node node))
        {
            Debug.LogError($"{Block.Name}'s NodeToBuild Prefab has no Node Component!");
        }
    }

    private void OnDestroy()
    {
        ClearBuildGhost();
    }

    private void OnEnable()
    {
        NodeActionSystem.OnChangeSelectedTile += HandleTileChanged;
    }

    private void OnDisable()
    {
        NodeActionSystem.OnChangeSelectedTile -= HandleTileChanged;
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
        {
            m_activeBuildGhost = Instantiate(BuildGhostOccupied, spawnPosition, Quaternion.identity);
        }
        else
        {
            m_activeBuildGhost = Instantiate(BuildGhostAllowed, spawnPosition, Quaternion.identity);
        }
    }

    private void TryToBuild()
    {
        if (m_selectedTile is null)
            return;

        Vector3 spawnPosition = WorldGrid.Instance.GetSpawnPosition(m_selectedTile.Value);
        if (!IsWithinBuildRange(spawnPosition) || WorldGrid.Instance.IsOccupied(m_selectedTile.Value))
            return;

        if (!WorldGrid.Instance.CreateNodeAt(m_selectedTile.Value, Block.NodeToBuild))
        {
            Debug.LogWarning($"Failed to create Node at {m_selectedTile}");
        }
        else
        {
            ItemStack blockStack = PlayerHand.Instance.ItemsInHand;
            --blockStack;
            PlayerHand.Instance.UpdateGameObject();
        }
        ChangeBuildGhost();
    }

    private bool IsWithinBuildRange(Vector3 spawnPosition)
    {
        return Vector3.Distance(spawnPosition, transform.position) <= BuildRadius;
    }

    private void ClearBuildGhost()
    {
        Destroy(m_activeBuildGhost);
        m_activeBuildGhost = null;
    }


}
