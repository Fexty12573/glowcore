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

        [Header("Map Expansion")]
        [Tooltip("Which item triggers incremental map expansion when fed. Leave empty for no expansion on feed.")]
        [SerializeField] private Item m_expansionItem;
        [Tooltip("How many of the expansion item are needed to grow the map by one tile.")]
        [SerializeField][Min(1)] private int m_expansionCostPerTile = 5;

        [Header("Level 1 Physics")]
        [Tooltip("How many logs are active when the GlowCore first spawns (Level 1 only).")]
        [SerializeField][Min(0)] private int m_initialActiveLogs = 3;

        public string LevelName => m_levelName;
        public int Level => m_level;
        public Sprite LevelIcon => m_levelIcon;
        public IReadOnlyList<Recipe.Ingredient> RequiredMaterials => m_requiredMaterials;
        public int TilesOnLevelUp => m_tilesOnLevelUp;
        public int TileCount => m_size;
        public Item ExpansionItem => m_expansionItem;
        public int ExpansionCostPerTile => m_expansionCostPerTile;
        public int InitialActiveLogs => m_initialActiveLogs;

        public string FormatPerk() =>
            m_nextLevelPerkTemplate.Replace("{tiles}", m_tilesOnLevelUp.ToString());
    }
}
