using GlowCore.World;
using ScriptableObjects;
using UnityEngine;

public class Replanter : MonoBehaviour
{
    [SerializeField] private Node m_replanterNode;
    [SerializeField] private Chest m_storage;
    [SerializeField] private float m_replantDelay = 0.5f; // when the tile is free, the replanter waits for this time before planting

    private float m_delayTimer;

    private void Update()
    {
        Block block = GetBlockToPlant();
        if (block is null)
        {
            m_delayTimer = 0f;
            return;
        }

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
        Vector3 targetWorldPosition = transform.position + Node.RotationToDirectionVector3(m_replanterNode.Rotation);
        Vector2Int targetTile = WorldGrid.Instance.WorldToGrid(targetWorldPosition);

        if (WorldGrid.Instance.IsOccupied(targetTile) || !WorldGrid.Instance.IsInBounds(targetTile) ||
            WorldGrid.Instance.IsPlayerObstructing(targetWorldPosition, 1.5f))
        {
            m_delayTimer = 0f;
            return false;
        }


        m_delayTimer += Time.deltaTime;
        if (m_delayTimer < m_replantDelay)
            return false;

        Node node = WorldGrid.Instance.CreateNodeAt(block.NodeToBuild, targetTile, m_replanterNode.Rotation, block.YRotationOffset);
        AudioManager.Instance.PlayOneShot(AudioManager.SoundType.Build, AudioManager.AudioChannel.Player);
        m_delayTimer = 0f;
        return node is not null;
    }
}
