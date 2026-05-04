using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
    public class Recipe : ScriptableObject
    {
        [Serializable]
        public struct Ingredient
        {
            public Item Item;
            [Min(1)] public int Amount;
        }

        public string Name;
        public Ingredient[] Ingredients;
        public Item ResultItem;
        [Min(1)] public int ResultAmount = 1;

        [SerializeField] private bool m_craftableInInventory;
        public bool CraftableInInventory => m_craftableInInventory;
    }
}
