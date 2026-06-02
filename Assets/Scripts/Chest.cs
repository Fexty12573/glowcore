using System;
using GlowCore.World;
using ScriptableObjects;
using UnityEngine;

public class Chest : MonoBehaviour, IItemContainer
{
    [Serializable]
    private struct StartingItem
    {
        public Item Item;
        public int Amount;
    }
    // Instance Fields
    [SerializeField, Min(1)] private int m_width = 8;
    [SerializeField, Min(1)] private int m_height = 2;
    [SerializeField] private StartingItem[] m_startingItems;

    // Block this chest represents. Used by the save system so pre-placed scene
    // chests get persisted the same way as player-built ones.
    [SerializeField] private Block m_block;

    private Inventory m_inventory;

    // IItemContainer — Properties
    public int SlotCount => m_inventory.Size;

    // Save/Load access — used by WorldGrid to persist chest contents.
    public Inventory GetInventory() => m_inventory;

    // IItemContainer — Events
    public event Action<SlotChangedEvent> OnSlotChanged;

    // IItemContainer — Queries
    public SlotData GetSlotData(int flatIndex)
    {
        var stack = m_inventory.GetSlot(flatIndex);
        return stack != null ? new SlotData(stack) : SlotData.Empty;
    }

    public bool HasEmptySlot() => m_inventory.GetFirstEmptySlot().HasValue;

    public int CountItem(Item item) => m_inventory.CountItem(item);

    public bool CanAcceptItem(Item item, int amount) => m_inventory.CanAccept(item, amount);

    // IItemContainer — Commands
    public void Swap(int indexA, int indexB) => m_inventory.Swap(indexA, indexB);

    public bool TryMerge(int srcIndex, int dstIndex) => m_inventory.TryMerge(srcIndex, dstIndex);

    public void SetSlot(int flatIndex, Item item, int amount) => m_inventory.SetSlot(flatIndex, item, amount);

    public int AddStack(Item item, int amount) => m_inventory.AddStack(item, amount);

    public int RemoveItems(Item item, int amount) => m_inventory.RemoveItems(item, amount);

    // Private Methods — Lifecycle
    private void Awake()
    {
        m_inventory = new Inventory(m_width, m_height);
        m_inventory.OnSlotChanged += OnInventorySlotChanged;

        if (m_block != null && TryGetComponent(out Node node) && node.SourceBlock == null)
            node.SourceBlock = m_block;

        if (m_startingItems != null)
        {
            foreach (StartingItem entry in m_startingItems)
            {
                if (entry.Item != null && entry.Amount > 0)
                    m_inventory.AddItems(new ItemStack(entry.Item, entry.Amount));
            }
        }
    }

    private void OnDestroy()
    {
        if (m_inventory == null)
            return;

        m_inventory.OnSlotChanged -= OnInventorySlotChanged;

        // Only drop items when the chest was actually broken by the player.
        // Node.Break() sets MarkedForDeletion = true before calling Destroy(),
        // so this filter skips scene unloads and play-mode exits.
        var node = GetComponent<Node>();
        if (node == null || !node.MarkedForDeletion)
            return;

        for (var i = 0; i < m_inventory.Size; i++)
        {
            var stack = m_inventory.GetSlot(i);
            if (stack == null || !stack.IsValid)
                continue;

            var offset = new Vector3(
                UnityEngine.Random.Range(-0.2f, 0.2f),
                0f,
                UnityEngine.Random.Range(-0.2f, 0.2f));
            ItemStackDrop.Spawn(stack, transform.position + offset);
        }
    }

    private void OnInventorySlotChanged(int flatIndex)
    {
        var data = new SlotData(m_inventory.GetSlot(flatIndex));
        OnSlotChanged?.Invoke(new SlotChangedEvent(flatIndex, data));
    }
}
