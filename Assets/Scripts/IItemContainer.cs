using System;
using ScriptableObjects;

public interface IItemContainer
{
    // Queries
    int SlotCount { get; }

    SlotData GetSlotData(int flatIndex);
    int CountItem(Item item);
    bool CanAcceptItem(Item item, int amount);

    // Commands
    void Swap(int indexA, int indexB);
    bool TryMerge(int srcIndex, int dstIndex);
    void SetSlot(int flatIndex, Item item, int amount);
    int AddStack(Item item, int amount);
    int RemoveItems(Item item, int amount);

    // Events
    event Action<SlotChangedEvent> OnSlotChanged;
}
