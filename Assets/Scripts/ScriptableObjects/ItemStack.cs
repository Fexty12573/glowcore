using System;
using UnityEngine;

namespace ScriptableObjects
{
    public class ItemStack : ScriptableObject
    {
        public Item Item;
        public int Amount;

        public bool IsFull => Item != null && Amount >= Item.MaxStack;
        public bool IsValid => Item != null && Amount != 0;

        public static ItemStack Create(Item item, int amount)
        {
            var stack = CreateInstance<ItemStack>();
            stack.Item = item;
            stack.Amount = amount;
            return stack;
        }

        public int Add(int amountToAdd)
        {
            var newAmount = Math.Clamp(Amount + amountToAdd, 0, Item.MaxStack);
            var added = newAmount - Amount;
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

        public void Set(Item item, int amount)
        {
            Item = item;
            Amount = amount;
        }
    }
}