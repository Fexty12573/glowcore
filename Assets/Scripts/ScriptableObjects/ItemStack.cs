using UnityEngine;

namespace ScriptableObjects
{
    public class ItemStack : ScriptableObject
    {
        public Item Item;
        public int Amount;

        public bool Valid => Item != null && Amount != 0;
    }
}