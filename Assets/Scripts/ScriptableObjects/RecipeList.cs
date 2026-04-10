using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "RecipeList", menuName = "Scriptable Objects/Recipe List")]
    public class RecipeList : ScriptableObject
    {
        [SerializeField] private Recipe[] m_recipes = System.Array.Empty<Recipe>();

        public Recipe[] Recipes => m_recipes;
    }
}
