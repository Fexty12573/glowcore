using System;
using System.ComponentModel;
using ScriptableObjects;
using UnityEngine;

[Serializable]
public class Inventory
{
    [SerializeField] private ItemStack[] m_items;
    [SerializeField][ReadOnly(true)] private int m_width;
    [SerializeField][ReadOnly(true)] private int m_height;

    public event Action OnInventoryChange;
    public Inventory(int width, int height)
    {
        m_items = new ItemStack[width * height];
        m_width = width;
        m_height = height;

        for (var i = 0; i < m_items.Length; i++)
            m_items[i] = new ItemStack();
    }

    public bool AddItems(ref ItemStack stack)
    {
        var existing = GetSlotWithItem(0, stack.Item);
        while (stack.Amount > 0 && existing != -1)
        {
            ref ItemStack toAddStack = ref m_items[existing];
            if (!toAddStack.IsFull)
                toAddStack.Add(ref stack);

            existing = GetSlotWithItem(existing + 1, stack.Item);
        }

        if (stack.Amount == 0)
        {
            OnInventoryChange?.Invoke();
            return true;
        }

        var empty = GetFirstEmptySlot();
        if (empty.HasValue)
        {
            this[empty.Value.x, empty.Value.y] = stack;
            stack.Amount = 0;
            stack.Item = null;
            OnInventoryChange?.Invoke();
            return true;
        }

        OnInventoryChange?.Invoke();
        return false;
    }

    public bool RemoveItemsAt(int x, int y, int amount)
    {
        ref ItemStack itemStack = ref this[x, y];
        if (itemStack.Amount < amount)
            return false;

        itemStack.Amount -= amount;
        if (itemStack.Amount == 0)
            itemStack.Item = null;

        OnInventoryChange?.Invoke();
        return true;
    }

    public ItemStack GetSlotWithItem(Item item)
    {
        foreach (var slot in m_items)
        {
            if (slot.Item == item && slot.Amount > 0)
                return slot;
        }

        return new ItemStack();
    }

    public int GetSlotWithItem(int startIndex, Item item)
    {
        for (var i = startIndex; i < m_items.Length; i++)
        {
            if (m_items[i].Item == item && m_items[i].Amount > 0)
                return i;
        }

        return -1;
    }

    public Vector2Int? GetFirstEmptySlot()
    {
        for (var y = 0; y < m_height; y++)
        {
            for (var x = 0; x < m_width; x++)
            {
                if (this[x, y].Amount == 0)
                    return new Vector2Int(x, y);
            }
        }

        return null;
    }

    public ref ItemStack this[int x, int y] => ref m_items[(y * m_width) + x];
}
