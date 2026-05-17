using System;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    public class CraftingStationUI : MonoBehaviour
    {
        // Instance Fields
        [Header("References")]
        [SerializeField] private CanvasGroup m_panelCanvasGroup;
        [SerializeField] private CanvasGroup m_backdropCanvasGroup;

        [Header("Close Button")]
        [SerializeField] private Button m_closeButton;

        [Header("Recipe Section")]
        [SerializeField] private TextMeshProUGUI m_craftingTitleText;
        [SerializeField] private TextMeshProUGUI m_craftingDescriptionText;
        [SerializeField] private Image m_craftingBackground;
        [SerializeField] private Transform m_recipeContentParent;
        [SerializeField] private GameObject m_recipeRowPrefab;
        [SerializeField] private TooltipUI m_tooltip;

        [Header("Inventory Display")]
        [SerializeField] private Transform m_inventoryUpperGridParent;
        [SerializeField] private Transform m_inventoryHotbarRowParent;
        [SerializeField] private GameObject m_slotPrefab;
        [SerializeField] private Image m_inventoryBackground;

        private IInventoryService m_inventoryService;
        private InventoryUI m_inventoryUI;
        private CraftingStation m_craftingStation;
        private ICraftingService m_craftingService;
        private RecipeRowUI[] m_rows;
        private ItemSlotUI[] m_inventorySlots;
        private bool m_isVisible;

        // Cached original visuals for the shared backgrounds, so we can restore them on Hide.
        // The InventoryCanvas is shared across UIs (chest / furnace / crafting / regular inventory),
        // and without this, the last-shown station's sprite leaks into the regular inventory view.
        private Sprite m_inventoryBackgroundOriginalSprite;
        private Color m_inventoryBackgroundOriginalColor;
        private Sprite m_craftingBackgroundOriginalSprite;
        private Color m_craftingBackgroundOriginalColor;
        private bool m_originalsCaptured;

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
                ApplyStationBackground();
                RefreshAllRows();
                RefreshInventoryDisplay();
            }
            else
            {
                RestoreBackgroundOriginals();
            }
        }

        private void CaptureBackgroundOriginals()
        {
            if (m_originalsCaptured)
                return;
            if (m_inventoryBackground != null)
            {
                m_inventoryBackgroundOriginalSprite = m_inventoryBackground.sprite;
                m_inventoryBackgroundOriginalColor = m_inventoryBackground.color;
            }
            if (m_craftingBackground != null)
            {
                m_craftingBackgroundOriginalSprite = m_craftingBackground.sprite;
                m_craftingBackgroundOriginalColor = m_craftingBackground.color;
            }
            m_originalsCaptured = true;
        }

        private void RestoreBackgroundOriginals()
        {
            if (!m_originalsCaptured)
                return;
            if (m_inventoryBackground != null)
            {
                m_inventoryBackground.sprite = m_inventoryBackgroundOriginalSprite;
                m_inventoryBackground.color = m_inventoryBackgroundOriginalColor;
            }
            if (m_craftingBackground != null)
            {
                m_craftingBackground.sprite = m_craftingBackgroundOriginalSprite;
                m_craftingBackground.color = m_craftingBackgroundOriginalColor;
            }
        }

        public void SetCraftingStation(CraftingStation craftingStation)
        {
            if (m_craftingStation == craftingStation)
                return;

            if (m_craftingService is not null)
            {
                m_craftingService.OnRecipesRefreshed -= OnRecipesRefreshed; //unsubscribe from old CraftingSystem
                m_craftingService.Dispose();
            }
            m_craftingStation = craftingStation;

            var stationRecipes = m_craftingStation.RecipeList.Recipes;
            m_craftingService = new CraftingSystem(m_inventoryService, stationRecipes);
            m_craftingService.OnRecipesRefreshed += OnRecipesRefreshed;

            RebuildUI();
        }

        public void Show() => SetVisible(true);

        public void Hide() => SetVisible(false);

        // Private Methods
        private void Start()
        {
            m_inventoryService = FindFirstObjectByType<PlayerInventory>();
            if (m_inventoryService == null)
            {
                Debug.LogError("CraftingStationUI: Could not find Inventory in scene.");
                return;
            }

            m_inventoryUI = FindFirstObjectByType<InventoryUI>();

            if (m_closeButton != null)
            {
                m_closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
            CaptureBackgroundOriginals();
            BuildInventoryDisplay();
            SetVisible(false);

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

        private void RebuildUI()
        {
            ApplyStationBackground();

            if (m_craftingTitleText != null)
                m_craftingTitleText.text = m_craftingStation.Name;
            if (m_craftingDescriptionText != null)
                m_craftingDescriptionText.text = m_craftingStation.Description;

            ClearRows();
            BuildRows();
        }

        // The InventoryCanvas backgrounds are shared and restored to their originals on Hide,
        // so the station sprite must be re-applied every time the panel is shown — not only
        // when the station changes (re-opening the same station skips RebuildUI).
        private void ApplyStationBackground()
        {
            if (m_craftingStation == null || m_craftingStation.BackgroundSprite == null)
                return;

            if (m_inventoryBackground != null)
            {
                m_inventoryBackground.sprite = m_craftingStation.BackgroundSprite;
                m_inventoryBackground.color = Color.white;
            }
            if (m_craftingBackground != null)
            {
                m_craftingBackground.sprite = m_craftingStation.BackgroundSprite;
                m_craftingBackground.color = Color.white;
            }
        }

        private void ClearRows()
        {
            if (m_rows == null)
                return;

            for (var i = 0; i < m_rows.Length; i++)
            {
                if (m_rows[i] != null)
                    Destroy(m_rows[i].gameObject);

            }

            m_rows = null;
        }

        private void BuildRows()
        {
            var recipes = m_craftingService.Recipes;
            m_rows = new RecipeRowUI[recipes.Count];
            for (var i = 0; i < recipes.Count; i++)
            {
                var go = Instantiate(m_recipeRowPrefab, m_recipeContentParent);
                var row = go.GetComponent<RecipeRowUI>();
                row.Initialize(m_craftingService, recipes[i], m_tooltip, m_craftingStation.RecipeRowTheme);
                m_rows[i] = row;
            }
        }

        private void BuildInventoryDisplay()
        {
            var totalSlots = m_inventoryService.SlotCount;
            var hotbarSlots = m_inventoryService.HotbarSlotCount;
            var container = (IItemContainer)m_inventoryService;
            m_inventorySlots = new ItemSlotUI[totalSlots];

            for (var i = 0; i < hotbarSlots; i++)
            {
                var go = Instantiate(m_slotPrefab, m_inventoryHotbarRowParent);
                var slot = go.GetComponent<ItemSlotUI>();
                slot.Initialize(m_inventoryUI, container, i, m_inventoryService.GetSlotData(i));
                slot.SetHotbarStyle(i + 1);
                m_inventorySlots[i] = slot;
            }

            for (var i = hotbarSlots; i < totalSlots; i++)
            {
                var go = Instantiate(m_slotPrefab, m_inventoryUpperGridParent);
                var slot = go.GetComponent<ItemSlotUI>();
                slot.Initialize(m_inventoryUI, container, i, m_inventoryService.GetSlotData(i));
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
