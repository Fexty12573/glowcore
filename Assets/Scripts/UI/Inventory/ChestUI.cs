using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    public class ChestUI : MonoBehaviour
    {
        // Instance Fields
        [Header("References")] [SerializeField]
        private CanvasGroup m_panelCanvasGroup;

        [SerializeField] private CanvasGroup m_backdropCanvasGroup;

        [Header("Close Button")] [SerializeField]
        private Button m_closeButton;

        [Header("Chest Section")] [SerializeField]
        private Transform m_chestGridParent;

        [SerializeField] private GameObject m_slotPrefab;

        [Header("Transfer Buttons")] [SerializeField]
        private Button m_takeAllButton;

        [SerializeField] private Button m_insertAllButton;

        [Header("Inventory Display")] [SerializeField]
        private Transform m_inventoryUpperGridParent;

        [SerializeField] private Transform m_inventoryHotbarRowParent;

        private IInventoryService m_inventoryService;
        private InventoryUI m_inventoryUI;
        private Chest m_chest;
        private ItemSlotUI[] m_chestSlots;
        private ItemSlotUI[] m_inventorySlots;
        private bool m_isVisible;

        // Properties
        public bool IsVisible => m_isVisible;

        // Events
        public event Action OnCloseRequested;

        // Public Methods
        public void Show(Chest chest)
        {
            EnsureInitialized();
            BindChest(chest);
            SetVisible(true);
            AudioManager.Instance.Play(AudioManager.SoundType.OpenChest, AudioManager.AudioChannel.Environment);
        }

        public void Hide()
        {
            SetVisible(false);
            AudioManager.Instance.Play(AudioManager.SoundType.CloseChest, AudioManager.AudioChannel.Environment);
        }

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
                UnbindChest();
            }
            else
            {
                RefreshInventoryDisplay();
                RefreshChestDisplay();
            }
        }

        // Private Methods
        private void Start()
        {
            EnsureInitialized();
            SetVisible(false);
        }

        private void EnsureInitialized()
        {
            if (m_inventoryService != null)
                return;

            m_inventoryService = FindFirstObjectByType<PlayerInventory>();
            if (m_inventoryService == null)
            {
                Debug.LogError("ChestUI: Could not find PlayerInventory in scene.");
                return;
            }

            m_inventoryUI = FindFirstObjectByType<InventoryUI>();

            if (m_closeButton != null)
                m_closeButton.onClick.AddListener(OnCloseButtonClicked);
            if (m_takeAllButton != null)
                m_takeAllButton.onClick.AddListener(OnTakeAllClicked);
            if (m_insertAllButton != null)
                m_insertAllButton.onClick.AddListener(OnInsertAllClicked);

            BuildInventoryDisplay();
            m_inventoryService.OnSlotChanged += OnInventorySlotChanged;
        }

        private void OnDestroy()
        {
            if (m_closeButton != null)
                m_closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            if (m_takeAllButton != null)
                m_takeAllButton.onClick.RemoveListener(OnTakeAllClicked);
            if (m_insertAllButton != null)
                m_insertAllButton.onClick.RemoveListener(OnInsertAllClicked);

            if (m_inventoryService != null)
                m_inventoryService.OnSlotChanged -= OnInventorySlotChanged;

            UnbindChest();
        }

        private void BindChest(Chest chest)
        {
            if (m_chest == chest)
                return;

            UnbindChest();

            m_chest = chest;
            if (m_chest == null)
                return;

            BuildChestDisplay();
            m_chest.OnSlotChanged += OnChestSlotChanged;
        }

        private void UnbindChest()
        {
            if (m_chest != null)
                m_chest.OnSlotChanged -= OnChestSlotChanged;

            ClearChestSlots();
            m_chest = null;
        }

        private void BuildInventoryDisplay()
        {
            ClearChildren(m_inventoryUpperGridParent);
            ClearChildren(m_inventoryHotbarRowParent);

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

        private static void ClearChildren(Transform parent)
        {
            if (parent == null)
                return;
            for (var i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);
        }

        private void BuildChestDisplay()
        {
            ClearChestSlots();

            var slotCount = m_chest.SlotCount;
            m_chestSlots = new ItemSlotUI[slotCount];

            for (var i = 0; i < slotCount; i++)
            {
                var go = Instantiate(m_slotPrefab, m_chestGridParent);
                var slot = go.GetComponent<ItemSlotUI>();
                slot.Initialize(m_inventoryUI, m_chest, i, m_chest.GetSlotData(i));
                m_chestSlots[i] = slot;
            }
        }

        private void ClearChestSlots()
        {
            if (m_chestSlots == null)
                return;

            for (var i = 0; i < m_chestSlots.Length; i++)
            {
                if (m_chestSlots[i] != null)
                    Destroy(m_chestSlots[i].gameObject);
            }

            m_chestSlots = null;
        }

        private void RefreshInventoryDisplay()
        {
            if (m_inventorySlots == null)
                return;
            for (var i = 0; i < m_inventorySlots.Length; i++)
                m_inventorySlots[i].Refresh(m_inventoryService.GetSlotData(i));
        }

        private void RefreshChestDisplay()
        {
            if (m_chestSlots == null || m_chest == null)
                return;
            for (var i = 0; i < m_chestSlots.Length; i++)
                m_chestSlots[i].Refresh(m_chest.GetSlotData(i));
        }

        private void OnInventorySlotChanged(SlotChangedEvent evt)
        {
            if (m_inventorySlots == null)
                return;
            if (evt.SlotIndex >= 0 && evt.SlotIndex < m_inventorySlots.Length)
                m_inventorySlots[evt.SlotIndex].Refresh(evt.Data);
        }

        private void OnChestSlotChanged(SlotChangedEvent evt)
        {
            if (m_chestSlots == null)
                return;
            if (evt.SlotIndex >= 0 && evt.SlotIndex < m_chestSlots.Length)
                m_chestSlots[evt.SlotIndex].Refresh(evt.Data);
        }

        private void OnCloseButtonClicked()
        {
            SetVisible(false);
            OnCloseRequested?.Invoke();
        }

        private void OnTakeAllClicked()
        {
            if (m_chest == null)
                return;
            ContainerOps.TransferAll(m_chest, (IItemContainer)m_inventoryService);
            DeselectFocus();
        }

        private void OnInsertAllClicked()
        {
            if (m_chest == null)
                return;
            ContainerOps.TransferAll((IItemContainer)m_inventoryService, m_chest);
            DeselectFocus();
        }

        // Clears the EventSystem's selection so the button drops out of its highlighted
        // / selected color-transition state and snaps back to normal after the click.
        private static void DeselectFocus()
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}