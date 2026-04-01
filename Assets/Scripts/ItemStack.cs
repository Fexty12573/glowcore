using System;
using ScriptableObjects;

[Serializable]
public struct ItemStack
{
    public Item Item;
    public int Amount;

    public bool IsFull => Item != null && Amount >= Item.MaxStack;
    public bool IsValid => Item != null && Amount != 0;

    public ItemStack(Item item, int amount)
    {
        Item = item;
        Amount = amount;
    }

    public int Add(int amountToAdd)
    {
        var newAmount = Math.Clamp(Amount + amountToAdd, 0, Item.MaxStack);
        var added = newAmount - Amount;
        Amount = newAmount;

        return added;
    }

    public void Add(ref ItemStack stack)
    {
        if (Item != stack.Item)
            return;

        var added = Add(stack.Amount);
        stack.Amount -= added;
    }
}