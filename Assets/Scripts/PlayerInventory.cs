using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour, IInventoryService, IItemContainer
{
    [Serializable]
    private struct StartingItem
    {
        public Item Item;
        public int Amount;
    }

    // Constants
    private const int kColumns = 8;
    private const int kRows = 4;
    private const int kHotbarSlots = 8;
    private const int kHotbarStartIndex = 0;

    // Instance Fields
    [SerializeField] private PlayerHand m_playerHand;
    [SerializeField] private StartingItem[] m_startingItems;

    private Inventory m_inventory;
    private int m_selectedHotbarIndex;
    private bool m_isOpen;
    private bool m_isCraftingStationOpen;
    private bool m_isGlowCoreUIOpen;
    private bool m_isChestOpen;

    // IInventoryService — Properties
    public int SlotCount => m_inventory.Size;
    public int HotbarSlotCount => kHotbarSlots;
    public int SelectedHotbarIndex => m_selectedHotbarIndex;
    public bool IsOpen => m_isOpen;
    public bool IsCraftingStationOpen => m_isCraftingStationOpen;
    public bool IsGlowCoreUIOpen => m_isGlowCoreUIOpen;
    public bool IsChestOpen => m_isChestOpen;

    // IInventoryService — Events
    public event Action<SlotChangedEvent> OnSlotChanged;
    public event Action<int> OnHotbarSelectionChanged;
    public event Action<bool> OnInventoryToggled;
    public event Action OnCraftingToggled;
    public event Action<bool> OnCraftingStationToggled;
    public event Action<bool> OnGlowCoreUIToggled;
    public event Action<bool> OnChestToggled;
    public event Action OnCloseUIRequested;

    // Public Methods — IInventoryService Queries
    public SlotData GetSlotData(int flatIndex)
    {
        var stack = m_inventory.GetSlot(flatIndex);
        return stack != null ? new SlotData(stack) : SlotData.Empty;
    }

    public int CountItem(Item item) => m_inventory.CountItem(item);

    public bool CanAcceptItem(Item item, int amount) => m_inventory.CanAccept(item, amount);

    public bool IsHotbarSlot(int flatIndex) => flatIndex is >= 0 and < kHotbarSlots;

    // Public Methods — IInventoryService Commands
    public void SelectHotbarSlot(int index)
    {
        if (index < 0 || index >= kHotbarSlots)
            return;
        m_selectedHotbarIndex = index;
        OnHotbarSelectionChanged?.Invoke(index);
        UpdatePlayerHand();
    }

    public void Swap(int indexA, int indexB) => m_inventory.Swap(indexA, indexB);

    public bool TryMerge(int srcIndex, int dstIndex) => m_inventory.TryMerge(srcIndex, dstIndex);

    public void DropItem(int slotIndex, Vector3 dropPosition)
    {
        ItemStackDrop.Spawn(m_inventory.GetSlot(slotIndex), dropPosition);
        m_inventory.ClearSlot(slotIndex);
    }

    public int RemoveItems(Item item, int amount) => m_inventory.RemoveItems(item, amount);

    public bool AddItem(Item item, int amount) => AddItem(new ItemStack(item, amount));

    public bool AddItem(ItemStack stack) => m_inventory.AddItems(stack, kHotbarStartIndex);

    public void ToggleInventory()
    {
        m_isOpen = !m_isOpen;
        OnInventoryToggled?.Invoke(m_isOpen);
    }

    public void SetInventoryOpen(bool open)
    {
        if (m_isOpen == open)
            return;
        m_isOpen = open;
        OnInventoryToggled?.Invoke(m_isOpen);
    }

    public void SetCraftingStationOpen(bool open)
    {
        m_isCraftingStationOpen = open;
        OnCraftingStationToggled?.Invoke(open);
    }

    public void SetGlowCoreUIOpen(bool open)
    {
        m_isGlowCoreUIOpen = open;
        OnGlowCoreUIToggled?.Invoke(open);
    }

    public void SetChestOpen(bool open)
    {
        m_isChestOpen = open;
        OnChestToggled?.Invoke(open);
    }

    // IItemContainer — Commands not already on IInventoryService
    public void SetSlot(int flatIndex, Item item, int amount) => m_inventory.SetSlot(flatIndex, item, amount);

    public int AddStack(Item item, int amount) => m_inventory.AddStack(item, amount, kHotbarStartIndex);

    // Public Methods — Game Logic
    public Inventory GetInventory() => m_inventory;

    public void ConsumeHandItem(int amount)
    {
        var slot = m_inventory.GetSlot(m_selectedHotbarIndex);
        if (slot == null || !slot.IsValid)
            return;
        m_inventory.RemoveItems(slot.Item, amount);
        UpdatePlayerHand();
    }

    // Private Methods — Lifecycle
    private void Start()
    {
        UpdatePlayerHand();
    }

    private void Awake()
    {
        m_inventory = new Inventory(kColumns, kRows);
        m_inventory.OnSlotChanged += OnInventorySlotChanged;

        if (m_startingItems != null)
        {
            foreach (StartingItem entry in m_startingItems)
            {
                if (entry.Item != null && entry.Amount > 0)
                    m_inventory.AddItems(new ItemStack(entry.Item, entry.Amount), kHotbarStartIndex);
            }
        }
    }

    private void OnDestroy()
    {
        if (m_inventory != null)
            m_inventory.OnSlotChanged -= OnInventorySlotChanged;
    }

    // Private Methods — Input Action Callbacks
    private void OnInventory(InputValue value) => ToggleInventory();

    private void OnCrafting(InputValue value)
    {
        if (m_isOpen)
            OnCraftingToggled?.Invoke();
    }

    private void OnCloseUI(InputValue value)
    {
        if (m_isOpen)
            SetInventoryOpen(false);
        OnCloseUIRequested?.Invoke();
    }

    private void OnHotbarSlot1(InputValue value) => SelectHotbarSlot(0);
    private void OnHotbarSlot2(InputValue value) => SelectHotbarSlot(1);
    private void OnHotbarSlot3(InputValue value) => SelectHotbarSlot(2);
    private void OnHotbarSlot4(InputValue value) => SelectHotbarSlot(3);
    private void OnHotbarSlot5(InputValue value) => SelectHotbarSlot(4);
    private void OnHotbarSlot6(InputValue value) => SelectHotbarSlot(5);
    private void OnHotbarSlot7(InputValue value) => SelectHotbarSlot(6);
    private void OnHotbarSlot8(InputValue value) => SelectHotbarSlot(7);

    private void OnPrevious(InputValue value) =>
        SelectHotbarSlot((m_selectedHotbarIndex + 1) % kHotbarSlots);

    private void OnNext(InputValue value) =>
        SelectHotbarSlot((m_selectedHotbarIndex - 1 + kHotbarSlots) % kHotbarSlots);

    // Private Methods — Internal
    private void OnInventorySlotChanged(int flatIndex)
    {
        if (flatIndex == m_selectedHotbarIndex)
            UpdatePlayerHand();

        var data = new SlotData(m_inventory.GetSlot(flatIndex));
        OnSlotChanged?.Invoke(new SlotChangedEvent(flatIndex, data));
    }

    private void UpdatePlayerHand()
    {
        if (m_playerHand == null)
            return;
        m_playerHand.SetItemInHand(m_inventory.GetSlot(m_selectedHotbarIndex));
    }

}
