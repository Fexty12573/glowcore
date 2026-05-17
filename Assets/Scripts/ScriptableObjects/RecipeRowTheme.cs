using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "RecipeRowTheme", menuName = "Scriptable Objects/RecipeRowTheme")]
    public class RecipeRowTheme : ScriptableObject
    {
        [SerializeField] private Sprite m_defaultBackground;
        [SerializeField] private Sprite m_craftableBackground;
        [SerializeField] private Sprite m_craftButtonActive;
        [SerializeField] private Sprite m_craftButtonInactive;

        public Sprite DefaultBackground => m_defaultBackground;
        public Sprite CraftableBackground => m_craftableBackground;
        public Sprite CraftButtonActive => m_craftButtonActive;
        public Sprite CraftButtonInactive => m_craftButtonInactive;
    }
}
