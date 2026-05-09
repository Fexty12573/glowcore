using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GlowCoreLevelConfig", menuName = "Scriptable Objects/GlowCore Level Config")]
    public class GlowCoreLevelConfig : ScriptableObject
    {
        [SerializeField] private string m_levelName = "GlowCore";
        [SerializeField][Min(1)] private int m_level = 1;
        [SerializeField] private Sprite m_levelIcon;
        [SerializeField] private Recipe.Ingredient[] m_requiredMaterials = System.Array.Empty<Recipe.Ingredient>();
        [SerializeField][Min(0)] private int m_tilesOnLevelUp = 5;
        [Tooltip("Tiles occupied by the GlowCore in the world grid (1=1×1, 4=2×2, 9=3×3, 16=4×4).")]
        [SerializeField][Min(1)] private int m_size = 1;
        [SerializeField]
        [TextArea]
        private string m_nextLevelPerkTemplate =
            "Next level expands the glow radius by +{tiles} tiles";

        public string LevelName => m_levelName;
        public int Level => m_level;
        public Sprite LevelIcon => m_levelIcon;
        public IReadOnlyList<Recipe.Ingredient> RequiredMaterials => m_requiredMaterials;
        public int TilesOnLevelUp => m_tilesOnLevelUp;
        public int TileCount => m_size;

        public string FormatPerk() =>
            m_nextLevelPerkTemplate.Replace("{tiles}", m_tilesOnLevelUp.ToString());
    }
}
