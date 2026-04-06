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

    public int Width => m_width;
    public int Height => m_height;
    public int Size => m_items.Length;

    public event Action<int> OnSlotChanged;

    public Inventory(int width, int height)
    {
        m_items = new ItemStack[width * height];
        m_width = width;
        m_height = height;

        for (var i = 0; i < m_items.Length; i++)
            m_items[i] = new ItemStack();
    }

    public bool AddItems(ItemStack stack, int emptySlotStart = 0)
    {
        if (stack == null)
            return false;

        for (var i = 0; i < m_items.Length && stack.Amount > 0; i++)
        {
            if (m_items[i].Item == stack.Item && m_items[i].Amount < m_items[i].Item.MaxStack)
            {
                m_items[i].Add(stack);
                OnSlotChanged?.Invoke(i);
            }
        }

        if (stack.Amount == 0)
            return true;

        var emptyIndex = FindFirstEmptySlotFrom(emptySlotStart);
        if (emptyIndex >= 0)
        {
            m_items[emptyIndex].Set(stack);
            OnSlotChanged?.Invoke(emptyIndex);
            return true;
        }

        return false;
    }

    private int FindFirstEmptySlotFrom(int startIndex)
    {
        for (var i = startIndex; i < m_items.Length; i++)
        {
            if (m_items[i].Amount == 0)
                return i;
        }

        for (var i = 0; i < startIndex; i++)
        {
            if (m_items[i].Amount == 0)
                return i;
        }

        return -1;
    }

    public void Swap(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= m_items.Length)
            return;
        if (indexB < 0 || indexB >= m_items.Length)
            return;
        if (indexA == indexB)
            return;

        var slotA = m_items[indexA];
        var slotB = m_items[indexB];

        var tempItem = slotA.Item;
        var tempAmount = slotA.Amount;

        slotA.Set(slotB.Item, slotB.Amount);
        slotB.Set(tempItem, tempAmount);

        OnSlotChanged?.Invoke(indexA);
        OnSlotChanged?.Invoke(indexB);
    }

    public bool TryMerge(int sourceIndex, int targetIndex)
    {
        if (sourceIndex < 0 || sourceIndex >= m_items.Length)
            return false;
        if (targetIndex < 0 || targetIndex >= m_items.Length)
            return false;
        if (sourceIndex == targetIndex)
            return false;

        var source = m_items[sourceIndex];
        var target = m_items[targetIndex];

        if (!source.IsValid || !target.IsValid)
            return false;
        if (source.Item != target.Item)
            return false;

        target.Add(source);
        OnSlotChanged?.Invoke(sourceIndex);
        OnSlotChanged?.Invoke(targetIndex);

        return source.Amount == 0;
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= m_items.Length)
            return;
        m_items[index].Set(null, 0);
        OnSlotChanged?.Invoke(index);
    }


    public ItemStack GetSlot(int index)
    {
        if (index < 0 || index >= m_items.Length)
            return null;
        return m_items[index];
    }

    public (ItemStack slot, int index) GetSlotWithItem(Item item)
    {
        for (var i = 0; i < m_items.Length; i++)
        {
            if (m_items[i].Item == item && m_items[i].Amount > 0)
                return (m_items[i], i);
        }

        return (null, -1);
    }

    public int CountItem(Item item)
    {
        var total = 0;
        for (var i = 0; i < m_items.Length; i++)
        {
            if (m_items[i].Item == item)
                total += m_items[i].Amount;
        }
        return total;
    }

    public int RemoveItems(Item item, int amount)
    {
        var remaining = amount;
        for (var i = 0; i < m_items.Length && remaining > 0; i++)
        {
            if (m_items[i].Item != item)
                continue;

            var take = Mathf.Min(m_items[i].Amount, remaining);
            m_items[i].Amount -= take;
            remaining -= take;

            if (m_items[i].Amount == 0)
                m_items[i].Set(null, 0);

            OnSlotChanged?.Invoke(i);
        }
        return amount - remaining;
    }

    public bool CanAccept(Item item, int amount)
    {
        var space = 0;
        for (var i = 0; i < m_items.Length; i++)
        {
            if (m_items[i].Amount == 0)
                space += item.MaxStack;
            else if (m_items[i].Item == item)
                space += item.MaxStack - m_items[i].Amount;

            if (space >= amount)
                return true;
        }
        return space >= amount;
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
