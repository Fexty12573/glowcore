using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "CraftingStation", menuName = "Scriptable Objects/CraftingStation")]
    public class CraftingStation : ScriptableObject
    {
        public string Name;
        public string Description;
        public RecipeList RecipeList;
        public Sprite BackgroundSprite;
    }
}
