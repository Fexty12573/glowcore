using System;
using System.Collections.Generic;
using GlowCore.UI.Inventory;
using GlowCore.World;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Upgrade
{
    public class GlowCoreUpgradeUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private CanvasGroup m_panelCanvasGroup;
        [SerializeField] private CanvasGroup m_backdropCanvasGroup;

        [Header("Close Button")]
        [SerializeField] private Button m_closeButton;

        [Header("Level Header")]
        [SerializeField] private Image m_currentLevelIcon;
        [SerializeField] private Image m_nextLevelIcon;
        [SerializeField] private TextMeshProUGUI m_levelText;

        [Header("Progress")]
        [SerializeField] private ProgressBarUI m_progressBar;
        [SerializeField] private TextMeshProUGUI m_perkInfo;

        [Header("Feed Materials")]
        [SerializeField] private Transform m_feedRowContentParent;
        [SerializeField] private GameObject m_feedRowPrefab;
        [SerializeField] private Button m_feedButton;
        [SerializeField] private TextMeshProUGUI m_feedButtonLabel;

        [Header("Inventory Display")]
        [SerializeField] private Transform m_inventoryUpperGridParent;
        [SerializeField] private Transform m_inventoryHotbarRowParent;
        [SerializeField] private GameObject m_slotPrefab;

        private IInventoryService m_inventoryService;
        private InventoryUI m_inventoryUI;
        private ItemSlotUI[] m_inventorySlots;
        private readonly List<FeedMaterialRowUI> m_rows = new();
        private IGlowCoreObject m_target;
        private bool m_isVisible;

        public bool IsVisible => m_isVisible;

        public event Action OnCloseRequested;

        public void Show(IGlowCoreObject target)
        {
            if (target == null)
                return;

            if (m_target != null)
                UnsubscribeFromTarget(m_target);

            m_target = target;
            SubscribeToTarget(m_target);

            BuildFeedRows();
            RefreshHeader();
            RefreshProgress();
            RefreshFeedButton();

            SetVisible(true);
            m_inventoryService?.SetGlowCoreUIOpen(true);
        }

        public void Hide() => SetVisible(false);

        public void SetVisible(bool visible)
        {
            m_isVisible = visible;

            m_panelCanvasGroup?.SetVisible(visible);

            if (m_backdropCanvasGroup != null)
            {
                m_backdropCanvasGroup.alpha = visible ? 1f : 0f;
                m_backdropCanvasGroup.blocksRaycasts = visible;
            }

            if (!visible)
            {
                if (m_inventoryUI != null)
                    m_inventoryUI.CancelHeldItem();

                if (m_target != null)
                {
                    UnsubscribeFromTarget(m_target);
                    m_target = null;
                }

                m_inventoryService?.SetGlowCoreUIOpen(false);
            }
            else
            {
                RefreshInventoryDisplay();
            }
        }

        private void Start()
        {
            m_inventoryService = FindFirstObjectByType<PlayerInventory>();
            if (m_inventoryService == null)
            {
                Debug.LogError("GlowCoreUpgradeUI: Could not find Inventory in scene.");
                return;
            }

            m_inventoryUI = FindFirstObjectByType<InventoryUI>();

            if (m_closeButton != null)
                m_closeButton.onClick.AddListener(OnCloseButtonClicked);

            if (m_feedButton != null)
                m_feedButton.onClick.AddListener(OnFeedButtonClicked);

            BuildInventoryDisplay();
            SetVisible(false);

            m_inventoryService.OnSlotChanged += OnSlotChanged;
            m_inventoryService.OnInventoryToggled += OnInventoryToggled;
            m_inventoryService.OnCloseUIRequested += OnCloseUIRequested;
        }

        private void OnDestroy()
        {
            if (m_closeButton != null)
                m_closeButton.onClick.RemoveListener(OnCloseButtonClicked);

            if (m_feedButton != null)
                m_feedButton.onClick.RemoveListener(OnFeedButtonClicked);

            if (m_inventoryService != null)
            {
                m_inventoryService.OnSlotChanged -= OnSlotChanged;
                m_inventoryService.OnInventoryToggled -= OnInventoryToggled;
                m_inventoryService.OnCloseUIRequested -= OnCloseUIRequested;
            }

            if (m_target != null)
                UnsubscribeFromTarget(m_target);
        }

        private void SubscribeToTarget(IGlowCoreObject target)
        {
            target.OnProgressChanged += OnTargetProgressChanged;
            target.OnLevelUp += OnTargetLevelUp;
        }

        private void UnsubscribeFromTarget(IGlowCoreObject target)
        {
            target.OnProgressChanged -= OnTargetProgressChanged;
            target.OnLevelUp -= OnTargetLevelUp;
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

        private void BuildFeedRows()
        {
            ClearFeedRows();

            if (m_target == null || m_target.LevelConfig == null || m_feedRowPrefab == null || m_feedRowContentParent == null)
            {
                Debug.LogWarning($"GlowCoreUpgradeUI.BuildFeedRows: early exit. target={m_target}, levelConfig={m_target?.LevelConfig}, prefab={m_feedRowPrefab}, parent={m_feedRowContentParent}");
                return;
            }

            IReadOnlyList<Recipe.Ingredient> materials = m_target.LevelConfig.RequiredMaterials;
            Debug.Log($"GlowCoreUpgradeUI.BuildFeedRows: {materials.Count} materials on config {m_target.LevelConfig.name}");
            for (var i = 0; i < materials.Count; i++)
            {
                Recipe.Ingredient ingredient = materials[i];
                if (ingredient.Item == null)
                    continue;

                GameObject go = Instantiate(m_feedRowPrefab, m_feedRowContentParent);
                var row = go.GetComponent<FeedMaterialRowUI>();
                if (row == null)
                {
                    Destroy(go);
                    continue;
                }

                row.Initialize(m_inventoryService, m_target, ingredient.Item, ingredient.Amount);
                row.OnSelectionChanged += OnRowSelectionChanged;
                m_rows.Add(row);
            }
        }

        private void ClearFeedRows()
        {
            for (var i = 0; i < m_rows.Count; i++)
            {
                FeedMaterialRowUI row = m_rows[i];
                if (row == null)
                    continue;
                row.OnSelectionChanged -= OnRowSelectionChanged;
                Destroy(row.gameObject);
            }
            m_rows.Clear();
        }

        private void RefreshHeader()
        {
            if (m_target == null)
                return;

            GlowCoreLevelConfig current = m_target.LevelConfig;
            GlowCoreLevelConfig next = m_target.NextLevelConfig;

            if (m_currentLevelIcon != null)
            {
                Sprite sprite = current != null ? current.LevelIcon : null;
                m_currentLevelIcon.sprite = sprite;
                m_currentLevelIcon.enabled = sprite != null;
                m_currentLevelIcon.color = Color.white;
            }

            if (m_nextLevelIcon != null)
            {
                Sprite sprite = next != null ? next.LevelIcon : null;
                m_nextLevelIcon.sprite = sprite;
                m_nextLevelIcon.enabled = sprite != null;
                m_nextLevelIcon.color = m_target.IsReadyToUpgrade ? Color.white : (Color)UIColors.WoodBorderLight;
            }

            if (m_levelText != null)
            {
                var currentLevel = current != null ? current.Level : m_target.Level;
                if (next != null)
                    m_levelText.text = $"{currentLevel} => {next.Level}";
                else
                    m_levelText.text = currentLevel.ToString();
            }

            if (m_perkInfo != null)
                m_perkInfo.text = current != null ? current.FormatPerk() : string.Empty;
        }

        private void RefreshProgress()
        {
            if (m_progressBar != null && m_target != null)
                m_progressBar.SetProgress(m_target.TotalProgress01);
        }

        private void RefreshRows()
        {
            for (var i = 0; i < m_rows.Count; i++)
                m_rows[i]?.Refresh();
        }

        private void RefreshFeedButton()
        {
            if (m_feedButton == null)
                return;

            if (m_target != null && m_target.IsReadyToUpgrade)
            {
                if (!m_target.HasNextLevel)
                {
                    m_feedButton.interactable = false;
                    if (m_feedButtonLabel != null)
                        m_feedButtonLabel.text = "MAX LEVEL REACHED";
                    return;
                }

                m_feedButton.interactable = true;
                if (m_feedButtonLabel != null)
                {
                    GlowCoreLevelConfig next = m_target.NextLevelConfig;
                    m_feedButtonLabel.text = next != null
                        ? $"UPGRADE TO LEVEL {next.Level}"
                        : "UPGRADE";
                }
                return;
            }

            Debug.Log(m_feedButtonLabel.text);
            if (m_feedButtonLabel != null)
                m_feedButtonLabel.text = "FEED MATERIALS";

            var anySelected = false;
            for (var i = 0; i < m_rows.Count; i++)
            {
                if (m_rows[i] != null && m_rows[i].SelectedAmount > 0)
                {
                    anySelected = true;
                    break;
                }
            }

            m_feedButton.interactable = anySelected;
        }

        private void RefreshInventoryDisplay()
        {
            if (m_inventorySlots == null)
                return;
            for (var i = 0; i < m_inventorySlots.Length; i++)
                m_inventorySlots[i].Refresh(m_inventoryService.GetSlotData(i));
        }

        private void OnCloseButtonClicked()
        {
            Hide();
            OnCloseRequested?.Invoke();
        }

        private void OnFeedButtonClicked()
        {
            if (m_target == null)
                return;

            if (m_target.IsReadyToUpgrade)
            {
                if (m_target.HasNextLevel)
                    m_target.Upgrade();
                return;
            }

            for (var i = 0; i < m_rows.Count; i++)
            {
                FeedMaterialRowUI row = m_rows[i];
                if (row == null)
                    continue;

                var amount = row.SelectedAmount;
                if (amount <= 0)
                    continue;

                m_target.FeedMaterial(row.Material, amount);
            }

            for (var i = 0; i < m_rows.Count; i++)
                m_rows[i]?.ResetSelection();

            RefreshFeedButton();
        }

        private void OnRowSelectionChanged() => RefreshFeedButton();

        private void OnTargetProgressChanged()
        {
            if (!m_isVisible)
                return;

            RefreshHeader();
            RefreshProgress();
            RefreshRows();
            RefreshFeedButton();
        }

        private void OnTargetLevelUp() => Hide();

        private void OnSlotChanged(SlotChangedEvent evt)
        {
            if (m_inventorySlots != null && evt.SlotIndex >= 0 && evt.SlotIndex < m_inventorySlots.Length)
                m_inventorySlots[evt.SlotIndex].Refresh(evt.Data);

            if (m_isVisible)
                RefreshRows();
        }

        private void OnInventoryToggled(bool isOpen)
        {
            if (isOpen && m_isVisible)
                Hide();
        }

        private void OnCloseUIRequested()
        {
            if (m_isVisible)
                Hide();
        }
    }
}
