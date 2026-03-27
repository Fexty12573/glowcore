using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Player inventory: grid of kColumns x kTotalRows. Last kHotbarSlots slots are the hotbar.</summary>
public class PlayerInventory : MonoBehaviour
{
    private const int kColumns = 4;
    private const int kTotalRows = 8;
    private const int kHotbarSlots = 8;

    private GameObject m_handItemGameObject;
    private int m_hotbarIndex = 0;
    [SerializeField] private Inventory m_inventory;
    [SerializeField] private PlayerHand m_playerHand;

    private int m_selectedHotbarIndex;

    public Inventory Inventory => m_inventory;
    public int Columns => kColumns;
    public int TotalRows => kTotalRows;
    public int HotbarSlots => kHotbarSlots;
    public int SelectedHotbarIndex => m_selectedHotbarIndex;

    /// <summary>Flat index where the hotbar region starts in the inventory array.</summary>
    public int HotbarStartIndex => m_inventory.Size - kHotbarSlots;

    /// <summary>Number of non-hotbar slots (the upper inventory grid).</summary>
    public int UpperSlotCount => m_inventory.Size - kHotbarSlots;

    /// <summary>Fired when the selected hotbar slot changes.</summary>
    public event Action<int> OnHotbarSelectionChanged;

    /// <summary>Fired when the inventory panel opens or closes.</summary>
    public event Action<bool> OnInventoryToggled;

    private bool m_isOpen;
    public bool IsOpen => m_isOpen;

    private void Awake()
    {
        m_inventory = new Inventory(kColumns, kTotalRows);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            ToggleInventory();
    }

    public void Add(ItemStack stack) => m_inventory.AddItems(stack);

    public void SelectHotbarSlot(int index)
    {
        if (index < 0 || index >= kHotbarSlots) return;
        m_selectedHotbarIndex = index;
        OnHotbarSelectionChanged?.Invoke(index);
        UpdatePlayerHand();
    }

    private void OnPrevious(InputValue value)
    {
        SelectHotbarSlot((m_selectedHotbarIndex + 1) % kHotbarSlots);
    }

    private void OnNext(InputValue value)
    {
        SelectHotbarSlot((m_selectedHotbarIndex - 1 + kHotbarSlots) % kHotbarSlots);
    }

    private void UpdatePlayerHand()
    {
        if (m_playerHand == null) return;
        m_playerHand.SetItemInHand(GetHotbarSlot(m_selectedHotbarIndex));
    }

    /// <summary>Get the hotbar ItemStack at the given hotbar position (0 to kHotbarSlots-1).</summary>
    public ItemStack GetHotbarSlot(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= kHotbarSlots) return null;
        return m_inventory.GetSlot(HotbarStartIndex + hotbarIndex);
    }

    /// <summary>Convert a hotbar index (0-7) to the flat inventory index.</summary>
    public int HotbarToInventoryIndex(int hotbarIndex) => HotbarStartIndex + hotbarIndex;

    /// <summary>Check if a flat inventory index is in the hotbar region.</summary>
    public bool IsHotbarSlot(int flatIndex) => flatIndex >= HotbarStartIndex;

    /// <summary>Convert a flat inventory index to a hotbar index. Returns -1 if not a hotbar slot.</summary>
    public int InventoryToHotbarIndex(int flatIndex)
    {
        if (flatIndex < HotbarStartIndex) return -1;
        return flatIndex - HotbarStartIndex;
    }

    public void ToggleInventory()
    {
        m_isOpen = !m_isOpen;
        OnInventoryToggled?.Invoke(m_isOpen);
    }

    public void SetInventoryOpen(bool open)
    {
        if (m_isOpen == open) return;
        m_isOpen = open;
        OnInventoryToggled?.Invoke(m_isOpen);
    }
}
