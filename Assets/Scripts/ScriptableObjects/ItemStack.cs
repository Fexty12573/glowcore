using System;
using UnityEngine;

namespace ScriptableObjects
{
    public class ItemStack : ScriptableObject
    {
        public Item Item;
        public int Amount;

        public bool Valid => Item != null && Amount != 0;

        public int Add(int amount)
        {
            var newAmount = Math.Clamp(Amount + amount, 0, Item.MaxStack);
            var added = newAmount + amount;
            Amount = newAmount;

            return added;
        }

        public void Add(ItemStack stack)
        {
            if (stack == null || Item != stack.Item)
                return;

            var added = Add(stack.Amount);
            stack.Amount -= added;
        }

        public void Set(ItemStack stack)
        {
            if (stack == null)
                return;

            Item = stack.Item;
            Amount = stack.Amount;

            stack.Amount = 0;
        }
    }
}