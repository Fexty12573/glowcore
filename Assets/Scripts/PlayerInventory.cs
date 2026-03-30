using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    private const int kColumns = 4;
    private const int kTotalRows = 8;
    private const int kHotbarSlots = 8;


    [SerializeField] private PlayerHand m_playerHand;

    private int m_selectedHotbarIndex;
    private Inventory m_inventory;

    public Inventory Inventory => m_inventory;
    public int Columns => kColumns;
    public int TotalRows => kTotalRows;
    public int HotbarSlots => kHotbarSlots;
    public int SelectedHotbarIndex => m_selectedHotbarIndex;

    public int HotbarStartIndex => 0;

    public int UpperSlotCount => m_inventory.Size - kHotbarSlots;

    public event Action<int> OnHotbarSelectionChanged;

    public event Action<bool> OnInventoryToggled;

    private bool m_isOpen;
    public bool IsOpen => m_isOpen;

    private void Awake()
    {
        m_inventory = new Inventory(kColumns, kTotalRows);
        m_inventory.OnSlotChanged += OnInventorySlotChanged;
    }

    private void OnDestroy()
    {
        if (m_inventory != null)
            m_inventory.OnSlotChanged -= OnInventorySlotChanged;
    }

    private void OnInventorySlotChanged(int flatIndex)
    {
        if (flatIndex == HotbarToInventoryIndex(m_selectedHotbarIndex))
            UpdatePlayerHand();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            ToggleInventory();
    }

    public void Add(ItemStack stack) => m_inventory.AddItems(stack, HotbarStartIndex);

    public void ConsumeHandItem()
    {
        var slot = GetHotbarSlot(m_selectedHotbarIndex);
        if (slot != null)
            slot.Set(null, 0);
        UpdatePlayerHand();
        m_inventory.NotifySlotChanged(m_selectedHotbarIndex);
    }

    public void SelectHotbarSlot(int index)
    {
        if (index < 0 || index >= kHotbarSlots)
            return;
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
        if (m_playerHand == null)
            return;
        m_playerHand.SetItemInHand(GetHotbarSlot(m_selectedHotbarIndex));
    }

    public ItemStack GetHotbarSlot(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= kHotbarSlots)
            return null;
        return m_inventory.GetSlot(HotbarStartIndex + hotbarIndex);
    }

    public int HotbarToInventoryIndex(int hotbarIndex) => hotbarIndex;

    public bool IsHotbarSlot(int flatIndex) => flatIndex < kHotbarSlots;

    public int InventoryToHotbarIndex(int flatIndex)
    {
        if (flatIndex >= kHotbarSlots)
            return -1;
        return flatIndex;
    }

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
}
