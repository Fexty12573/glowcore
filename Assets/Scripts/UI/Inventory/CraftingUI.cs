using ScriptableObjects;
using UnityEngine;

namespace GlowCore.UI.Inventory
{
    public class CraftingUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInventory m_playerInventory;
        [SerializeField] private CanvasGroup m_panelCanvasGroup;
        [SerializeField] private Transform m_contentParent;
        [SerializeField] private GameObject m_recipeRowPrefab;
        [SerializeField] private TooltipUI m_tooltip;

        [Header("Recipes")]
        [SerializeField] private Recipe[] m_recipes;

        private IInventoryService m_inventoryService;
        private ICraftingService m_craftingService;
        private RecipeRowUI[] m_rows;
        private bool m_isVisible;

        public bool IsVisible => m_isVisible;

        private void Start()
        {
            m_inventoryService = m_playerInventory;
            m_craftingService = new CraftingSystem(m_inventoryService, m_recipes);

            BuildRows();
            SetVisible(false);

            m_craftingService.OnRecipesRefreshed += OnRecipesRefreshed;
            m_inventoryService.OnCraftingToggled += Toggle;
        }

        private void OnDestroy()
        {
            if (m_craftingService != null)
            {
                m_craftingService.OnRecipesRefreshed -= OnRecipesRefreshed;
                m_craftingService.Dispose();
            }

            if (m_inventoryService != null)
                m_inventoryService.OnCraftingToggled -= Toggle;
        }

        private void BuildRows()
        {
            var recipes = m_craftingService.Recipes;
            m_rows = new RecipeRowUI[recipes.Count];
            for (var i = 0; i < recipes.Count; i++)
            {
                var go = Instantiate(m_recipeRowPrefab, m_contentParent);
                var row = go.GetComponent<RecipeRowUI>();
                row.Initialize(m_craftingService, recipes[i], m_tooltip);
                m_rows[i] = row;
            }
        }

        private void OnRecipesRefreshed()
        {
            if (!m_isVisible)
                return;
            RefreshAll();
        }

        public void Toggle()
        {
            SetVisible(!m_isVisible);
        }

        public void Show() => SetVisible(true);

        public void Hide() => SetVisible(false);

        private void SetVisible(bool visible)
        {
            m_isVisible = visible;

            m_panelCanvasGroup?.SetVisible(visible);

            if (visible)
                RefreshAll();
        }

        private void RefreshAll()
        {
            if (m_rows == null)
                return;
            for (var i = 0; i < m_rows.Length; i++)
                m_rows[i].Refresh();
        }
    }
}
