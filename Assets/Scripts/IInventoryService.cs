using System;
using ScriptableObjects;

public interface IInventoryService
{
    // Queries
    int SlotCount { get; }
    int HotbarSlotCount { get; }
    int SelectedHotbarIndex { get; }
    bool IsOpen { get; }

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
    int RemoveItems(Item item, int amount);
    bool AddItem(Item item, int amount);

    // Events
    event Action<SlotChangedEvent> OnSlotChanged;
    event Action<int> OnHotbarSelectionChanged;
    event Action<bool> OnInventoryToggled;
    event Action OnCraftingToggled;
}
