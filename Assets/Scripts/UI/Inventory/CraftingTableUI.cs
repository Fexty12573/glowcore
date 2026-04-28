using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    public class CraftingTableUI : MonoBehaviour
    {
        // Instance Fields
        [Header("References")]
        [SerializeField] private CanvasGroup m_panelCanvasGroup;
        [SerializeField] private CanvasGroup m_backdropCanvasGroup;

        [Header("Close Button")]
        [SerializeField] private Button m_closeButton;

        [Header("Recipe Section")]
        [SerializeField] private Transform m_recipeContentParent;
        [SerializeField] private GameObject m_recipeRowPrefab;
        [SerializeField] private TooltipUI m_tooltip;

        [Header("Recipes")]
        [SerializeField] private RecipeList m_recipeList;

        [Header("Inventory Display")]
        [SerializeField] private Transform m_inventoryUpperGridParent;
        [SerializeField] private Transform m_inventoryHotbarRowParent;
        [SerializeField] private GameObject m_slotPrefab;

        private IInventoryService m_inventoryService;
        private InventoryUI m_inventoryUI;
        private ICraftingService m_craftingService;
        private RecipeRowUI[] m_rows;
        private ItemSlotUI[] m_inventorySlots;
        private bool m_isVisible;

        // Properties
        public bool IsVisible => m_isVisible;

        // Events
        public event Action OnCloseRequested;

        // Public Methods
        public void SetVisible(bool visible)
        {
            m_isVisible = visible;
            m_panelCanvasGroup?.SetVisible(visible);

            if (m_backdropCanvasGroup != null)
            {
                m_backdropCanvasGroup.alpha = visible ? 1f : 0f;
                m_backdropCanvasGroup.blocksRaycasts = visible;
            }

            if (!visible && m_inventoryUI != null)
                m_inventoryUI.CancelHeldItem();

            if (visible)
            {
                RefreshAllRows();
                RefreshInventoryDisplay();
            }
        }

        public void Show() => SetVisible(true);

        public void Hide() => SetVisible(false);

        // Private Methods
        private void Start()
        {
            m_inventoryService = FindFirstObjectByType<PlayerInventory>();
            if (m_inventoryService == null)
            {
                Debug.LogError("CraftingTableUI: Could not find PlayerInventory in scene.");
                return;
            }

            m_inventoryUI = FindFirstObjectByType<InventoryUI>();

            if (m_closeButton != null)
                m_closeButton.onClick.AddListener(OnCloseButtonClicked);

            var tableRecipes = m_recipeList.Recipes;
            m_craftingService = new CraftingSystem(m_inventoryService, tableRecipes);

            BuildRows();
            BuildInventoryDisplay();
            SetVisible(false);

            m_craftingService.OnRecipesRefreshed += OnRecipesRefreshed;
            m_inventoryService.OnSlotChanged += OnSlotChanged;
        }

        private void OnDestroy()
        {
            if (m_closeButton != null)
                m_closeButton.onClick.RemoveListener(OnCloseButtonClicked);

            if (m_craftingService != null)
            {
                m_craftingService.OnRecipesRefreshed -= OnRecipesRefreshed;
                m_craftingService.Dispose();
            }

            if (m_inventoryService != null)
                m_inventoryService.OnSlotChanged -= OnSlotChanged;
        }

        private void BuildRows()
        {
            var recipes = m_craftingService.Recipes;
            m_rows = new RecipeRowUI[recipes.Count];
            for (var i = 0; i < recipes.Count; i++)
            {
                var go = Instantiate(m_recipeRowPrefab, m_recipeContentParent);
                var row = go.GetComponent<RecipeRowUI>();
                row.Initialize(m_craftingService, recipes[i], m_tooltip);
                m_rows[i] = row;
            }
        }

        private void BuildInventoryDisplay()
        {
            var totalSlots = m_inventoryService.SlotCount;
            var hotbarSlots = m_inventoryService.HotbarSlotCount;
            m_inventorySlots = new ItemSlotUI[totalSlots];

            for (var i = 0; i < hotbarSlots; i++)
            {
                var go = Instantiate(m_slotPrefab, m_inventoryHotbarRowParent);
                var slot = go.GetComponent<ItemSlotUI>();
                slot.Initialize(m_inventoryUI, i, m_inventoryService.GetSlotData(i));
                slot.SetHotbarStyle(i + 1);
                m_inventorySlots[i] = slot;
            }

            for (var i = hotbarSlots; i < totalSlots; i++)
            {
                var go = Instantiate(m_slotPrefab, m_inventoryUpperGridParent);
                var slot = go.GetComponent<ItemSlotUI>();
                slot.Initialize(m_inventoryUI, i, m_inventoryService.GetSlotData(i));
                m_inventorySlots[i] = slot;
            }
        }

        private void OnCloseButtonClicked()
        {
            SetVisible(false);
            OnCloseRequested?.Invoke();
        }

        private void OnRecipesRefreshed()
        {
            if (!m_isVisible)
                return;
            RefreshAllRows();
        }

        private void OnSlotChanged(SlotChangedEvent evt)
        {
            if (evt.SlotIndex >= 0 && evt.SlotIndex < m_inventorySlots.Length)
                m_inventorySlots[evt.SlotIndex].Refresh(evt.Data);
        }

        private void RefreshAllRows()
        {
            if (m_rows == null)
                return;
            for (var i = 0; i < m_rows.Length; i++)
                m_rows[i].Refresh();
        }

        private void RefreshInventoryDisplay()
        {
            if (m_inventorySlots == null)
                return;
            for (var i = 0; i < m_inventorySlots.Length; i++)
                m_inventorySlots[i].Refresh(m_inventoryService.GetSlotData(i));
        }
    }
}
