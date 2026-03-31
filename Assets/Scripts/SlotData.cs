using ScriptableObjects;
using UnityEngine;

public readonly struct SlotData
{
    public readonly Item Item;
    public readonly int Amount;
    public readonly bool IsValid;
    public readonly Texture2D Icon;

    public SlotData(ItemStack stack)
    {
        if (stack != null && stack.IsValid)
        {
            Item = stack.Item;
            Amount = stack.Amount;
            IsValid = true;
            Icon = stack.Item != null ? stack.Item.Icon : null;
        }
        else
        {
            Item = null;
            Amount = 0;
            IsValid = false;
            Icon = null;
        }
    }

    public SlotData(Item item, int amount)
    {
        Item = item;
        Amount = amount;
        IsValid = item != null && amount > 0;
        Icon = item != null ? item.Icon : null;
    }

    public static readonly SlotData Empty = new(null, 0);
}

public readonly struct SlotChangedEvent
{
    public readonly int SlotIndex;
    public readonly SlotData Data;

    public SlotChangedEvent(int slotIndex, SlotData data)
    {
        SlotIndex = slotIndex;
        Data = data;
    }
}
