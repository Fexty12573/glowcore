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

    /// <summary>Fired whenever a slot changes. Argument is the flat index.</summary>
    public event Action<int> OnSlotChanged;

    public Inventory(int width, int height)
    {
        m_items = new ItemStack[width * height];
        m_width = width;
        m_height = height;

        for (var i = 0; i < m_items.Length; i++)
            m_items[i] = ScriptableObject.CreateInstance<ItemStack>();
    }

    public bool AddItems(ref ItemStack stack)
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
            if (m_items[i].Amount == 0) return i;
        for (var i = 0; i < startIndex; i++)
            if (m_items[i].Amount == 0) return i;
        return -1;
    }

    /// <summary>Swap the contents of two slots by flat index.</summary>
    public void Swap(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= m_items.Length) return;
        if (indexB < 0 || indexB >= m_items.Length) return;
        if (indexA == indexB) return;

        var slotA = m_items[indexA];
        var slotB = m_items[indexB];

        var tempItem = slotA.Item;
        var tempAmount = slotA.Amount;

        slotA.Set(slotB.Item, slotB.Amount);
        slotB.Set(tempItem, tempAmount);

        OnSlotChanged?.Invoke(indexA);
        OnSlotChanged?.Invoke(indexB);
    }

    /// <summary>Try to stack sourceIndex onto targetIndex. Returns true if fully merged.</summary>
    public bool TryMerge(int sourceIndex, int targetIndex)
    {
        if (sourceIndex < 0 || sourceIndex >= m_items.Length) return false;
        if (targetIndex < 0 || targetIndex >= m_items.Length) return false;
        if (sourceIndex == targetIndex) return false;

        var source = m_items[sourceIndex];
        var target = m_items[targetIndex];

        if (!source.Valid || !target.Valid) return false;
        if (source.Item != target.Item) return false;

        target.Add(source);
        OnSlotChanged?.Invoke(sourceIndex);
        OnSlotChanged?.Invoke(targetIndex);

        return source.Amount == 0;
    }

    /// <summary>Notify listeners that the slot at the given flat index has changed.</summary>
    public void NotifySlotChanged(int index) => OnSlotChanged?.Invoke(index);

    /// <summary>Get slot by flat index.</summary>
    public ItemStack GetSlot(int index)
    {
        if (index < 0 || index >= m_items.Length) return null;
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
