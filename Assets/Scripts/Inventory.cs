using System;
using System.ComponentModel;
using ScriptableObjects;
using UnityEngine;

[Serializable]
public struct Inventory
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

        OnInventoryChange = null;
    }

    public bool AddItems(ItemStack stack)
    {
        if (stack == null)
            return false;

        var existing = GetSlotWithItem(0, stack.Item);
        while (stack.Amount > 0 && existing != -1)
        {
            if (!m_items[existing].IsFull)
                m_items[existing].Add(stack);

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
            this[empty.Value.x, empty.Value.y].Set(stack);
            OnInventoryChange?.Invoke();
            return true;
        }

        OnInventoryChange?.Invoke();
        return false;
    }

    public bool RemoveItemsAt(int x, int y, int amount)
    {
        ItemStack itemStack = this[x, y];
        if (itemStack is null || itemStack.Amount < amount)
            return false;

        itemStack.Amount -= amount;
        if (itemStack.Amount == 0)
            itemStack.Clear();

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

        return null;
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

    public ItemStack this[int x, int y] => m_items[(y * m_width) + x];
}
