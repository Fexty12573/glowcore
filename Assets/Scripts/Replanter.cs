using GlowCore.World;
using ScriptableObjects;
using UnityEngine;

public class Replanter : MonoBehaviour
{
    [SerializeField] private Chest m_storage;

    private void Update()
    {
        Block block = GetBlockToPlant();
        if (block is null)
            return;

        if (TryPlaceBlock(block))
            m_storage.GetInventory().RemoveItems(block, 1);
    }

    private Block GetBlockToPlant()
    {
        Inventory inventory = m_storage.GetInventory();
        var slot = inventory.GetFirstNonEmptySlot();
        if (slot is null)
            return null;

        var stack = inventory[slot.Value.x, slot.Value.y];
        if (stack.Item is Block block)
            return block;

        return null;
    }

    private bool TryPlaceBlock(Block block)
    {
        Vector3 targetWorldPosition = transform.position + Vector3.back;
        Vector2Int targetTile =  WorldGrid.Instance.WorldToGrid(targetWorldPosition);

        if (WorldGrid.Instance.IsOccupied(targetTile) || !WorldGrid.Instance.IsInBounds(targetTile) || WorldGrid.Instance.IsPlayerObstructing(targetWorldPosition, 1.5f))
            return false;

        Node node = WorldGrid.Instance.CreateNodeAt(block.NodeToBuild, targetTile);
        return node is not null;
    }
}
