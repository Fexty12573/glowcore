using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private RecipeRowUI[] m_rows;
        private bool m_isVisible;

        public bool IsVisible => m_isVisible;

        private void Start()
        {
            BuildRows();
            SetVisible(false);

            m_playerInventory.Inventory.OnSlotChanged += OnSlotChanged;
        }

        private void OnDestroy()
        {
            if (m_playerInventory != null && m_playerInventory.Inventory != null)
                m_playerInventory.Inventory.OnSlotChanged -= OnSlotChanged;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame && m_playerInventory.IsOpen)
                Toggle();
        }

        private void BuildRows()
        {
            m_rows = new RecipeRowUI[m_recipes.Length];
            for (var i = 0; i < m_recipes.Length; i++)
            {
                var go = Instantiate(m_recipeRowPrefab, m_contentParent);
                var row = go.GetComponent<RecipeRowUI>();
                row.Initialize(this, m_recipes[i], m_tooltip);
                m_rows[i] = row;
            }
        }

        private void OnSlotChanged(int slotIndex)
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

            if (m_panelCanvasGroup != null)
            {
                m_panelCanvasGroup.alpha = visible ? 1f : 0f;
                m_panelCanvasGroup.interactable = visible;
                m_panelCanvasGroup.blocksRaycasts = visible;
            }

            if (visible)
                RefreshAll();
        }

        public bool CanCraft(Recipe recipe)
        {
            var inv = m_playerInventory.Inventory;

            foreach (var ingredient in recipe.Ingredients)
            {
                if (inv.CountItem(ingredient.Item) < ingredient.Amount)
                    return false;
            }

            return inv.CanAccept(recipe.ResultItem, recipe.ResultAmount);
        }

        public void Craft(Recipe recipe)
        {
            if (!CanCraft(recipe))
                return;

            var inv = m_playerInventory.Inventory;

            foreach (var ingredient in recipe.Ingredients)
                inv.RemoveItems(ingredient.Item, ingredient.Amount);

            var result = new ItemStack(recipe.ResultItem, recipe.ResultAmount);
            inv.AddItems(result);

            RefreshAll();
        }

        public int GetItemCount(Item item) => m_playerInventory.Inventory.CountItem(item);

        private void RefreshAll()
        {
            if (m_rows == null)
                return;
            for (var i = 0; i < m_rows.Length; i++)
                m_rows[i].Refresh();
        }
    }
}
