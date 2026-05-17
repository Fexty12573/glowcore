using System;
using ScriptableObjects;

public interface IInventoryService
{
    // Queries
    int SlotCount { get; }
    int HotbarSlotCount { get; }
    int SelectedHotbarIndex { get; }
    bool IsOpen { get; }
    bool IsCraftingStationOpen { get; }
    bool IsGlowCoreUIOpen { get; }
    bool IsChestOpen { get; }

    SlotData GetSlotData(int flatIndex);
    int CountItem(Item item);
    bool CanAcceptItem(Item item, int amount);
    bool IsHotbarSlot(int flatIndex);

    // Commands
    void Swap(int indexA, int indexB);
    bool TryMerge(int srcIndex, int dstIndex);
    void DropItem(int slotIndex, UnityEngine.Vector3 dropPosition);
    void SelectHotbarSlot(int index);
    void ToggleInventory();
    void SetInventoryOpen(bool open);
    void SetCraftingStationOpen(bool open);
    void SetGlowCoreUIOpen(bool open);
    void SetChestOpen(bool open);
    void RequestCloseUI();
    int RemoveItems(Item item, int amount);
    bool AddItem(Item item, int amount);
    bool AddItem(ItemStack stack);

    // Events
    event Action<SlotChangedEvent> OnSlotChanged;
    event Action<int> OnHotbarSelectionChanged;
    event Action<bool> OnInventoryToggled;
    event Action OnCraftingToggled;
    event Action<bool> OnCraftingStationToggled;
    event Action<bool> OnGlowCoreUIToggled;
    event Action<bool> OnChestToggled;
    event Action OnCloseUIRequested;
}
