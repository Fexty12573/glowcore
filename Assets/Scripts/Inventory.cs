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

    public Inventory(int width, int height)
    {
        m_items = new ItemStack[width * height];
        m_width = width;
        m_height = height;

        for (var i = 0; i < m_items.Length; i++)
            m_items[i] = ScriptableObject.CreateInstance<ItemStack>();
    }

    public bool AddItems(ItemStack stack)
    {
        if (stack == null)
            return false;

        var existing = GetSlotWithItem(stack.Item);
        while (stack.Amount > 0 && existing != null)
        {
            if (!existing.IsFull)
                existing.Add(stack);

            existing = GetSlotWithItem(stack.Item);
        }

        if (stack.Amount == 0)
            return true;

        var empty = GetFirstEmptySlot();
        if (empty.HasValue)
        {
            this[empty.Value.x, empty.Value.y].Set(stack);
            return true;
        }

        return false;
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

    public ItemStack this[int x, int y]
    {
        get => m_items[(y * m_width) + x];
        set => m_items[(y * m_width) + x] = value;
    }
}
