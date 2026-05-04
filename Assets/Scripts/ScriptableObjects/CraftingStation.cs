using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "CraftingStation", menuName = "Scriptable Objects/CraftingStation")]
    public class CraftingStation : ScriptableObject
    {
        public string Name;
        public string Description;
        public RecipeList RecipeList;
        public Color BackgroundColor = new Color32(0x1C,0x14,0x0C,255);
    }
}
